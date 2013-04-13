using System;
using System.Web;

namespace AlchemyAPI
{
	public class AlchemyAPI_BaseParams
	{
		public enum OutputMode
		{
			NONE,
			XML,
			RDF
		};

		private String url;
		private String html;
		private String text;
		private OutputMode outputMode = OutputMode.XML;
		private String customParameters;
	
		public String getUrl()
		{
			return url;
		}

		public void setUrl(String url)
		{
			this.url = url;
		}

		public String getHtml()
		{
			return html;
		}

		public void setHtml(String html)
		{
			this.html = html;
		}

		public String getText()
		{
			return text;
	   	}

		public void setText(String text)
		{
			this.text = text;
		}

		public OutputMode getOutputMode()
		{
			return outputMode;
		}

		public void setOutputMode(OutputMode outputMode)
		{
			this.outputMode = outputMode;
		}

		public String getCustomParameters()
		{
			return customParameters;
		}

		public void setCustomParameters(params object[] argsRest)
		{
			string returnString = "";

			for (int i = 0; i < argsRest.Length; ++i)
			{
				returnString = returnString + '&' + argsRest[i];
				if (++i < argsRest.Length)
					returnString = returnString + '=' + HttpUtility.UrlEncode((string)argsRest[i]);
			}

			this.customParameters = returnString;
		}

		public void resetBaseParams()
		{
			url = null;
			html = null;
			text = null;
		}

		virtual public String getParameterString()
		{
			String retString = "";

			if (url != null) retString += "&url=" + HttpUtility.UrlEncode(url);
			if (html != null) retString += "&html=" + HttpUtility.UrlEncode(html);
			if (text != null) retString += "&text=" + HttpUtility.UrlEncode(text);
			if (customParameters!=null) retString+=customParameters;
			if (outputMode != OutputMode.NONE)
			{
				if (outputMode == OutputMode.XML)
					retString += "&outputMode=xml";
				else if (outputMode == OutputMode.RDF)
					retString += "&outputMode=rdf";
			} 

			return retString;
		}
	}
}
