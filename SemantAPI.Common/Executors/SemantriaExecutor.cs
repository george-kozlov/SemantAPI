//
// SemantAPI.Robot, SemantAPI.Human
// Copyright (C) 2013 George Kozlov
// These programs are free software: you can redistribute them and/or modify them under the terms of the GNU General Public License as published by the Free Software Foundation. either version 3 of the License, or any later version.
// These programs are distributed in the hope that they will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License for more details. You should have received a copy of the GNU General Public License along with this program. If not, see http://www.gnu.org/licenses/.
// For further questions or inquiries, please contact semantapi (at) gmail (dot) com
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using Semantria.Com;
using Semantria.Com.Serializers;
using Semantria.Com.Mapping;
using Semantria.Com.Mapping.Output;
using Semantria.Com.Mapping.Configuration;

namespace SemantAPI.Common.Executors
{
	public sealed class SemantriaExecutor : IExecutor
	{
		#region Private members

		AnalysisExecutionContext _context = null;
		IList<string> _languages = null;

		#endregion
        
		#region Constructor

		public SemantriaExecutor()
		{
			_languages = new List<string>() { "English", "French", "Spanish", "German", "Portuguese", "Chinese" };
		}

		#endregion
        
		#region Private methods

		private List<Document> GetSourceData(Dictionary<string, ResultSet> results, int charactersLimit)
		{
			List<Document> documents = new List<Document>(results.Count);

			foreach (KeyValuePair<string, ResultSet> item in results)
			{
				if (item.Value.Source.Length < charactersLimit)
				{
					Document doc = new Document();
					doc.Id = item.Key;
					doc.Text = item.Value.Source;
					documents.Add(doc);
				}
				else
					item.Value.AddOutput("Semantria", 0, "failed");
			}

			return documents;
		}

		private Configuration GetOrCreateConfiguration(Session session, string language)
		{
			Configuration config = null;

			if (_context.UseDebugMode)
			{
				TimeSpan time = TimeSpan.Zero;
				config = BenchmarkHelper.Invoke(new InvokeBenchmarkHandler(delegate(object state)
				{
					return session.GetConfigurations().First(item => item.Language == language);
				}), null, out time) as Configuration;

				if (config == null)
					Console.WriteLine("\tSemantria: Configuration for {0} language isn't available. Execution time is: {1} ms", language, time.TotalMilliseconds);
				else
					Console.WriteLine("\tSemantria: Configuration for {0} language has been retreived. Execution time is: {1} ms", language, time.TotalMilliseconds);
			}
			else
				config = session.GetConfigurations().First(item => item.Language == language);

			if (config == null)
			{
				config = new Configuration();
				config.Language = language;

				if (_context.UseDebugMode)
				{
					TimeSpan time = TimeSpan.Zero;
					int result = (int)BenchmarkHelper.Invoke(new InvokeBenchmarkHandler(delegate(object state)
					{
						return session.AddConfigurations(new List<Configuration>() { config });
					}), null, out time);

					if (result != -1)
						Console.WriteLine("\tSemantria: Configuration for {0} language has been created. Execution time is: {1} ms", language, time.TotalMilliseconds);

					time = TimeSpan.Zero;
					config = BenchmarkHelper.Invoke(new InvokeBenchmarkHandler(delegate(object state)
					{
						return session.GetConfigurations().First(item => item.Language == language);
					}), null, out time) as Configuration;

					if (config != null)
						Console.WriteLine("\tSemantria: Configuration for {0} language has been retreived. Execution time is: {1} ms", language, time.TotalMilliseconds);
				}
				else
				{
					session.AddConfigurations(new List<Configuration>() { config });
					config = session.GetConfigurations().First(item => item.Language == language);
				}
			}

			return config;
		}

		private void session_Error(object sender, ResponseErrorEventArgs e)
		{
			if (_context.UseDebugMode)
				Console.WriteLine("\tSemantria executor got an error with {0} status code. The reason is: {1}", e.Status, e.Message);

			AnalysisExecutionProgressEventArgs ea = new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Failed, 0, 0);
			ea.Reason = e.Message;
			_context.OnExecutionProgress("Semantria", ea);
		}

		#endregion

		#region IExecutor members

		public void Execute(AnalysisExecutionContext context)
		{
			_context = context;

			if (context.Results.Count <= 0)
			{
				context.OnExecutionProgress("Semantria", new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Canceled, 0, 0, 0));
				return;
			}

			ISerializer serializer = new JsonSerializer();
			if (context.Format == DataFormat.XML)
				serializer = new XmlSerializer();

			int index = 0;
			int failed = 0;

			try
			{
				using (Session session = Session.CreateSession(context.Key, context.Secret, serializer))
				{
					session.Error += session_Error;

					Subscription subscription = null;
					if (context.UseDebugMode)
					{
						TimeSpan time = TimeSpan.Zero;
						subscription = BenchmarkHelper.Invoke(new InvokeBenchmarkHandler(delegate(object state)
						{
							return session.GetSubscription();
						}), null, out time) as Subscription;

						Console.WriteLine("\tSemantria: Subscrition object has been obtained. Execution time is: {0} ms", time.TotalMilliseconds);
					}
                    else
						subscription = session.GetSubscription();

					int docLimit = subscription.BasicSettings.CharactersLimit;
					List<Document> documents = GetSourceData(context.Results, docLimit);
					List<DocAnalyticData> output = new List<DocAnalyticData>(documents.Count);

					int batchSize = subscription.BasicSettings.BatchLimit;
					int respBatchLimit = subscription.BasicSettings.ProcessedBatchLimit;
					Configuration config = GetOrCreateConfiguration(session, context.Language);

					int pushProgress = 50 / ((documents.Count < batchSize) ? 1 : (documents.Count / batchSize));
					batchSize = (documents.Count < batchSize) ? documents.Count : batchSize;
					int progress = 0;
                    Dictionary<string, bool> queue = new Dictionary<string, bool>(documents.Count);
					while (index < documents.Count)
					{
						int execResult = -1;
                        batchSize = ((index + batchSize) > documents.Count) ? (documents.Count % batchSize) : batchSize;

                        if (context.UseDebugMode)
                        {
                            TimeSpan time = TimeSpan.Zero;
                            execResult = (int)BenchmarkHelper.Invoke(new InvokeBenchmarkHandler(delegate(object state)
                            {
                                List<Document> docs = documents.GetRange(index, batchSize);
                                foreach (Document doc in docs)
                                    queue.Add(doc.Id, false);

                                return session.QueueBatchOfDocuments(documents.GetRange(index, batchSize), config.ConfigId);
                            }), null, out time);

                            Console.WriteLine("\tSemantria: Batch of {0} documents has been queued. Eexecution time is: {1} ms", batchSize, time.TotalMilliseconds);
                        }
                        else
                        {
                            List<Document> docs = documents.GetRange(index, batchSize);
                            foreach (Document doc in docs)
                                queue.Add(doc.Id, false);
                            
                            execResult = session.QueueBatchOfDocuments(documents.GetRange(index, batchSize), config.ConfigId);
                        }

                        progress = (queue.Count == 1) ? 1 : ((int)(queue.Count / 2));

						if (execResult != -1)
						{
							AnalysisExecutionProgressEventArgs ea = new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Processed, context.Results.Count, progress, failed);
							context.OnExecutionProgress("Semantria", ea);

							if (ea.Cancel)
								break;
						}
						else
						{
							failed += batchSize;
							AnalysisExecutionProgressEventArgs ea = new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Failed, context.Results.Count, progress, failed);
							context.OnExecutionProgress("Semantria", ea);

							if (ea.Cancel)
								break;
						}

						index += batchSize;
					}

					index = 0;
					while (queue.Values.Contains(false))
					{
						Thread.Sleep(1000);

						IList<DocAnalyticData> temp = null;
						if (context.UseDebugMode)
						{
							TimeSpan time = TimeSpan.Zero;
							temp = BenchmarkHelper.Invoke(new InvokeBenchmarkHandler(delegate(object state)
							{
								return session.GetProcessedDocuments(config.ConfigId);
							}), null, out time) as IList<DocAnalyticData>;

							Console.WriteLine("\tSemantria: Batch of {0} documents has been received. Eexecution time is: {1} ms", temp.Count, time.TotalMilliseconds);
						}
						else
							temp = session.GetProcessedDocuments(config.ConfigId);

                        foreach (DocAnalyticData data in temp)
                        {
                            if (context.Results.ContainsKey(data.Id))
                            {
                                context.Results[data.Id].AddOutput("Semantria", data.SentimentScore, data.SentimentPolarity);
                                queue[data.Id] = true;
                            }
                        }

                        index = queue.Values.Count(item => item == true);
                        progress += ((int)(100 * index) / queue.Count);

						AnalysisExecutionProgressEventArgs ea = new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Processed, context.Results.Count, progress, failed);
						context.OnExecutionProgress("Semantria", ea);

						if (ea.Cancel)
							break;
					}
				}
			}
			catch (Exception ex)
			{
				AnalysisExecutionProgressEventArgs ea = new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Failed, context.Results.Count, index, failed);
				ea.Reason = ex.Message;
				context.OnExecutionProgress("Semantria", ea);

				if (ea.Cancel)
					return;
			}

			context.OnExecutionProgress("Semantria", new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Success, context.Results.Count, index, failed));
		}

		public AnalysisExecutionContext Context
		{
			get { return _context; }
		}

		public bool IsLanguageSupported(string language)
		{
			return _languages.Contains(language);
		}
        
		#endregion
	}
}
