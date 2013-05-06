// <copyright file="ChatterboxExecutor.cs" company="13th Parish">
// Copyright (c) 13th Parish 2013 All Rights Reserved
// </copyright>
// <author>George Kozlov (george.kozlov@outlook.com)</author>
// <date>03/30/2013</date>
// <summary>ChatterboxSentiment and ChatterboxExecutor classes</summary>

using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace SemantAPI.Common.Executors
{
	#region Serialization type

	[DataContract(Name = "sentiment")]
	public sealed class ChatterboxSentiment
	{
		[DataMember(Name = "language")]
		public string Language { get; set; }

		[DataMember(Name = "value")]
		public double Value { get; set; }

		[DataMember(Name = "sent")]
		public int Sent { get; set; }
	}

	#endregion

	public sealed class ChatterboxExecutor : IExecutor
	{
		#region Private members

		AnalysisExecutionContext _context = null;

		#endregion

		#region Constructor

		public ChatterboxExecutor()
		{
		}

		#endregion

		#region Private methods

		private string FormatParameters(Dictionary<string, string> parameters)
		{  
			var paramsBuilder = new StringBuilder();
			var counter = 0;

			foreach (KeyValuePair<string, string> pair in parameters)  
			{
				paramsBuilder.AppendFormat("{0}={1}", pair.Key, pair.Value);  
  
				if (counter != parameters.Count - 1)  
					paramsBuilder.Append("&");  
  
				counter++;  
			}

			return paramsBuilder.ToString();
		}

		private string GetSentimentPolarity(double score)
		{
			if (score < -0.25)
				return "negative";
			else if (score > 0.25)
				return "positive";

			return "neutral";
		}

		#endregion

		#region Public methods and properties

		public void Execute(AnalysisExecutionContext context)
		{
			_context = context;

			if (context.Results.Count <= 0)
			{
				context.OnExecutionProgress("Chatterbox", new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Canceled, 0, 0, 0));
				return;
			}

			int processed = 0;
			int failed = 0;
			foreach (KeyValuePair<string, ResultSet> document in context.Results)
			{
				if (document.Value.Source.Length > 300)
				{
					failed++;
					document.Value.AddOutput("Chatterbox", 0, "failed");

					AnalysisExecutionProgressEventArgs ea = new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Failed, context.Results.Count, processed, failed);
					context.OnExecutionProgress("Bitext", ea);

					if (ea.Cancel)
						break;

					continue;
				}

				Dictionary<string, string> parameters = new Dictionary<string, string>();
				parameters.Add("lang", LocaleHelper.GetLanguageAbbreviation(context.Language));
				parameters.Add("text", Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(document.Value.Source)));

				WebRequest request = WebRequest.Create("https://chatterbox-analytics-sentiment-analysis-free.p.mashape.com/sentiment/current/classify_text/");
				request.Headers.Add("X-Mashape-Authorization", context.Key);
				request.ContentType = "application/x-www-form-urlencoded";
				request.Method = "POST";

				using (StreamWriter writer = new StreamWriter(request.GetRequestStream()))
				{

					writer.Write(FormatParameters(parameters));
					writer.Flush();
				}

				try
				{
					HttpWebResponse response = null;
					if (context.UseDebugMode)
					{
						TimeSpan time = TimeSpan.Zero;
						response = BenchmarkHelper.Invoke(new InvokeBenchmarkHandler(delegate(object state)
						{
							return request.GetResponse();
						}), null, out time) as HttpWebResponse;

						Console.WriteLine("\tChatterbox: Sentiment for the document {0} has been retreived. Execution time is: {1}", document.Key, time.TotalMilliseconds);
					}
					else
						response = request.GetResponse() as HttpWebResponse;

					if (response.StatusCode != HttpStatusCode.Accepted && response.StatusCode != HttpStatusCode.OK)
					{
						failed++;
						document.Value.AddOutput("Chatterbox", 0, "failed");
						
						AnalysisExecutionProgressEventArgs ea = new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Processed, context.Results.Count, processed, failed);
						context.OnExecutionProgress("Chatterbox", ea);
						response.Close();

						if (ea.Cancel)
							break;
					}
					else
					{
						using (Stream stream = response.GetResponseStream())
						{
							DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(ChatterboxSentiment));
							ChatterboxSentiment sentiment = (ChatterboxSentiment)serializer.ReadObject(stream);

							processed++;
							string polarity = GetSentimentPolarity(sentiment.Value);
							document.Value.AddOutput("Chatterbox", sentiment.Value, polarity);
							AnalysisExecutionProgressEventArgs ea = new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Processed, context.Results.Count, processed, failed);
							context.OnExecutionProgress("Chatterbox", ea);

							if (ea.Cancel)
								break;
						}
					}
				}
				catch (Exception ex)
				{
					failed++;
					document.Value.AddOutput("Chatterbox", 0, "failed");
					AnalysisExecutionProgressEventArgs ea = new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Failed, context.Results.Count, processed, failed);
					ea.Reason = ex.Message;
					context.OnExecutionProgress("Chatterbox", ea);

					if (ea.Cancel)
						break;
				}
			}

			context.OnExecutionProgress("Chatterbox", new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Success, context.Results.Count, processed, failed));
		}


		public AnalysisExecutionContext Context
		{
			get { return _context; }
		}

		#endregion
	}
}
