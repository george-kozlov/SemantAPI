#region Copyright & license notice
/*
 * Copyright: Copyright (c) 2007 Amazon Technologies, Inc.
 * License:   Apache License, Version 2.0
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Amazon.WebServices.MechanicalTurk.Advanced
{
    /// <summary>
    /// Logs messages to the trace log. A <see cref="System.Diagnostics.TraceListener"/> can be used
    /// to capture this messages and output them to a custom sink.
    /// </summary>
    internal class TraceLogger : ILog
    {
        private static void LogMessage(string level, string format, params object[] args)
        {
            Trace.WriteLine(string.Format("[{0}_{1}] {2:yyyy-MM-ddTHH:mm:ss.fffZ} {3}", 
                AppDomain.CurrentDomain.FriendlyName, 
                System.Threading.Thread.CurrentThread.GetHashCode(), 
                DateTime.Now, string.Format(format, args)), 
                level);
        }

        #region ILog Members
        public void Debug(string format, params object[] args)
        {
            LogMessage("DEBUG", format, args);
        }

        public void Error(string format, params object[] args)
        {
            LogMessage("ERROR", format, args);
        }

        public void Info(string format, params object[] args)
        {
            LogMessage("INFO", format, args);
        }

        public void Warn(string format, params object[] args)
        {
            LogMessage("WARN", format, args);
        }
        #endregion
    }
}
