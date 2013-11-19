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
using System.Text;
using System.Web;
using System.Net;
using System.IO;

namespace RepustateAPI
{
    public class RepustateClient
    {
        #region Public Members

        public string API_KEY;
        public string DOMAIN;
        public string BASE_URL;

        public string FORMAT_JSON;
        public string FORMAT_XML;

        public string URL_ARGUMENT_STRING;

        public string SENTIMENT_PATH;
        public string SENTIMENT_BULK_PATH;
        public string ADJECTIVES_PATH;
        public string NOUNS_PATH;
        public string VERBS_PATH;
        public string CLEAN_HTML_PATH;
        public string NGRAMS_PATH;
        public string EXTRACT_DATES_PATH;
        public string POWERPOINT_PATH;

        #endregion

        #region Constructor

        public RepustateClient(string key)
        {
            API_KEY = key;
            DOMAIN = "http://api.repustate.com/v2/";
            BASE_URL = DOMAIN;// + API_KEY;

            FORMAT_JSON = ".json";
            FORMAT_XML = ".xml";

            URL_ARGUMENT_STRING = "?url=";

            SENTIMENT_PATH = "/score";
            SENTIMENT_BULK_PATH = "/bulk-score";
            ADJECTIVES_PATH = "/adj";
            NOUNS_PATH = "/noun";
            VERBS_PATH = "/verb";
            CLEAN_HTML_PATH = "/clean-html";
            NGRAMS_PATH = "/ngrams";
            EXTRACT_DATES_PATH = "/extract-dates";
            POWERPOINT_PATH = "/powerpoint/";
        }

        #endregion

        #region Public Methods

        public Dictionary<string, string> GetDocumentsQueue(int processedBatches, int BatchSize, Dictionary<string, string> TotalScoreDataMap)
        {
            Dictionary<string, string> scoreDataMap = new Dictionary<string, string>();
            if ((processedBatches + BatchSize) < TotalScoreDataMap.Count)
            {
                for (int j = processedBatches; j < processedBatches + BatchSize; j++)
                    scoreDataMap.Add("text" + (j + 1), TotalScoreDataMap["text" + (j + 1)]);
            }
            else
            {
                for (int j = processedBatches; j < TotalScoreDataMap.Count; j++)
                    scoreDataMap.Add("text" + (j + 1), TotalScoreDataMap["text" + (j + 1)]);
            }
            return scoreDataMap;
        }

        public string GetRepustateData(string path, string format, string httpRequestType, Dictionary<string, string> data, string api)
        {
            string output = SendRequest(path + format, "", httpRequestType, GetContentFromDictionary(data), api);
            return output;
        }
        
        /// <summary>
        /// This method takes a map of strings as argument. Each key value pair will represent a block of text. 
        /// Each argument starting with 'text' will be the only ones scored. To help you reconcile scores with blocks of text, 
        /// Repustate requires that you append some sort of ID to the POST argument. For example, if you had 50 blocks of text, 
        /// you could enumerate them from 1 to 50 as text1, text2, ..., text50.
        /// </summary>
        public string GetSentimentBulk(Dictionary<string, string> data)
        {
            //curl -d "text1=first block of text&text2=second block of text"// http://api.repustate.com/v2/demokey/bulk-score.json
            string output = SendRequest(SENTIMENT_BULK_PATH + FORMAT_JSON, "", "POST", GetContentFromDictionary(data), API_KEY);
            return output;
        }

        public string GetSentiment(string format, Dictionary<string, string> data, string api)
        {
            //curl -d "url=www.twitter.com" http://api.repustate.com/v2/demokey/score.json
            string output = SendRequest(SENTIMENT_PATH + format, "", "POST", GetContentFromDictionary(data), api);
            return output;
        }

        public string GetNouns(string format, Dictionary<string, string> data, string api)
        {
            //curl -d "text=This is a big block of new text" http://api.repustate.com/v2/demokey/adj.xml
            string output = SendRequest(NOUNS_PATH + format, "", "POST", GetContentFromDictionary(data), api);
            return output;
        }

        public string GetAdjectives(string format, Dictionary<string, string> data, string api)
        {
            //curl -d "text=This is a big block of new text" http://api.repustate.com/v2/demokey/adj.xml
            string output = SendRequest(ADJECTIVES_PATH + format, "", "POST", GetContentFromDictionary(data), api);
            return output;
        }

        public string GetVerbs(string format, Dictionary<string, string> data, string api)
        {
            //curl -d "text=This man is buying iPads." http://api.repustate.com/v2/demokey/verb.json
            string output = SendRequest(VERBS_PATH + format, "", "POST", GetContentFromDictionary(data), api);
            return output;
        }

        /// <summary>
        /// API notes from Repustate
        /// Note: all of the above API calls use this implicitly so you don't have to
        /// clean your HTML before passing it to Repustate. Only use this API call if
        /// you intend to do your own processing after the fact with the cleaned up HTML.
        /// Note 2: This call doesn't work on home pages e.g. cnn.com, ebay.com,
        /// nytimes.com. Its intended use is for single article pages.
        /// </summary>
        public string GetCleanHTML(string format, Dictionary<string, string> data, string url, string api)
        {
            //http://api.repustate.com/v2/demokey/clean-html.json?url=http://tcrn.ch/aav9Ty
            string urlStr = CLEAN_HTML_PATH + format;
            if (url != null)
            {
                urlStr = urlStr + URL_ARGUMENT_STRING + url;
            }
            string output = SendRequest(urlStr, "", "GET", GetContentFromDictionary(data), api);
            return output;
        }

        /// <summary>
        /// If the url argument is specified, we will use the HTTP GET method, or if
        /// the text is specified, then we will use the POST method according to the
        /// repustate APIs. if you are using HTTP GET, any optional arguments need to
        /// be URL-encoded. If you are using HTTP POST, any optional arguments should
        /// be part of your POST'ed data.
        /// </summary>
        public string GetNgrams(string format, Dictionary<string, string> data, string url, string api)
        {
            //http://api.repustate.com/v2/demokey/ngrams.json?url=http://tcrn.ch/aav9Ty&max=4&min=2&freq=2

            string output = "";
            if (url != null && !url.Equals(""))
            {
                //In this case, we need to use HTTP GET and all the arguments need to be URL encoded
                string urlStr = NGRAMS_PATH + format;
                urlStr += URL_ARGUMENT_STRING + url + "&";
                urlStr += GetContentFromDictionary(data);
                output = SendRequest(urlStr, "", "GET", "", api);
            }
            else
            {
                //In this case we need to use HTTP POST and pass the arguments in the POST data
                output = SendRequest(NGRAMS_PATH + format, "", "POST", GetContentFromDictionary(data), api);
            }

            return output;
        }

        public string GetExtractedDates(string format, Dictionary<string, string> data, string api)
        {
            //curl -d "text=I can't wait to go to school tomorrow" http://api.repustate.com/v2/demokey/extract-dates.json
            string output = SendRequest(EXTRACT_DATES_PATH + format, "", "POST", GetContentFromDictionary(data), api);
            return output;
        }

        public void SavePowerpointSlides(Dictionary<string, string> data, string fileName)
        {
            //curl -d "author=Martin
            //&report_id=10
            //&report_title=My first report
            //&slide_1_title=My first slide
            //&slide_1_image=<b64 encoding>
            //&slide_1_notes=A great slide to show people" 
            //http://api.repustate.com/v2/your_api_key_goes_here/powerpoint/

            SendPowerPointRequest(POWERPOINT_PATH, "", "POST", GetContentFromDictionary(data), fileName);
        }

        #endregion

        #region Private Methods

        private void SendPowerPointRequest(string path, string contentType, string method, string data, string fileName)
        {
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(BASE_URL + path);
            myHttpWebRequest.Method = method.ToUpper();

            //Write the data to the stream
            if (data != null && !data.Equals(""))
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(data);
                Stream dataStream = myHttpWebRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
            }

            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            Stream streamResponse = myHttpWebResponse.GetResponseStream();

            BinaryReader binReader = new BinaryReader(streamResponse);

            System.IO.FileStream fileStream = new System.IO.FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);

            byte[] buff = new byte[2048];
            int bytesRead;
            while (0 != (bytesRead = binReader.Read(buff, 0, buff.Length)))
            {
                fileStream.Write(buff, 0, bytesRead);
            }
            fileStream.Close();
            binReader.Close();
            streamResponse.Close();
            myHttpWebResponse.Close();
        }

        private string SendRequest(string path, string contentType, string method, string data, string api)
        {
            string outputData = "";
            HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(BASE_URL + api + path);
            myHttpWebRequest.Method = method.ToUpper();

            //Write the data to the stream
            if (data != null && !data.Equals(""))
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(data);
                Stream dataStream = myHttpWebRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();
            }

            HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
            Stream streamResponse = myHttpWebResponse.GetResponseStream();
            StreamReader streamRead = new StreamReader(streamResponse);

            Char[] readBuff = new Char[256];
            int count = streamRead.Read(readBuff, 0, 256);
            while (count > 0)
            {
                outputData += new string(readBuff, 0, count);
                count = streamRead.Read(readBuff, 0, 256);
            }

            streamResponse.Close();
            streamRead.Close();
            myHttpWebResponse.Close();

            return outputData;
        }

        private string SendRequest(string path, string contentType, string api)
        {
            return SendRequest(path, contentType, "GET", "", api);
        }

        private string GetContentFromDictionary(Dictionary<string, string> data)
        {
            string content = "";
            if (data != null)
            {
                int i = 0;
                foreach (KeyValuePair<string, string> kvp in data)
                {
                    if (i != 0)
                    {
                        content += "&";
                    }

                    content += kvp.Key + "=" + HttpUtility.UrlEncode(kvp.Value, Encoding.UTF8);
                    i++;
                }
            }
            else
            {
                content = "";
            }
            return content;
        }

        #endregion
    }
}
