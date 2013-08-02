// <copyright file="Form.cs" company="13th Parish">
// Copyright (c) 13th Parish 2013 All Rights Reserved
// </copyright>
// <author>George Kozlov (george.kozlov@outlook.com)</author>
// <date>03/30/2013</date>
// <summary>Main AnalysisComparer form class</summary>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Reflection;

using SemantAPI.Common;
using SemantAPI.Common.Executors;

namespace SemantAPI.Human
{
	public sealed partial class SemantAPIHuman : Form
	{
		#region Private members

		bool _cancelFlag = false;
		Dictionary<string, ResultSet> _documents = new Dictionary<string, ResultSet>();

		#endregion

		#region Thread safe control execution

		private delegate object GetControlPropertyThreadSafeDelegate(Control control, string propertyName);
		private delegate void SetControlPropertyThreadSafeDelegate(Control control, string propertyName, object propertyValue);

		private object GetControlPropertyThreadSafe(Control control, string propertyName)
		{
			if (control.InvokeRequired)
				return control.Invoke(new GetControlPropertyThreadSafeDelegate(GetControlPropertyThreadSafe), new object[] { control, propertyName });
			else
				return control.GetType().InvokeMember(propertyName, BindingFlags.GetProperty, null, control, new object[]{});
		}

		private void SetControlPropertyThreadSafe(Control control, string propertyName, object propertyValue)
		{
			if (control.InvokeRequired)
				control.Invoke(new SetControlPropertyThreadSafeDelegate(SetControlPropertyThreadSafe), new object[] { control, propertyName, propertyValue });
			else
				control.GetType().InvokeMember(propertyName, BindingFlags.SetProperty, null, control, new object[] { propertyValue });
		}

		#endregion

		#region Constructor

		public SemantAPIHuman(bool useDebugMode)
		{
			UseDebugMode = useDebugMode;
			InitializeComponent();

			IList<string> countries = LocaleHelper.GetAllCountries();
			BindingSource bindSource = new BindingSource();
			bindSource.DataSource = countries;
			cbMechanicalTurkLocale.DataSource = bindSource;
			cbMechanicalTurkLocale.SelectedIndex = -1;

			cbLanguage.SelectedIndex = 0;
			dtMechanicalTurk.Value = DateTime.Now.AddDays(1);
		}

		#endregion

		#region Private methods & properties

		private bool UseDebugMode
		{
			get;
			set;
		}

		internal bool Benchmark
		{
			get;
			set;
		}

		private void WriteDebugInfo(string value)
		{
			if (UseDebugMode)
				Console.WriteLine(value);
		}

		private void WriteDebugInfo(string format, params object[] args)
		{
			if (UseDebugMode)
				Console.WriteLine(format, args);
		}

		private void ClearDebugInfo()
		{
			if (UseDebugMode)
				Console.Clear();
		}

		private bool VerifyInput()
		{
			if (string.IsNullOrEmpty(tbSource.Text))
			{
				MessageBox.Show("Source file isn't selected. Please select the file to proceed.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}

			if (string.IsNullOrEmpty(tbMechanicalTurkKey.Text) || string.IsNullOrEmpty(tbMechanicalTurkSecret.Text))
			{
				MessageBox.Show("Mechanical Turk key or secret isn't provided. Please provide the keys to proceed.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}

			if (cbMechanicalTurkNotification.Checked &&
				!Regex.IsMatch(tbMechanicalTurkEmail.Text, @"^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$"))
			{
				MessageBox.Show("Provided email address isn't valid. Please correct it.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}

			return true;
		}

		private bool CancelIfNecessary()
		{
			if (btProcess.Text == "Cancel")
			{
				_cancelFlag = true;
				btClose.Enabled = true;
				btProcess.Text = "Process";
				pbMechanicalTurk.Value = 0;

				WriteDebugInfo("Data analysis process has been canceled!");
				return true;
			}

			return false;
		}

		private void ReadTheSource(string path)
		{
			if (!File.Exists(path) || new FileInfo(path).Length == 0)
				throw new FileNotFoundException("Source file isn't exists or it's empty!");

			_documents.Clear();
			WriteDebugInfo("Start reading of the file...");
			try
			{
                using (CsvFileReader reader = new CsvFileReader(path, EmptyLineBehavior.Ignore))
                {
                    bool isFirstRow = true;
                    bool hasComplianceScore = false;
                    List<string> row = new List<string>();
                    List<string> services = new List<string>();

                    while (reader.ReadRow(row))
                    {
                        if (isFirstRow)
                        {
                            isFirstRow = false;
                            if (row.Count <= 1)
                                throw new Exception("Source file isn't proper CSV file for SemantAPI.Human application");
                            if (!row[0].Equals("Document ID"))
                                throw new Exception("Source file doesn't have \"Document ID\" column or it's not the first one.");
                            if (row[row.Count - 1] != "Source text")
                                throw new Exception("Source file doesn't have \"Source text\" column or it's not the last one.");

                            for (int index = 1; index < row.Count - 1; index++)
                            {
                                string service = row[index].Split(' ').First();
                                if (!services.Contains(service))
                                    services.Add(service);
                            }

                            hasComplianceScore = row.Any(item => item.Contains("Compliance"));
                            continue;
                        }

                        int col = 1;
                        string polarity = string.Empty;
                        ResultSet result = new ResultSet(row[row.Count - 1]);

                        foreach (string service in services)
                        {
                            result.AddOutput(service, double.Parse(row[col + 1]), row[col]);
                            if (hasComplianceScore)
                                polarity = row[col + 2];

                            col += (hasComplianceScore && service != "MechanicalTurk") ? 3 : 2;
                        }

                        if (hasComplianceScore)
                            result.AddReferencePolarity(polarity, "MechanicalTurk");

                        _documents.Add(row[0], result);
                        Application.DoEvents();
                    }

                    WriteDebugInfo("Source file has been loaded.");
                }
			}
			catch (IOException ex)
			{
				throw new Exception("File is locked or another IO problem happened.", ex);
			}
			catch (Exception ex)
			{
				throw new Exception("File is locked or another IO problem happened.", ex);
			}

			return;
		}

		private void NormalizeResultSet(Dictionary<string, ResultSet> results)
		{
			foreach (KeyValuePair<string, ResultSet> item in results)
				if (!item.Value.HasReferencePolarity())
					item.Value.AddReferencePolarity("undefined", "MechanicalTurk");
		}

		#endregion

		#region Dummy event handlers

		private void btnSource_Click(object sender, EventArgs e)
		{
			FileDialog dialog = new OpenFileDialog();
			dialog.Filter = "CSV files|*.csv";

			DialogResult result = dialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				tbSource.Text = dialog.FileName;
			}
		}

		private void NumericTextBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			if (textBox == null)
				return;

			if ((!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) || textBox.Text.Length > 5)
				e.Handled = true;
		}

		private void tbMechanicalTurkPercent_KeyPress(object sender, KeyPressEventArgs e)
		{
			if ((!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) || tbMechanicalTurkPercent.Text.Length > 3)
				e.Handled = true;

			int value = int.Parse(tbMechanicalTurkPercent.Text);
			if (value < 0) value = 0;
			if (value > 100) value = 100;
		}

		private void llMechanicalTurk_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			llMechanicalTurk.LinkVisited = true;
			System.Diagnostics.Process.Start("https://www.mturk.com");
		}

		private void btAbout_Click(object sender, EventArgs e)
		{
			About about = new About();
			about.ShowDialog(this);
		}

		private void btClose_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void cbMechanicalTurkNotification_CheckedChanged(object sender, EventArgs e)
		{
			tbMechanicalTurkEmail.Enabled = cbMechanicalTurkNotification.Checked;
		}

		#endregion

		#region Context execution handlers

		private void ExecutionProgress(string service, AnalysisExecutionProgressEventArgs ea)
		{
			if (_cancelFlag)
			{
				ea.Cancel = true;
				return;
			}

			if (ea.Status == AnalysisExecutionStatus.Canceled)
			{
				MessageBox.Show(ea.Reason, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				_cancelFlag = true;
				return;
			}

			if (ea.Status == AnalysisExecutionStatus.Processed || ea.Status == AnalysisExecutionStatus.Failed)
			{
				WriteDebugInfo("Got the {0} progress event from {1}. Total documents: {2}\tProcessed: {3}\tFailed:{4}", ea.Progress, service, ea.TotalDocuments, ea.ProcessedDocuments, ea.FailedDocuments);
				SetControlPropertyThreadSafe(pbMechanicalTurk, "Value", ea.Progress);
			}

			if (ea.Status == AnalysisExecutionStatus.Success)
			{
				WriteDebugInfo("All documents have been sent for processing. Sent documents: {0} Failed documents: {1}", ea.ProcessedDocuments, ea.FailedDocuments);

				bool hasComplianceScore = _documents.Any(item => item.Value.HasReferencePolarity());
				if (hasComplianceScore)
					NormalizeResultSet(_documents);

				string outputFile = GetControlPropertyThreadSafe(tbSource, "Text") as string;
				try
				{
                    using (CsvFileWriter writter = new CsvFileWriter(outputFile))
                    {
                        List<string> columns = new List<string>();
                        columns.Add("Document ID");

                        IList<string> services = _documents.First().Value.GetServices();
                        foreach (string item in services)
                        {
                            if (item == "MechanicalTurk")
                                columns.AddRange(new List<string>()
                                {
                                    string.Format("{0} Polarity", item),
                                    string.Format("{0} Confidence Score", item)
                                });
                            else if (item == "Bitext")
                                columns.AddRange(new List<string>()
                                {
                                    string.Format("{0} Polarity", item),
                                    string.Format("{0} Average Sentiment Score", item)
                                });
                            else if (item == "Viralheat")
                                columns.AddRange(new List<string>()
                                {
                                    string.Format("{0} Polarity", item),
                                    string.Format("{0} Probability Score", item)
                                });
                            else
                                columns.AddRange(new List<string>()
                                {
                                    string.Format("{0} Polarity", item),
                                    string.Format("{0} Sentiment Score", item)
                                });
                        }

                        columns.Add("Source text");
                        writter.WriteRow(columns);

                        foreach (KeyValuePair<string, ResultSet> data in _documents)
                        {
                            columns = new List<string>();
                            columns.Add(data.Key);

                            foreach (string srvc in services)
                            {
                                ResultSet results = data.Value;
                                columns.Add(results.GetPolarity(srvc));
                                columns.Add(results.GetScore(srvc).ToString("F"));
                            }

                            columns.Add(data.Value.Source);
                            writter.WriteRow(columns);
                        }
                    }
				}
				catch (IOException ex)
				{
					MessageBox.Show("Looks like the file is in use. Close another program that locks the file.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					WriteDebugInfo("IO Exception: {0}", ex.Message);
				}
				WriteDebugInfo("The output file is written!");

				SetControlPropertyThreadSafe(btClose, "Enabled", true);
				SetControlPropertyThreadSafe(btProcess, "Text", "Process");
				SetControlPropertyThreadSafe(pbMechanicalTurk, "Value", 0);
				MessageBox.Show("Analysis is done! Check the output file for results.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
		}

		#endregion

		#region Process button handler

		private void btProcess_Click(object sender, EventArgs e)
		{
			if (CancelIfNecessary() || !VerifyInput())
				return;

			try
			{
				ReadTheSource(tbSource.Text);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			btProcess.Text = "Cancel";
			btClose.Enabled = false;
			_cancelFlag = false;

			AnalysisExecutionContext context = new AnalysisExecutionContext(_documents);
			context.ExecutionProgress += ExecutionProgress;
			context.Key = tbMechanicalTurkKey.Text;
			context.Secret = tbMechanicalTurkSecret.Text;
			context.Language = cbLanguage.Text;

			MechanicalTurkSettings settings = new MechanicalTurkSettings(null,
				int.Parse(cbMechanicalTurkAssignments.Text), decimal.Parse(tbMechanicalTurkReward.Text),
				(int.Parse(tbMechanicalTurkTime.Text) * 60), (int.Parse(tbMechanicalTurkApprove.Text) * 60),
				int.Parse(tbMechanicalTurkPercent.Text));
			if (cbMechanicalTurkNotification.Checked)
				settings.Email = tbMechanicalTurkEmail.Text;
			if (cbMechanicalTurkLocale.SelectedIndex > -1)
				settings.Locale = LocaleHelper.GetCountryAbbreviation(cbMechanicalTurkLocale.SelectedItem as string);
			context.CustomField = settings;

			if (UseDebugMode && Benchmark)
				context.UseDebugMode = true;

			WriteDebugInfo("Running the dedicated thread for MechanicalTurk context serving...");
			bool isFirstRun = !_documents.First().Value.GetServices().Contains("MechanicalTurk");
			MechanicalTurkExecutor executor = new MechanicalTurkExecutor();
		
			ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object obj)
			{
				if (isFirstRun)
					executor.Execute(context);
				else
					executor.Request(context);
			}), null);
		}

		#endregion
	}
}
