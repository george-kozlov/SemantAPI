#region Copyright & license notice
/*
 * Copyright: Copyright (c) 2007 Amazon Technologies, Inc.
 * License:   Apache License, Version 2.0
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace Amazon.WebServices.MechanicalTurk
{
    /// <summary>
    /// Miscellaneous Mechanical Turk constants
    /// </summary>
    public sealed class MTurkConstants
    {
        private MTurkConstants()
        {
        }

        /// <summary>
        /// Notification version used
        /// </summary>
        public const string NotificationVersion = "2006-05-05";

        /// <summary>
        /// Full response for HITs
        /// </summary>
        public static string[] ResponseGroupFullHIT 
            = new string[] { "Minimal", "HITDetail", "HITQuestion", "HITAssignmentSummary" };

        /// <summary>
        /// Full response for assignments
        /// </summary>
        public static string[] ResponseGroupFullAssignment 
            = new string[] { "Minimal", "AssignmentFeedback" };
    }

    /// <summary>
    /// Values for the various system qualification types
    /// </summary>
    public sealed class MTurkSystemQualificationTypes
    {
        private MTurkSystemQualificationTypes()
        {
        }

        /// <summary>
        /// System qualification identifier for abandonment rate
        /// </summary>
        public const string AbandonmentRateQualification = "00000000000000000070";

        /// <summary>
        /// System qualification identifier for approval rate
        /// </summary>
        public const string ApprovalRateQualification = "000000000000000000L0";

        /// <summary>
        /// System qualification identifier for rejection rate
        /// </summary>
        public const string RejectionRateQualification = "000000000000000000S0";

        /// <summary>
        /// System qualification identifier for return rate
        /// </summary>
        public const string ReturnRateQualification = "000000000000000000E0";

        /// <summary>
        /// System qualification identifier for submission rate
        /// </summary>
        public const string SubmissionRateQualification = "00000000000000000000";

        /// <summary>
        /// System qualification identifier for worker locale
        /// </summary>
        public const string LocaleQualification = "00000000000000000071";
    }

    /// <summary>
    /// Supported file types for model objects
    /// </summary>
    public enum MTurkSerializationFormat
    {
        /// <summary>
        /// XML format
        /// </summary>
        Xml,
        /// <summary>
        /// MTurk property format
        /// </summary>
        Property
    }

}
