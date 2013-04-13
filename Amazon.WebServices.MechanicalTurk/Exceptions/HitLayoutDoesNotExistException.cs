#region Copyright & license notice
/*
 * Copyright: Copyright (c) 2011 Amazon Technologies, Inc.
 * License:   Apache License, Version 2.0
 */
#endregion


using System;
using Amazon.WebServices.MechanicalTurk.Domain;

namespace Amazon.WebServices.MechanicalTurk.Exceptions
{
    /// <summary>
    /// The specified HITLayoutId is not valid. Please use the requester web interface to create an ID.
    /// </summary>
    public class HitLayoutDoesNotExistException : ServiceException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HitLayoutDoesNotExistException"/> class.
        /// </summary>
        /// <param name="serviceError">The service error.</param>
        /// <param name="serviceResponse">The service response.</param>
        public HitLayoutDoesNotExistException(ErrorsError serviceError, object serviceResponse)
            : base(serviceError, serviceResponse)
        {
        }
    }
}
