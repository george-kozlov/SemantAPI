#region Copyright & license notice
/*
 * Copyright: Copyright (c) 2007 Amazon Technologies, Inc.
 * License:   Apache License, Version 2.0
 */
#endregion

using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Net;
using System.Web.Services;
using System.ComponentModel;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using System.Reflection;
using Amazon.WebServices.MechanicalTurk.Domain;
using Amazon.WebServices.MechanicalTurk.Exceptions;
using Amazon.WebServices.MechanicalTurk.Advanced;

namespace Amazon.WebServices.MechanicalTurk
{
    /// <summary>
    /// Protocol used to send soap requests to Mechanical turk
    /// </summary>
    public class SoapHttpClientTurkProtocol : AWSMechanicalTurkRequester, ITransportProtocol
    {
        // The current response for the request thread
        [ThreadStatic]
        private static WebResponse currentResponse;

        #region Constructors
        /// <summary>
        /// Construct a SOAP protocol object.
        /// </summary>
        /// <param name="url">The service endpoint url</param>
        /// <param name="proxy">The proxy to send requests through. This may be null.</param>
        public SoapHttpClientTurkProtocol(string url, IWebProxy proxy)
        {
            this.Url = url;
            this.Proxy = proxy;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Overrides <c>GetWebRequest</c> to inject custom HTTP headers
        /// </summary>
        protected override System.Net.WebRequest GetWebRequest(Uri uri)
        {
            currentResponse = null;
            System.Net.WebRequest req = base.GetWebRequest(uri);

            // when log level is set to debug, log the SOAP traffic
            if (MTurkLog.Level==1)
            {
                req = new LoggedWebReqest(req);
            }
            req.Headers.Add("X-Amazon-Software", MTurkConfig.SdkVersion);

            return req;
        }

        /// <summary>
        /// Overrides <c>GetWebResponse</c> to get the actual HTTP response returned from the service
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected override WebResponse GetWebResponse(WebRequest request)
        {
            currentResponse = base.GetWebResponse(request);
            
            return currentResponse;
        }
        #endregion

        #region ITransportProtocol Members

        /// <summary>
        /// Sends the request for the given Mechanical Turk operation
        /// </summary>
        /// <param name="request">Request to send</param>
        /// <param name="operation">Operation to invoke</param>
        /// <returns>Service response</returns>
        public object DoTransport(object request, string operation)
        {
            return ReflectionUtil.InvokeMethod(this, operation, new object[] { request });
        }

        /// <summary>
        /// Gets the current HTTP response received from Amazon Web Services
        /// </summary>
        /// <value></value>
        public System.Net.HttpWebResponse CurrentWebResponse
        {
            get
            {
                return currentResponse as HttpWebResponse;
            }
        }

        #endregion
    }
}
