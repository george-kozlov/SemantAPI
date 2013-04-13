#region Copyright & license notice
/*
 * Copyright: Copyright (c) 2007 Amazon Technologies, Inc.
 * License:   Apache License, Version 2.0
 */
#endregion

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Diagnostics;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using Amazon.WebServices.MechanicalTurk.Domain;
using Amazon.WebServices.MechanicalTurk.Advanced;


namespace Amazon.WebServices.MechanicalTurk
{
    /// <summary>
    /// Client used to invoke single operations for Mechanical Turk
    /// </summary>
    /// <remarks>
    /// This client is a wrapper around the core Mechanical Turk client (<see cref="MTurkClient"/>)
    /// to facilitate rapid development of MTurk application through convenience methods.
    /// </remarks>
    public class SimpleClient : ITurkConfig
    {
        #region Constants
        // Note: assignment status and response groups remain type arrays rather than IList<Type>, since these are fairly static and we
        // want to avoid unnecessary object instantiations when passing these on to the WSDL layer
        private static AssignmentStatus[] DefaultAssignmentStatus = new AssignmentStatus[] { AssignmentStatus.Approved, AssignmentStatus.Rejected, AssignmentStatus.Submitted };
        private static long DefaultAssignmentDurationInSeconds = (long)60 * 60; // 1 hour
        private static long DefaultAutoApprovalDelayInSeconds = (long)60 * 60 * 24 * 15; // 15 days
        private static long DefaultLifeTimeInSeconds = (long)60 * 60 * 24 * 3; // 3 days
        #endregion

        #region Properties
        private ITurkOperations _proxy;
        /// <summary>
        /// The client proxy used to send the messages
        /// </summary>
        public ITurkOperations Proxy
        {
            get { return this._proxy; }
            set { this._proxy = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a client using the configuration from the application configuration file (see <see cref="MTurkConfig"/>)
        /// </summary>
        public SimpleClient()
        {
            Proxy = new MTurkClient();
        }

        /// <summary>
        /// Initializes a client using a custom client configuration
        /// </summary>
        /// <remarks>Custom configurations can be useful e.g. when the AWS credentials are retrieved from a different storage
        /// mechanism than the application configuration file</remarks>
        public SimpleClient(MTurkConfig config)
        {
            Proxy = new MTurkClient(config);
        }
        #endregion

        #region Convenience methods
        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_CreateHITOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="hitTypeId">The hit type id.</param>
        /// <param name="title">The title.</param>
        /// <param name="description">The description.</param>
        /// <param name="keywords">The keywords.</param>
        /// <param name="question">The question. If the question is not wrapped as XML, it will be wrapped as a simple free text question (<see cref="QuestionUtil"/>)</param>
        /// <param name="reward">The reward.</param>
        /// <param name="assignmentDurationInSeconds">The assignment duration in seconds.</param>
        /// <param name="autoApprovalDelayInSeconds">The auto approval delay in seconds.</param>
        /// <param name="lifetimeInSeconds">The lifetime in seconds. If 0, defaults to 3 days.</param>
        /// <param name="maxAssignments">The max assignments.</param>
        /// <param name="requesterAnnotation">The requester annotation.</param>
        /// <param name="qualificationRequirements">The qualification requirements.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns>A <see cref="HIT"/> instance</returns>
        public HIT CreateHIT(string hitTypeId, string title, string description, string keywords, string question,
            decimal? reward, long? assignmentDurationInSeconds, long? autoApprovalDelayInSeconds, long lifetimeInSeconds,
            int? maxAssignments, string requesterAnnotation, List<QualificationRequirement> qualificationRequirements,
            string[] responseGroup)
        {
            CreateHITRequest req = new CreateHITRequest();
            req.Question = question;
            req.LifetimeInSeconds = (lifetimeInSeconds <= 30) ? DefaultLifeTimeInSeconds : lifetimeInSeconds;
            req.HITTypeId = hitTypeId;
            req.Title = title;
            req.Keywords = keywords;
            req.Description = description;            
            req.ResponseGroup = responseGroup;
            req.RequesterAnnotation = requesterAnnotation;

            if (maxAssignments.HasValue)
            {
                req.MaxAssignments = maxAssignments.Value;
                req.MaxAssignmentsSpecified = true;
            }
            if (assignmentDurationInSeconds.HasValue)
            {
                req.AssignmentDurationInSeconds = assignmentDurationInSeconds.Value;
                req.AssignmentDurationInSecondsSpecified = true;
            }
            if (autoApprovalDelayInSeconds.HasValue)
            {
                req.AutoApprovalDelayInSeconds = autoApprovalDelayInSeconds.Value;
                req.AutoApprovalDelayInSecondsSpecified = true;
            }

            if (qualificationRequirements != null)
            {
                req.QualificationRequirement = qualificationRequirements.ToArray();
            }


            if (reward.HasValue)
            {
                Price p = new Price();
                p.Amount = reward.Value;
                p.CurrencyCode = "USD";
                req.Reward = p;
            }

            return Proxy.CreateHIT(req);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_CreateHITOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="hitTypeId">The hit type id.</param>
        /// <param name="title">The title.</param>
        /// <param name="description">The description.</param>
        /// <param name="keywords">The keywords.</param>
        /// <param name="question">The question.</param>
        /// <param name="reward">The reward.</param>
        /// <param name="assignmentDurationInSeconds">The assignment duration in seconds.</param>
        /// <param name="autoApprovalDelayInSeconds">The auto approval delay in seconds.</param>
        /// <param name="lifetimeInSeconds">The lifetime in seconds. If 0, defaults to 3 days.</param>
        /// <param name="maxAssignments">The max assignments.</param>
        /// <param name="requesterAnnotation">The requester annotation.</param>
        /// <param name="qualificationRequirements">The qualification requirements.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns>A <see cref="HIT"/> instance</returns>
        public HIT CreateHIT(string hitTypeId, string title, string description, string keywords, QuestionForm question,
            decimal? reward, long? assignmentDurationInSeconds, long? autoApprovalDelayInSeconds, long lifetimeInSeconds,
            int? maxAssignments, string requesterAnnotation, List<QualificationRequirement> qualificationRequirements,
            string[] responseGroup)
        {
            return CreateHIT(hitTypeId, title, description, keywords,
                QuestionUtil.SerializeQuestionForm(question),
                reward, assignmentDurationInSeconds, autoApprovalDelayInSeconds, lifetimeInSeconds,
                maxAssignments, requesterAnnotation, qualificationRequirements, responseGroup);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_CreateHITOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="hitTypeId">The hit type id.</param>
        /// <param name="title">The title.</param>
        /// <param name="description">The description.</param>
        /// <param name="keywords">The keywords.</param>
        /// <param name="externalQuestion">External question</param>
        /// <param name="reward">The reward.</param>
        /// <param name="assignmentDurationInSeconds">The assignment duration in seconds.</param>
        /// <param name="autoApprovalDelayInSeconds">The auto approval delay in seconds.</param>
        /// <param name="lifetimeInSeconds">The lifetime in seconds. If 0, defaults to 3 days.</param>
        /// <param name="maxAssignments">The max assignments.</param>
        /// <param name="requesterAnnotation">The requester annotation.</param>
        /// <param name="qualificationRequirements">The qualification requirements.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns>A <see cref="HIT"/> instance</returns>
        public HIT CreateHIT(string hitTypeId, string title, string description, string keywords, ExternalQuestion externalQuestion,
            decimal? reward, long? assignmentDurationInSeconds, long? autoApprovalDelayInSeconds, long lifetimeInSeconds,
            int? maxAssignments, string requesterAnnotation, List<QualificationRequirement> qualificationRequirements,
            string[] responseGroup)
        {
            return CreateHIT(hitTypeId, title, description, keywords,
                QuestionUtil.SerializeExternalQuestion(externalQuestion),
                reward, assignmentDurationInSeconds, autoApprovalDelayInSeconds, lifetimeInSeconds,
                maxAssignments, requesterAnnotation, qualificationRequirements, responseGroup);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_CreateHITOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="hitTypeId">The hit type id.</param>
        /// <param name="title">The title.</param>
        /// <param name="description">The description.</param>
        /// <param name="keywords">The keywords.</param>
        /// <param name="question">The HTMLQuestion.</param>
        /// <param name="reward">The reward.</param>
        /// <param name="assignmentDurationInSeconds">The assignment duration in seconds.</param>
        /// <param name="autoApprovalDelayInSeconds">The auto approval delay in seconds.</param>
        /// <param name="lifetimeInSeconds">The lifetime in seconds. If 0, defaults to 3 days.</param>
        /// <param name="maxAssignments">The max assignments.</param>
        /// <param name="requesterAnnotation">The requester annotation.</param>
        /// <param name="qualificationRequirements">The qualification requirements.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns>A <see cref="HIT"/> instance</returns>
        public HIT CreateHIT(string hitTypeId, string title, string description, string keywords, HTMLQuestion question,
            decimal? reward, long? assignmentDurationInSeconds, long? autoApprovalDelayInSeconds, long lifetimeInSeconds,
            int? maxAssignments, string requesterAnnotation, List<QualificationRequirement> qualificationRequirements,
            string[] responseGroup)
        {
            return CreateHIT(hitTypeId, title, description, keywords,
                QuestionUtil.SerializeHTMLQuestion(question),
                reward, assignmentDurationInSeconds, autoApprovalDelayInSeconds, lifetimeInSeconds,
                maxAssignments, requesterAnnotation, qualificationRequirements, responseGroup);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_CreateHITOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="hitTypeId">The hit type id.</param>
        /// <param name="title">The title.</param>
        /// <param name="description">The description.</param>
        /// <param name="keywords">The keywords.</param>
        /// <param name="hitLayoutId">HITLayoutId</param>
        /// <param name="hitLayoutParameters">HITLayoutParameters</param>
        /// <param name="reward">The reward.</param>
        /// <param name="assignmentDurationInSeconds">The assignment duration in seconds.</param>
        /// <param name="autoApprovalDelayInSeconds">The auto approval delay in seconds.</param>
        /// <param name="lifetimeInSeconds">The lifetime in seconds. If 0, defaults to 3 days.</param>
        /// <param name="maxAssignments">The max assignments.</param>
        /// <param name="requesterAnnotation">The requester annotation.</param>
        /// <param name="qualificationRequirements">The qualification requirements.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns>A <see cref="HIT"/> instance</returns>
        public HIT CreateHIT(string hitTypeId, string title, string description, string keywords, 
            string hitLayoutId, Dictionary<string, string> hitLayoutParameters,
            decimal? reward, long? assignmentDurationInSeconds, long? autoApprovalDelayInSeconds, long lifetimeInSeconds,
            int? maxAssignments, string requesterAnnotation, List<QualificationRequirement> qualificationRequirements,
            string[] responseGroup)
        {
            CreateHITRequest req = new CreateHITRequest();

            req.HITLayoutId = hitLayoutId;
            List<HITLayoutParameter> paramList = new List<HITLayoutParameter>();
            foreach (KeyValuePair<string,string> value in hitLayoutParameters)
            {
                HITLayoutParameter p = new HITLayoutParameter();
                p.Name = value.Key;
                p.Value = value.Value;
                paramList.Add(p);
            }
            req.HITLayoutParameter = paramList.ToArray();
            
            req.LifetimeInSeconds = (lifetimeInSeconds <= 30) ? DefaultLifeTimeInSeconds : lifetimeInSeconds;
            req.HITTypeId = hitTypeId;
            req.Title = title;
            req.Keywords = keywords;
            req.Description = description;
            req.ResponseGroup = responseGroup;
            req.RequesterAnnotation = requesterAnnotation;

            if (maxAssignments.HasValue)
            {
                req.MaxAssignments = maxAssignments.Value;
                req.MaxAssignmentsSpecified = true;
            }
            if (assignmentDurationInSeconds.HasValue)
            {
                req.AssignmentDurationInSeconds = assignmentDurationInSeconds.Value;
                req.AssignmentDurationInSecondsSpecified = true;
            }
            if (autoApprovalDelayInSeconds.HasValue)
            {
                req.AutoApprovalDelayInSeconds = autoApprovalDelayInSeconds.Value;
                req.AutoApprovalDelayInSecondsSpecified = true;
            }

            if (qualificationRequirements != null)
            {
                req.QualificationRequirement = qualificationRequirements.ToArray();
            }


            if (reward.HasValue)
            {
                Price p = new Price();
                p.Amount = reward.Value;
                p.CurrencyCode = "USD";
                req.Reward = p;
            }

            return Proxy.CreateHIT(req);
        }


        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_RegisterHITTypeOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="description">The description.</param>
        /// <param name="autoApprovalDelayInSeconds">The auto approval delay in seconds.</param>
        /// <param name="assignmentDurationInSeconds">The assignment duration in seconds.</param>
        /// <param name="reward">The reward.</param>
        /// <param name="keywords">The keywords.</param>
        /// <param name="qualificationRequirements">The qualification requirements.</param>
        /// <returns>ID of new HIT type</returns>
        public string RegisterHITType(string title, string description,
            long? autoApprovalDelayInSeconds, long assignmentDurationInSeconds,
            decimal? reward, string keywords,
            List<QualificationRequirement> qualificationRequirements)
        {
            RegisterHITTypeRequest req = new RegisterHITTypeRequest();
            req.Title = title;
            req.Description = description;
            req.Keywords = keywords;
            req.AssignmentDurationInSeconds = assignmentDurationInSeconds;

            if (qualificationRequirements != null)
            {
                req.QualificationRequirement = qualificationRequirements.ToArray();
            }

            if (autoApprovalDelayInSeconds.HasValue)
            {
                req.AutoApprovalDelayInSeconds = autoApprovalDelayInSeconds.Value;
                req.AutoApprovalDelayInSecondsSpecified = true;
            }

            if (reward.HasValue)
            {
                Price p = new Price();
                p.Amount = reward.Value;
                p.CurrencyCode = "USD";
                req.Reward = p;
            }

            return Proxy.RegisterHITType(req);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_SetHITTypeNotificationOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="hitTypeId">The hit type id.</param>
        /// <param name="notification">The notification.</param>
        /// <param name="active">The active.</param>
        public void SetHITTypeNotification(string hitTypeId, NotificationSpecification notification, bool? active)
        {
            SetHITTypeNotificationRequest req = new SetHITTypeNotificationRequest();
            req.HITTypeId = hitTypeId;
            req.Notification = notification;

            if (active.HasValue)
            {
                req.Active = active.Value;
                req.ActiveSpecified = true;
            }

            Proxy.SetHITTypeNotification(req);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_SendTestEventNotificationOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="notification">The notification.</param>
        /// <param name="testEventType">Type of the test event.</param>
        public void SendTestEventNotification(NotificationSpecification notification, EventType ?testEventType)
        {
            SendTestEventNotificationRequest req = new SendTestEventNotificationRequest();
            req.Notification = notification;
            if (testEventType.HasValue)
            {
                req.TestEventType = testEventType.Value;
                req.TestEventTypeSpecified = true;
            }

            Proxy.SendTestEventNotification(req);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_DisposeHITOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="hitId">The hit ID.</param>
        public void DisposeHIT(string hitId)
        {
            DisposeHITRequest request = new DisposeHITRequest();
            request.HITId = hitId;

            Proxy.DisposeHIT(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_DisableHITOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="hitId">The hit ID.</param>
        public void DisableHIT(string hitId)
        {
            DisableHITRequest request = new DisableHITRequest();
            request.HITId = hitId;

            Proxy.DisableHIT(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_GetHITOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="hitId">The hit id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns>A <see cref="HIT"/> instance</returns>
        public HIT GetHIT(string hitId, string[] responseGroup)
        {
            GetHITRequest request = new GetHITRequest();
            request.HITId = hitId;
            request.ResponseGroup = responseGroup;

            return Proxy.GetHIT(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_GetAssignmentOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="assignmentId">The assignment id.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns>A <see cref="GetAssignmentResult"/> instance</returns>
        public GetAssignmentResult GetAssignment(string assignmentId, string[] responseGroup)
        {
            GetAssignmentRequest request = new GetAssignmentRequest();
            request.AssignmentId = assignmentId;
            request.ResponseGroup = responseGroup;

            return Proxy.GetAssignment(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_GetReviewableHITsOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="hitTypeId">The hit type id.</param>
        /// <param name="status">The status.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="sortProperty">The sort property.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>
        /// A <see cref="GetReviewableHITsResult"/> instance
        /// </returns>
        public GetReviewableHITsResult GetReviewableHITs(string hitTypeId, ReviewableHITStatus? status,
            SortDirection? sortDirection, GetReviewableHITsSortProperty? sortProperty,
            int? pageNumber, int? pageSize)
        {
            GetReviewableHITsRequest request = new GetReviewableHITsRequest();
            request.HITTypeId = hitTypeId;
            if (status.HasValue)
            {
                request.Status = status.Value;
                request.StatusSpecified = true;
            }

            if (sortDirection.HasValue)
            {
                request.SortDirection = sortDirection.Value;
                request.SortDirectionSpecified = true;
            }

            if (sortProperty.HasValue)
            {
                request.SortProperty = sortProperty.Value;
                request.SortPropertySpecified = true;
            }

            if (pageNumber.HasValue)
            {
                request.PageNumber = pageNumber.Value;
                request.PageNumberSpecified = true;
            }

            if (pageSize.HasValue)
            {
                request.PageSize = pageSize.Value;
                request.PageSizeSpecified = true;
            }

            return Proxy.GetReviewableHITs(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_GetHITsForQualificationTypeOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="qualificationTypeId">The qualification type id.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>A <see cref="GetHITsForQualificationTypeResult"/> instance</returns>
        public GetHITsForQualificationTypeResult GetHITsForQualificationType(string qualificationTypeId, 
            int? pageNumber, int? pageSize)
        {
            GetHITsForQualificationTypeRequest request = new GetHITsForQualificationTypeRequest();
            request.QualificationTypeId = qualificationTypeId;

            if (pageNumber.HasValue)
            {
                request.PageNumber = pageNumber.Value;
                request.PageNumberSpecified = true;
            }

            if (pageSize.HasValue)
            {
                request.PageSize = pageSize.Value;
                request.PageSizeSpecified = true;
            }

            return Proxy.GetHITsForQualificationType(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_GetQualificationsForQualificationTypeOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="qualificationTypeId">The qualification type id.</param>
        /// <param name="status">The status.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>A <see cref="GetQualificationsForQualificationTypeResult"/> instance</returns>
        public GetQualificationsForQualificationTypeResult GetQualificationsForQualificationType(string qualificationTypeId,
            QualificationStatus? status, int? pageNumber, int? pageSize)
        {
            GetQualificationsForQualificationTypeRequest request = new GetQualificationsForQualificationTypeRequest();
            request.QualificationTypeId = qualificationTypeId;
            if (status.HasValue)
            {
                request.Status = status.Value;
                request.StatusSpecified = true;
            }

            if (pageNumber.HasValue)
            {
                request.PageNumber = pageNumber.Value;
                request.PageNumberSpecified = true;
            }

            if (pageSize.HasValue)
            {
                request.PageSize = pageSize.Value;
                request.PageSizeSpecified = true;
            }

            return Proxy.GetQualificationsForQualificationType(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_SetHITAsReviewingOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="hitId">The hit id.</param>
        /// <param name="revert">if set to <c>true</c>, sets the HIT to be reviewable.</param>
        public void SetHITAsReviewing(string hitId, bool? revert)
        {
            SetHITAsReviewingRequest request = new SetHITAsReviewingRequest();
            request.HITId = hitId;

            if (revert.HasValue)
            {
                request.Revert = revert.Value;
                request.RevertSpecified = true;
            }

            Proxy.SetHITAsReviewing(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_ExtendHITOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="hitId">The hit id.</param>
        /// <param name="maxAssignmentsIncrement">The max assignments increment.</param>
        /// <param name="expirationIncrementInSeconds">The expiration increment in seconds.</param>
        public void ExtendHIT(string hitId, int? maxAssignmentsIncrement, long? expirationIncrementInSeconds)
        {
            ExtendHITRequest request = new ExtendHITRequest();
            request.HITId = hitId;
            if (maxAssignmentsIncrement.HasValue)
            {
                request.MaxAssignmentsIncrement = maxAssignmentsIncrement.Value;
                request.MaxAssignmentsIncrementSpecified = true;
            }

            if (expirationIncrementInSeconds.HasValue)
            {
                request.ExpirationIncrementInSeconds = expirationIncrementInSeconds.Value;
                request.ExpirationIncrementInSecondsSpecified = true;
            }

            Proxy.ExtendHIT(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_ForceExpireHITOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="hitId">The hit id.</param>
        public void ForceExpireHIT(string hitId)
        {
            ForceExpireHITRequest request = new ForceExpireHITRequest();
            request.HITId = hitId;

            Proxy.ForceExpireHIT(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_ApproveAssignmentOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="assignmentId">The assignment id.</param>
        /// <param name="requesterFeedback">The requester feedback.</param>
        public void ApproveAssignment(string assignmentId, string requesterFeedback)
        {
            ApproveAssignmentRequest request = new ApproveAssignmentRequest();
            request.AssignmentId = assignmentId;
            request.RequesterFeedback = requesterFeedback;

            Proxy.ApproveAssignment(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_RejectAssignmentOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="assignmentId">The assignment id.</param>
        /// <param name="requesterFeedback">The requester feedback.</param>
        public void RejectAssignment(string assignmentId, string requesterFeedback)
        {
            RejectAssignmentRequest request = new RejectAssignmentRequest();
            request.AssignmentId = assignmentId;
            request.RequesterFeedback = requesterFeedback;

            Proxy.RejectAssignment(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_ApproveRejectedAssignmentOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="assignmentId">The assignment id.</param>
        /// <param name="requesterFeedback">The requester feedback.</param>
        public void ApproveRejectedAssignment(string assignmentId, string requesterFeedback)
        {
            ApproveRejectedAssignmentRequest request = new ApproveRejectedAssignmentRequest();
            request.AssignmentId = assignmentId;
            request.RequesterFeedback = requesterFeedback;

            Proxy.ApproveRejectedAssignment(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_GetAssignmentsForHITOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="hitId">The hit id.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="status">The status.</param>
        /// <param name="sortProperty">The sort property.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns>
        /// A <see cref="GetAssignmentsForHITResult"/> instance
        /// </returns>
        public GetAssignmentsForHITResult GetAssignmentsForHIT(string hitId, SortDirection? sortDirection,
            AssignmentStatus[] status, GetAssignmentsForHITSortProperty? sortProperty,
            int? pageNumber, int? pageSize, string[] responseGroup)
        {
            GetAssignmentsForHITRequest request = new GetAssignmentsForHITRequest();
            request.HITId = hitId;
            if (sortDirection.HasValue)
            {
                request.SortDirection = sortDirection.Value;
                request.SortDirectionSpecified = true;
            }

            if (sortProperty.HasValue)
            {
                request.SortProperty = sortProperty.Value;
                request.SortPropertySpecified = true;
            }

            if (pageNumber.HasValue)
            {
                request.PageNumber = pageNumber.Value;
                request.PageNumberSpecified = true;
            }

            if (pageSize.HasValue)
            {
                request.PageSize = pageSize.Value;
                request.PageSizeSpecified = true;
            }
            request.ResponseGroup = responseGroup;

            if (status != null && status.Length > 0)
            {
                request.AssignmentStatus = status;
            }

            return Proxy.GetAssignmentsForHIT(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_GetFileUploadURLOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="assignmentId">The assignment id.</param>
        /// <param name="questionIdentifier">The question identifier.</param>
        /// <returns>A <see cref="String"/> instance</returns>
        public string GetFileUploadURL(string assignmentId, string questionIdentifier)
        {
            GetFileUploadURLRequest request = new GetFileUploadURLRequest();
            request.AssignmentId = assignmentId;
            request.QuestionIdentifier = questionIdentifier;

            return Proxy.GetFileUploadURL(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_SearchHITsOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="sortProperty">The sort property.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="responseGroup">The response group.</param>
        /// <returns>
        /// A <see cref="SearchHITsResult"/> instance
        /// </returns>
        public SearchHITsResult GetHITs(SortDirection? sortDirection, SearchHITsSortProperty? sortProperty,
            int? pageNumber, int? pageSize, string[] responseGroup)
        {
            SearchHITsRequest request = new SearchHITsRequest();
            if (sortDirection.HasValue)
            {
                request.SortDirection = sortDirection.Value;
                request.SortDirectionSpecified = true;
            }

            if (sortProperty.HasValue)
            {
                request.SortProperty = sortProperty.Value;
                request.SortPropertySpecified = true;
            }

            if (pageNumber.HasValue)
            {
                request.PageNumber = pageNumber.Value;
                request.PageNumberSpecified = true;
            }

            if (pageSize.HasValue)
            {
                request.PageSize = pageSize.Value;
                request.PageSizeSpecified = true;
            }
            request.ResponseGroup = responseGroup;

            return Proxy.SearchHITs(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_GrantBonusOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="workerId">The worker id.</param>
        /// <param name="bonusAmount">The bonus amount.</param>
        /// <param name="assignmentId">The assignment id.</param>
        /// <param name="reason">The reason.</param>
        public void GrantBonus(string workerId, decimal? bonusAmount, string assignmentId, string reason)
        {
            GrantBonusRequest request = new GrantBonusRequest();
            request.WorkerId = workerId;
            request.AssignmentId = assignmentId;
            request.Reason = reason;

            if (bonusAmount.HasValue)
            {
                Price p = new Price();
                p.Amount = bonusAmount.Value;
                p.CurrencyCode = "USD";
                request.BonusAmount = p;
            }

            Proxy.GrantBonus(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_GetBonusPaymentsOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="hitId">The hit id.</param>
        /// <param name="assignmentId">The assignment id.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>
        /// A <see cref="GetBonusPaymentsResult"/> instance
        /// </returns>
        public GetBonusPaymentsResult GetBonusPayments(string hitId, string assignmentId, int? pageNumber, int? pageSize)
        {
            GetBonusPaymentsRequest request = new GetBonusPaymentsRequest();
            request.HITId = hitId;
            request.AssignmentId = assignmentId;

            if (pageNumber.HasValue)
            {
                request.PageNumber = pageNumber.Value;
                request.PageNumberSpecified = true;
            }

            if (pageSize.HasValue)
            {
                request.PageSize = pageSize.Value;
                request.PageSizeSpecified = true;
            }

            return Proxy.GetBonusPayments(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_CreateQualificationTypeOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="keywords">The keywords.</param>
        /// <param name="description">The description.</param>
        /// <param name="status">The status.</param>
        /// <param name="retryDelayInSeconds">The retry delay in seconds.</param>
        /// <param name="test">The test. If the question is not wrapped as XML, it will be wrapped as a simple free text question (<see cref="QuestionUtil"/>)</param>
        /// <param name="answerKey">The answer key.</param>
        /// <param name="testDurationInSeconds">The test duration in seconds.</param>
        /// <param name="autoGranted">The auto granted.</param>
        /// <param name="autoGrantedValue">The auto granted value.</param>
        /// <returns>A <see cref="QualificationType"/> instance</returns>
        public QualificationType CreateQualificationType(string name, string keywords, string description,
            QualificationTypeStatus? status, long retryDelayInSeconds, string test, string answerKey,
            long? testDurationInSeconds, bool?autoGranted, int? autoGrantedValue)
        {
            CreateQualificationTypeRequest request = new CreateQualificationTypeRequest();
            request.Name = name;
            request.Keywords = keywords;
            request.Description = description;
            request.RetryDelayInSeconds = retryDelayInSeconds;
            request.Test = test;
            request.AnswerKey = answerKey;

            if (status.HasValue)
            {
                request.QualificationTypeStatus = status.Value;
                request.QualificationTypeStatusSpecified = true;
            }
            if (testDurationInSeconds.HasValue)
            {
                request.TestDurationInSeconds = testDurationInSeconds.Value;
                request.TestDurationInSecondsSpecified = true;
            }
            if (autoGranted.HasValue)
            {
                request.AutoGranted = autoGranted.Value;
                request.AutoGrantedSpecified = true;
            }
            if (autoGrantedValue.HasValue)
            {
                request.AutoGrantedValue = autoGrantedValue.Value;
                request.AutoGrantedValueSpecified = true;
            }

            return Proxy.CreateQualificationType(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_GetQualificationRequestsOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="qualificationTypeId">The qualification type id.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="sortProperty">A <see cref="Nullable&lt;GetQualificationRequestsSortProperty&gt;"/> instance containing the request parameters</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>
        /// A <see cref="GetQualificationRequestsResult"/> instance
        /// </returns>
        public GetQualificationRequestsResult GetQualificationRequests(string qualificationTypeId, SortDirection? sortDirection,
            GetQualificationRequestsSortProperty? sortProperty, int? pageNumber, int? pageSize)
        {
            GetQualificationRequestsRequest request = new GetQualificationRequestsRequest();
            request.QualificationTypeId = qualificationTypeId;

            if (sortDirection.HasValue)
            {
                request.SortDirection = sortDirection.Value;
                request.SortDirectionSpecified = true;
            }

            if (sortProperty.HasValue)
            {
                request.SortProperty = sortProperty.Value;
                request.SortPropertySpecified = true;
            }

            if (pageNumber.HasValue)
            {
                request.PageNumber = pageNumber.Value;
                request.PageNumberSpecified = true;
            }

            if (pageSize.HasValue)
            {
                request.PageSize = pageSize.Value;
                request.PageSizeSpecified = true;
            }
            return Proxy.GetQualificationRequests(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_RejectQualificationRequestOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="qualificationRequestId">The qualification request id.</param>
        /// <param name="reason">The reason.</param>
        public void RejectQualificationRequest(string qualificationRequestId, string reason)
        {
            RejectQualificationRequestRequest request = new RejectQualificationRequestRequest();
            request.QualificationRequestId = qualificationRequestId;
            request.Reason = reason;

            Proxy.RejectQualificationRequest(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_GrantQualificationOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="qualificationRequestId">The qualification request id.</param>
        /// <param name="qualificationValue">The integer value.</param>
        public void GrantQualification(string qualificationRequestId, int? qualificationValue)
        {
            GrantQualificationRequest request = new GrantQualificationRequest();
            request.QualificationRequestId = qualificationRequestId;

            if (qualificationValue.HasValue)
            {
                request.IntegerValue = qualificationValue.Value;
                request.IntegerValueSpecified = true;
            }

            Proxy.GrantQualification(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_AssignQualificationOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="qualificationTypeId">The qualification type id.</param>
        /// <param name="workerId">The worker id.</param>
        /// <param name="qualificationValue">The qualification value.</param>
        /// <param name="sendNotification">The send notification.</param>
        public void AssignQualification(string qualificationTypeId, string workerId, int? qualificationValue, bool?sendNotification)
        {
            AssignQualificationRequest request = new AssignQualificationRequest();
            request.QualificationTypeId = qualificationTypeId;
            request.WorkerId = workerId;

            if (qualificationValue.HasValue)
            {
                request.IntegerValue = qualificationValue.Value;
                request.IntegerValueSpecified = true;
            }

            if (sendNotification.HasValue)
            {
                request.SendNotification = sendNotification.Value;
                request.SendNotificationSpecified = true;
            }

            Proxy.AssignQualification(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_RevokeQualificationOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="qualificationTypeId"></param>
        /// <param name="subjectId"></param>
        /// <param name="reason"></param>
        public void RevokeQualification(string qualificationTypeId, string subjectId, string reason)
        {
            RevokeQualificationRequest request = new RevokeQualificationRequest();
            request.QualificationTypeId = qualificationTypeId;
            request.SubjectId = subjectId;
            request.Reason = reason;

            Proxy.RevokeQualification(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_UpdateQualificationScoreOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="qualificationTypeId"></param>
        /// <param name="subjectId"></param>
        /// <param name="qualificationValue"></param>
        public void UpdateQualificationScore(string qualificationTypeId, string subjectId, int? qualificationValue)
        {
            UpdateQualificationScoreRequest request = new UpdateQualificationScoreRequest();
            request.QualificationTypeId = qualificationTypeId;
            request.SubjectId = subjectId;

            if (qualificationValue.HasValue)
            {
                request.IntegerValue = qualificationValue.Value;
                request.IntegerValueSpecified = true;
            }

            Proxy.UpdateQualificationScore(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_GetQualificationTypeOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="qualificationTypeId">The qualification type id.</param>
        /// <returns>A <see cref="QualificationType"/> instance</returns>
        public QualificationType GetQualificationType(string qualificationTypeId)
        {
            GetQualificationTypeRequest request = new GetQualificationTypeRequest();
            request.QualificationTypeId = qualificationTypeId;
            
            return Proxy.GetQualificationType(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_GetQualificationScoreOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="qualificationTypeId">The qualification type id.</param>
        /// <param name="subjectId">The subject id.</param>
        /// <returns>A <see cref="Qualification"/> instance</returns>
        public Qualification GetQualificationScore(string qualificationTypeId, string subjectId)
        {
            GetQualificationScoreRequest request = new GetQualificationScoreRequest();
            request.QualificationTypeId = qualificationTypeId;
            request.SubjectId = subjectId;

            return Proxy.GetQualificationScore(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_SearchQualificationTypesOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="mustBeRequestable">if set to <c>true</c> [must be requestable].</param>
        /// <param name="mustBeOwnedByCaller">The must be owned by caller.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <param name="sortProperty">The sort property.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <returns>
        /// A <see cref="SearchQualificationTypesResult"/> instance
        /// </returns>
        public SearchQualificationTypesResult SearchQualificationTypes(string query, bool mustBeRequestable, 
            bool?mustBeOwnedByCaller, SortDirection? sortDirection, 
            SearchQualificationTypesSortProperty? sortProperty, int? pageNumber, int? pageSize)
        {
            SearchQualificationTypesRequest request = new SearchQualificationTypesRequest();
            request.Query = query;
            request.MustBeRequestable = mustBeRequestable;

            if (mustBeOwnedByCaller.HasValue)
            {
                request.MustBeOwnedByCaller = mustBeOwnedByCaller.HasValue;
                request.MustBeOwnedByCallerSpecified = true;
            }

            if (sortDirection.HasValue)
            {
                request.SortDirection = sortDirection.Value;
                request.SortDirectionSpecified = true;
            }

            if (sortProperty.HasValue)
            {
                request.SortProperty = sortProperty.Value;
                request.SortPropertySpecified = true;
            }

            if (pageNumber.HasValue)
            {
                request.PageNumber = pageNumber.Value;
                request.PageNumberSpecified = true;
            }

            if (pageSize.HasValue)
            {
                request.PageSize = pageSize.Value;
                request.PageSizeSpecified = true;
            }
            

            return Proxy.SearchQualificationTypes(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_UpdateQualificationTypeOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="qualificationTypeId">The qualification type id.</param>
        /// <param name="description">The description.</param>
        /// <param name="status">The status.</param>
        /// <param name="test">The test. If the question is not wrapped as XML, it will be wrapped as a simple free text question (<see cref="QuestionUtil"/>)</param>
        /// <param name="answerKey">The answer key.</param>
        /// <param name="testDurationInSeconds">The test duration in seconds.</param>
        /// <param name="retryDelayInSeconds">The retry delay in seconds.</param>
        /// <param name="autoGranted">The auto granted.</param>
        /// <param name="autoGrantedValue">The auto granted value.</param>
        /// <returns>A <see cref="QualificationType"/> instance</returns>
        public QualificationType UpdateQualificationType(string qualificationTypeId, string description, 
            QualificationTypeStatus? status, string test, string answerKey, long? testDurationInSeconds, 
            long? retryDelayInSeconds, bool? autoGranted, int? autoGrantedValue)
        {
            UpdateQualificationTypeRequest request = new UpdateQualificationTypeRequest();
            request.QualificationTypeId = qualificationTypeId;
            request.Description = description;
            request.Test = test;
            request.AnswerKey = answerKey;

            if (status.HasValue)
            {
                request.QualificationTypeStatus = status.Value;
                request.QualificationTypeStatusSpecified = true;
            }
            if (testDurationInSeconds.HasValue)
            {
                request.TestDurationInSeconds = testDurationInSeconds.Value;
                request.TestDurationInSecondsSpecified = true;
            }
            if (retryDelayInSeconds.HasValue)
            {
                request.RetryDelayInSeconds = retryDelayInSeconds.Value;
                request.RetryDelayInSecondsSpecified = true;
            }
            if (autoGranted.HasValue)
            {
                request.AutoGranted = autoGranted.Value;
                request.AutoGrantedSpecified = true;
            }
            if (autoGrantedValue.HasValue)
            {
                request.AutoGrantedValue = autoGrantedValue.Value;
                request.AutoGrantedValueSpecified = true;
            }  

            return Proxy.UpdateQualificationType(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_GetAccountBalanceOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <returns>
        /// A <see cref="GetAccountBalanceResult"/> instance
        /// </returns>
        public GetAccountBalanceResult GetAccountBalance()
        {
            GetAccountBalanceRequest request = new GetAccountBalanceRequest();
            
            return Proxy.GetAccountBalance(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_GetRequesterStatisticOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="statistic">A <see cref="RequesterStatistic"/> instance containing the request parameters</param>
        /// <param name="timePeriod">The time period.</param>
        /// <param name="count">The count.</param>
        /// <returns>A <see cref="DataPoint"/> list</returns>
        public IList<DataPoint> GetRequesterStatistic(RequesterStatistic statistic, TimePeriod? timePeriod, int? count)
        {
            GetRequesterStatisticRequest request = new GetRequesterStatisticRequest();
            request.Statistic = statistic;

            if (timePeriod.HasValue)
            {
                request.TimePeriod = timePeriod.Value;
                request.TimePeriodSpecified = true;
            }
            if (count.HasValue)
            {
                request.Count = count.Value;
                request.CountSpecified = true;
            }

            DataPoint[] proxyResult = Proxy.GetRequesterStatistic(request).DataPoint;
            List<DataPoint> ret = new List<DataPoint>(proxyResult);

            return ret;
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_NotifyWorkersOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="subject">The subject.</param>
        /// <param name="messageText">The message text.</param>
        /// <param name="workerIDs">The IDs of the workers to notify.</param>
        public void NotifyWorkers(string subject, string messageText, List<string> workerIDs) 
        {
            if (workerIDs == null || workerIDs.Count == 0)
            {
                throw new ArgumentException("No worker IDs specified", "workerIDs");
            }

            NotifyWorkersRequest request = new NotifyWorkersRequest();
            request.MessageText = messageText;
            request.Subject = subject;
            request.WorkerId = workerIDs.ToArray();

            Proxy.NotifyWorkers(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_HelpOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="about">The topics to retrieve help for</param>
        /// <param name="type">A <see cref="Nullable&lt;HelpRequestHelpType&gt;"/> instance containing the request parameters</param>
        /// <returns>A <see cref="Information"/> instance</returns>
        public Information Help(List<string> about, HelpRequestHelpType? type)
        {
            if (about == null || about.Count == 0)
            {
                throw new ArgumentException("No help topics specified", "about");
            }

            HelpRequest request = new HelpRequest();
            request.About = about.ToArray();

            if (type.HasValue)
            {
                request.HelpType = type.Value;
                request.HelpTypeSpecified = true;
            }

            return Proxy.Help(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_BlockWorkerOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="workerId">The worker ID.</param>
        /// <param name="reason">The reason.</param>
        public void BlockWorker(string workerId, string reason)
        {
            BlockWorkerRequest request = new BlockWorkerRequest();
            request.WorkerId = workerId;
            request.Reason = reason;

            Proxy.BlockWorker(request);
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_GetBlockedWorkersOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="workerId">The worker ID.</param>
        /// <param name="reason">The reason.</param>
        public void UnblockWorker(string workerId, string reason)
        {
            UnblockWorkerRequest request = new UnblockWorkerRequest();
            request.WorkerId = workerId;
            request.Reason = reason;

            Proxy.UnblockWorker(request);
        }


        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_GetBlockedWorkerOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="pageNumber">The page number to return results from.</param>
        public List<WorkerBlock> GetBlockedWorkers(int pageNumber)
        {
            GetBlockedWorkersRequest request = new GetBlockedWorkersRequest();

            request.PageNumber = pageNumber;
            request.PageNumberSpecified = true;

            GetBlockedWorkersResult result = Proxy.GetBlockedWorkers(request);
            return (result.WorkerBlock == null) ? new List<WorkerBlock>() : new List<WorkerBlock>(result.WorkerBlock);
        }

        /// <summary>
        /// Enumerates through all active qualifications in the system
        /// </summary>
        public IEnumerable<WorkerBlock> GetAllBlockedWorkersIterator()
        {
            int pageNum = 1;
            IList<WorkerBlock> curResult = null;

            do
            {
                curResult = GetBlockedWorkers(pageNum++);
                if (curResult != null)
                {
                    for (int i = 0; i < curResult.Count; i++)
                    {
                        yield return curResult[i];
                    }
                }
            } while (curResult != null && curResult.Count > 0);

            yield break;
        }

        /// <summary>
        /// See <a href="http://docs.amazonwebservices.com/AWSMechTurk/2012-03-25/AWSMturkAPI/ApiReference_ChangeHITTypeOfHITOperation.html">online documentation for this operation.</a>
        /// </summary>
        /// <param name="hitId">The hit ID.</param>
        /// <param name="hitTypeId">The hit type ID.</param>
        public void ChangeHITTypeOfHIT(string hitId, string hitTypeId)
        {
            ChangeHITTypeOfHITRequest request = new ChangeHITTypeOfHITRequest();
            request.HITId = hitId;
            request.HITTypeId = hitTypeId;

            Proxy.ChangeHITTypeOfHIT(request);
        }
        #endregion

        #region Convenience overloads

        /// <summary>
        /// Creates a HIT using defaults for the HIT properties not given as parameters.
        /// The request uses either the default or full responseGroup.
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="description">The description.</param>
        /// <param name="reward">The reward.</param>
        /// <param name="question">The question. If the question is not wrapped as XML, it will be wrapped as a simple free text question (<see cref="QuestionUtil"/>)</param>
        /// <param name="maxAssignments">The max assignments.</param>
        /// <param name="getFullResponse">if set to <c>true</c> the full dataset gets returned.</param>
        /// <returns>A <see cref="HIT"/> instance</returns>
        public HIT CreateHIT(string title, string description, decimal reward, string question, 
            int? maxAssignments, bool getFullResponse) 
        {
            return CreateHIT(
                null,                       // HITTypeId
                title, 
                description,
                null,                       // Keywords
                question,
                reward,
                (long) 60 * 60,             // assignmentDurationInSeconds, 1 hour
                (long) 60 * 60 * 24 * 15,   // autoApprovalDelayInSeconds, 15 days
                (long) 60 * 60 * 24 * 3,    // lifetimeInSeconds, 3 days
                maxAssignments,
                null, //requesterAnnotation
                null, // qualificationRequirements
                getFullResponse ? MTurkConstants.ResponseGroupFullHIT : null);
        }

        /// <summary>
        /// Creates a HIT using default values for the HIT properties not given as parameters.
        /// The request uses the default responseGroup of "Minimal".
        /// </summary>
        /// <param name="title">The title.</param>
        /// <param name="description">The description.</param>
        /// <param name="reward">The reward.</param>
        /// <param name="question">The question. If the question is not wrapped as XML, it will be wrapped as a simple free text question (<see cref="QuestionUtil"/>)</param>
        /// <param name="maxAssignments">The max assignments.</param>
        /// <returns>A <see cref="HIT"/> instance</returns>
        public HIT CreateHIT(string title, string description, decimal reward, string question, int? maxAssignments) 
        {
            return CreateHIT(
                title,
                description,
                reward,
                question,
                maxAssignments,
                false);
        }


        /// <summary>
        /// Creates a new HIT from a populated HIT domain object
        /// </summary>
        /// <param name="newHit">The new hit to be created</param>
        /// <returns>A <see cref="HIT"/> instance with the populated HIT ID</returns>
        public HIT CreateHIT(HIT newHit)
        {
            return CreateHIT(
                newHit.HITTypeId,
                newHit.Title,
                newHit.Description,
                newHit.Keywords,
                newHit.Question,
                newHit.Reward != null ? newHit.Reward.Amount : (decimal?)null,
                newHit.AssignmentDurationInSecondsSpecified ? newHit.AssignmentDurationInSeconds : (long?)null,
                newHit.AutoApprovalDelayInSecondsSpecified ? newHit.AutoApprovalDelayInSeconds : (long?)null,
                newHit.ExpirationSpecified ? (long)(newHit.Expiration-DateTime.Now).TotalSeconds : 0,
                newHit.MaxAssignmentsSpecified ? newHit.MaxAssignments : (int?)null,
                newHit.RequesterAnnotation,
                newHit.QualificationRequirement != null ? new List<QualificationRequirement>(newHit.QualificationRequirement) : null,
                null);
        }

        /// <summary>
        /// Retrieves a HIT by HIT Id. The request uses full responseGroup.
        /// </summary>
        /// <param name="hitId">The hit id.</param>
        /// <returns>A <see cref="HIT"/> instance</returns>
        public HIT GetHIT(string hitId)
        {
            return GetHIT(hitId, MTurkConstants.ResponseGroupFullHIT);
        }

        /// <summary>
        /// Creates a Qualification Type using default values for the Qualification Type properties 
        /// not given as parameters.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="keywords">The keywords.</param>
        /// <param name="description">The description.</param>
        /// <returns>A <see cref="QualificationType"/> instance</returns>
        public QualificationType CreateQualificationType(string name, string keywords, string description)
        {
            return CreateQualificationType(
                name,
                keywords,
                description,
                QualificationTypeStatus.Active,
                0,          // retryDelayInSeconds
                null,       // test
                null,       // answer key
                null,       // testDurationInSeconds
                null,       // autoGranted
                null);      // autoGrantedValue
        }

        /// <summary>
        /// Updates the description and status of a qualification type
        /// </summary>
        /// <param name="qualificationTypeId">The qualification type id.</param>
        /// <param name="description">The description.</param>
        /// <param name="status">The status.</param>
        /// <returns>A <see cref="QualificationType"/> instance</returns>
        public QualificationType UpdateQualificationType(string qualificationTypeId, string description, QualificationTypeStatus status)
        {
            return UpdateQualificationType(
                qualificationTypeId,
                description,
                status,
                null,       // test 
                null,       // answerKey 
                null,       // testDurationInSeconds 
                null,       // retryDelayInSeconds 
                null,       // autoGranted 
                null);      // autoGrantedValue
        }

        /// <summary>
        /// Retrieves workers' Qualifications found on the requested page for the given 
        /// Qualification Type.  The request uses the default responseGroup of "Minimal".
        /// </summary>
        /// <param name="qualificationTypeId">The qualification type id.</param>
        /// <param name="pageNum">The page number to return results from.</param>
        /// <returns>A <see cref="Qualification"/> list</returns>
        public IList<Qualification> GetQualificationsForQualificationType(string qualificationTypeId, int pageNum)
        {
            GetQualificationsForQualificationTypeResult result = GetQualificationsForQualificationType(qualificationTypeId,
                  QualificationStatus.Granted,
                  pageNum,
                  null);

            return (result.Qualification==null) ? new List<Qualification>() : new List<Qualification>(result.Qualification);
        }

        /// <summary>
        /// Retrieves workers' QualificationRequests found on the first page for the given 
        /// Qualification Type.  The results are sorted by SubmitTime.
        /// </summary>
        /// <param name="qualificationTypeId">The qualification type id.</param>
        /// <returns>
        /// A <see cref="QualificationRequest"/> array
        /// </returns>
        public IList<QualificationRequest> GetQualificationRequests(string qualificationTypeId)
        {
            GetQualificationRequestsResult result = GetQualificationRequests(
                qualificationTypeId,
                null,
                GetQualificationRequestsSortProperty.SubmitTime,
                null,
                null);

            return (result.QualificationRequest == null) ? new List<QualificationRequest>() : new List<QualificationRequest>(result.QualificationRequest);
        }

        /// <summary>
        /// Retrieves workers' Assignments found on the first page for the given HIT.
        /// The results are sorted by SubmitTime.  The request uses either the default 
        /// or full responseGroup.
        /// </summary>
        /// <param name="hitId">The hit id.</param>
        /// <param name="pageNum">The page number to return results from.</param>
        /// <param name="getFullResponse">if set to <c>true</c> the full dataset gets returned.</param>
        /// <returns>A <see cref="Assignment"/> list</returns>
        public IList<Assignment> GetAssignmentsForHIT(string hitId, int pageNum, bool getFullResponse)
        {
            GetAssignmentsForHITResult result = GetAssignmentsForHIT(
                hitId,
                null,   // sort direction
                DefaultAssignmentStatus,
                GetAssignmentsForHITSortProperty.SubmitTime,
                pageNum, 
                null,   // page size
                getFullResponse ? MTurkConstants.ResponseGroupFullAssignment : null
                );

            return (result.Assignment == null) ? new List<Assignment>() : new List<Assignment>(result.Assignment);
        }

        /// <summary>
        /// Retrieves workers' Assignments found on the requested page for the given HIT.  
        /// The request uses the default responseGroup of "Minimal".
        /// </summary>
        /// <param name="hitId">The hit id.</param>
        /// <param name="pageNum">The page number to return results from.</param>
        /// <returns>A <see cref="Assignment"/> list</returns>
        public IList<Assignment> GetAssignmentsForHIT(string hitId, int pageNum)
        {
            return GetAssignmentsForHIT(hitId, pageNum, false);
        }

        /// <summary>
        /// Gets the available account balance.
        /// </summary>
        /// <returns>The available account balance</returns>
        public decimal GetAvailableAccountBalance()
        {
            return GetAccountBalance().AvailableBalance.Amount;
        }

        /// <summary>
        /// Retrieves requester's reviewable HITs found on the requested page for the given HIT Type.
        /// </summary>
        /// <param name="hitTypeId">The hit type id.</param>
        /// <param name="pageNum">The page number to return results from.</param>
        /// <returns>A <see cref="HIT"/> list</returns>
        public IList<HIT> GetReviewableHITs(string hitTypeId, int pageNum)
        {
            GetReviewableHITsResult result = GetReviewableHITs(
                hitTypeId,
                ReviewableHITStatus.Reviewable,
                null,
                GetReviewableHITsSortProperty.Enumeration,
                pageNum,
                null);

            return (result.HIT == null) ? new List<HIT>() : new List<HIT>(result.HIT);
        }

        /// <summary>
        /// Retrieves any HITs found on the requested page. The request uses the default responseGroup of "Minimal".
        /// </summary>
        /// <param name="pageNum">The page number to return results from.</param>
        /// <returns>A <see cref="HIT"/> list</returns>
        public IList<HIT> GetHITs(int pageNum)
        {
            return GetHITs(pageNum, false);
        }

        /// <summary>
        /// Retrieves requester's reviewable HITs found on the requested page for the given HIT Type. 
        /// The request uses either the default or full responseGroup.
        /// </summary>
        /// <param name="pageNum">The page number to return results from.</param>
        /// <param name="getFullResponse">if set to <c>true</c> the full dataset gets returned.</param>
        /// <returns>A <see cref="HIT"/> list</returns>
        public IList<HIT> GetHITs(int pageNum, bool getFullResponse)
        {
            SearchHITsResult result = GetHITs(
                null, 
                SearchHITsSortProperty.Enumeration,
                pageNum, 
                null,
                getFullResponse ? MTurkConstants.ResponseGroupFullHIT : null);

            return (result.HIT == null) ? new List<HIT>() : new List<HIT>(result.HIT);
        }

        /// <summary>
        /// Retrieves any Qualification Types found on the requested page.
        /// </summary>
        /// <param name="pageNum">The page number to return results from.</param>
        /// <returns>
        /// A <see cref="QualificationType"/> array
        /// </returns>
        public QualificationType[] GetQualificationTypes(int pageNum)
        {
            SearchQualificationTypesResult result = SearchQualificationTypes(
                null,   // Query
                false,  // mustBeRequestable
                true,   // mustBeOwnedByCaller
                null,
                SearchQualificationTypesSortProperty.Name,
                pageNum,
                null);

            return result.QualificationType;
        }

        /// <summary>
        /// Sets the status of the given HIT as Reviewable.
        /// </summary>
        /// <param name="hitId">The hit id.</param>
        public void SetHITAsReviewable(string hitId)
        {
            SetHITAsReviewing(hitId,true);
        }

        /// <summary>
        /// Sets the status of the given HIT as Reviewing.
        /// </summary>
        /// <param name="hitId">The hit id.</param>
        public void SetHITAsReviewing(string hitId)
        {
            SetHITAsReviewing(hitId, false);
        }

        /// <summary>
        /// Retrieves the total number of active HITs for the requester.
        /// </summary>
        /// <returns>A <see cref="Int32"/> instance</returns>
        public int GetTotalNumHITsInAccount()
        {
            SearchHITsResult result = GetHITs(
                null,
                SearchHITsSortProperty.Expiration,
                1, 
                null, 
                null);

            return (result.TotalNumResultsSpecified) ? result.TotalNumResults : 0;
        }

        /// <summary>
        /// Disposes the given Qualification Type.  The Qualification Type becomes inactive.
        /// </summary>
        /// <param name="qualificationTypeId">The qualification type id.</param>
        /// <returns>A <see cref="QualificationType"/> instance</returns>
        public QualificationType DisposeQualificationType(string qualificationTypeId)
        {

            return UpdateQualificationType(qualificationTypeId,
                null, // don't change description 
                QualificationTypeStatus.Inactive);
        }

        /// <summary>
        /// Sets up an email notification setting for the given HIT Type.
        /// </summary>
        /// <param name="hitTypeId">The hit type id.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="eventType">Type of the event.</param>
        public void SendTestEmailEventNotification(string hitTypeId, string emailAddress, EventType eventType)
        {
            NotificationSpecification spec = new NotificationSpecification();
            spec.Destination = emailAddress;
            spec.Transport = NotificationTransport.Email;
            spec.Version = MTurkConstants.NotificationVersion;
            spec.EventType = new EventType[] { eventType };

            SetHITTypeNotification(hitTypeId, spec, true);
        }

        /// <summary>
        /// Updates a HIT using defaults for the HIT properties not given as parameters.
        /// </summary>
        /// <param name="hitId">The hit id.</param>
        /// <param name="title">The title (if null, the HIT's current title is used)</param>
        /// <param name="description">The description (if null, the HIT's current description is used)</param>
        /// <param name="keywords">The keywords (if null, the HIT's current keywords are used)</param>
        /// <param name="reward">The reward (if null, the HIT's current reward is used)</param>
        /// <returns>ID of the new HIT type for the hit</returns>
        public string UpdateHIT(string hitId, string title, string description, string keywords, decimal? reward)
        {
            if (title == null || description == null || keywords == null || reward == null)
            {
                // load current hit values
                HIT currentHIT = GetHIT(hitId);
                if (title == null)
                {
                    title = currentHIT.Title;
                }
                if (description == null)
                {
                    description = currentHIT.Description;
                }
                if (keywords == null)
                {
                    keywords = currentHIT.Keywords;
                }
                if (!reward.HasValue)
                {
                    reward = currentHIT.Reward.Amount;
                }
            }

            // create new type
            string newHITTypeId = RegisterHITType(title, 
                description,
                DefaultAutoApprovalDelayInSeconds,
                DefaultAssignmentDurationInSeconds,
                reward,
                keywords,
                null); // qualificationRequirements

            // Change hit for new HIT type ID
            this.ChangeHITTypeOfHIT(hitId, newHITTypeId);

            return newHITTypeId;
        }

        /// <summary>
        /// Updates a list of hits to reflect the properties of a new hit type (previously created with <see cref="RegisterHITType"/>)
        /// </summary>
        /// <param name="hitIdList">The hit id list.</param>
        /// <param name="hitTypeId">The hit type id.</param>
        /// <returns>Returns a map of errors (HitID->error description) for the HITs that failed to update.</returns>
        public IDictionary<string, string> UpdateHITs(string[] hitIdList, string hitTypeId)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();

            for (int i = 0; i < hitIdList.Length; i++)
            {
                try
                {
                    ChangeHITTypeOfHIT(hitIdList[i], hitTypeId);
                }
                catch (Exception ex)
                {
                    errors.Add(hitIdList[i], ex.Message);
                }
            }

            return errors;
        }


        #region GetAll... methods
        /// <summary>
        /// Gets the type of all qualifications for qualification.
        /// </summary>
        /// <param name="qualificationTypeId">The qualification type id.</param>
        /// <returns>A <see cref="Qualification"/> list</returns>
        public IList<Qualification> GetAllQualificationsForQualificationType(string qualificationTypeId)
        {
            List<Qualification> results = new List<Qualification>();

            int pageNum = 1;
            IList<Qualification> curResult = null;

            do
            {
                curResult = GetQualificationsForQualificationType(qualificationTypeId, pageNum++);
                if (curResult != null)
                {
                    results.AddRange(curResult);
                }
            } while (curResult != null && curResult.Count > 0);

            return results;
        }

        /// <summary>
        /// Enumerates through all qualifications for qualification.
        /// </summary>
        /// <param name="qualificationTypeId">The qualification type id.</param>
        public IEnumerable<Qualification> GetAllQualificationsForQualificationTypeIterator(string qualificationTypeId)
        {
            int pageNum = 1;
            IList<Qualification> curResult = null;

            do
            {
                curResult = GetQualificationsForQualificationType(qualificationTypeId, pageNum++);
                if (curResult != null)
                {
                    for (int i = 0; i < curResult.Count; i++)
                    {
                        yield return curResult[i];
                    }
                }
            } while (curResult != null && curResult.Count > 0);

            yield break;
        }

        /// <summary>
        /// Retrieves all active HITs in the system.  The request uses the full responseGroup.
        /// </summary>
        /// <returns>A <see cref="HIT"/> list</returns>
        public IList<HIT> GetAllHITs()
        {
            List<HIT> results = new List<HIT>();

            int pageNum = 1;
            IList<HIT> curResult = null;

            do
            {
                curResult = GetHITs(pageNum++, true);
                if (curResult != null)
                {
                    results.AddRange(curResult);
                }
            } while (curResult != null && curResult.Count > 0);

            return results;
        }

        /// <summary>
        /// Enumerates through all HITs.
        /// </summary>
        public IEnumerable<HIT> GetAllHITsIterator()
        {
            int pageNum = 1;
            IList<HIT> curResult = null;

            do
            {
                curResult = GetHITs(pageNum++, true);
                if (curResult != null)
                {
                    for (int i = 0; i < curResult.Count; i++)
                    {
                        yield return curResult[i];
                    }
                }
            } while (curResult != null && curResult.Count > 0);

            yield break;
        }

        /// <summary>
        /// Retrieves all active Qualifications in the system.
        /// </summary>
        /// <returns>
        /// A <see cref="QualificationType"/> array
        /// </returns>
        public IList<QualificationType> GetAllQualificationTypes()
        {
            List<QualificationType> results = new List<QualificationType>();

            int pageNum = 1;
            IList<QualificationType> curResult = null;

            do
            {
                curResult = GetQualificationTypes(pageNum++);
                if (curResult != null)
                {
                    results.AddRange(curResult);
                }
            } while (curResult != null && curResult.Count > 0);

            return results;
        }

        /// <summary>
        /// Enumerates through all active qualifications in the system
        /// </summary>
        public IEnumerable<QualificationType> GetAllQualificationTypesIterator()
        {
            int pageNum = 1;
            IList<QualificationType> curResult = null;

            do
            {
                curResult = GetQualificationTypes(pageNum++);
                if (curResult != null)
                {
                    for (int i = 0; i < curResult.Count; i++)
                    {
                        yield return curResult[i];
                    }
                }
            } while (curResult != null && curResult.Count > 0);

            yield break;
        }

        /// <summary>
        /// Retrieves all of requester's reviewable HITs of the given HIT Type.
        /// </summary>
        /// <param name="hitTypeId">The hit type id.</param>
        /// <returns>A <see cref="HIT"/> list</returns>
        public IList<HIT> GetAllReviewableHITs(string hitTypeId)
        {
            List<HIT> results = new List<HIT>();

            int pageNum = 1;
            IList<HIT> curResult = null;

            do
            {
                curResult = GetReviewableHITs(hitTypeId, pageNum++);
                if (curResult != null)
                {
                    results.AddRange(curResult);
                }
            } while (curResult != null && curResult.Count > 0);

            return results;
        }

        /// <summary>
        /// Enumerates through of requester's reviewable HITs of the given HIT Type.
        /// </summary>
        /// <param name="hitTypeId">The hit type id.</param>
        public IEnumerable<HIT> GetAllReviewableHITsIterator(string hitTypeId)
        {
            int pageNum = 1;
            IList<HIT> curResult = null;

            do
            {
                curResult = GetReviewableHITs(hitTypeId, pageNum++);
                if (curResult != null)
                {
                    for (int i = 0; i < curResult.Count; i++)
                    {
                        yield return curResult[i];
                    }
                }
            } while (curResult != null && curResult.Count > 0);

            yield break;
        }


        /// <summary>
        /// Retrieves all of requester's assignments for the given HIT.
        /// </summary>
        /// <param name="hitId">The hit id.</param>
        /// <returns>A <see cref="Assignment"/> list</returns>
        public IList<Assignment> GetAllAssignmentsForHIT(string hitId)
        {
            List<Assignment> results = new List<Assignment>();

            int pageNum = 1;
            IList<Assignment> curResult = null;

            do
            {
                curResult = GetAssignmentsForHIT(hitId, pageNum++);
                if (curResult != null)
                {
                    results.AddRange(curResult);
                }
            } while (curResult != null && curResult.Count > 0);

            return results;
        }

        /// <summary>
        /// Enumerates through all of requester's assignments for the given HIT.
        /// </summary>
        /// <param name="hitId">The hit id.</param>
        public IEnumerable<Assignment> GetAllAssignmentsForHITIterator(string hitId)
        {
            int pageNum = 1;
            IList<Assignment> curResult = null;

            do
            {
                curResult = GetAssignmentsForHIT(hitId, pageNum++);
                if (curResult != null)
                {
                    for (int i = 0; i < curResult.Count; i++)
                    {
                        yield return curResult[i];
                    }
                }
            } while (curResult != null && curResult.Count > 0);

            yield break;
        }
        #endregion

        #endregion

        #region HIT serialization and deserialization
        /// <summary>
        /// Serializes the HIT in XML format and saves it to the specified file
        /// </summary>
        /// <param name="hit">HIT to serialize</param>
        /// <param name="fileName">Filename to save to</param>
        public void SerializeHIT(HIT hit, string fileName)
        {
            SerializeHIT(hit, fileName, MTurkSerializationFormat.Xml);
        }

        /// <summary>
        /// Loads and deserializes a HIT from the specified XML file (or xml fragment)
        /// </summary>
        /// <param name="fileNameOrContent">The file (or content) containing the HIT in 
        /// XML serialized form</param>
        public HIT DeserializeHIT(string fileNameOrContent)
        {
            return DeserializeHIT(fileNameOrContent, MTurkSerializationFormat.Xml);
        }

        /// <summary>
        /// Serializes the HIT in the specified format and saves it to the specified file
        /// </summary>
        /// <param name="hit">HIT to serialize</param>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="format">The format to use for serialization</param>
        public void SerializeHIT(HIT hit, string fileName, MTurkSerializationFormat format)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("No filename specified to save object to", "fileName");
            }

            using (StreamWriter writer = new StreamWriter(fileName))
            {
                SerializeHIT(hit, writer, format);
            }
        }

        /// <summary>
        /// Loads and deserializes a HIT from the specified file (or content string)
        /// </summary>
        /// <param name="fileNameOrContent">The file or content containing the object in the 
        /// specified format</param>
        /// <param name="format">The format of the file</param>
        public HIT DeserializeHIT(string fileNameOrContent, MTurkSerializationFormat format)
        {
            if (string.IsNullOrEmpty(fileNameOrContent))
            {
                throw new ArgumentException("No filename or content specified to load object from", "fileNameOrContent");
            }

            if (fileNameOrContent[0] == '<' || !File.Exists(fileNameOrContent))
            {
                using (StringReader reader = new StringReader(fileNameOrContent))
                {
                    return DeserializeHIT(reader, format);
                }
            }
            else
            {
                using (StreamReader reader = new StreamReader(fileNameOrContent))
                {
                    return DeserializeHIT(reader, format);
                }
            }
        }

        /// <summary>
        /// Serializes the HIT in the specified format and writes it to the specified writer
        /// </summary>
        /// <param name="hit">HIT to serialize</param>
        /// <param name="writer">The output writer.</param>
        /// <param name="format">The format to use for object serialization</param>
        public void SerializeHIT(HIT hit, TextWriter writer, MTurkSerializationFormat format)
        {

            if (writer == null)
            {
                throw new ArgumentNullException("writer", "No text writer specified to serialize object to");
            }

            GetFormatter(format).Format(writer, new HITData(hit));
        }

        /// <summary>
        /// Loads and deserializes the HIT from the specified reader
        /// </summary>
        /// <param name="reader">The input reader</param>
        /// <param name="format">The format of the file</param>
        public HIT DeserializeHIT(TextReader reader, MTurkSerializationFormat format)
        {
            HIT ret = null;
            if (reader == null)
            {
                throw new ArgumentNullException("reader", "No reader specified to load object from");
            }

            object o = GetFormatter(format).Parse(reader, typeof(HITData));

            if (o is HIT)
            {
                ret = (HIT)o;
            }
            else if (o is HITData)
            {
                ret = ((HITData)o).Proxy;
            }
            else
            {
                throw new InvalidDataException("Expected data of type hit. Got " + o.GetType().FullName);
            }

            return ret;
        }

        /// <summary>
        /// Returns the formatter implementation for the specified format
        /// </summary>
        private static IModelObjectFormatter GetFormatter(MTurkSerializationFormat format)
        {
            IModelObjectFormatter formatter = null;
            if (format == MTurkSerializationFormat.Xml)
            {
                formatter = XmlFormatter.Instance;
            }
            else if (format == MTurkSerializationFormat.Property)
            {
                formatter = PropertyFormatter.Instance;
            }
            else
            {
                throw new InvalidOperationException("Not implemented. Persisting objects to file currently is only supported for XML and property files");
            }

            return formatter;
        }

        #endregion

        #region Miscellaneous helpers
        /// <summary>
        /// Returns the URL, where the HIT can be viewed at the worker website or "n/a" if HIT was not 
        /// yet created or loaded.
        /// </summary>
        public string GetPreviewURL(string hitTypeID)
        {
            return Config.GetPreviewURL(hitTypeID);
        }
        #endregion

        #region ITurkConfig Members

        /// <summary>
        /// Returns the configuration used by the Mechanical Turk client.
        /// </summary>
        public MTurkConfig Config
        {
            get { return Proxy.Config; }
        }

        #endregion
    }
}
