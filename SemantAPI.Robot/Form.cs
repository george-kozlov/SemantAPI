// <copyright file="Form.cs" company="13th Parish">
// Copyright (c) 13th Parish 2013 All Rights Reserved
// </copyright>
// <author>George Kozlov (george.kozlov@outlook.com)</author>
// <date>03/30/2013</date>
// <summary>Main SemantAPIRobot form class</summary>

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
				pbChatterbox.Value = 0;
				pbSemantria.Value = 0;
				pbViralheat.Value = 0;
				pbBitext.Value = 0;

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
				using (StreamReader stream = new StreamReader(File.Open(tbSource.Text, FileMode.OpenOrCreate, FileAccess.Read)))
				{
					while (!stream.EndOfStream)
					{
						if (_cancelFlag)
							break;

						string line = stream.ReadLine();
						if (line.Trim().Length <= 1)
							continue;

						string text = string.Empty;
						if (isCSV)
						{
							string[] columns = line.Split(',');
							text = columns.First();
						}
						else
						{
							int cutLimit = (line.Length < cutBy) ? line.Length - 1 : cutBy;
							text = line.Substring(0, cutLimit);
						}
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
			dialog.Filter = "Text files (*.txt)|*.txt|CSV files (*.csv)|*.csv";

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
					SetControlPropertyThreadSafe(pbChatterbox, "Value", 0);
					SetControlPropertyThreadSafe(pbSemantria, "Value", 0);
					SetControlPropertyThreadSafe(pbViralheat, "Value", 0);
					SetControlPropertyThreadSafe(pbBitext, "Value", 0);
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

					case "Chatterbox":
						SetControlPropertyThreadSafe(pbChatterbox, "Value", ea.Progress);
						break;

					case "Viralheat":
						SetControlPropertyThreadSafe(pbViralheat, "Value", ea.Progress);
						break;

					case "Bitext":
						SetControlPropertyThreadSafe(pbBitext, "Value", ea.Progress);
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
						using (StreamWriter stream = File.CreateText(outputFile))
						{
							StringBuilder builder = new StringBuilder();
							builder.Append("Document ID,");

							IList<string> services = _documents.First().Value.GetServices();
							foreach (string item in services)
							{
								//A trick for the column name. We're calculating an average  score for Bitext, so the name has been adjusted accordingly.
								if (item == "Bitext")
									builder.AppendFormat("{0} Polarity,{0} Average Sentiment Score,", item);
								else if (item == "Viralheat")
									builder.AppendFormat("{0} Polarity,{0} Probability Score,", item);
								else
									builder.AppendFormat("{0} Polarity,{0} Sentiment Score,", item);
							}

							builder.Append("Source text");
							stream.WriteLine(builder.ToString());

							foreach (KeyValuePair<string, ResultSet> data in _documents)
								stream.WriteLine(string.Format("{0},{1}", data.Key, data.Value.ToString()));
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
					SetControlPropertyThreadSafe(pbChatterbox, "Value", 0);
					SetControlPropertyThreadSafe(pbSemantria, "Value", 0);
					SetControlPropertyThreadSafe(pbViralheat, "Value", 0);
					SetControlPropertyThreadSafe(pbBitext, "Value", 0);

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
					semExecutor.Execute(semContext);
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
					alcExecutor.Execute(alcContext);
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
					cbExecutor.Execute(cbContext);
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
					vhExecutor.Execute(vhContext);
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
					btExecutor.Execute(btContext);
				}), null);
			}
		}

		#endregion
	}
}
