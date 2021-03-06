﻿//
// SemantAPI.Robot, SemantAPI.Human
// Copyright (C) 2013 George Kozlov
// These programs are free software: you can redistribute them and/or modify them under the terms of the GNU General Public License as published by the Free Software Foundation. either version 3 of the License, or any later version.
// These programs are distributed in the hope that they will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License for more details. You should have received a copy of the GNU General Public License along with this program. If not, see http://www.gnu.org/licenses/.
// For further questions or inquiries, please contact semantapi (at) gmail (dot) com
//

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

namespace SemantAPI.Robot
{
	public sealed partial class SemantAPIRobot : Form
	{
		#region Private members

		bool _cancelFlag = false;
		Dictionary<string, bool> _marker = new Dictionary<string, bool>();
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

		public SemantAPIRobot(bool useDebugMode)
		{
			UseDebugMode = useDebugMode;
			InitializeComponent();
			cbLanguage.SelectedIndex = 0;
		}

		#endregion

		#region Private methods & properties

		private bool UseDebugMode
		{
			get;
			set;
		}

		internal List<string> Benchmark
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

		private void InitMarker(string service, bool create)
		{
			if (create)
			{
				if (_marker.ContainsKey(service))
					_marker[service] = false;
				else
					_marker.Add(service, false);
			}
			else
			{
				if (_marker.ContainsKey(service))
					_marker.Remove(service);
			}

		}

		private void UpdateMarker(string service, bool status)
		{
			if (_marker.ContainsKey(service))
				_marker[service] = status;
			else
				_marker.Add(service, status);
		}

		private bool IsAnalysysDone()
		{
			return !_marker.Any(item => item.Value == false);
		}

		private bool VerifyInput()
		{
			if (_marker.Count == 0)
			{
				MessageBox.Show("You need to select at least one service to process your data!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}

			if (int.Parse(tbDocSize.Text) <= 10)
			{
				MessageBox.Show("Text cutting threshold should be at least 10 characters.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}

			if (string.IsNullOrEmpty(tbSource.Text) || !File.Exists(tbSource.Text) ||
				string.IsNullOrEmpty(tbOutput.Text) || Uri.IsWellFormedUriString(tbOutput.Text, UriKind.Absolute))
			{
				MessageBox.Show("No Source or Output file has been selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
				return false;
			}

			if (cbSemantria.Checked)
			{
				if (string.IsNullOrEmpty(tbSemantriaKey.Text) || string.IsNullOrEmpty(tbSemantriaSecret.Text))
				{
					MessageBox.Show("Semantria API Key or Secret is missing.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}
				else if (!new SemantriaExecutor().IsLanguageSupported(cbLanguage.Text))
				{
					MessageBox.Show(string.Format("Semantria doesn't support {0} language. Please uncheck Semantria service or select another language.", cbLanguage.Text),
						"Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}
			}

			if (cbAlchemy.Checked)
			{
				if (string.IsNullOrEmpty(tbAlchemyKey.Text))
				{
					MessageBox.Show("Alchemy API Key is missing.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}
				else if (!new AlchemyExecutor().IsLanguageSupported(cbLanguage.Text))
				{
					MessageBox.Show(string.Format("Alchemy doesn't support {0} language. Please uncheck Alchemy service or select another language.", cbLanguage.Text),
						"Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}
			}

            if (cbRepustate.Checked)
            {
                if (string.IsNullOrEmpty(tbRepustateKey.Text))
                {
                    MessageBox.Show("Repustate API Key is missing.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                else if (!new RepustateExecutor().IsLanguageSupported(cbLanguage.Text))
                {
                    MessageBox.Show(string.Format("Repustate doesn't support {0} language. Please uncheck Repustate service or select another language.", cbLanguage.Text),
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }

			if (cbChatterbox.Checked)
			{
				if (string.IsNullOrEmpty(tbChatterboxKey.Text))
				{
					MessageBox.Show("Chatterbox API Key is missing.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}
				else if (!new ChatterboxExecutor().IsLanguageSupported(cbLanguage.Text))
				{
					MessageBox.Show(string.Format("Chatterbox doesn't support {0} language. Please uncheck Chatterbox service or select another language.", cbLanguage.Text),
						"Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}
			}

			if (cbViralheat.Checked)
			{
				if (string.IsNullOrEmpty(tbViralheatKey.Text))
				{
					MessageBox.Show("Viralheat API Key is missing.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}
				else if (!new ViralheatExecutor().IsLanguageSupported(cbLanguage.Text))
				{
					MessageBox.Show(string.Format("Viralheat doesn't support {0} language. Please uncheck Viralheat service or select another language.", cbLanguage.Text),
						"Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}
			}

			if (cbBitext.Checked)
			{
				if (string.IsNullOrEmpty(tbBitextLogin.Text) || string.IsNullOrEmpty(tbBitextPassword.Text))
				{
					MessageBox.Show("Bitext Login or Password is missing.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}
				else if (!new BitextExecutor().IsLanguageSupported(cbLanguage.Text))
				{
					MessageBox.Show(string.Format("Bitext doesn't support {0} language. Please uncheck Bitext service or select another language.", cbLanguage.Text),
						"Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
					return false;
				}
			}

            if (cbSkyttle.Checked)
            {
                if (string.IsNullOrEmpty(tbSkyttleKey.Text))
                {
                    MessageBox.Show("Skyttle API Key is missing.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                else if (!new SkyttleExecutor().IsLanguageSupported(cbLanguage.Text))
                {
                    MessageBox.Show(string.Format("Skyttle doesn't support {0} language. Please uncheck Skyttle service or select another language.", cbLanguage.Text),
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
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
				pbAlchemy.Value = 0;
                pbRepustate.Value = 0;
				pbChatterbox.Value = 0;
				pbSemantria.Value = 0;
				pbViralheat.Value = 0;
				pbBitext.Value = 0;
                pbSkyttle.Value = 0;

				WriteDebugInfo("Data analysis process has been canceled!");
				return true;
			}

			return false;
		}

		private void ReadTheSource(string path, int cutBy)
		{
			if (!File.Exists(path) || new FileInfo(path).Length == 0)
				throw new FileNotFoundException("Source file isn't exists or it's empty!");

			bool isCSV = path.Contains(".csv");

			_documents.Clear();
			WriteDebugInfo("Start reading of the file...");

			try
			{
                using (CsvFileReader reader = new CsvFileReader(path, EmptyLineBehavior.Ignore))
                {
                    List<string> row = new List<string>();
                    while (reader.ReadRow(row))
                    {
                        if (row == null || row.Count < 1)
                            continue;

                        string text = row[row.Count - 1];
                        if (text.Length > cutBy)
                            text = text.Substring(0, cutBy);

                        _documents.Add(Guid.NewGuid().ToString(), new ResultSet(text));
                        Application.DoEvents();
                    }

                    WriteDebugInfo("Source file has been loaded.");
                }
			}
			catch (IOException ex)
			{
				throw new Exception("File is locked or another IO problem happened.", ex);
			}

			return;
		}

		#endregion

		#region Dummy event handlers

		private void btnSource_Click(object sender, EventArgs e)
		{
			FileDialog dialog = new OpenFileDialog();
			dialog.Filter = "CSV files (*.csv)|*.csv";

			DialogResult result = dialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				tbSource.Text = dialog.FileName;
			}
		}

		private void btnOutput_Click(object sender, EventArgs e)
		{
			FileDialog dialog = new SaveFileDialog();
			dialog.Filter = "CSV files|*.csv";

			DialogResult result = dialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				tbOutput.Text = dialog.FileName;
			}
		}

		private void tbDocSize_KeyPress(object sender, KeyPressEventArgs e)
		{
			if ((!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)))
				e.Handled = true;

			if (tbDocSize.Text.Length > 5)
				tbDocSize.Text = tbDocSize.Text.Substring(0, 4);
		}

		private void tbDocSize_TextChanged(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(tbDocSize.Text))
				return;

			int value = int.Parse(tbDocSize.Text);
			if (value < 0) tbDocSize.Text = "0";
			if (value > 65535) tbDocSize.Text = "65535";
		}

		private void cbSemantria_CheckedChanged(object sender, EventArgs e)
		{
			tbSemantriaKey.Enabled = cbSemantria.Checked;
			tbSemantriaSecret.Enabled = cbSemantria.Checked;
			InitMarker("Semantria", cbSemantria.Checked);
		}

		private void cbAlchemy_CheckedChanged(object sender, EventArgs e)
		{
			tbAlchemyKey.Enabled = cbAlchemy.Checked;
			InitMarker("Alchemy", cbAlchemy.Checked);
		}


        private void cbRepustate_CheckedChanged(object sender, EventArgs e)
        {
            tbRepustateKey.Enabled = cbRepustate.Checked;
            InitMarker("Repustate", cbRepustate.Checked);
        }

		private void cbChatterbox_CheckedChanged(object sender, EventArgs e)
		{
			tbChatterboxKey.Enabled = cbChatterbox.Checked;
			InitMarker("Chatterbox", cbChatterbox.Checked);
		}

		private void cbViralheat_CheckedChanged(object sender, EventArgs e)
		{
			tbViralheatKey.Enabled = cbViralheat.Checked;
			InitMarker("Viralheat", cbViralheat.Checked);
		}

		private void cbBitext_CheckedChanged(object sender, EventArgs e)
		{
			tbBitextLogin.Enabled = cbBitext.Checked;
			tbBitextPassword.Enabled = cbBitext.Checked;
			InitMarker("Bitext", cbBitext.Checked);
		}

        private void cbSkyttle_CheckedChanged(object sender, EventArgs e)
        {
            tbSkyttleKey.Enabled = cbSkyttle.Checked;
            InitMarker("Skyttle", cbSkyttle.Checked);
        }

		private void llSemantria_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			llSemantria.LinkVisited = true;
			System.Diagnostics.Process.Start("http://semantria.com");
		}

		private void llAlchemy_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			llAlchemy.LinkVisited = true;
			System.Diagnostics.Process.Start("http://alchemyapi.com");
		}

		private void llChatterbox_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			llChatterbox.LinkVisited = true;
			System.Diagnostics.Process.Start("http://chatterbox.co");
		}
        
        private void llRepustate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            llRepustate.LinkVisited = true;
            System.Diagnostics.Process.Start("http://www.repustate.com");
        }

		private void llViralheat_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			llViralheat.LinkVisited = true;
			System.Diagnostics.Process.Start("https://www.viralheat.com");
		}

		private void llBitext_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			llBitext.LinkVisited = true;
			System.Diagnostics.Process.Start("http://www.bitext.com");
		}

        private void llSkyttle_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            llSkyttle.LinkVisited = true;
            System.Diagnostics.Process.Start("http://www.skyttle.com");
        }

		private void btClose_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void btAbout_Click(object sender, EventArgs e)
		{
			About about = new About();
			about.ShowDialog(this);
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
				UpdateMarker(service, true);
				ea.Cancel = true;

				if (IsAnalysysDone())
				{
					SetControlPropertyThreadSafe(btClose, "Enabled", true);
					SetControlPropertyThreadSafe(btProcess, "Text", "Process");
					SetControlPropertyThreadSafe(pbAlchemy, "Value", 0);
                    SetControlPropertyThreadSafe(pbRepustate, "Value", 0);
					SetControlPropertyThreadSafe(pbChatterbox, "Value", 0);
					SetControlPropertyThreadSafe(pbSemantria, "Value", 0);
					SetControlPropertyThreadSafe(pbViralheat, "Value", 0);
					SetControlPropertyThreadSafe(pbBitext, "Value", 0);
                    SetControlPropertyThreadSafe(pbSkyttle, "Value", 0);
				}

				return;
			}

			if (ea.Status == AnalysisExecutionStatus.Processed || ea.Status == AnalysisExecutionStatus.Failed)
			{
				WriteDebugInfo("Got the {0} progress event from {1}. Total documents: {2}\tProcessed: {3}\tFailed:{4}", ea.Progress, service, ea.TotalDocuments, ea.ProcessedDocuments, ea.FailedDocuments);
				switch (service)
				{
					case "Semantria":
						SetControlPropertyThreadSafe(pbSemantria, "Value", ea.Progress);
						break;

					case "Alchemy":
						SetControlPropertyThreadSafe(pbAlchemy, "Value", ea.Progress);
						break;

                    case "Repustate":
                        SetControlPropertyThreadSafe(pbRepustate, "Value", ea.Progress);
                        break;

					case "Chatterbox":
						SetControlPropertyThreadSafe(pbChatterbox, "Value", ea.Progress);
						break;

					case "Viralheat":
						SetControlPropertyThreadSafe(pbViralheat, "Value", ea.Progress);
						break;

					case "Bitext":
						SetControlPropertyThreadSafe(pbBitext, "Value", ea.Progress);
						break;
                    
                    case "Skyttle":
                        SetControlPropertyThreadSafe(pbSkyttle, "Value", ea.Progress);
                        break;
				}
			}

			if (ea.Status == AnalysisExecutionStatus.Success)
			{
				WriteDebugInfo("Analysis on {0} service has been finished. Total documents: {1}\tProcessed: {2}\tFailed:{3}", service, ea.TotalDocuments, ea.ProcessedDocuments, ea.FailedDocuments);
				UpdateMarker(service, (ea.Status == AnalysisExecutionStatus.Success) ? true : false);

				if (IsAnalysysDone())
				{
					WriteDebugInfo("All services have finished analysis! Writing the output...");
					string outputFile = GetControlPropertyThreadSafe(tbOutput, "Text") as string;

					try
					{
                        using (CsvFileWriter writter = new CsvFileWriter(outputFile))
                        {
                            List<string> columns = new List<string>();
                            columns.Add("Document ID");

                            IList<string> services = _documents.First().Value.GetServices();
                            foreach (string item in services)
                            {
                                if (item == "Bitext")
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
					SetControlPropertyThreadSafe(pbAlchemy, "Value", 0);
                    SetControlPropertyThreadSafe(pbRepustate, "Value", 0);
					SetControlPropertyThreadSafe(pbChatterbox, "Value", 0);
					SetControlPropertyThreadSafe(pbSemantria, "Value", 0);
					SetControlPropertyThreadSafe(pbViralheat, "Value", 0);
					SetControlPropertyThreadSafe(pbBitext, "Value", 0);
                    SetControlPropertyThreadSafe(pbSkyttle, "Value", 0);

					MessageBox.Show("Analysis is done! Check the output file for results.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
				}
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
				ReadTheSource(tbSource.Text, int.Parse(tbDocSize.Text));
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			btProcess.Text = "Cancel";
			btClose.Enabled = false;
			_cancelFlag = false;

			if (cbSemantria.Checked)
			{
				WriteDebugInfo("Semantria service is checked. Preparing the context...");

				AnalysisExecutionContext semContext = new AnalysisExecutionContext(_documents);
				semContext.ExecutionProgress += ExecutionProgress;
				semContext.Key = tbSemantriaKey.Text;
				semContext.Secret = tbSemantriaSecret.Text;
				semContext.Language = cbLanguage.Text;
				semContext.Format = DataFormat.JSON;
				semContext.DocumentLength = int.Parse(tbDocSize.Text);
				if (UseDebugMode && Benchmark.Contains("Semantria"))
					semContext.UseDebugMode = true;
				SemantriaExecutor semExecutor = new SemantriaExecutor();

				ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object obj)
				{
					WriteDebugInfo("Running the dedicated thread for Semantria context serving...");

                    TimeSpan time = TimeSpan.Zero;
                    BenchmarkHelper.Invoke(new InvokeBenchmarkHandler(delegate(object state)
                    {
                        semExecutor.Execute(semContext);
                        return null;
                    }), null, out time);

                    WriteDebugInfo("Semantria thread execution time: {0} milliseconds.", time.TotalMilliseconds);
				}), null);
			}

			if (cbAlchemy.Checked)
			{
				WriteDebugInfo("Alchemy service is checked. Preparing the context...");

				AnalysisExecutionContext alcContext = new AnalysisExecutionContext(_documents);
				alcContext.ExecutionProgress += ExecutionProgress;
				alcContext.Key = tbAlchemyKey.Text;
				if (UseDebugMode && Benchmark.Contains("Alchemy"))
					alcContext.UseDebugMode = true;
				AlchemyExecutor alcExecutor = new AlchemyExecutor();

				ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object obj)
				{
					WriteDebugInfo("Running the dedicated thread for Alchemy context serving...");

                    TimeSpan time = TimeSpan.Zero;
                    BenchmarkHelper.Invoke(new InvokeBenchmarkHandler(delegate(object state)
                    {
                        alcExecutor.Execute(alcContext);
                        return null;
                    }), null, out time);

                    WriteDebugInfo("Alchemy thread execution time: {0} milliseconds.", time.TotalMilliseconds);
				}), null);
			}

            if (cbRepustate.Checked)
            {
                WriteDebugInfo("Repustate service is checked. Preparing the context...");

                AnalysisExecutionContext rsContext = new AnalysisExecutionContext(_documents);
                rsContext.ExecutionProgress += ExecutionProgress;
                rsContext.Key = tbRepustateKey.Text;
                rsContext.Language = cbLanguage.Text;
                if (UseDebugMode && Benchmark.Contains("Repustate"))
                    rsContext.UseDebugMode = true;
                RepustateExecutor rsExecutor = new RepustateExecutor();
                
                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object obj)
                {
                    WriteDebugInfo("Running the dedicated thread for Repustate context serving...");

                    TimeSpan time = TimeSpan.Zero;
                    BenchmarkHelper.Invoke(new InvokeBenchmarkHandler(delegate(object state)
                    {
                        rsExecutor.Execute(rsContext);
                        return null;
                    }), null, out time);

                    WriteDebugInfo("Repustate thread execution time: {0} milliseconds.", time.TotalMilliseconds);
                }), null);
            }

			if (cbChatterbox.Checked)
			{
				WriteDebugInfo("Chatterbox service is checked. Preparing the context...");

				AnalysisExecutionContext cbContext = new AnalysisExecutionContext(_documents);
				cbContext.ExecutionProgress += ExecutionProgress;
				cbContext.Key = tbChatterboxKey.Text;
				cbContext.Language = cbLanguage.Text;
				cbContext.DocumentLength = int.Parse(tbDocSize.Text);
				if (UseDebugMode && Benchmark.Contains("Chatterbox"))
					cbContext.UseDebugMode = true;
				ChatterboxExecutor cbExecutor = new ChatterboxExecutor();

				ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object obj)
				{
					WriteDebugInfo("Running the dedicated thread for Chatterbox context serving...");

                    TimeSpan time = TimeSpan.Zero;
                    BenchmarkHelper.Invoke(new InvokeBenchmarkHandler(delegate(object state)
                    {
                        cbExecutor.Execute(cbContext);
                        return null;
                    }), null, out time);

                    WriteDebugInfo("Chatterbox thread execution time: {0} milliseconds.", time.TotalMilliseconds);
				}), null);
			}

			if (cbViralheat.Checked)
			{
				WriteDebugInfo("Viralheat service is checked. Preparing the context...");

				AnalysisExecutionContext vhContext = new AnalysisExecutionContext(_documents);
				vhContext.ExecutionProgress += ExecutionProgress;
				vhContext.Key = tbViralheatKey.Text;
				vhContext.Language = cbLanguage.Text;
				vhContext.DocumentLength = int.Parse(tbDocSize.Text);
				if (UseDebugMode && Benchmark.Contains("Viralheat"))
					vhContext.UseDebugMode = true;
				ViralheatExecutor vhExecutor = new ViralheatExecutor();

				ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object obj)
				{
					WriteDebugInfo("Running the dedicated thread for Viralheat context serving...");

                    TimeSpan time = TimeSpan.Zero;
                    BenchmarkHelper.Invoke(new InvokeBenchmarkHandler(delegate(object state)
                    {
                        vhExecutor.Execute(vhContext);
                        return null;
                    }), null, out time);

                    WriteDebugInfo("Viralheat thread execution time: {0} milliseconds.", time.TotalMilliseconds);
				}), null);
			}

			if (cbBitext.Checked)
			{
				WriteDebugInfo("Bitext service is checked. Preparing the context...");

				AnalysisExecutionContext btContext = new AnalysisExecutionContext(_documents);
				btContext.ExecutionProgress += ExecutionProgress;
				btContext.Key = tbBitextLogin.Text;
				btContext.Secret = tbBitextPassword.Text;
				btContext.Language = cbLanguage.Text;
				btContext.Format = DataFormat.XML;
				btContext.DocumentLength = int.Parse(tbDocSize.Text);
				if (UseDebugMode && Benchmark.Contains("Bitext"))
					btContext.UseDebugMode = true;
				BitextExecutor btExecutor = new BitextExecutor();

				ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object obj)
				{
					WriteDebugInfo("Running the dedicated thread for Bitext context serving...");

                    TimeSpan time = TimeSpan.Zero;
                    BenchmarkHelper.Invoke(new InvokeBenchmarkHandler(delegate(object state)
                    {
                        btExecutor.Execute(btContext);
                        return null;
                    }), null, out time);

                    WriteDebugInfo("Bitext thread execution time: {0} milliseconds.", time.TotalMilliseconds);
				}), null);
			}

            if (cbSkyttle.Checked)
            {
                WriteDebugInfo("Skyttle service is checked. Preparing the context...");

                AnalysisExecutionContext skContext = new AnalysisExecutionContext(_documents);
                skContext.ExecutionProgress += ExecutionProgress;
                skContext.Key = tbSkyttleKey.Text;
                skContext.Language = cbLanguage.Text;
                if (UseDebugMode && Benchmark.Contains("Skyttle"))
                    skContext.UseDebugMode = true;
                SkyttleExecutor skExecutor = new SkyttleExecutor();

                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate(object obj)
                {
                    WriteDebugInfo("Running the dedicated thread for Skyttle context serving...");

                    TimeSpan time = TimeSpan.Zero;
                    BenchmarkHelper.Invoke(new InvokeBenchmarkHandler(delegate(object state)
                    {
                        skExecutor.Execute(skContext);
                        return null;
                    }), null, out time);

                    WriteDebugInfo("Skyttle thread execution time: {0} milliseconds.", time.TotalMilliseconds);
                }), null);
            }
		}

		#endregion

        #region Language based enable/disable services

        private void cbLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (new SemantriaExecutor().IsLanguageSupported(cbLanguage.Text))
            {
                gpSemantria.Enabled = true;
                if (tbSemantriaKey.Text != "" || tbSemantriaSecret.Text != "")
                    cbSemantria.Checked = true;

                cbSemantria.Enabled = true;
            }
            else
            {
                gpSemantria.Enabled = false;
                cbSemantria.Checked = false;
                cbSemantria.Enabled = false;
                tbSemantriaKey.Enabled = false;
                tbSemantriaSecret.Enabled = false;           
            }

            if (new AlchemyExecutor().IsLanguageSupported(cbLanguage.Text))
            {
                gpAlchemy.Enabled = true;
                if (tbAlchemyKey.Text != "")
                    cbAlchemy.Checked = true;

                cbAlchemy.Enabled = true;
            }
            else
            {
                gpAlchemy.Enabled = false;
                cbAlchemy.Checked = false;
                cbAlchemy.Enabled = false;
                tbAlchemyKey.Enabled = false;
            }

            if (new RepustateExecutor().IsLanguageSupported(cbLanguage.Text))
            {
                gpRepustate.Enabled = true;
                if (tbRepustateKey.Text != "")
                    cbRepustate.Checked = true;

                cbRepustate.Enabled = true;
            }
            else
            {
                gpRepustate.Enabled = false;
                cbRepustate.Checked = false;
                cbRepustate.Enabled = false;
                tbRepustateKey.Enabled = false;
            }
            
            if (new ChatterboxExecutor().IsLanguageSupported(cbLanguage.Text))
            {
                if (tbChatterboxKey.Text != "")
                    cbChatterbox.Checked = true;

                cbChatterbox.Enabled = true;
            }
            else
            {
                cbChatterbox.Checked = false;
                cbChatterbox.Enabled = false;
                tbChatterboxKey.Enabled = false;
            }

            if (new ViralheatExecutor().IsLanguageSupported(cbLanguage.Text))
            {
                gbViralheat.Enabled = true;
                if (tbViralheatKey.Text != "")
                    cbViralheat.Checked = true;

                cbViralheat.Enabled = true;
            }
            else
            {
                gbViralheat.Enabled = false;
                cbViralheat.Checked = false;
                cbViralheat.Enabled = false;
                tbViralheatKey.Enabled = false;
            }

            if (new BitextExecutor().IsLanguageSupported(cbLanguage.Text))
            {
                gbBitext.Enabled = true;
                if (tbBitextLogin.Text != "" || tbBitextPassword.Text != "")
                    cbBitext.Checked = true;

                cbBitext.Enabled = true;
            }
            else
            {
                gbBitext.Enabled = false;
                cbBitext.Checked = false;
                cbBitext.Enabled = false;
                tbBitextLogin.Enabled = false;
                tbBitextPassword.Enabled = false;
            }

            if (new SkyttleExecutor().IsLanguageSupported(cbLanguage.Text))
            {
                gbSkyttle.Enabled = true;
                if (tbSkyttleKey.Text != "")
                    cbSkyttle.Checked = true;

                cbSkyttle.Enabled = true;
            }
            else
            {
                gbSkyttle.Enabled = false;
                cbSkyttle.Checked = false;
                cbSkyttle.Enabled = false;
                tbSkyttleKey.Enabled = false;
            }

        }

        #endregion

	}
}
