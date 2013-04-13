// <copyright file="ResultSet.cs" company="13th Parish">
// Copyright (c) 13th Parish 2013 All Rights Reserved
// </copyright>
// <author>George Kozlov (george.kozlov@outlook.com)</author>
// <date>03/30/2013</date>
// <summary>SentimentDetails and ResultSet classes</summary>

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemantAPI.Common
{
	#region SentimentDetails class

	public sealed class SentimentDetails
	{
		public SentimentDetails(double score)
		{
			Score = score;
			Polarity = string.Empty;
			Confidence = double.NaN;
		}

		public SentimentDetails(double score, string polarity)
			: this(score)
		{
			Polarity = polarity;
			Confidence = double.NaN;
		}

		public SentimentDetails(double score, string polarity, double confidence)
			: this(score, polarity)
		{
			Confidence = confidence;
		}

		public double Score
		{
			get;
			private set;
		}

		public string Polarity
		{
			get;
			private set;
		}

		public double Confidence
		{
			get;
			private set;
		}

		public override string ToString()
		{
			if (double.IsNaN(Score) && !double.IsNaN(Confidence))
				return string.Format("{0},{1}", Polarity, Confidence.ToString("F"));

			return string.Format("{0},{1}", Polarity, Score.ToString("F")); 
		}
	}

	#endregion

	#region ResultSet class

	public sealed class ResultSet
	{
		Dictionary<string, SentimentDetails> _output = null;

		public ResultSet(string source)
		{
			Source = source;
			_output = new Dictionary<string, SentimentDetails>(5);
		}

		public string Source
		{
			get;
			private set;
		}

		public double GetScore(string service)
		{
			if (!_output.ContainsKey(service))
				return double.NaN;

			return _output[service].Score;
		}

		public string GetPolarity(string service)
		{
			if (!_output.ContainsKey(service))
				return "None";

			return _output[service].Polarity;
		}

		public void AddOutput(string service, double score, string polarity, double confidence = double.NaN)
		{
			if (_output.ContainsKey(service))
				_output[service] = new SentimentDetails(score, polarity, confidence);
			else
				_output.Add(service, new SentimentDetails(score, polarity, confidence));
		}

		public List<string> GetServices()
		{
			List<string> result = _output.Keys.ToList();
			result.Sort();

			return result;
		}

		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			IList<string> services = GetServices();

			foreach (string item in services)
				builder.AppendFormat("{0},", _output[item].ToString());

			builder.AppendFormat("\"{0}\"", Source);
			return builder.ToString();
		}
	}

	#endregion
}
