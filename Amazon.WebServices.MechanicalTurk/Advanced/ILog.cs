#region Copyright
/*
 * Amazon Mechanical Turk .Net SDK (API Version 2007-06-21)
 *
 * This software code is made available "AS IS" without warranties of any kind.  
 * You may copy, display, modify and redistribute the software code either by 
 * itself or as incorporated into your code; provided that you do not remove 
 * any proprietary notices.  Your use of this software code is at your own risk 
 * and you waive any claim against Amazon Web Services LLC or its affiliates with 
 * respect to your use of this software code. 
 * 
 * (c) Amazon Web Services LLC or its affiliates.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace Amazon.WebServices.MechanicalTurk.Advanced
{
    /// <summary>
    /// Logging interface used to log events
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Logs a warning to designate fine-grained informational events that are most useful 
        /// to debug an application.
        /// </summary>
        /// <param name="format">The log message containing 0 or more format items</param>
        /// <param name="args">The items to be formatted within the log message</param>
        void Debug(string format, params object[] args);
        /// <summary>
        /// Logs a warning to designate error events that might still allow the application 
        /// to continue running.
        /// </summary>
        /// <param name="format">The log message containing 0 or more format items</param>
        /// <param name="args">The items to be formatted within the log message</param>
        void Error(string format, params object[] args);
        /// <summary>
        /// Logs a warning to designate informational events that might be of interest to 
        /// the application administrator.
        /// </summary>
        /// <param name="format">The log message containing 0 or more format items</param>
        /// <param name="args">The items to be formatted within the log message</param>
        void Info(string format, params object[] args);
        /// <summary>
        /// Logs a warning to designate a potentially harmful situation.
        /// </summary>
        /// <param name="format">The log message containing 0 or more format items</param>
        /// <param name="args">The items to be formatted within the log message</param>
        void Warn(string format, params object[] args);
    }
}
