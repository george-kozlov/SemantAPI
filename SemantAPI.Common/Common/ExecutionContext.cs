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

namespace SemantAPI.Common
{
	#region AnalysisExecutionProgressHandler delegate

	public delegate void AnalysisExecutionProgressHandler(string service, AnalysisExecutionProgressEventArgs ea);

	#endregion

	#region Enums

	public enum DataFormat
	{
		XML,
		JSON
	}

	public enum AnalysisExecutionStatus
	{
		Processed,
		Failed,
		Success,
		Canceled
	}

	#endregion

	#region AnalysisExecutionProgressEventArgs class

	public sealed class AnalysisExecutionProgressEventArgs : EventArgs
	{
		public AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus status, int total, int processed, int failed = 0)
		{
			Status = status;
			TotalDocuments = total;
			ProcessedDocuments = processed;
			FailedDocuments = failed;
			Cancel = false;
		}

		public int TotalDocuments
		{
			get;
			private set;
		}

		public int ProcessedDocuments
		{
			get;
			private set;
		}

		public int FailedDocuments
		{
			get;
			private set;
		}

		public int Progress
		{
			get
			{
				if ((ProcessedDocuments + FailedDocuments) < TotalDocuments)
					return ((ProcessedDocuments + FailedDocuments) * 100) / TotalDocuments;
				return 100;
			}
		}

		public AnalysisExecutionStatus Status
		{
			get;
			set;
		}

		public bool Cancel
		{
			get;
			set;
		}

		public string Reason
		{
			get;
			set;
		}
	}

	#endregion

	#region AnalysisExecutionContext class

	public sealed class AnalysisExecutionContext
	{
		Dictionary<string, ResultSet> _results = null;

		public AnalysisExecutionContext(Dictionary<string, ResultSet> results)
		{
			_results = results;
		}

		public string Key
		{
			get;
			set;
		}

		public string Secret
		{
			get;
			set;
		}

		public string Language
		{
			get;
			set;
		}

		public DataFormat Format
		{
			get;
			set;
		}

		public int DocumentLength
		{
			get;
			set;
		}

		public Dictionary<string, ResultSet> Results
		{
			get
			{
				return _results;
			}
		}

		public bool UseDebugMode
		{
			get;
			set;
		}

		public object CustomField
		{
			get;
			set;
		}

		public event AnalysisExecutionProgressHandler ExecutionProgress;

		public void OnExecutionProgress(string service, AnalysisExecutionProgressEventArgs ea)
		{
			if (ExecutionProgress != null)
				ExecutionProgress(service, ea);
		}
	}

	#endregion
}
