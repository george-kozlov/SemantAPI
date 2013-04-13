#region Copyright & license notice
/*
 * Copyright: Copyright (c) 2007 Amazon Technologies, Inc.
 * License:   Apache License, Version 2.0
 */
#endregion

using System;
using Amazon.WebServices.MechanicalTurk.Domain;


namespace Amazon.WebServices.MechanicalTurk.Advanced
{
    /// <summary>
    /// Interface for Mechanical Turk SOAP operations.
    /// </summary>
    /// <remarks>
    /// Detailed information about the operations and their parameters
    /// are maintained on 
    /// <a href="http://docs.amazonwebservices.com/AWSMechanicalTurkRequester/2007-06-21/">
    /// Amazon Mechanical Turk Developer Guide</a> site.
    /// </remarks>
    public interface ITurkOperations : ITurkConfig
    {
        /// <summary>
        /// The ApproveAssignment operation approves the results of a completed assignment.
        /// </summary>
        /// <param name="request">A <see cref="ApproveAssignmentRequest"/> instance containing 
        /// the request parameters</param>
        /// <remarks>
        /// Approving an assignment initiates two payments from the Requester's Amazon.com account:
        /// the Worker that submitted the results is paid the reward specified in the HIT, and
        /// Mechanical Turk fees are debited. If the Requester's account does not have adequate funds
        /// for these payments, the call to ApproveAssignment will return an exception, and the approval
        /// will not be processed.
        /// </remarks>
        void ApproveAssignment(ApproveAssignmentRequest request);

        /// <summary>
        /// The ApproveRejectedAssignment operation approves a rejected assignment. 
        /// </summary>
        /// <param name="request">A <see cref="ApproveRejectedAssignmentRequest"/> instance containing 
        /// the request parameters</param>
        /// <remarks>
        /// Approving a rejected assignment reverses the rejection of an assignment and peforms an approval.
        /// If the Requester's account does not have adequate funds for the payments associated with approval, 
        /// the call to ApproveRejectedAssignment will return an exception, and the approval
        /// will not be processed.
        /// </remarks>
        void ApproveRejectedAssignment(ApproveRejectedAssignmentRequest request);

        /// <summary>
        ///  The AssignQualification operation gives a Worker a Qualification. AssignQualification 
        /// does not require that the Worker submit a Qualification request: It gives the Qualification 
        /// directly to the Worker.
        /// </summary>
        /// <param name="request">A <see cref="AssignQualificationRequest"/> instance containing the 
        /// request parameters</param>
        /// <remarks>
        /// You can assign a Qualification to any Worker that has submitted one of your HITs in the past.
        /// You can only assign a Qualification of a Qualification type that you created.
        /// </remarks>
        void AssignQualification(AssignQualificationRequest request);

        /// <summary>
        ///  The CreateHIT operation creates a new HIT. The new HIT is made available for Workers to 
        /// find and accept on the Mechanical Turk web site.
        /// </summary>
        /// <param name="request">A <see cref="CreateHITRequest"/> instance containing 
        /// the request parameters</param>
        /// <remarks> Once a HIT has been created, it cannot be deleted. A HIT may be removed from the 
        /// web site using the DisableHIT operation, but Workers that have already accepted the HIT will 
        /// still be allowed to submit results to claim rewards. 
        /// See DisableHIT for more information.</remarks>
        /// <returns>A <see cref="HIT"/> instance</returns>
        HIT CreateHIT(CreateHITRequest request);


        /// <summary>
        /// The CreateQualificationType operation creates a new Qualification type.
        /// </summary>
        /// <param name="request">Type of the create qualification.</param>
        /// <returns>
        /// A <see cref="QualificationType"/> instance
        /// </returns>
        /// <remarks> Every Qualification has a Qualification type. The creator of the type can assign
        /// Qualifications of that type to Workers, and grant requests for Qualifications of the type 
        /// made by Workers. A Qualification can be considered a statement about a Worker made by the 
        /// Qualification type's owner. A Qualification type may include a Qualification test, a set 
        /// of questions a Worker must answer to request the Qualification. The type may also include 
        /// an answer key for the test. Qualification requests for types with answer keys are granted 
        /// automatically by Mechanical Turk, using a value calculated from the answer key and the 
        /// Worker's test answers. If the Qualification type does not have a test, or does
        /// not have an answer key, the type's owner is responsible for polling for and granting 
        /// Qualification requests.</remarks>
        QualificationType CreateQualificationType(CreateQualificationTypeRequest request);

        /// <summary>
        /// The DisableHIT operation removes a HIT from the Mechanical Turk marketplace, approves all 
        /// submitted assignments that have not already been approved or rejected, and disposes of the 
        /// HIT and all assignment data.
        /// </summary>
        /// <param name="request">A <see cref="DisableHITRequest"/> instance containing 
        /// the request parameters</param>
        /// <remarks> Assignments for the HIT that have already been submitted, but not yet approved 
        /// or rejected, will be automatically approved. Assignments in progress at the time of the call 
        /// to DisableHIT will be approved once the assignments are submitted. You will be charged for approval of these assignments.</remarks>
        void DisableHIT(DisableHITRequest request);

        /// <summary>
        /// The DisposeHIT operation disposes of a HIT that is no longer needed.
        /// </summary>
        /// <param name="request">A <see cref="DisposeHITRequest"/> instance containing the 
        /// request parameters</param>
        /// <remarks> Only HITs in the "reviewable" state, with all submitted assignments 
        /// approved or rejected, can be disposed. A Requester can call GetReviewableHITs
        /// to determine which HITs are reviewable, then call GetAssignmentsForHIT to retrieve
        /// the assignments. Disposing of a HIT removes the HIT from the results of a call to 
        /// GetReviewableHITs. If DisposeHIT is called on a HIT that is not "reviewable"
        /// (that has not expired or has active assignments), or on a HIT that is "reviewable"
        /// but not all of the submitted assignments have been approved or rejected, the service 
        /// will return an error.</remarks>
        void DisposeHIT(DisposeHITRequest request);


        /// <summary>
        ///  The ExtendHIT operation increases the maximum number of assignments, or extends 
        /// the expiration date, of an existing HIT.
        /// </summary>
        /// <param name="request">A <see cref="ExtendHITRequest"/> instance containing 
        /// the request parameters</param>
        /// <remarks> If a HIT is not assignable (with a status of Unassignable or Reviewable) 
        /// due to either having reached its maximum number of assignments or having reached 
        /// its expiration date, extending the HIT can make it available again.</remarks>
        void ExtendHIT(ExtendHITRequest request);


        /// <summary>
        /// The ForceExpireHIT operation causes a HIT to expire immediately, as if the 
        /// HIT's <c>LifetimeInSeconds</c> had elapsed.
        /// </summary>
        /// <param name="request">A <see cref="ForceExpireHITRequest"/> instance containing 
        /// the request parameters</param>
        /// <remarks> The effect is identical to the HIT expiring on its own: The HIT no 
        /// longer appears on the Mechanical Turk web site, and no new Workers are allowed 
        /// to accept the HIT. Workers who have accepted the HIT prior to expiration 
        /// are allowed to complete it or return it, or allow the assignment duration to 
        /// elapse (abandon the HIT). Once all remaining assignments have been submitted, 
        /// the expired HIT becomes "reviewable", and will be returned by a call 
        /// to GetReviewableHITs.</remarks>
        void ForceExpireHIT(ForceExpireHITRequest request);

        /// <summary>
        /// The GetAccountBalance operation retrieves the amount of money your 
        /// Amazon Mechanical Turk account, as well as the amount of money "on hold" pending 
        /// the completion of transfers from your bank account to your Amazon account.
        /// </summary>
        /// <param name="request">A <see cref="GetAccountBalanceRequest"/> instance containing 
        /// the request parameters</param>
        /// <returns>
        /// A <see cref="GetAccountBalanceResult"/> instance
        /// </returns>
        GetAccountBalanceResult GetAccountBalance(GetAccountBalanceRequest request);

        /// <summary>
        ///  The GetAssignment operation retrieves an assignment. 
        /// </summary>
        /// <param name="request">A <see cref="GetAssignmentRequest"/> instance containing 
        /// the request parameters</param>
        /// <remarks> You can get an assignment at any time, even if the HIT is not 
        /// yet "reviewable". </remarks>
        /// <returns>
        /// A <see cref="GetAssignmentResult"/> instance
        /// </returns>
        GetAssignmentResult GetAssignment(GetAssignmentRequest request);

        /// <summary>
        ///  The GetAssignmentsForHIT operation retrieves completed assignments for a HIT. 
        /// You can use this operation to retrieve the results for a HIT.
        /// </summary>
        /// <param name="request">A <see cref="GetAssignmentsForHITRequest"/> instance containing 
        /// the request parameters</param>
        /// <remarks> You can get assignments for a HIT at any time, even if the HIT is not 
        /// yet "reviewable". If a HIT requested multiple assignments, and has received some 
        /// results but has not yet become "reviewable", you can still retrieve the partial 
        /// results with GetAssignmentsForHIT.</remarks>
        /// <returns>
        /// A <see cref="GetAssignmentsForHITResult"/> instance
        /// </returns>
        GetAssignmentsForHITResult GetAssignmentsForHIT(GetAssignmentsForHITRequest request);

        /// <summary>
        ///  The GetBonusPayments operation retrieves the amounts of bonuses you have paid to 
        /// Workers for a given HIT or assignment.
        /// </summary>
        /// <param name="request">A <see cref="GetBonusPaymentsRequest"/> instance containing 
        /// the request parameters</param>
        /// <returns>
        /// A <see cref="GetBonusPaymentsResult"/> instance
        /// </returns>
        GetBlockedWorkersResult GetBlockedWorkers(GetBlockedWorkersRequest request);

        /// <summary>
        ///  The GetBonusPayments operation retrieves the amounts of bonuses you have paid to 
        /// Workers for a given HIT or assignment.
        /// </summary>
        /// <param name="request">A <see cref="GetBonusPaymentsRequest"/> instance containing 
        /// the request parameters</param>
        /// <returns>
        /// A <see cref="GetBonusPaymentsResult"/> instance
        /// </returns>
        GetBonusPaymentsResult GetBonusPayments(GetBonusPaymentsRequest request);

        /// <summary>
        /// The GetFileUploadURL operation generates and returns a temporary URL for the 
        /// purposes of retrieving a file uploaded by a Worker as an answer to a FileUploadAnswer 
        /// question for a HIT.
        /// </summary>
        /// <param name="request">A <see cref="GetFileUploadURLRequest"/> instance containing 
        /// the request parameters</param>
        /// <remarks> The temporary URL is generated the instant the GetFileUploadURL operation 
        /// is called, and is valid for 60 seconds.</remarks>
        /// <returns>A <see cref="String"/> containing the upload URL</returns>
        string GetFileUploadURL(GetFileUploadURLRequest request);

        /// <summary>
        /// The GetHIT operation retrieves the details of a HIT, using its HIT ID.
        /// </summary>
        /// <param name="request">A <see cref="GetHITRequest"/> instance containing the 
        /// request parameters</param>
        /// <returns>A <see cref="HIT"/> instance</returns>
        HIT GetHIT(GetHITRequest request);

        /// <summary>
        ///  The GetHITsForQualificationType operation returns the HITs that use the given 
        /// Qualification type for a Qualification requirement.
        /// </summary>
        /// <param name="request">Type of the get HITs for qualification.</param>
        /// <remarks> The operation returns HITs of any status, except for HITs that 
        /// have been disposed with the DisposeHIT  operation.
        /// <para></para>
        /// Only HITs that you created will be returned by the query. </remarks>
        /// <returns>A <see cref="GetHITsForQualificationTypeResult"/> instance</returns>
        GetHITsForQualificationTypeResult GetHITsForQualificationType(GetHITsForQualificationTypeRequest request);

        /// <summary>
        ///  The GetQualificationsForQualificationType operation returns all of the Qualifications 
        /// granted to Workers for a given Qualification type.
        /// </summary>
        /// <param name="request">A <see cref="GetQualificationsForQualificationTypeRequest"/> instance 
        /// containing the request parameters</param>
        /// <remarks> Results are divided into numbered "pages," and a single page of results is 
        /// returned by the operation. Pagination can be controlled with parameters to the operation.</remarks>
        /// <returns>A <see cref="GetQualificationsForQualificationTypeResult"/> instance</returns>
        GetQualificationsForQualificationTypeResult GetQualificationsForQualificationType(GetQualificationsForQualificationTypeRequest request);

        /// <summary>
        /// The GetQualificationRequests operation retrieves requests for Qualifications of a particular 
        /// Qualification type. The Qualification type's owner calls this operation to poll for 
        /// pending requests, and grants Qualifications based on the requests using the 
        /// GrantQualification operation.
        /// </summary>
        /// <param name="request">A <see cref="GetQualificationRequestsRequest"/> instance containing 
        /// the request parameters</param>
        /// <remarks> Only requests for Qualifications that require the type owner's attention are 
        /// returned by GetQualificationRequests. Requests awaiting Qualification test answers, and 
        /// requests that have already been granted, are not returned.
        /// <para></para>
        /// Only the owner of the Qualification type can retrieve its requests. </remarks>
        /// <returns>
        /// A <see cref="GetQualificationRequestsResult"/> instance
        /// </returns>
        GetQualificationRequestsResult GetQualificationRequests(GetQualificationRequestsRequest request);

        /// <summary>
        /// The GetQualificationScore operation returns the value of a user's Qualification for a 
        /// given Qualification type.
        /// </summary>
        /// <param name="request">A <see cref="GetQualificationScoreRequest"/> instance containing 
        /// the request parameters</param>
        /// <remarks> To get a user's Qualification, you must know the user's ID. A Worker's user ID 
        /// is included in the assignment data returned by the GetAssignmentsForHIT operation.</remarks>
        /// <returns>A <see cref="Qualification"/> instance</returns>
        Qualification GetQualificationScore(GetQualificationScoreRequest request);

        /// <summary>
        /// The GetQualificationType operation retrieves information about a Qualification type 
        /// using its ID.
        /// </summary>
        /// <param name="request">Type of the get qualification.</param>
        /// <returns>A <see cref="QualificationType"/> instance</returns>
        QualificationType GetQualificationType(GetQualificationTypeRequest request);

        /// <summary>
        /// The GetRequesterStatistic operation retrieves the value of one of several statistics 
        /// about you (the Requester calling the operation).
        /// </summary>
        /// <param name="request">A <see cref="GetRequesterStatisticRequest"/> instance containing 
        /// the request parameters</param>
        /// <remarks>
        ///  Mechanical Turk keeps track of many statistics about users and system activity. 
        /// Statistics are calculated and recorded for each calendar day. GetRequesterStatistic  
        /// can return data points for each of multiple days up to the current day, or an aggregate 
        /// value for a longer time period up to the current day.
        /// <para></para>
        ///  A single day's statistic represents the change in an overall value that has resulted 
        /// from the day's activity. For example, the NumberAssignmentsApproved statistic reports 
        /// the number of assignments you have approved in a given day. If you do not approve any 
        /// assignments for a day, the value will be 0 for that day. If you approve fifty 
        /// assignments that day, the value will be 50. 
        /// </remarks>
        /// <returns>
        /// A <see cref="GetStatisticResult"/> instance
        /// </returns>
        GetStatisticResult GetRequesterStatistic(GetRequesterStatisticRequest request);

        /// <summary>
        /// The GetRequesterStatistic operation retrieves the value of one of several statistics 
        /// about you (the Requester calling the operation).
        /// </summary>
        /// <param name="request">A <see cref="GetRequesterStatisticRequest"/> instance containing 
        /// the request parameters</param>
        /// <remarks>
        ///  Mechanical Turk keeps track of many statistics about users and system activity. 
        /// Statistics are calculated and recorded for each calendar day. GetRequesterStatistic  
        /// can return data points for each of multiple days up to the current day, or an aggregate 
        /// value for a longer time period up to the current day.
        /// <para></para>
        ///  A single day's statistic represents the change in an overall value that has resulted 
        /// from the day's activity. For example, the NumberAssignmentsApproved statistic reports 
        /// the number of assignments you have approved in a given day. If you do not approve any 
        /// assignments for a day, the value will be 0 for that day. If you approve fifty 
        /// assignments that day, the value will be 50. 
        /// </remarks>
        /// <returns>
        /// A <see cref="GetStatisticResult"/> instance
        /// </returns>
        GetStatisticResult GetRequesterWorkerStatistic(GetRequesterWorkerStatisticRequest request);

        /// <summary>
        ///  The GetReviewableHITs operation retrieves the HITs that have a status of Reviewable, 
        /// or HITs that have a status of Reviewing, and that belong to the Requester calling the 
        /// operation.
        /// </summary>
        /// <param name="request">A <see cref="GetReviewableHITsRequest"/> instance containing 
        /// the request parameters</param>
        /// <remarks> You can use this operation to determine which of your HITs have results, 
        /// then retrieve those results with the GetAssignmentsForHIT operation. Once a HIT's 
        /// results have been retrieved and the assignments have been approved or rejected 
        /// (with ApproveAssignment or RejectAssignment), you can call DisposeHIT to remove 
        /// the HIT from the results of a call to GetReviewableHITs.</remarks>
        /// <returns>
        /// A <see cref="GetReviewableHITsResult"/> instance
        /// </returns>
        GetReviewableHITsResult GetReviewableHITs(GetReviewableHITsRequest request);

        /// <summary>
        /// Get a list of review policy results for a HIT. This is only applicable to HITs that were created with an assignment-level or hit-level review policy.
        /// </summary>
        /// <param name="request">A <see cref="GetReviewResultsForHITRequest"/> instance containing the request parameters</param>
        /// <remarks>
        /// Use this operation to see what the results of your review policies were, including a list of any actions taken on your behalf.
        /// </remarks>
        /// <returns>
        /// A <see cref="GetReviewResultsForHITResult"/> instance
        /// </returns>
        GetReviewResultsForHITResult GetReviewResultsForHIT(GetReviewResultsForHITRequest request);

        /// <summary>
        ///  The GrantBonus operation issues a payment of money from your account to a Worker. 
        /// To be eligible for a bonus, the Worker must have submitted results for one of your HITs, 
        /// and have had those results approved or rejected. This payment happens separately from 
        /// the reward you pay to the Worker when you approve the Worker's assignment.
        /// </summary>
        /// <param name="request">A <see cref="GrantBonusRequest"/> instance containing 
        /// the request parameters</param>
        void GrantBonus(GrantBonusRequest request);

        /// <summary>
        ///  The GrantQualification operation grants a user's request for a Qualification.
        /// </summary>
        /// <param name="request">A <see cref="GrantQualificationRequest"/> instance containing 
        /// the request parameters</param>
        /// <remarks> Only the owner of the Qualification type can grant a Qualification request 
        /// for that type.</remarks>
        void GrantQualification(GrantQualificationRequest request);

        /// <summary>
        /// The Help operation returns information about the Mechanical Turk Service operations 
        /// and response groups. You can use it to facilitate development and documentation of 
        /// your web site and tools.
        /// </summary>
        /// <param name="request">A <see cref="HelpRequest"/> instance containing 
        /// the request parameters</param>
        /// <returns>A <see cref="Information"/> instance</returns>
        Information Help(HelpRequest request);

        /// <summary>
        /// The NotifyWorkers operation sends e-mail to one or more Workers, given the 
        /// recipients' Worker IDs.
        /// </summary>
        /// <param name="request">A <see cref="NotifyWorkersRequest"/> instance containing 
        /// the request parameters</param>
        /// <remarks>
        ///  Worker IDs are included in the assignment data returned by GetAssignmentsForHIT. 
        /// You can send e-mail to any Worker who has ever submitted results for a HIT you created 
        /// that you have approved or rejected.
        ///  <para></para>The e-mail sent to Workers includes your e-mail address as the "reply-to" 
        ///  address, so Workers can respond to the e-mail</remarks>
        void NotifyWorkers(NotifyWorkersRequest request);

        /// <summary>
        /// The RegisterHITType operation creates a new HIT type, a set of HIT properties which 
        /// can be used to create new HITs.
        /// </summary>
        /// <param name="request">A <see cref="RegisterHITTypeRequest"/> instance containing 
        /// the request parameters</param>
        /// <returns>The ID of the new HIT type</returns>
        string RegisterHITType(RegisterHITTypeRequest request);

        /// <summary>
        /// The RejectAssignment operation rejects the results of a completed assignment.
        /// </summary>
        /// <param name="request">A <see cref="RejectAssignmentRequest"/> instance containing 
        /// the request parameters</param>
        /// <remarks> Rejecting an assignment indicates that the Requester believes the results 
        /// submitted by the Worker do not properly answer the question described by the HIT. 
        /// The Worker is not paid for a rejected assignment.</remarks>
        void RejectAssignment(RejectAssignmentRequest request);

        /// <summary>
        /// The RejectQualificationRequest operation rejects a user's request for a Qualification. 
        /// Once a Qualification request is rejected, 
        /// it will no longer be returned by a call to the GetQualificationRequests operation.
        /// </summary>
        /// <param name="request">A <see cref="RejectQualificationRequestRequest"/> instance 
        /// containing the request parameters</param>
        /// <remarks> Rejecting the Qualification request does not change the user's Qualifications: 
        /// If the user already has a Qualification of the corresponding Qualification type, the user 
        /// will continue to have the Qualification with the previously assigned score. If the user 
        /// does not have the Qualification, the user will still not have it after the 
        /// request is rejected.</remarks>
        void RejectQualificationRequest(RejectQualificationRequestRequest request);

        /// <summary>
        /// The RevokeQualification operation revokes a previously granted Qualification from a user. 
        /// If the user had a Qualification of the given Qualification type, after revoking it, the 
        /// user will no longer have the Qualification, and will not qualify for HITs whose 
        /// Qualification requirements say the user must have the Qualification.
        /// </summary>
        /// <param name="request">A <see cref="RevokeQualificationRequest"/> instance containing 
        /// the request parameters</param>
        /// <remarks> Once a Qualification is revoked from a user, the user cannot be granted the 
        /// Qualification until the user requests the Qualification again. Depending on how the 
        /// Qualification type's retry policy is configured, the user may be restricted from 
        /// requesting the Qualification a second time.</remarks>
        void RevokeQualification(RevokeQualificationRequest request);

        /// <summary>
        /// The SearchHITs operation returns all of a Requester's HITs, on behalf of the Requester.
        /// </summary>
        /// <param name="request">A <see cref="SearchHITsRequest"/> instance containing 
        /// the request parameters</param>
        /// <remarks> The operation returns HITs of any status, except for HITs that have been 
        /// disposed with the DisposeHIT  operation.</remarks>
        /// <returns>
        /// A <see cref="SearchHITsResult"/> instance
        /// </returns>
        SearchHITsResult SearchHITs(SearchHITsRequest request);

        /// <summary>
        ///  The SearchQualificationTypes operation searches for Qualification types using the 
        /// specified search query, and returns a list of Qualification types.
        /// </summary>
        /// <param name="request">A <see cref="SearchQualificationTypesRequest"/> instance 
        /// containing the request parameters</param>
        /// <remarks> Results are sorted and divided into numbered "pages," and a single page 
        /// of results is returned by the operation. 
        /// Sorting and pagination can be controlled with parameters to the operation.</remarks>
        /// <returns>
        /// A <see cref="SearchQualificationTypesResult"/> instance
        /// </returns>
        SearchQualificationTypesResult SearchQualificationTypes(SearchQualificationTypesRequest request);

        /// <summary>
        /// The SendTestEventNotification operation causes Mechanical Turk to send a notification 
        /// message as if a HIT event occurred, according to the provided notification specification. 
        /// This allows you to test your notification receptor logic without setting up notifications 
        /// for a real HIT type and trying to trigger them using the web site.
        /// </summary>
        /// <param name="request">A <see cref="SendTestEventNotificationRequest"/> instance containing 
        /// the request parameters</param>
        void SendTestEventNotification(SendTestEventNotificationRequest request);

        /// <summary>
        /// The SetHITAsReviewing operation updates a HIT with a status of Reviewable to have a  
        /// status of Reviewing, or reverts a Reviewing HIT back to the Reviewable status.
        /// </summary>
        /// <param name="request">A <see cref="SetHITAsReviewingRequest"/> instance containing 
        /// the request parameters</param>
        /// <remarks> The Reviewable and Reviewing status values for a HIT control which HITs are 
        /// returned by the GetReviewableHITs operation. A HIT's status is also returned with a 
        /// HIT's data, such as by a call to the GetHIT operation. Your application can manipulate 
        /// and query these status values as part of the HIT review process. For example, if 
        /// verification for a HIT's results is pending further information, the HIT can be moved 
        /// to the Reviewing status to prevent it from being returned by subsequent calls to 
        /// GetReviewableHITs.</remarks>
        void SetHITAsReviewing(SetHITAsReviewingRequest request);

        /// <summary>
        /// The SetHITTypeNotification operation creates, updates, disables or re-enables 
        /// notifications for a HIT type.
        /// </summary>
        /// <param name="request">A <see cref="SetHITTypeNotificationRequest"/> instance containing 
        /// the request parameters</param>
        /// <remarks> When a HIT type has an active notification, Mechanical Turk will attempt to 
        /// send a notification message when a HIT of the type changes state, such as when an assignment 
        /// is submitted for the HIT. The state changes to watch and the method of notification are described 
        /// in the notification specification given to SetHITTypeNotification.</remarks>
        void SetHITTypeNotification(SetHITTypeNotificationRequest request);

        /// <summary>
        ///  The UpdateQualificationScore operation changes the value of a Qualification 
        /// previously granted to a user.
        /// </summary>
        /// <param name="request">A <see cref="UpdateQualificationScoreRequest"/> instance 
        /// containing the request parameters</param>
        /// <remarks> To update a user's Qualification, you must know the user's ID. A 
        /// Worker's user ID is included in the assignment data returned by the 
        /// GetAssignmentsForHIT operation.</remarks>
        void UpdateQualificationScore(UpdateQualificationScoreRequest request);

        /// <summary>
        /// The UpdateQualificationType operation modifies attributes of an existing 
        /// Qualification type.
        /// </summary>
        /// <param name="request">A <see cref="UpdateQualificationTypeRequest"/> instance 
        /// containing the request parameters</param>
        /// <remarks> Most attributes of a Qualification type can be changed after the type 
        /// has been created. The Name and Keywords fields cannot be modified. If you create 
        /// a Qualification type and decide you do not wish to use it with its name or keywords 
        /// as they were created, update the type with a new QualificationTypeStatus of Inactive, 
        /// then create a new type using CreateQualificationType with the desired values.</remarks>
        /// <returns>A <see cref="QualificationType"/> instance</returns>
        QualificationType UpdateQualificationType(UpdateQualificationTypeRequest request);

        /// <summary>
        /// Blocks a worker from accepting your HITs
        /// </summary>
        /// <param name="request">A <see cref="BlockWorkerRequest"/> instance containing the 
        /// request parameters</param>
        void BlockWorker(BlockWorkerRequest request);

        /// <summary>
        /// Unblocks a worker who was previously blocked from accepting your HITs
        /// </summary>
        /// <param name="request">A <see cref="UnblockWorkerRequest"/> instance containing the 
        /// request parameters</param>
        void UnblockWorker(UnblockWorkerRequest request);

        /// <summary>
        /// Changes the HIT type for a HIT
        /// </summary>
        /// <param name="request">A <see cref="UnblockWorkerRequest"/> instance containing the 
        /// request parameters</param>
        void ChangeHITTypeOfHIT(ChangeHITTypeOfHITRequest request);
    }
}
