//
// SemantAPI.Robot, SemantAPI.Human
// Copyright (C) 2013 George Kozlov
// These programs are free software: you can redistribute them and/or modify them under the terms of the GNU General Public License as published by the Free Software Foundation. either version 3 of the License, or any later version.
// These programs are distributed in the hope that they will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License for more details. You should have received a copy of the GNU General Public License along with this program. If not, see http://www.gnu.org/licenses/.
// For further questions or inquiries, please contact semantapi (at) gmail (dot) com
//

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
			ReferencePolarity = string.Empty;
		}

		public SentimentDetails(double score, string polarity)
			: this(score)
		{
			Polarity = polarity;
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

		public string ReferencePolarity
		{
			get;
			set;
		}

		public override string ToString()
		{
			string result = string.Empty;

			if (double.IsNaN(Score) && !double.IsNaN(Confidence))
				result = string.Format("{0},{1}", Polarity, Confidence.ToString("F"));
			else
				result = string.Format("{0},{1}", Polarity, Score.ToString("F")); 

			if (!string.IsNullOrEmpty(ReferencePolarity))
				result += string.Format(",{0}", (Polarity.Equals(ReferencePolarity) ? "agrees" : "disagrees"));

			return result;
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

		public void AddReferencePolarity(string polarity, string exclude)
		{
			foreach (KeyValuePair<string, SentimentDetails> data in _output)
				if (data.Key != exclude)
					data.Value.ReferencePolarity = polarity;
		}

		public bool HasReferencePolarity()
		{
			return _output.Any(item => string.IsNullOrEmpty(item.Value.ReferencePolarity) == false);
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
