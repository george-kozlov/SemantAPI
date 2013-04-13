#region Copyright & license notice
/*
 * Copyright: Copyright (c) 2007 Amazon Technologies, Inc.
 * License:   Apache License, Version 2.0
 */
#endregion

using System;
using Amazon.WebServices.MechanicalTurk.Domain;
using Amazon.WebServices.MechanicalTurk.Advanced;

namespace Amazon.WebServices.MechanicalTurk.Exceptions
{
	/// <summary>
	/// Generic exception raised by the SDK when an error occurred.
	/// </summary>  
	public class ServiceException : Exception
	{
		private string _errorCode;
		/// <summary>
		/// Contains the error errorCode returned by the service
		/// </summary>
        /// <remarks>Errors can either be generic Amazon Web Service platform (ie. "<c>AWS.*</c>")
        /// or errors specific to the Mechanical Turk Service (ie. "<c>AWS.MechanicalTurk.*</c>"</remarks>
		public string ErrorCode {
			get { return _errorCode; }
		}
		
		private object _serviceResponse;
		/// <summary>
		/// Returns the response for the request that caused the error (if any).
		/// This response can be inspected for detailed information about the
		/// errors that occurred.
		/// </summary>
		public object ServiceResponse {
			get { return _serviceResponse; }
		}
		
		#region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceException"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorMessage">The error message.</param>
        public ServiceException(string errorCode, string errorMessage) : base(errorMessage)
		{
			_errorCode = errorCode;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceException"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="inner">The inner.</param>
        public ServiceException(string errorCode, string errorMessage, Exception inner) : base(errorMessage, inner)
		{
			_errorCode = errorCode;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceException"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="serviceResponse">The service response.</param>
        public ServiceException(string errorCode, string errorMessage, object serviceResponse) : this(errorCode, errorMessage)
		{
			_serviceResponse = serviceResponse;
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceException"/> class.
        /// </summary>
        /// <param name="serviceError">The service error.</param>
        /// <param name="serviceResponse">The service response.</param>
        public ServiceException(ErrorsError serviceError, object serviceResponse)
            : base(serviceError != null ? serviceError.Message : "Unknown")
		{
            if (serviceError != null)
            {
                _errorCode = serviceError.Code;
            }
			_serviceResponse = serviceResponse;
		}
		#endregion
	}
}
