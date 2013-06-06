// <copyright file="ViralheatExecutor.cs" company="13th Parish">
// Copyright (c) 13th Parish 2013 All Rights Reserved
// </copyright>
// <author>George Kozlov (george.kozlov@outlook.com)</author>
// <date>03/30/2013</date>
// <summary>ViralheatSentiment and ViralheatExecutor classes</summary>

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Web;
using System.IO;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace SemantAPI.Common.Executors
{
	#region Serialization type

	[DataContract(Name = "sentiment")]
	public sealed class ViralheatSentiment
	{
		[DataMember(Name = "text")]
		public string Text { get; set; }

		[DataMember(Name = "prob")]
		public double Prob { get; set; }

		[DataMember(Name = "mood")]
		public string Mood { get; set; }
	}

	#endregion

	public sealed class ViralheatExecutor : IExecutor
	{
		#region Private members

		AnalysisExecutionContext _context = null;
		IList<string> _languages = null;

		#endregion

		#region Constructor

		public ViralheatExecutor()
		{
			_languages = new List<string>() { "English" };
		}

		#endregion

		#region IExecutor members

		public void Execute(AnalysisExecutionContext context)
		{
			_context = context;

			if (context.Results.Count <= 0)
			{
				context.OnExecutionProgress("Viralheat", new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Canceled, 0, 0, 0));
				return;
			}

			int processed = 0;
			int failed = 0;
			foreach (KeyValuePair<string, ResultSet> document in context.Results)
			{
				if (document.Value.Source.Length > 360)
				{
					failed++;
					document.Value.AddOutput("Viralheat", 0, "failed");

					AnalysisExecutionProgressEventArgs ea = new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Failed, context.Results.Count, processed, failed);
					context.OnExecutionProgress("Viralheat", ea);

					if (ea.Cancel)
						break;

					continue;
				}

				StringBuilder builder = new StringBuilder();
				builder.Append("http://www.viralheat.com/api/sentiment/review.json?");
				builder.AppendFormat("api_key={0}&", context.Key);
				builder.AppendFormat("text={0}", HttpUtility.UrlEncode(document.Value.Source));

				try
				{
					HttpWebResponse response = null;
					WebRequest request = WebRequest.Create(builder.ToString());
					if (context.UseDebugMode)
					{
						TimeSpan time = TimeSpan.Zero;
						response = BenchmarkHelper.Invoke(new InvokeBenchmarkHandler(delegate(object state)
						{
							return request.GetResponse();
						}), null, out time) as HttpWebResponse;
						Console.WriteLine("\tViralheat: Sentiment for the document {0} has been retreived. Execution time is: {1}", document.Key, time.TotalMilliseconds);
					}
					else
						response = request.GetResponse() as HttpWebResponse;
					
					if (response.StatusCode != HttpStatusCode.Accepted && response.StatusCode != HttpStatusCode.OK)
					{
						failed++;
						document.Value.AddOutput("Viralheat", 0, "failed");

						AnalysisExecutionProgressEventArgs ea = new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Processed, context.Results.Count, processed, failed);
						context.OnExecutionProgress("Viralheat", ea);
						response.Close();

						if (ea.Cancel)
							break;
					}
					else
					{
						using (Stream stream = response.GetResponseStream())
						{

							DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ViralheatSentiment));
							ViralheatSentiment sentiment = (ViralheatSentiment)serializer.ReadObject(stream);

							processed++;
							document.Value.AddOutput("Viralheat", sentiment.Prob, sentiment.Mood);
							AnalysisExecutionProgressEventArgs ea = new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Processed, context.Results.Count, processed, failed);
							context.OnExecutionProgress("Viralheat", ea);

							if (ea.Cancel)
								break;
						}
					}
				}
				catch (Exception ex)
				{
					failed++;
					document.Value.AddOutput("Viralheat", 0, "failed");
					AnalysisExecutionProgressEventArgs ea = new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Failed, context.Results.Count, processed, failed);
					ea.Reason = ex.Message;
					context.OnExecutionProgress("Viralheat", ea);

					if (ea.Cancel)
						break;
				}
			}

			context.OnExecutionProgress("Viralheat", new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Success, context.Results.Count, processed, failed));
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
