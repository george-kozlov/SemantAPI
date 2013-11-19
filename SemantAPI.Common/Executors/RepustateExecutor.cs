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
using System.Web.Script.Serialization;

using RepustateAPI;

namespace SemantAPI.Common.Executors
{

    public sealed class RepustateExecutor : IExecutor
    {
        #region Private members

        AnalysisExecutionContext _context = null;
        IList<string> _languages = null;

        #endregion

        #region Constructor

        public RepustateExecutor()
        {
            _languages = new List<string>() { "English", "Arabic", "Chinese", "German", "French", "Spanish", "Italian" };
        }

        #endregion

        #region IExecutor members

        public void Execute(AnalysisExecutionContext context)
        {
            _context = context;

            if (context.Results.Count <= 0)
            {
                context.OnExecutionProgress("Repustate", new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Canceled, 0, 0, 0));
                return;
            }

            int processed = 0;
            int failed = 0;
            bool isTrial = false;

            try
            {
                string lang = LocaleHelper.GetDoubleLanguageAbbreviation(context.Language);
                RepustateClient Repustate = new RepustateClient(context.Key);
                Dictionary<string, string> TotalScoreDataMap = new Dictionary<string, string>();
                int count = 0;
                foreach (KeyValuePair<string, ResultSet> document in context.Results)
                {
                    count++;
                    string ky = "text" + count;
                    TotalScoreDataMap.Add(ky, document.Value.ToString());
                }

                int BatchSize = 500;
                int processedBatches = 0;

                if (TotalScoreDataMap.Count < BatchSize)
                {
                    Dictionary<string, string> scoreDataMap = new Dictionary<string, string>();
                    scoreDataMap = Repustate.GetDocumentsQueue(processedBatches, BatchSize, TotalScoreDataMap);                    

                    scoreDataMap.Add("lang", lang);
                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    Array RepustateSentiments;

                    if (context.UseDebugMode)
                    {
                        TimeSpan time = TimeSpan.Zero;
                        RepustateSentiments = (Array)BenchmarkHelper.Invoke(new InvokeBenchmarkHandler(delegate(object state)
                        {
                            string response = Repustate.GetSentimentBulk(scoreDataMap);
                            IDictionary<string, object> deserializedResponse = serializer.Deserialize<IDictionary<string, object>>(response) as IDictionary<string, object>;
                            return deserializedResponse["results"] as Array;

                        }), null, out time);

                        Console.WriteLine("\tRepustate: Batch of {0} documents has been recieved. Eexecution time is: {1} ms", scoreDataMap.Count - 1, time.TotalMilliseconds);
                    }
                    else
                    {
                        string response = Repustate.GetSentimentBulk(scoreDataMap);
                        IDictionary<string, object> deserializedResponse = serializer.Deserialize<IDictionary<string, object>>(response) as IDictionary<string, object>;
                        RepustateSentiments =  deserializedResponse["results"] as Array;
                    }
                    
                    SortedDictionary<int, string> rs = new SortedDictionary<int, string>();

                    for (int i = 0; i < RepustateSentiments.Length; i++)
                    {
                        int id = Int32.Parse(((Dictionary<string, object>)RepustateSentiments.GetValue(i))["id"].ToString());
                        string score = ((Dictionary<string, object>)RepustateSentiments.GetValue(i))["score"].ToString();
                        rs.Add(id, score);
                    }

                    int c = 1;
                    foreach (KeyValuePair<string, ResultSet> document in context.Results)
                    {
                        double score = double.Parse(rs[c]);
                        processed++;

                        if (score <= -0.05)
                            document.Value.AddOutput("Repustate", score, "negative");
                        if (score >= 0.05)
                            document.Value.AddOutput("Repustate", score, "positive");
                        else
                            document.Value.AddOutput("Repustate", score, "neutral");

                        AnalysisExecutionProgressEventArgs ea = new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Processed, context.Results.Count, processed, failed);
                        context.OnExecutionProgress("Repustate", ea);

                        c++;

                        if (ea.Cancel)
                            break;
                    }
                }
                else
                {
                    int totalBatches = (TotalScoreDataMap.Count / BatchSize) + 1;
                    processedBatches = 0;

                    for (int i = 0; i < totalBatches; i++)
                    {
                        Dictionary<string, string> scoreDataMap = new Dictionary<string, string>();

                        scoreDataMap = Repustate.GetDocumentsQueue(processedBatches, BatchSize, TotalScoreDataMap);
                        
                        scoreDataMap.Add("lang", lang);
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        Array RepustateSentiments;

                        if (context.UseDebugMode)
                        {
                            TimeSpan time = TimeSpan.Zero;
                            RepustateSentiments = (Array)BenchmarkHelper.Invoke(new InvokeBenchmarkHandler(delegate(object state)
                            {
                                string response = Repustate.GetSentimentBulk(scoreDataMap);
                                IDictionary<string, object> deserializedResponse = serializer.Deserialize<IDictionary<string, object>>(response) as IDictionary<string, object>;
                                return deserializedResponse["results"] as Array;

                            }), null, out time);

                            Console.WriteLine("\tRepustate: Batch of {0} documents has been recieved. Eexecution time is: {1} ms", scoreDataMap.Count - 1, time.TotalMilliseconds);
                        }
                        else
                        {
                            string response = Repustate.GetSentimentBulk(scoreDataMap);
                            IDictionary<string, object> deserializedResponse = serializer.Deserialize<IDictionary<string, object>>(response) as IDictionary<string, object>;
                            RepustateSentiments = deserializedResponse["results"] as Array;
                        }
                        
                        SortedDictionary<int, string> rs = new SortedDictionary<int, string>();

                        for (int j = 0; j < RepustateSentiments.Length; j++)
                        {
                            int id = Int32.Parse(((Dictionary<string, object>)RepustateSentiments.GetValue(j))["id"].ToString());
                            string score = ((Dictionary<string, object>)RepustateSentiments.GetValue(j))["score"].ToString();
                            rs.Add(id, score);
                        }

                        int key = 1;
                        int UpperCounter = processedBatches;
                        int LowerCounter = BatchSize;
                        foreach (KeyValuePair<string, ResultSet> document in context.Results)
                        {
                            if (UpperCounter > 0)
                            {
                                key++;
                                UpperCounter--;
                                continue;
                            }
                            if (LowerCounter < 1)
                                break;
                            LowerCounter--;

                            double score = double.Parse(rs[key]);
                            processed++;

                            if (score <= -0.05)
                                document.Value.AddOutput("Repustate", score, "negative");
                            if (score >= 0.05)
                                document.Value.AddOutput("Repustate", score, "positive");
                            else
                                document.Value.AddOutput("Repustate", score, "neutral");

                            AnalysisExecutionProgressEventArgs ea = new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Processed, context.Results.Count, processed, failed);
                            context.OnExecutionProgress("Repustate", ea);

                            key++;

                            if (ea.Cancel)
                                break;
                        }

                        processedBatches = processedBatches + BatchSize;
                    }
                }
            }
            catch (WebException ex)
            {
                if (!(ex.Response.ContentLength == -1))
                {
                    isTrial = true;
                    AnalysisExecutionProgressEventArgs ea = new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Canceled, context.Results.Count, 0, 0);
                    ea.Reason = "Your Repustate account doesn’t support " + context.Language + " language";
                    context.OnExecutionProgress("Repustate", ea);
                }
                else
                {
                    isTrial = false;
                    foreach (KeyValuePair<string, ResultSet> document in context.Results)
                    {
                        failed++;
                        document.Value.AddOutput("Repustate", 0, "failed");
                        AnalysisExecutionProgressEventArgs ea = new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Failed, context.Results.Count, processed, failed);
                        ea.Reason = ex.Message;
                        context.OnExecutionProgress("Repustate", ea);
                        
                        if (ea.Cancel)
                           break;
                    }                    
                }

            }
            
            if(!isTrial)
                context.OnExecutionProgress("Repustate", new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Success, context.Results.Count, processed, failed));
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
