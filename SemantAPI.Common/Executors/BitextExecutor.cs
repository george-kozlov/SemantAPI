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
using System.Net;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Linq;
using System.Xml.Serialization;

namespace SemantAPI.Common.Executors
{
	#region Serialization type

	[XmlRootAttribute("RESULT")]
	public sealed class BitextSentiment
	{
		[XmlElementAttribute("BLOCK")]
		public List<BitextSentenceSentiment> Blocks { get; set; }
	}

	[XmlRootAttribute("BLOCK")]
	public sealed class BitextSentenceSentiment
	{
		[XmlElementAttribute("ID")]
		public string Id { get; set; }

		[XmlElementAttribute("GLOBAL_VALUE")]
		public double Value { get; set; }

		[XmlElementAttribute("TEXT")]
		public string Text { get; set; }
	}

	#endregion

	public sealed class BitextExecutor : IExecutor
	{
		#region Private members

		AnalysisExecutionContext _context = null;
		IList<string> _languages = null;

		#endregion

		#region Constructor

		public BitextExecutor()
		{
			_languages = new List<string>() { "English", "Spanish", "Portuguese" };
		}

		#endregion

		#region Private methods

		private string FormatParameters(Dictionary<string, string> parameters)
		{
            StringBuilder paramsBuilder = new StringBuilder();
			int counter = 0;

			foreach (KeyValuePair<string, string> pair in parameters)
			{
				paramsBuilder.AppendFormat("{0}={1}", pair.Key, pair.Value);

				if (counter != parameters.Count - 1)
					paramsBuilder.Append("&");

				counter++;
			}

			return paramsBuilder.ToString();
		}

		private double MergeSentimentScore(BitextSentiment sentiment)
		{
			return sentiment.Blocks.Average(item => item.Value);
		}

		private string GetSentimentPolarity(double score)
		{
			if (score < 0)
				return "negative";
			else if (score > 0)
				return "positive";

			return "neutral";
		}

		#endregion

		#region IExecutor members

		public void Execute(AnalysisExecutionContext context)
		{
			_context = context;

			if (context.Results.Count <= 0)
			{
				context.OnExecutionProgress("Bitext", new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Canceled, 0, 0, 0));
				return;
			}

			int processed = 0;
			int failed = 0;
			foreach (KeyValuePair<string, ResultSet> document in context.Results)
			{
				if (document.Value.Source.Length > 8192)
				{
					failed++;
					document.Value.AddOutput("Bitext", 0, "failed");

					AnalysisExecutionProgressEventArgs ea = new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Failed, context.Results.Count, processed, failed);
					context.OnExecutionProgress("Bitext", ea);

					if (ea.Cancel)
						break;

					continue;
				}

				Dictionary<string, string> parameters = new Dictionary<string, string>();
				parameters.Add("User", context.Key);
				parameters.Add("Pass", context.Secret);
				parameters.Add("OutFormat", context.Format.ToString());
				parameters.Add("Detail", "Global");
				parameters.Add("Normalized", "No");
				parameters.Add("Theme", "Gen");
				parameters.Add("ID", document.Key);
				parameters.Add("Lang", LocaleHelper.GetTripleLanguageAbbreviation(context.Language));
				parameters.Add("Text", HttpUtility.UrlEncode(document.Value.Source));

				byte[] data = Encoding.UTF8.GetBytes(FormatParameters(parameters));
				WebRequest request = WebRequest.Create("http://svc9.bitext.com/WS_NOps_Val/Service.aspx");
				request.ContentType = "application/x-www-form-urlencoded";
				request.Method = "POST";
				request.ContentLength = data.Length;

				using (Stream writer = request.GetRequestStream())
				{
					writer.Write(data, 0, data.Length);
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

						Console.WriteLine("Bitext: Sentiment for the document {0} has been retreived. Execution time is: {1}", document.Key, time.TotalMilliseconds);
					}
					else
						response = request.GetResponse() as HttpWebResponse;

					if (response.StatusCode != HttpStatusCode.Accepted && response.StatusCode != HttpStatusCode.OK)
					{
						failed++;
						document.Value.AddOutput("Bitext", 0, "failed");

						AnalysisExecutionProgressEventArgs ea = new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Processed, context.Results.Count, processed, failed);
						context.OnExecutionProgress("Bitext", ea);
						response.Close();

						if (ea.Cancel)
							break;
					}
					else
					{
						using (StreamReader reader = new StreamReader(response.GetResponseStream()))
						{
							string result = reader.ReadToEnd();
							result = result.Replace("\r\n", string.Empty)
								.Replace("\r", string.Empty)
								.Replace("\n", string.Empty)
								.Replace(">\"", ">")
								.Replace("\"<", "<");

							Regex regex = new Regex(@"(?<=\bencoding="")[^""]*");
							Match match = regex.Match(result);

							Encoding encoding = null;
							if (match.Success)
								encoding = Encoding.GetEncoding(match.Value);
							else
								encoding = Encoding.UTF8;

							BitextSentiment sentiment = null;
							using (Stream stream = new MemoryStream(encoding.GetBytes(result)))
							{
								XmlSerializer serializer = new XmlSerializer(typeof(BitextSentiment));
								sentiment = (BitextSentiment)serializer.Deserialize(stream);
							}

							processed++;
							double score = MergeSentimentScore(sentiment);
							string polarity = GetSentimentPolarity(score);
							document.Value.AddOutput("Bitext", score, polarity);
							AnalysisExecutionProgressEventArgs ea = new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Processed, context.Results.Count, processed, failed);
							context.OnExecutionProgress("Bitext", ea);

							if (ea.Cancel)
								break;
						}
					}
				}
				catch (Exception ex)
				{
					failed++;
					document.Value.AddOutput("Bitext", 0, "failed");
					AnalysisExecutionProgressEventArgs ea = new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Failed, context.Results.Count, processed, failed);
					ea.Reason = ex.Message;
					context.OnExecutionProgress("Bitext", ea);

					if (ea.Cancel)
						break;
				}
			}

			context.OnExecutionProgress("Bitext", new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Success, context.Results.Count, processed, failed));
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
