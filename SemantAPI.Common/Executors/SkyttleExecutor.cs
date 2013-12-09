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
using System.Text;
using System.Web;
using System.IO;
using System.Web.Script.Serialization;

namespace SemantAPI.Common.Executors
{
    public class SkyttleExecutor
    {
        #region Private members

        AnalysisExecutionContext _context = null;
        IList<string> _languages = null;

        #endregion

        #region Constructor

        public SkyttleExecutor()
		{
			_languages = new List<string>() { "English", "French","German", "Russian" };
		}

		#endregion

        #region IExecutor members

        public void Execute(AnalysisExecutionContext context)
        {
            _context = context;
            
            if (context.Results.Count <= 0)
            {
                context.OnExecutionProgress("Skyttle", new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Canceled, 0, 0, 0));
                return;
            }

            int processed = 0;
            int failed = 0;
            foreach (KeyValuePair<string, ResultSet> document in context.Results)
            {
                try { 
                    Dictionary<string, string> scoreDataMap = new Dictionary<string, string>();
                    scoreDataMap.Add("text", document.Value.ToString());

                    string url = "https://sentinelprojects-skyttle20.p.mashape.com/";
                    string text = document.Value.ToString();
                    string lang = LocaleHelper.GetDoubleLanguageAbbreviation(context.Language);

                    string keywords = "1";
                    string sentiment = "1";
                    string annotate = "0";

                    string param = "text=" + HttpUtility.UrlEncode(text, System.Text.Encoding.UTF8) +
                        "&lang=" + HttpUtility.UrlEncode(lang, System.Text.Encoding.UTF8) +
                        "&keywords=" + HttpUtility.UrlEncode(keywords, System.Text.Encoding.UTF8) +
                        "&sentiment=" + HttpUtility.UrlEncode(sentiment, System.Text.Encoding.UTF8) +
                        "&annotate=" + HttpUtility.UrlEncode(annotate, System.Text.Encoding.UTF8);
                    
                    string response = "";

                    if (context.UseDebugMode)
					{
						TimeSpan time = TimeSpan.Zero;
                        response = BenchmarkHelper.Invoke(new InvokeBenchmarkHandler(delegate(object state)
                        {
                            return MakeCall(url, param, context.Key);
                        }), null, out time) as string;
						Console.WriteLine("\tSkyttle: Sentiment for the document {0} has been retreived. Execution time is: {1}", document.Key, time.TotalMilliseconds);
					}
					else
                        response = MakeCall(url, param, context.Key);

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    IDictionary<string, object> dict = serializer.Deserialize<IDictionary<string, object>>(response);
                    
                    processed++;

                    IDictionary<string, object> obj = ((object[])dict["docs"])[0] as IDictionary<string, object>; 
                    string language = obj["language"] as string;
                    IDictionary<string, object> sentiments = obj["sentiment_scores"] as IDictionary<string, object>;
                    decimal negScore = (decimal)sentiments["neg"];
                    decimal posScore = (decimal)sentiments["pos"];
                    decimal neuScore = (decimal)sentiments["neu"];

                    if (neuScore > 50)
                        document.Value.AddOutput("Skyttle", 0.0, "neutral");
                    else if (negScore > posScore)
                        document.Value.AddOutput("Skyttle", Convert.ToDouble(-1 * negScore / 100), "negative");
                    else if (posScore > negScore)
                        document.Value.AddOutput("Skyttle", Convert.ToDouble(posScore / 100), "positive");
                    
                    AnalysisExecutionProgressEventArgs ea = new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Processed, context.Results.Count, processed, failed);
                    context.OnExecutionProgress("Skyttle", ea);

                    if (ea.Cancel)
                        break;
                }
                catch (Exception ex)
                {
                    failed++;
                    document.Value.AddOutput("Skyttle", 0, "failed");
                    AnalysisExecutionProgressEventArgs ea = new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Failed, context.Results.Count, processed, failed);
                    ea.Reason = ex.Message;
                    context.OnExecutionProgress("Skyttle", ea);

                    if (ea.Cancel)
                        break;
                
                }
            }

            context.OnExecutionProgress("Skyttle", new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Success, context.Results.Count, processed, failed));
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

        #region Private Members

        private string MakeCall(String targetURL, String urlParameters, string key)
        {
            Uri url;
            StringBuilder sb;
            HttpWebRequest connection = null;

            // create connection
            url = new Uri(targetURL);
            connection = (HttpWebRequest)HttpWebRequest.Create(url);
            connection.Method = "POST";
            connection.ContentType = "application/x-www-form-urlencoded";
            connection.Headers.Add("X-Mashape-Authorization", key);

            byte[] byteArray = Encoding.UTF8.GetBytes(urlParameters);
            connection.ContentLength = byteArray.Length;
            Stream dataStream = connection.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse response = connection.GetResponse();
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);

            sb = new StringBuilder();
            string line;

            while ((line = reader.ReadLine()) != null)
            {
                sb.Append(line);
                sb.Append('\r');
            }

            reader.Close();
            dataStream.Close();
            response.Close();

            return sb.ToString();
        }

        #endregion
    }
}
