#region Copyright & license notice
/*
 * Copyright: Copyright (c) 2007 Amazon Technologies, Inc.
 * License:   Apache License, Version 2.0
 */
#endregion

using System;
using System.Net;

namespace Amazon.WebServices.MechanicalTurk
{
    /// <summary>
    ///  Interface for different types of request transport, e.g. SOAP or REST.
    /// </summary>
    public interface ITransportProtocol : IDisposable
    {
        /// <summary>
        /// Sends the request for the given Mechanical Turk operation
        /// </summary>
        /// <param name="request">Request to send</param>
        /// <param name="operation">Operation to invoke</param>
        /// <returns>Service response</returns>
        object DoTransport(object request, string operation);

        /// <summary>
        /// Gets the current HTTP response received from Amazon Web Services
        /// </summary>
        System.Net.HttpWebResponse CurrentWebResponse
        {            
            get;
        }
    }
}
