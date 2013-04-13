#region Copyright & license notice
/*
 * Copyright: Copyright (c) 2007 Amazon Technologies, Inc.
 * License:   Apache License, Version 2.0
 */
#endregion

using System;
using System.Net;
using System.IO;
using System.Configuration;
using System.Reflection;
using Amazon.WebServices.MechanicalTurk.Advanced;

namespace Amazon.WebServices.MechanicalTurk
{
	/// <summary>
	/// Holds the client configuration necessary to communicate with the mechanical turk webservice.
	/// A <c>MTurkConfig</c> instance must be passed to the <see cref="SimpleClient"/> or 
    /// <see cref="MTurkClient"/> instance to establish connectivity.
	/// <para>
	/// The <c>DefaultInstance</c> can be used when the configuration is retrieved from the application
	/// configuration file. If the configuration is stored elsewhere, a separate <c>MTurkConfig</c> instance
	/// must be instantiated and passed to the requester.
	/// </para>
	/// </summary>
	public class MTurkConfig
	{
		#region Properties
        /// <summary>
        /// Additional key-value-pairs that can be passed into the SDK when the configuration
        /// was not loaded from the application configuration file
        /// </summary>
        private System.Collections.Specialized.StringDictionary _context;
        private System.Collections.Specialized.StringDictionary Context
        {
            get { return _context; }
        }

		private string _serviceEndpoint;
        /// <summary>
        /// Web service endpoint for Mechanical Turk
        /// </summary>
		public string ServiceEndpoint {
			get { return _serviceEndpoint; }
			set 
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("Cannot accept null or empty URL for service endpoint");
                }

                try
                {
                    Uri uri = new Uri(value);
                }
                catch (UriFormatException uriEx)
                {
                    throw new ArgumentException("Invalid service endpoint URI", uriEx);
                }

                MTurkLog.Debug("Configuring service endpoint: {0}", value);

                _serviceEndpoint = value; 
            }
		}	
		
		private string _accessKeyId;
		/// <summary>
        /// Access Key ID of Amazon Web Services account
		/// </summary>
		public string AccessKeyId {
			get { return _accessKeyId; }
			set 
			{
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("Access key ID may not be null or empty");
                }

				if (_signer == null)
				{
					// create signer only if not already assigned
					// this signer is overridden when secret access key is set
					_signer = new HMACSigner(value);
				}
								
				_accessKeyId = value; 
			}
		}
		
		private string _secretAccessKey;
        /// <summary>
        /// Secret Access key for Amazon Web Services account matching the <see cref="AccessKeyId"/>
        /// </summary>
		public string SecretAccessKey {
			get { return _secretAccessKey; }
			set 
			{
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("Secret access key may not be null or empty");
                }

				if (string.IsNullOrEmpty(_secretAccessKey) || !string.Equals(_secretAccessKey, value))
				{
					// create signer for this access key
					_signer = new HMACSigner(value);
				}
				_secretAccessKey = value; 				
			}
		}

        private string _websiteURL;
        /// <summary>
        /// Returns the worker website associated with the configured endpoint
        /// </summary>
        public string WebsiteURL
        {
            get { return (this._websiteURL); }
            set 
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException("Cannot accept null or empty URL for worker website URL");
                }

                try
                {
                    Uri uri = new Uri(value);
                }
                catch (UriFormatException uriEx)
                {
                    throw new ArgumentException("Invalid worker website URI", uriEx);
                }

                MTurkLog.Debug("Configuring website URL: {0}", value);

                _websiteURL = value; 
            }
        }

        private System.Net.IWebProxy _proxy = System.Net.WebRequest.DefaultWebProxy;
        /// <summary>
        /// Proxy used when SDK application sits behind firewall
        /// </summary>
        public System.Net.IWebProxy Proxy
        {
            get { return (this._proxy); }
            set 
            {
                if (value != null)
                {
                    MTurkLog.Debug("Configuring proxy {0}", value);
                }
                this._proxy = value; 
            }
        }

        private bool _ensureQuestionsAreXmlWrappedBeforeSending=true;
        /// <summary>
        /// If true (default), makes sure that questions (e.g. to create a HIT)
        /// are wrapped as XML prior to sending the request
        /// </summary>
        /// <remarks>
        /// If a HIT question is not wrapped in XML (e.g. by using <see cref="QuestionUtil"/>),
        /// then this option implicitly wrappes the question as single freetext question.
        /// If this behaviour is undesired, it can be turned off by setting the
        /// <c>MechanicalTurk.MiscOptions.EnsureQuestionsAreXmlWrapped</c> key in the 
        /// application configuration to <c>false</c>.
        /// </remarks>
        public bool EnsureQuestionsAreXmlWrappedBeforeSending
        {
            get { return (this._ensureQuestionsAreXmlWrappedBeforeSending); }
            set 
            {
                MTurkLog.Debug("Ensure question XML wrapping: {0}", value);
                this._ensureQuestionsAreXmlWrappedBeforeSending = value; 
            }
        }

        private bool _validateQuestionBeforeSending=false;
        /// <summary>
        /// If true (default is false), then questions will be validated prior to sending
        /// a request (such as CreateHIT).
        /// </summary>
        /// <remarks>
        /// To enable this feature set the <c>MechanicalTurk.MiscOptions.EnsureQuestionValidity</c> 
        /// key in the application configuration to <c>true</c>.
        /// </remarks>
        public bool ValidateQuestionBeforeSending
        {
            get { return (this._validateQuestionBeforeSending); }
            set 
            {
                MTurkLog.Debug("Ensure question validation: {0}", value);
                this._validateQuestionBeforeSending = value; 
            }
        }

        private int _maxRetryDelay=8000;
        /// <summary>
        /// Maximum time in milliseconds allowed for a retry attempt after throttling errors occured
        /// </summary>
        /// <remarks>
        /// When testing against the Mechanical Turk Sandbox, the service endpoint may reject a request
        /// because of service throttling. The SDK will retry to deliver the request as long as the
        /// value for <c>MaxRetryDelay</c> is not exceeded.
        /// </remarks>
        public int MaxRetryDelay
        {
            get { return (this._maxRetryDelay); }
            set { this._maxRetryDelay = value; }
        }


		private HMACSigner _signer;
		internal HMACSigner Signer
		{
			get { return _signer; }		
		}

		#endregion
		
		#region Class properties
        /// <summary>
        /// The configuration used in the current thread
        /// </summary>
        [ThreadStatic]
        private static MTurkConfig _currentInstance;
        internal static MTurkConfig CurrentInstance
        {
            get { return _currentInstance; }
            set { _currentInstance = value; }
        }

        private static string _sdkVersion = "MTurkDotNetSDK/" + typeof(MTurkConfig).Assembly.GetName().Version.ToString(4) + ",";
        /// <summary>
        /// Current version of the SDK
        /// </summary>
        public static string SdkVersion
        {
            get { return _sdkVersion; }
        }

		#endregion
		
		#region Constructors
		/// <summary>
		/// Instantiates the client configuration by reading the application configuration file (app.config). 
		/// </summary>
        /// <remarks>
        /// The following values are read from the application configuration file:
        /// <list type="bullet">
        /// <item>
        /// 	<term>MechanicalTurk.ServiceEndpoint</term>
        /// 	<description>
        /// 	The service endpoint receiving and processing mturk requests. Defaults to the sandbox endpoint at
        ///     <c>https://mechanicalturk.sandbox.amazonaws.com?Service=AWSMechanicalTurkRequester</c>
        /// 	</description>
        /// </item>
        /// <item>
        /// 	<term>MechanicalTurk.AccessKeyId</term>
        /// 	<description>Required: The access key ID for the Amazon Web Services account</description>
        /// </item>
        /// <item>
        /// 	<term>MechanicalTurk.SecretAccessKey</term>
        /// 	<description>Required: The secret access key for the Amazon Web Services account</description>
        /// </item>
        /// <item>
        /// 	<term>MechanicalTurk.Log.Level</term>
        /// 	<description>The log level for SDK log messages (<see cref="ILog"/> for details). Defaults to 2 (Info)</description>
        /// </item>
        /// <item>
        /// 	<term>MechanicalTurk.Proxy.Url</term>
        /// 	<description>
        ///     Proxy URL (only necessary when used through a web proxy that requires authentication)
        ///     </description>
        /// </item>
        /// <item>
        /// 	<term>MechanicalTurk.Proxy.User</term>
        /// 	<description>
        ///     User name for proxy credentials (only necessary when used through a web proxy that 
        ///     requires authentication)
        ///     </description>
        /// </item>
        /// <item>
        /// 	<term>MechanicalTurk.Proxy.Password</term>
        /// 	<description>
        ///     Password for proxy credentials (only necessary when used through a web proxy 
        ///     that requires authentication)
        ///     </description>
        /// </item>
        /// <item>
        /// 	<term>MechanicalTurk.Proxy.Domain</term>
        /// 	<description>
        ///     User domain for proxy credentials (only necessary when used through a web proxy 
        ///     that requires authentication)
        ///     </description>
        /// </item>
        /// <item>
        /// 	<term>MechanicalTurk.MiscOptions.EnsureQuestionsAreXmlWrapped</term>
        /// 	<description>
        ///     If set to true (default), ensures that questions are wrapped as XML when sent to
        ///     Mechanical Turk.
        ///     </description>
        /// </item>
        /// <item>
        /// 	<term>MechanicalTurk.MiscOptions.EnsureQuestionValidity</term>
        /// 	<description>
        ///     If set to true (default is false), ensures that questions are validated before 
        ///     they are sent to Mechanical Turk
        ///     </description>
        /// </item>
        /// </list>
        /// </remarks>
		public MTurkConfig()
		{
            string configFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            MTurkLog.Info("Loading config from " + configFile);
            if (!File.Exists(configFile))
            {
                throw new IOException(string.Format("Cannot find Mechanical turk configuration file at '{0}'", configFile));
            }
            Init();
		}

        /// <summary>
        /// Instantiates the client configuration by reading the application configuration from the context passed.
        /// </summary>
        /// <param name="context">Configuration with the following values:
        /// <list type="bullet">
        /// <item>
        /// 	<term>MechanicalTurk.ServiceEndpoint</term>
        /// 	<description>
        /// 	The service endpoint receiving and processing mturk requests. Defaults to the sandbox endpoint at
        ///     <c>https://mechanicalturk.sandbox.amazonaws.com?Service=AWSMechanicalTurkRequester</c>
        /// 	</description>
        /// </item>
        /// <item>
        /// 	<term>MechanicalTurk.AccessKeyId</term>
        /// 	<description>Required: The access key ID for the Amazon Web Services account</description>
        /// </item>
        /// <item>
        /// 	<term>MechanicalTurk.SecretAccessKey</term>
        /// 	<description>Required: The secret access key for the Amazon Web Services account</description>
        /// </item>
        /// <item>
        /// 	<term>MechanicalTurk.Log.Level</term>
        /// 	<description>The log level for SDK log messages (<see cref="ILog"/> for details). Defaults to 2 (Info)</description>
        /// </item>
        /// <item>
        /// 	<term>MechanicalTurk.Proxy.Url</term>
        /// 	<description>
        ///     Proxy URL (only necessary when used through a web proxy that requires authentication)
        ///     </description>
        /// </item>
        /// <item>
        /// 	<term>MechanicalTurk.Proxy.User</term>
        /// 	<description>
        ///     User name for proxy credentials (only necessary when used through a web proxy that 
        ///     requires authentication)
        ///     </description>
        /// </item>
        /// <item>
        /// 	<term>MechanicalTurk.Proxy.Password</term>
        /// 	<description>
        ///     Password for proxy credentials (only necessary when used through a web proxy 
        ///     that requires authentication)
        ///     </description>
        /// </item>
        /// <item>
        /// 	<term>MechanicalTurk.Proxy.Domain</term>
        /// 	<description>
        ///     User domain for proxy credentials (only necessary when used through a web proxy 
        ///     that requires authentication)
        ///     </description>
        /// </item>
        /// <item>
        /// 	<term>MechanicalTurk.MiscOptions.EnsureQuestionsAreXmlWrapped</term>
        /// 	<description>
        ///     If set to true (default), ensures that questions are wrapped as XML when sent to
        ///     Mechanical Turk.
        ///     </description>
        /// </item>
        /// <item>
        /// 	<term>MechanicalTurk.MiscOptions.EnsureQuestionValidity</term>
        /// 	<description>
        ///     If set to true (default is false), ensures that questions are validated before 
        ///     they are sent to Mechanical Turk
        ///     </description>
        /// </item>
        /// </list>
        /// </param>
        public MTurkConfig(System.Collections.Specialized.StringDictionary context)
        {
            _context = context;
            Init();
        }

        /// <summary>
        /// Instantiates a client for a service endpoint, access key ID and secret access key.
        /// </summary>
        /// <param name="serviceEndpoint">The service endpoint receiving and processing mturk 
        /// requests. If null, defaults to the sandbox endpoint at
        /// <c>https://mechanicalturk.sandbox.amazonaws.com?Service=AWSMechanicalTurkRequester</c></param>
        /// <param name="accessKeyId">Required: The access key ID for the Amazon Web Services account</param>
        /// <param name="secretAccessKey">Required: The secret access key for the Amazon Web Services account</param>
        public MTurkConfig(string serviceEndpoint, string accessKeyId, string secretAccessKey)
        {
            System.Collections.Specialized.StringDictionary ctx = new System.Collections.Specialized.StringDictionary();
            ctx.Add("MechanicalTurk.ServiceEndpoint", serviceEndpoint);
            ctx.Add("MechanicalTurk.AccessKeyId", accessKeyId);
            ctx.Add("MechanicalTurk.SecretAccessKey", secretAccessKey);

            _context = ctx;
            Init();

        }
		#endregion

        private void Init()
        {
            // log level
            string logLevel = GetConfig("MechanicalTurk.Log.Level", "2");
            byte level = 2;
            if (!byte.TryParse(logLevel, out level))
            {
                MTurkLog.Warn("Invalid log level specified in configuration file ({0}). Defaulting to 'INFO'", logLevel);
            }
            MTurkLog.Level = level;

            // AWS settings
            string s = GetConfig("MechanicalTurk.ServiceEndpoint", null);
            if (string.IsNullOrEmpty(s))
            {
                this.ServiceEndpoint = "https://mechanicalturk.sandbox.amazonaws.com?Service=AWSMechanicalTurkRequester";
            }
            else
            {
                this.ServiceEndpoint = s;
            }

            AccessKeyId = GetConfig("MechanicalTurk.AccessKeyId", null);
            SecretAccessKey = GetConfig("MechanicalTurk.SecretAccessKey", null);

            // Proxy settings when connecting through a proxy that requires authentication
            string proxyURL = GetConfig("MechanicalTurk.Proxy.Url", null);
            if (!string.IsNullOrEmpty(proxyURL))
            {                
                Proxy = new WebProxy(proxyURL);
            }

            // set default credentials
            Proxy.Credentials = CredentialCache.DefaultNetworkCredentials;

            string proxyUser = GetConfig("MechanicalTurk.Proxy.User", null);
            string proxyPassword = GetConfig("MechanicalTurk.Proxy.Password", null);
            string proxyDomain = GetConfig("MechanicalTurk.Proxy.Domain", null);
            if (!string.IsNullOrEmpty(proxyUser) && !string.IsNullOrEmpty(proxyPassword))
            {
                // override default credentials
                Proxy.Credentials = new NetworkCredential(proxyUser, proxyPassword, proxyDomain);
            }

            // worker website
            string url = GetConfig("MechanicalTurk.Url", null);
            if (url != null)
            {
                this.WebsiteURL = url;
            }
            else
            {
                if (this.ServiceEndpoint.ToLower().IndexOf("sandbox") != -1)
                {
                    this.WebsiteURL = "http://workersandbox.mturk.com";
                }
                else
                {
                    this.WebsiteURL = "http://www.mturk.com";
                }
            }

            // question settings: Implicitly wrap questions
            string wrap = GetConfig("MechanicalTurk.MiscOptions.EnsureQuestionsAreXmlWrapped", null);
            if (wrap != null)
            {
                this.EnsureQuestionsAreXmlWrappedBeforeSending = wrap.ToLower().Equals("true");
            }

            // question settings: Implicitly validate
            string validate = GetConfig("MechanicalTurk.MiscOptions.EnsureQuestionValidity", null);
            if (validate != null)
            {
                this.ValidateQuestionBeforeSending = validate.ToLower().Equals("true");
            }

            // retry delay for sandbox throttling
            MaxRetryDelay = int.Parse(GetConfig("MechanicalTurk.MaxRetryDelay", "8000"));

            MTurkLog.Info("Initialized configuration for '{0}'", this.ServiceEndpoint);
        }

        /// <summary>
        /// Returns a value from the configuration
        /// </summary>
        /// <param name="key">Configuration key</param>
        /// <param name="defaultValue">Value returned, if no value is configured for the 
        /// requested key</param>
        /// <returns>Configured value (or default)</returns>
        public string GetConfig(string key, string defaultValue)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key", "Can't get configuration value for null key");
            }

            string ret = (Context == null) ? ConfigurationManager.AppSettings.Get(key) : Context[key];
            if (ret == null)
            {
                ret = defaultValue;
            }

            if (ret != null)
            {
                ret = ret.Trim();
            }

            return ret;
        }

        /// <summary>
        /// Returns the URL, where the HIT can be viewed at the worker website or "n/a" if HIT was not 
        /// yet created or loaded.
        /// </summary>
        /// <param name="hitTypeID">The HIT type ID of an existing HIT</param>
        public string GetPreviewURL(string hitTypeID)
        {
            if (string.IsNullOrEmpty(hitTypeID))
            {
                return "n/a";
            }
            else
            {
                return string.Format("{0}/mturk/preview?groupId={1}", this.WebsiteURL, hitTypeID);
            }
        }



        /// <summary>
        /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        /// </returns>
        public override string ToString()
        {
            return string.Format("Url: {0}, AWS Info: {1}, Proxy: {2}",
                this.ServiceEndpoint,
                this.AccessKeyId.GetHashCode(),
                this.Proxy != null);
        }
	}
}
