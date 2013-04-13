#region Copyright & license notice
/*
 * Copyright: Copyright (c) 2007 Amazon Technologies, Inc.
 * License:   Apache License, Version 2.0
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Amazon.WebServices.MechanicalTurk.Domain;

namespace Amazon.WebServices.MechanicalTurk.Advanced
{
    /// <summary>
    /// HIT serialization helper (<see cref="SimpleClient"/>)
    /// </summary>
    public class HITData
    {       
        #region Domain Object Adapter
        private HIT _hit = new HIT();
        internal HIT Proxy
        {
            get { return (this._hit); }
            set { this._hit = value; }
        }

        /// <summary>
        /// Gets or sets the HIT id.
        /// </summary>
        /// <value>The HIT id.</value>
        public string HITId
        {
            get { return (Proxy.HITId); }
            set { Proxy.HITId = value; }
        }

        /// <summary>
        /// Gets or sets the HIT type id.
        /// </summary>
        /// <value>The HIT type id.</value>
        public string HITTypeId
        {
            get { return (Proxy.HITTypeId); }
            set { Proxy.HITTypeId = value; }
        }


        /// <summary>
        /// Gets or sets the creation time.
        /// </summary>
        /// <value>The creation time.</value>
        public System.DateTime? CreationTime
        {
            get
            {
                if (Proxy.CreationTimeSpecified)
                {
                    return Proxy.CreationTime;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value.HasValue)
                {
                    Proxy.CreationTime = value.Value;
                }
                Proxy.CreationTimeSpecified = value.HasValue;
            }
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title
        {
            get { return (Proxy.Title); }
            set { Proxy.Title = value; }
        }


        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description
        {
            get { return (Proxy.Description); }
            set { Proxy.Description = value; }
        }


        /// <summary>
        /// Gets or sets the question.
        /// </summary>
        /// <value>The question.</value>
        public string Question
        {
            get { return (Proxy.Question); }
            set { Proxy.Question = value; }
        }


        /// <summary>
        /// Gets or sets the keywords.
        /// </summary>
        /// <value>The keywords.</value>
        public string Keywords
        {
            get { return (Proxy.Keywords); }
            set { Proxy.Keywords = value; }
        }


        /// <summary>
        /// Gets or sets the HIT status.
        /// </summary>
        /// <value>The HIT status.</value>
        public HITStatus? HITStatus
        {
            get
            {
                if (Proxy.HITStatusSpecified)
                {
                    return Proxy.HITStatus;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value.HasValue)
                {
                    Proxy.HITStatus = value.Value;
                }
                Proxy.HITStatusSpecified = value.HasValue;
            }
        }

        /// <summary>
        /// Gets or sets the max assignments.
        /// </summary>
        /// <value>The max assignments.</value>
        public int? MaxAssignments
        {
            get
            {
                if (Proxy.MaxAssignmentsSpecified)
                {
                    return Proxy.MaxAssignments;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value.HasValue)
                {
                    Proxy.MaxAssignments = value.Value;
                }
                Proxy.MaxAssignmentsSpecified = value.HasValue;
            }
        }

        /// <summary>
        /// Gets or sets the reward.
        /// </summary>
        /// <value>The reward.</value>
        public Price Reward
        {
            get { return (Proxy.Reward); }
            set { Proxy.Reward = value; }
        }

        /// <summary>
        /// Gets or sets the auto approval delay in seconds.
        /// </summary>
        /// <value>The auto approval delay in seconds.</value>
        public long? AutoApprovalDelayInSeconds
        {
            get
            {
                if (Proxy.AutoApprovalDelayInSecondsSpecified)
                {
                    return Proxy.AutoApprovalDelayInSeconds;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value.HasValue)
                {
                    Proxy.AutoApprovalDelayInSeconds = value.Value;
                }
                Proxy.AutoApprovalDelayInSecondsSpecified = value.HasValue;
            }
        }

        /// <summary>
        /// Gets or sets the expiration.
        /// </summary>
        /// <value>The expiration.</value>
        public System.DateTime? Expiration
        {
            get
            {
                if (Proxy.ExpirationSpecified)
                {
                    return Proxy.Expiration;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value.HasValue)
                {
                    Proxy.Expiration = value.Value;
                }
                Proxy.ExpirationSpecified = value.HasValue;
            }
        }

        /// <summary>
        /// Gets or sets the requester annotation.
        /// </summary>
        /// <value>The requester annotation.</value>
        public string RequesterAnnotation
        {
            get { return (Proxy.RequesterAnnotation); }
            set { Proxy.RequesterAnnotation = value; }
        }

        /// <summary>
        /// Gets or sets the assignment duration in seconds.
        /// </summary>
        /// <value>The assignment duration in seconds.</value>
        public long? AssignmentDurationInSeconds
        {
            get
            {
                if (Proxy.AssignmentDurationInSecondsSpecified)
                {
                    return Proxy.AssignmentDurationInSeconds;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value.HasValue)
                {
                    Proxy.AssignmentDurationInSeconds = value.Value;
                }
                Proxy.AssignmentDurationInSecondsSpecified = value.HasValue;
            }
        }

        /// <summary>
        /// Gets or sets the HIT review status.
        /// </summary>
        /// <value>The HIT review status.</value>
        public HITReviewStatus? HITReviewStatus
        {
            get
            {
                if (Proxy.HITReviewStatusSpecified)
                {
                    return Proxy.HITReviewStatus;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value.HasValue)
                {
                    Proxy.HITReviewStatus = value.Value;
                }
                Proxy.HITReviewStatusSpecified = value.HasValue;
            }
        }

        /// <summary>
        /// Gets or sets the number of assignments pending.
        /// </summary>
        /// <value>The number of assignments pending.</value>
        public int? NumberOfAssignmentsPending
        {
            get
            {
                if (Proxy.NumberOfAssignmentsPendingSpecified)
                {
                    return Proxy.NumberOfAssignmentsPending;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value.HasValue)
                {
                    Proxy.NumberOfAssignmentsPending = value.Value;
                }
                Proxy.NumberOfAssignmentsPendingSpecified = value.HasValue;
            }
        }

        /// <summary>
        /// Gets or sets the number of assignments available.
        /// </summary>
        /// <value>The number of assignments available.</value>
        public int? NumberOfAssignmentsAvailable
        {
            get
            {
                if (Proxy.NumberOfAssignmentsAvailableSpecified)
                {
                    return Proxy.NumberOfAssignmentsAvailable;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value.HasValue)
                {
                    Proxy.NumberOfAssignmentsAvailable = value.Value;
                }
                Proxy.NumberOfAssignmentsAvailableSpecified = value.HasValue;
            }
        }

        /// <summary>
        /// Gets or sets the number of assignments completed.
        /// </summary>
        /// <value>The number of assignments completed.</value>
        public int? NumberOfAssignmentsCompleted
        {
            get
            {
                if (Proxy.NumberOfAssignmentsCompletedSpecified)
                {
                    return Proxy.NumberOfAssignmentsCompleted;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value.HasValue)
                {
                    Proxy.NumberOfAssignmentsCompleted = value.Value;
                }
                Proxy.NumberOfAssignmentsCompletedSpecified = value.HasValue;
            }
        }

        /// <summary>
        /// Gets or sets the lifetime in seconds.
        /// </summary>
        /// <value>The lifetime in seconds.</value>
        public long? LifetimeInSeconds
        {
            get
            {
                if (Proxy.ExpirationSpecified)
                {
                    return (long)((Proxy.Expiration - DateTime.Now).TotalSeconds);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value.HasValue)
                {
                    Proxy.Expiration = DateTime.Now.AddSeconds(value.Value);
                }
                Proxy.ExpirationSpecified = value.HasValue;
            }

        }

        /// <summary>
        /// Gets or sets the qualification requirements.
        /// </summary>
        /// <value>The qualification requirements.</value>
        public QualificationRequirement[] QualificationRequirements
        {
            get 
            {
                return Proxy.QualificationRequirement;
            }
            set 
            {
                Proxy.QualificationRequirement = value;
            }
        }        
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="HITData"/> class.
        /// </summary>
        public HITData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HITData"/> class.
        /// </summary>
        /// <param name="hit">The hit.</param>
        public HITData(HIT hit)
        {
            _hit = hit;
        }
        #endregion
    }
}
