#region Copyright & license notice
/*
 * Copyright: Copyright (c) 2007 Amazon Technologies, Inc.
 * License:   Apache License, Version 2.0
 */
#endregion

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;

namespace Amazon.WebServices.MechanicalTurk.Advanced
{
    /// <summary>
    /// Logger used by the SDK to log events.
    /// </summary>
    public sealed class MTurkLog 
    {
        private static ILog _logger = new TraceLogger();

        private MTurkLog()
        {
        }

        /// <summary>
        /// Sets the logger used to log SDK messages. By default, the a trace logger is
        /// used, but any other class implementing the <see cref="ILog"/> can be set
        /// here to replace this default logger. As such it is easy to hook in other
        /// log mechanisms and frameworks such as 
        /// <a href="http://logging.apache.org/log4net/">log4net</a>, 
        /// the <a href="http://msdn2.microsoft.com/en-us/library/Aa480464.aspx">
        /// Microsoft logging application block</a>
        /// or simply the event log.
        /// </summary>
        public static ILog Logger
        {
            get { return _logger; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", "Can't set null logger");
                }

                MTurkLog.Info("Setting logger of type '{0}'", value.GetType().FullName);

                _logger = value;

                MTurkLog.Info("New logger of type '{0}' set", value.GetType().FullName);
            }            
        }

        /// <summary>
        /// Logs a warning to designate fine-grained informational events that are most 
        /// useful to debug an application.
        /// </summary>
        /// <param name="format">The log message containing 0 or more format items</param>
        /// <param name="args">The items to be formatted within the log message</param>
        internal static void Debug(string format, params object[] args)
        {
            if (Level <= 1)
            {
                _logger.Debug(format, args);
            }
        }

        /// <summary>
        /// Logs a warning to designate error events that might still allow the application 
        /// to continue running.
        /// </summary>
        /// <param name="format">The log message containing 0 or more format items</param>
        /// <param name="args">The items to be formatted within the log message</param>
        internal static void Info(string format, params object[] args)
        {
            if (Level <= 2)
            {
                _logger.Info(format, args);
            }
        }

        /// <summary>
        /// Logs a warning to designate informational events that might be of interest to 
        /// the application administrator.
        /// </summary>
        /// <param name="format">The log message containing 0 or more format items</param>
        /// <param name="args">The items to be formatted within the log message</param>
        internal static void Warn(string format, params object[] args)
        {
            if (Level <= 3)
            {
                _logger.Warn(format, args);
            }
        }

        /// <summary>
        /// Logs a warning to designate a potentially harmful situation.
        /// </summary>
        /// <param name="format">The log message containing 0 or more format items</param>
        /// <param name="args">The items to be formatted within the log message</param>
        internal static void Error(string format, params object[] args)
        {
            if (Level <= 4)
            {
                _logger.Error(format, args);
            }
        }

        private static byte _level = 2; 
        /// <summary>
        /// Defined log levels are:
        /// <list type="bullet">
        ///     <item>
        ///         <term>1</term>
        ///         <description>Debug (messages for debugging an application)</description>
        ///     </item>
        ///     <item>
        ///         <term>2</term>
        ///         <description>
        ///             Info (messages that might be of interest to an application administrator)
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>3</term>
        ///         <description>
        ///             Warn (messages that might indicate a potential problem)
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>4</term>
        ///         <description>Error (messages that indicate an error)</description>
        ///     </item>
        ///     <item>
        ///         <term>5</term><description>None (no logging)</description>
        ///     </item>
        /// </list>
        /// </summary>
        /// <remarks>The default log level is 2 (Info). This default can be overridden either
        /// programmatically or through configuration by setting the "MechanicalTurk.Log.Level"
        /// key in the application configuration file (refer to <see cref="MTurkConfig"/> 
        /// for configuration details)</remarks>
        public static byte Level
        {
            get { return _level; }
            set 
            {
                if (value < 1 || value > 5)
                {
                    throw new ArgumentException("Log level value must be between 1 (Debug) and 5 (Error)", "value");
                }
                _level = value; 
            }
        }
    }
}
