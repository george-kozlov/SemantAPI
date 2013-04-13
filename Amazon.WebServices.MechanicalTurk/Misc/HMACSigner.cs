#region Copyright & license notice
/*
 * Copyright: Copyright (c) 2007 Amazon Technologies, Inc.
 * License:   Apache License, Version 2.0
 */
#endregion

using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Amazon.WebServices.MechanicalTurk
{
	/// <summary>
	/// Creates the signature used for authentication of an mturk/AWS request
	/// </summary>
	internal class HMACSigner
	{
		#region Constants
		private static string SERVICE_NAME = "AWSMechanicalTurkRequester";  // requester service for MTurk
		private static string TIMESTAMP_FORMAT = "yyyy-MM-ddTHH:mm:ss.fffZ";
		#endregion
				
        private string awsService;   // service to create signatures for
        private HMACSHA1 hmac;       // hash algorithm instance for a specific key passed to the ctor

        #region Constructors
        /// <summary>
		/// Instantiates the signer for the MTurk requester service with a secret key
		/// </summary>
		/// <param name="key">Key, that matches the AWS access ID</param>
		internal HMACSigner(string key) : this(key, SERVICE_NAME)
		{
		}

        /// <summary>
        /// Instantiates the signer with a secret key and a specific AWS service
        /// </summary>
        /// <param name="key">Key, that matches the AWS access ID</param>
        /// <param name="service">Name of the service for which to sign requests for</param>
        internal HMACSigner(string key, string service)
        {
            awsService = service;
            hmac = new HMACSHA1(Encoding.UTF8.GetBytes(key));
        }
        #endregion
        
        #region AWS authentication helpers       
        /// <summary>
        /// Gets a Datetime that can be used with the Amazon Web Services methods
        /// </summary>
        internal static DateTime GetAwsDateStamp()
        {
			DateTime now = DateTime.UtcNow;
			// Important to use DateTimeKind for proper serialization 
			// see http://blogs.msdn.com/mattavis/archive/2005/10/11/479782.aspx
			DateTime ret = new DateTime(now.Year, now.Month, now.Day, now.Hour, 
                now.Minute, now.Second, now.Millisecond, DateTimeKind.Utc);
           			
			// Workaround for issue where valid signatures cannot be produced
        	// for timestamps that end with '...0Z', e.g "2007-06-29T15:53:38.770Z"
        	//
        	// Without the workaround 10% of all requests result in AWS.NotAuthorized
        	//
        	// Another option would be to simply use a fixed millisecond in the above
        	// DateTime constructor. The workaround chosen here favors accuracy over
        	// performance
			if (ret.Millisecond % 10 == 0)
			{
				ret = ret.AddMilliseconds(1);
			}
			
			return ret;
        }

        /// <summary>
        /// Creates signature used for Amazon Web Service method calls
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="timeStamp"></param>
        /// <returns></returns>
        public string GetAwsSignature(string operation, DateTime timeStamp)
        {
            string dataToSign = awsService + 
                operation + 
                timeStamp.ToString(TIMESTAMP_FORMAT, System.Globalization.CultureInfo.InvariantCulture);

            //hmac not guaranteed to be threadsafe -> synchronize
            lock (hmac)
            {
                return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(dataToSign.ToCharArray())));
            }
        }
        #endregion        
	}
}
