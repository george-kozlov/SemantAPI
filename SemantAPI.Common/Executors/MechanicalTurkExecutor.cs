// <copyright file="MechanicalTurkExecutor.cs" company="13th Parish">
// Copyright (c) 13th Parish 2013 All Rights Reserved
// </copyright>
// <author>George Kozlov (george.kozlov@outlook.com)</author>
// <date>03/30/2013</date>
// <summary>MechanicalTurkSettings and MechanicalTurkExecutor classes</summary>

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;

using Amazon.WebServices.MechanicalTurk;
using Amazon.WebServices.MechanicalTurk.Domain;

namespace SemantAPI.Common.Executors
{
	#region MechanicalTurkSettings class

	public sealed class MechanicalTurkSettings
	{
		public MechanicalTurkSettings(string email, int assignments, decimal reward)
		{
			Email = email;
			Assignments = assignments;
			Reward = reward;
			Locale = null;
		}

		public MechanicalTurkSettings(string email, int assignments, decimal reward, int timeToFinish, int timeToApprove, int percentOfSuccess) :
			this (email, assignments, reward)
		{
			TimeToFinish = timeToFinish;
			TimeToApprove = timeToApprove;
			PercentOfSuccess = percentOfSuccess;
		}

		public string Email
		{
			get;
			set;
		}

		public decimal Reward
		{
			get;
			set;
		}

		public int Assignments
		{
			get;
			set;
		}

		public int TimeToFinish
		{
			get;
			set;
		}

		public int TimeToApprove
		{
			get;
			set;
		}

		public string Locale
		{
			get;
			set;
		}

		public int PercentOfSuccess
		{
			get;
			set;
		}
	}

	#endregion

	public sealed class MechanicalTurkExecutor : IExecutor 
	{
		#region Private members

		AnalysisExecutionContext _context = null;
		string _serviceURL = "https://mechanicalturk.amazonaws.com/?Service=AWSMechanicalTurkRequester";
		//string _serviceURL = "https://mechanicalturk.sandbox.amazonaws.com?Service=AWSMechanicalTurkRequester";

		#endregion

		#region Constructor

		public MechanicalTurkExecutor()
		{
		}

		#endregion

		#region Private methods

		private IList<string> DeserializeAnswers(IList<Assignment> assignments)
		{
			IList<string> result = new List<string>();

			if (assignments.Count == 0)
				return result;

			foreach (Assignment assmnt in assignments)
			{
				if (assmnt.AssignmentStatus == AssignmentStatus.Rejected)
					continue;

				QuestionFormAnswers answer = QuestionUtil.DeserializeQuestionFormAnswers(assmnt.Answer);
				if (answer != null)
				{
					if (answer.Answer != null && answer.Answer.Count() > 0)
					{
						QuestionFormAnswersAnswer eachAnswer = answer.Answer.First();
						if (eachAnswer.Items != null && eachAnswer.Items.Count() > 0)
							result.Add(eachAnswer.Items.First());
						else
							result.Add("undefined");
					}
				}
			}

			return result;
		}

		private string MergeSentimentPolarity(IList<string> answers, out double confidence)
		{
			Dictionary<string, int> result = new Dictionary<string,int>(3);

			foreach (string answ in answers)
			{
				if (result.ContainsKey(answ))
					result[answ]++;
				else
					result.Add(answ, 1);
			}

			int max = result.Max(item => item.Value);
			confidence = ((double)max / answers.Count);
			if (max == 1)
				return "undefined";

			return result.Single(item => item.Value == max).Key;
		}

		#endregion

		#region Public methods and properties

		public void Execute(AnalysisExecutionContext context)
		{
			_context = context;

			if (context.Results.Count <= 0)
			{
				context.OnExecutionProgress("MechanicalTurk", new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Canceled, 0, 0, 0));
				return;
			}

			MechanicalTurkSettings settings = context.CustomField as MechanicalTurkSettings;
			if (settings == null)
				settings = new MechanicalTurkSettings(null, 3, new decimal(0.10), 300, 1800, 75);

			string key = context.Key;
			string secret = context.Secret;

			MTurkConfig config = new MTurkConfig(_serviceURL, key, secret);
			SimpleClient client = new SimpleClient(config);

			List<QualificationRequirement> requirements = new List<QualificationRequirement>();
			QualificationRequirement sucRequirement = new QualificationRequirement();
			sucRequirement.QualificationTypeId = MTurkSystemQualificationTypes.ApprovalRateQualification;
			sucRequirement.IntegerValueSpecified = true;
			sucRequirement.IntegerValue = settings.PercentOfSuccess;
			sucRequirement.Comparator = Comparator.GreaterThanOrEqualTo;
			requirements.Add(sucRequirement);

			if (settings.Locale != null)
			{
				QualificationRequirement qualReq = new QualificationRequirement();
				qualReq.LocaleValue = new Locale() { Country = settings.Locale };
				qualReq.Comparator = Comparator.EqualTo;
				qualReq.QualificationTypeId = MTurkSystemQualificationTypes.LocaleQualification;
				requirements.Add(qualReq);
			}

			string hitType = string.Empty;
			try
			{
				if (context.UseDebugMode)
				{
					TimeSpan time = TimeSpan.Zero;
					hitType = BenchmarkHelper.Invoke(new InvokeBenchmarkHandler(delegate(object state)
					{
						return hitType = client.RegisterHITType("Sentiment analysis", "Judge the sentiment expressed by the following text.",
							settings.TimeToApprove, settings.TimeToFinish, settings.Reward, "sentiment nlp", requirements);
					}), null, out time) as string;
					Console.WriteLine("MechanicalTurk: HIT type for sentiment analysis has been created. HIT type ID is: {0} Execution time is: {1}", hitType, time.TotalMilliseconds);
				}
				else
					hitType = client.RegisterHITType("Sentiment analysis", "Judge the sentiment expressed by the following text.",
						settings.TimeToApprove, settings.TimeToFinish, settings.Reward, "sentiment, nlp", requirements);

				NotificationSpecification notification = new NotificationSpecification();
				notification.Transport = NotificationTransport.Email;
				notification.EventType = new EventType[] { EventType.AssignmentReturned };
				notification.Destination = settings.Email;
				notification.Version = "2006-05-05";

				if (settings.Email != null)
					client.SetHITTypeNotification(hitType, notification, true);
				else
				{
					notification.Destination = "no-reply@sample.com";
					client.SetHITTypeNotification(hitType, notification, false);
				}
			}
			catch (Exception ex)
			{
				AnalysisExecutionProgressEventArgs ea = new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Failed, context.Results.Count, 0, 0);
				ea.Reason = ex.Message;
				context.OnExecutionProgress("MechanicalTurk", ea);

				if (ea.Cancel)
					return;
			}

			string questionFile = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "SentimentTemplate.xml");
			string template = File.ReadAllText(questionFile);
			QuestionForm formTemplate = QuestionUtil.DeserializeQuestionForm(template);

			int processed = 0;
			int failed = 0;
			foreach (KeyValuePair<string, ResultSet> document in context.Results)
			{
				formTemplate.Question[0].QuestionIdentifier = document.Key;
				formTemplate.Question[0].QuestionContent.Items[0] = Encoding.UTF8.GetString(Encoding.Default.GetBytes(document.Value.Source));
				string question = QuestionUtil.SerializeQuestionForm(formTemplate);

				HIT hit = new HIT();
				hit.Expiration = DateTime.Now.AddDays(1);
				hit.ExpirationSpecified = true;
				hit.HITGroupId = "SentimentAnalysis";
				hit.HITTypeId = hitType;
				hit.MaxAssignments = settings.Assignments;
				hit.MaxAssignmentsSpecified = true;
				hit.Question = question;

				HIT serverHit = null;
				try
				{
					processed++;
					if (context.UseDebugMode)
					{
						TimeSpan time = TimeSpan.Zero;
						serverHit = BenchmarkHelper.Invoke(new InvokeBenchmarkHandler(delegate(object state)
						{
							return client.CreateHIT(hit);
						}), null, out time) as HIT;
						Console.WriteLine("MechanicalTurk: HIT {0} has been sent to Mechanical turk for processing. Execution time is: {1}", serverHit.HITId, time.TotalMilliseconds);
					}
					else
						serverHit = client.CreateHIT(hit);

					document.Value.AddOutput("MechanicalTurk", settings.Assignments, serverHit.HITId);
					AnalysisExecutionProgressEventArgs e = new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Processed, context.Results.Count, processed, failed);
					context.OnExecutionProgress("MechanicalTurk", e);

					if (e.Cancel)
						break;
				}
				catch (Exception ex)
				{
					failed++;
					document.Value.AddOutput("MechanicalTurk", 0, "failed");
					AnalysisExecutionProgressEventArgs ea = new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Failed, context.Results.Count, processed, failed);
					ea.Reason = ex.Message;
					context.OnExecutionProgress("MechanicalTurk", ea);

					if (ea.Cancel)
						break;
				}
			}

			context.OnExecutionProgress("MechanicalTurk", new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Success, context.Results.Count, processed, failed));
		}

		public void Request(AnalysisExecutionContext context)
		{
			_context = context;

			if (context.Results.Count <= 0)
			{
				context.OnExecutionProgress("MechanicalTurk", new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Canceled, 0, 0, 0));
				return;
			}

			if (!context.Results.Values.First().GetServices().Contains("MechanicalTurk"))
			{
				context.OnExecutionProgress("MechanicalTurk", new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Canceled, 0, 0, 0));
				return;
			}

			MechanicalTurkSettings settings = context.CustomField as MechanicalTurkSettings;
			if (settings == null)
				settings = new MechanicalTurkSettings(null, 3, new decimal(0.10), 300, 1800, 75);

			string key = context.Key;
			string secret = context.Secret;

			MTurkConfig config = new MTurkConfig(_serviceURL, key, secret);
			SimpleClient client = new SimpleClient(config);

			int failed = 0;
			int processed = 0;
			try
			{
				foreach (KeyValuePair<string, ResultSet> document in context.Results)
				{
					string id = document.Value.GetPolarity("MechanicalTurk");
					double count = document.Value.GetScore("MechanicalTurk");
					count = (double.IsNaN(count)) ? 0 : count;
					if (id == "negative" || id == "neutral" || id == "positive" || id == "failed" || id == "undefined")
						continue;

					IList<Assignment> assignments = null;
					if (context.UseDebugMode)
					{
						TimeSpan time = TimeSpan.Zero;
						assignments = BenchmarkHelper.Invoke(new InvokeBenchmarkHandler(delegate(object state)
						{
							return client.GetAssignmentsForHIT(id, 1, true);
						}), null, out time) as IList<Assignment>;
						Console.WriteLine("MechanicalTurk: Answers for {0} HIT have been received. Execution time is: {1}", id, time.TotalMilliseconds);
					}
					else
						assignments = client.GetAssignmentsForHIT(id, 1, true);

					if (assignments.Count < count)
					{
						processed++;
						continue;
					}

					double confidence = 0;
					IList<string> answers = DeserializeAnswers(assignments);
					string polarity = MergeSentimentPolarity(answers, out confidence);

					processed++;
					document.Value.AddOutput("MechanicalTurk", double.NaN, polarity, confidence);
					AnalysisExecutionProgressEventArgs e = new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Processed, context.Results.Count, processed, failed);
					context.OnExecutionProgress("MechanicalTurk", e);

					if (e.Cancel)
						break;
				}
			}
			catch (Exception ex)
			{
				failed++;
				AnalysisExecutionProgressEventArgs ea = new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Failed, context.Results.Count, processed, failed);
				ea.Reason = ex.Message;
				context.OnExecutionProgress("MechanicalTurk", ea);

				if (ea.Cancel)
					return;
			}

			context.OnExecutionProgress("MechanicalTurk", new AnalysisExecutionProgressEventArgs(AnalysisExecutionStatus.Success, context.Results.Count, processed, failed));
		}

		public AnalysisExecutionContext Context
		{
			get { return _context; }
		}

		#endregion
	}
}
