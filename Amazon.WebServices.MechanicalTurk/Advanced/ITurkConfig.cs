#region Copyright & license notice
/*
 * Copyright: Copyright (c) 2007 Amazon Technologies, Inc.
 * License:   Apache License, Version 2.0
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using Amazon.WebServices.MechanicalTurk;

namespace Amazon.WebServices.MechanicalTurk.Advanced
{
    /// <summary>
    /// Interface implemented by the Mechanical Turk clients to
    /// expose the configuration they use.
    /// </summary>
    public interface ITurkConfig
    {
        /// <summary>
        /// Returns the configuration used by the Mechanical Turk client.
        /// </summary>
        MTurkConfig Config
        {
            get;
        }
    }
}
