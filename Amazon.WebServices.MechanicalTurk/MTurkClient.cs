#region Copyright & license notice
/*
 * Copyright: Copyright (c) 2007 Amazon Technologies, Inc.
 * License:   Apache License, Version 2.0
 */
#endregion

using System;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;
using System.Xml.Serialization;
using System.Web.Services;
using System.Web.Services.Protocols;
using Amazon.WebServices.MechanicalTurk.Domain;
using Amazon.WebServices.MechanicalTurk.Advanced;
using Amazon.WebServices.MechanicalTurk.Exceptions;


namespace Amazon.WebServices.MechanicalTurk
{
    /// <summary>
    /// Core client used to invoke soap operations on the Mechanical Turk endpoint with a set of client credentials.
    /// </summary>
    /// <remarks>
    /// By default, the credentials are stored in the application configuration file. If credentials are stored
    /// elsewhere or the application requires multiple connections with different credentials, the 
    /// <c>MTurkClient(MTurkConfig config)</c>
    /// constructor can be used to do this.
    /// </remarks>
	public class MTurkClient : ITurkOperations, IDisposable
	{        
        // maps the response type of a SOAP response to its result property
        private static Dictionary<Type, string>htResultMap = new Dictionary<Type,string>();
        static MTurkClient()
        {
            htResultMap.Add(typeof(CreateHITResponse), "HIT");
            htResultMap.Add(typeof(GetHITResponse), "HIT");
            htResultMap.Add(typeof(CreateQualificationTypeResponse), "QualificationType");
            htResultMap.Add(typeof(GetQualificationTypeResponse), "QualificationType");
            htResultMap.Add(typeof(GetQualificationScoreResponse), "Qualification");
            htResultMap.Add(typeof(UpdateQualificationTypeResponse), "QualificationType");
            htResultMap.Add(typeof(GetRequesterStatisticResponse), "GetStatisticResult");
            htResultMap.Add(typeof(GetRequesterWorkerStatisticResponse), "GetStatisticResult");
            htResultMap.Add(typeof(HelpResponse), "Information");
        }

        #region Properties
        // transport protocol used (currently only support for SOAP/Http-POST)
        private ITransportProtocol _protocol;
        /// <summary>
        /// Gets or sets the transport protocol
        /// </summary>
        public ITransportProtocol Transport
        {
            get { return _protocol; }
            set { _protocol = value; }
        }
        
        private MTurkConfig _config;
        /// <summary>
        /// Gets or sets the configuration containing the service endpoint and the AWS credentials
        /// </summary>
        /// <value>The config.</value>
		public MTurkConfig Config {
			get { return _config; }
			set 
			{
                if (value == null)
                {
                    throw new ArgumentNullException("value", "No (null) MTurkConfig object passed to client");
                }

                if (_config != null)
                {
                    // new config is set
                    MTurkLog.Info("Setting new config '{0}' (was '{1}')", value, _config);
                }

				_config = value;

                // only Soap for now
                this.Transport = new SoapHttpClientTurkProtocol(_config.ServiceEndpoint, _config.Proxy);   

                // automatically throttle sandbox requests
                if (_throttler==null && _config.ServiceEndpoint.ToLower().IndexOf("sandbox") != -1)
                {
                    MTurkLog.Info("Initializing sandbox throttler for current configuration ({0})", _config.ServiceEndpoint);
                    _throttler = LeakyBucketRequestThrottler.GetInstance(_config.ServiceEndpoint, 20, 2);
                }

                MTurkLog.Info("Initialized SDK instance for {0} (ID: {1})", value.ServiceEndpoint, value.AccessKeyId);
			}
		}

        private IRequestThrottler _throttler;
        /// <summary>
        /// Gets or sets the throttler used to throttle SOAP requests to the service (usually used for sandbox testing)
        /// </summary>
        /// <remarks>See <a href="http://sandbox.mturk.com">Guidelines and policies</a> for details</remarks>
        /// <value>The throttler.</value>
        public IRequestThrottler Throttler
        {
            get { return (this._throttler); }
            set { this._throttler = value; }
        }
		#endregion

        #region Constructors
        /// <summary>
		/// Initializes a Mechanical Turk client using the configuration from the application configuration file (see <see cref="MTurkConfig"/>)
		/// </summary>
		public MTurkClient()
		{
            Config = new MTurkConfig();
		}

        /// <summary>
        /// Initializes a Mechanical Turk client using a custom client configuration
        /// </summary>
        /// <param name="config">The configuration used by this client</param>
        /// <remarks>Custom configurations can be useful e.g. when the AWS credentials are retrieved from a different storage
        /// mechanism than the application configuration file</remarks>
        public MTurkClient(MTurkConfig config)
	    {	    	                                                      
	        Config = config;
	    }
	    #endregion        
	    
        #region Helpers
        /// <summary>
        /// Ensures that the question settings from <see cref="MTurkConfig"/> are enforced 
        /// (implicit wrapping and validation)
        /// </summary>
        private string EnsureQuestionIsWrappedAndValid(string question, bool allowExternalQuestion)
        {
            if (string.IsNullOrEmpty(question))
            {
                return question;
            }

            string ret = question;

            if (Config.EnsureQuestionsAreXmlWrappedBeforeSending)
            {
                // check if question is XML wrapped. If not wrap it as simple freetext question
                // to disable, set the "MechanicalTurk.MiscOptions.EnsureQuestionsAreXmlWrapped" 
                // option to false
                if ((!string.IsNullOrEmpty(ret)) &&
                    (ret[0] != '<') &&
                    (ret.IndexOf("</QuestionForm>") == -1) &&
                    (ret.IndexOf("</ExternalQuestion>") == -1))
                {
                    // neither question nor external question -> wrap it
                    ret = QuestionUtil.ConvertSingleFreeTextQuestionToXML(ret);
                }
            }

            // this is disabled by default, but can be enabled to implicitly validate prior to sending
            // by setting the "MechanicalTurk.MiscOptions.EnsureQuestionValidity" option to true
            if (Config.ValidateQuestionBeforeSending)
            {
                string schemaName = null;
                if (ret.IndexOf("</QuestionForm>") != -1) schemaName = "QuestionForm.xsd";
                if (ret.IndexOf("</ExternalQuestion>") != -1) schemaName = "ExternalQuestion.xsd";
                if (ret.IndexOf("</HTMLQuestion>") != -1) schemaName = "HTMLQuestion.xsd";
                if (schemaName == null)
                {
                    throw new Exception("unable to detect question type");
                }
                XmlUtil.ValidateXML(schemaName, ret);
            }

            return ret;
        }

        /// <summary>
        /// Returns the result property for the soap response by inspecting the <c>htResultMap</c>.
        /// If no entry can be found, it attempts to lookup the property by naming convention 
        /// (e.g.RegisterHITTypeResponse->RegisterHITTypeResult);
        /// </summary>
        /// <param name="soapResponse">The response returned from the web service</param>
        /// <returns>The result object for this response</returns>
        /// <exception cref="ServiceException">A service exception is thrown when no result can be found. 
        /// This usually indicates that a new operation was introduced for which the result property is neither mapped
        /// nor can be resolved by naming convention</exception>
        private static object[] GetResult(object soapResponse)
        {
            object ret = null;
            string propName = null;
            if (htResultMap.ContainsKey(soapResponse.GetType()))
            {
                propName = htResultMap[soapResponse.GetType()];
            }
            else
            {
                propName = soapResponse.GetType().Name.Replace("Response", "Result");
            }

            try
            {
                ret = ReflectionUtil.GetPropertyValue(propName, soapResponse);
            }
            catch (Exception ex)
            {
                throw new ServiceException("AWS.MechanicalTurk.Version", "It appears you are running an old API version and attempt to invoke a new service operation", ex);
            }

            return (object[])ret;
        }

        /// <summary>
        /// Processes errors found in a response
        /// </summary>
        private static void ProcessErrors(ErrorsError[] errors, object ret, OperationRequest opsReq)
        {
            StringBuilder sb = new StringBuilder("Errors for request ID ");
            sb.Append(opsReq.RequestId);
            sb.Append(":");
            sb.Append(Environment.NewLine);

            ErrorsError err = null;
            for (int i = 0; i < errors.Length; i++)
            {
                // append error information to generic error message
                err = errors[i];
                sb.Append("Error #");
                sb.Append(i + 1);
                sb.Append(" - ");
                sb.Append(err.Code);
                sb.Append(": ");
                sb.Append(err.Message);

                // add additional data returned with the error (if any)
                if (err.Data != null && err.Data.Length > 0)
                {
                    sb.Append(" (Data: ");

                    for (int j = 0; j < err.Data.Length - 1; j++)
                    {
                        sb.Append("[");
                        sb.Append(err.Data[j].Key);
                        sb.Append("=");
                        sb.Append(err.Data[j].Value);
                        sb.Append("], ");
                    }

                    sb.Append("[");
                    sb.Append(err.Data[err.Data.Length - 1].Key);
                    sb.Append("=");
                    sb.Append(err.Data[err.Data.Length - 1].Value);
                    sb.Append("]");

                    sb.Append(" )");
                }

                sb.Append(Environment.NewLine);

                // check for specific exceptions we want to raise 
                if (err.Code != null)
                {
                    string code = err.Code;
                    if ("AWS.BadCredentialSupplied|AWS.NotAuthorized|AWS.BadClaimsSupplied".IndexOf(code) != -1)
                    {
                        throw new AccessKeyException(err, ret);
                    }

                    if ("AWS.MechanicalTurk.NoMoreWorkableHITsInGroupException|AWS.MechanicalTurk.NoHITsAvailableInGroupException|AWS.MechanicalTurk.NoHITsAvailableForIterator".IndexOf(code) != -1)
                    {
                        throw new NoHITsAvailableException(err, ret);
                    }

                    if ("AWS.MechanicalTurk.QualificationDoesNotExist|AWS.MechanicalTurk.QualificationRequestDoesNotExist|AWS.MechanicalTurk.QualificationTypeDoesNotExist|AWS.MechanicalTurk.AssignmentDoesNotExist|AWS.MechanicalTurk.HITDoesNotExist".IndexOf(code) != -1)
                    {
                        throw new ObjectDoesNotExistException(err, ret);
                    }

                    if ("AWS.MechanicalTurk.InvalidHITState|AWS.MechanicalTurk.InvalidQualificationTypeState|AWS.MechanicalTurk.InvalidQualificationState|AWS.MechanicalTurk.InvalidQualificationRequestState|AWS.MechanicalTurk.InvalidAssignmentState|AWS.MechanicalTurk.HITAlreadyPassedReview".IndexOf(code) != -1)
                    {
                        throw new InvalidStateException(err, ret);
                    }

                    if ("AWS.MechanicalTurk.AssignmentAlreadyExists|AWS.MechanicalTurk.QualificationTypeAlreadyExists|AWS.MechanicalTurk.QualificationAlreadyExists".IndexOf(code) != -1)
                    {
                        throw new ObjectAlreadyExistsException(err, ret);
                    }

                    if ("AWS.MechanicalTurk.PermissionDeniedException|AWS.MechanicalTurk.DoesNotMeetRequirements".IndexOf(code) != -1)
                    {
                        throw new PermissionDeniedException(err, ret);
                    }

                    if ("AWS.MechanicalTurk.QualificationTypeRetryDelayNotElapsed|AWS.MechanicalTurk.QualificationTypeDoesNotAllowRetake".IndexOf(code) != -1)
                    {
                        throw new QualificationTypeRetryException(err, ret);
                    }

                    if ("AWS.MechanicalTurk.XMLParseError|AWS.MechanicalTurk.XHTMLParseError".IndexOf(code) != -1)
                    {
                        throw new ParseErrorException(err, ret);
                    }

                    if ("AWS.MechanicalTurk.InvalidTransportEndpoint".Equals(code))
                    {
                        throw new InvalidTransportEndpointException(err, ret);
                    }

                    if ("AWS.MechanicalTurk.InvalidParameterValue".Equals(code))
                    {
                        throw new InvalidParameterValueException(err, ret);
                    }

                    if ("AWS.MechanicalTurk.ExceedsMaxAssignmentsPerWorker".Equals(code))
                    {
                        throw new ExceedsMaxAssignmentsPerWorkerException(err, ret);
                    }

                    if ("AWS.MechanicalTurk.InsufficientFunds".Equals(code))
                    {
                        throw new InsufficientFundsException(err, ret);
                    }

                    if ("AWS.MechanicalTurk.HITLayoutDoesNotExist".Equals(code))
                    {
                        throw new HitLayoutDoesNotExistException(err, ret);
                    }
                }
            }

            throw new ServiceException("AWS.MechanicalTurk.GenericError", sb.ToString(), ret);
        }
        #endregion

        /// <summary>
        /// Attempt to deliver the message and retry on throttling errors until all
        ///  retries are exhausted
        /// </summary>
        private object AttemptSend(object envelope, string operation, int retryDelayInMilliseconds)
        {            
            SoapException soapEx;
            Exception lastEx;
            HttpWebResponse httpResponse;
            do
            {
                // sign the envelope (=resign on retry)
                DateTime now = HMACSigner.GetAwsDateStamp();

                ReflectionUtil.SetPropertyValue("AWSAccessKeyId", envelope, Config.AccessKeyId);
                ReflectionUtil.SetPropertyValue("Timestamp", envelope, now);
                ReflectionUtil.SetPropertyValue("Signature",
                    envelope,
                    Config.Signer.GetAwsSignature(envelope.GetType().Name, now));


                soapEx = null;
                httpResponse = null;

                try
                {
                    return _protocol.DoTransport(envelope, envelope.GetType().Name);
                }
                catch (System.Reflection.TargetInvocationException invocationEx)
                {
                    lastEx = invocationEx.InnerException;
                    soapEx = lastEx as SoapException;
                    bool retry = false;
                    if (soapEx == null)
                    {
                        // check for timeout
                        WebException webEx = lastEx as WebException;
                        if (webEx != null && webEx.Status == WebExceptionStatus.Timeout)
                        {
                            // retry on timeout
                            MTurkLog.Info("Request timed out");
                            retry = true;
                        }
                        else
                        {
                            // invalid service endpoint is null reference exception
                            if (lastEx is NullReferenceException)
                            {
                                throw new ServiceException("AWS.MechanicalTurk.InvalidUrl", 
                                    "Could not send request to endpoint. Check endpoint URL " + Config.ServiceEndpoint,
                                    lastEx);
                            }

                            // raise inner exception (cannot be retried)
                            throw lastEx;
                        }
                    }
                    else
                    {
                        httpResponse = _protocol.CurrentWebResponse;
                        if (httpResponse == null)
                        {
                            throw new ServiceException("AWS.MechanicalTurk.NoResponse", "No HTTP response received for request");
                        }
                        else
                        {

                            if (httpResponse.StatusCode == HttpStatusCode.InternalServerError
                                || httpResponse.StatusCode == HttpStatusCode.ServiceUnavailable)
                            {
                                // retry due to AWS service throttling
                                MTurkLog.Info("Received HTTP error {0} ({1})", httpResponse.StatusCode, httpResponse.StatusDescription);
                                retry = true;
                            }
                        }
                    }

                    if (retry)
                    {
                        // retry due to AWS service throttling or http timeout
                        MTurkLog.Debug("Retrying request in {0} msecs.", retryDelayInMilliseconds);
                        System.Threading.Thread.Sleep(retryDelayInMilliseconds);

                        retryDelayInMilliseconds = retryDelayInMilliseconds * 2;
                    }
                }
            } while (retryDelayInMilliseconds <= Config.MaxRetryDelay);

            // all retries are exhausted
            // gather additional information from the soap exception and http response
            if (soapEx != null)
            {
                StringBuilder sb = new StringBuilder("SOAP exception: ");
                sb.Append(soapEx.Message);
                sb.Append(" (");
                sb.Append(soapEx.Code);
                sb.Append(") Http info: ");
                sb.Append(" Status code=").Append((int)httpResponse.StatusCode);
                sb.Append(", Status=").Append(httpResponse.StatusDescription);
                sb.Append(", Server=").Append(httpResponse.Server);

                MTurkLog.Error(sb.ToString());

                throw new ServiceException("AWS.MechanicalTurk.Service", sb.ToString(), soapEx);
            }
            else
            {
                throw lastEx;
            }

            
        }


        /// <summary>
	    /// Signs and sends the request and parses errors (if any)
	    /// </summary>
        /// <param name="req">
        /// A request envelope, a request item or an array of request items of the same type.
        /// </param>
        /// <remarks>This method introduces flexibility in how requests can be 
        /// sent to the Mechanical Turk Web Service.</remarks>
        /// <example>
        /// <para>The following code samples show the creation of a HIT in 4 different ways 
        /// (single or batch creation)</para>
        /// <para>Method 1: Use of the convenience method</para>
        /// <code>
        /// CreateHITRequest req = new CreateHITRequest();			
		///	req.Title = string.Format("{0} (Test suite: {1})", title, DateTime.Now.ToString("g"));
        /// ...
        /// CreateHIT hit = new CreateHIT();
		///	hit.Request = new CreateHITRequest[] { req };
        /// CreateHITResponse response = Client.CreateHIT(hit);
        /// </code>
        /// <para>Method 2: Use of the generic send method to create a single HIT</para>
        /// <code>
        /// CreateHITRequest req = new CreateHITRequest();			
        ///	req.Title = string.Format("{0} (Test suite: {1})", title, DateTime.Now.ToString("g"));
        /// ...
        /// CreateHITResponse response = (CreateHITResponse)Client.SendRequest(req);
        /// </code>
        /// <para>Method 3: Use of the generic send method to create a multiple HITs</para>
        /// <code>
        /// CreateHITRequest req1 = new CreateHITRequest();			
        ///	req1.Title = string.Format("{0} (Test suite: {1})", title, DateTime.Now.ToString("g"));
        /// ...
        /// CreateHITRequest[] reqArray = new CreateHITRequest[] { req1, req2, ..., reqX };
        /// CreateHITResponse response = (CreateHITResponse)Client.SendRequest(reqArray);
        /// </code>
        /// <para>Method 4: Use of the generic send method to create a multiple HITs 
        /// utilizing generics</para>
        /// <code>
        /// List&lt;CreateHITRequest&gt; items = new List&lt;CreateHITRequest&gt;;
        /// CreateHITRequest req = new CreateHITRequest();			
        ///	req.Title = string.Format("{0} (Test suite: {1})", title, DateTime.Now.ToString("g"));
        /// ...
        /// items.Add(req);
        /// ...
        /// CreateHITResponse response = (CreateHITResponse)Client.SendRequest(items.ToArray());
        /// </code>
        /// </example>
	    public object SendRequest(object req)
        {
            if (req == null)
            {
                throw new ArgumentNullException("req", "Can't send null request");
            }

            MTurkLog.Debug("Sending request {0}", req.GetType().Name);
            object ret = null;

            try
            {
                // set config for current thread
                MTurkConfig.CurrentInstance = Config;

                // envelope holding the common request parameters
                object envelope = null;

                // if the request item (or an array of them) is passed, then we need to
                // wrap them in a request envelope holding the common properties. Otherwise
                // the envelope itself was passed and we simply need to set the reference to it.
                if (req.GetType().IsArray || req.GetType().Name.EndsWith("Request"))
                {
                    envelope = ReflectionUtil.CreateRequestEnvelope(req);
                }
                else
                {
                    envelope = req;
                }

                // throttle request if necessary
                if (_throttler != null)
                {
                    _throttler.StartRequest();
                }

                // set common properties
                object request = ReflectionUtil.GetPropertyValue("Request", envelope);

                // set list properties (pagenumber/size) automatically (if not explicitly set)
                ReflectionUtil.SetPagingProperties(request);

                // preprocess optional parameters so we don't 
                // need to set the "xxxSpecified" properties explicitly
                ReflectionUtil.PreProcessOptionalRequestProperties(request);

                // send the request (with retry attempts)
                ret = AttemptSend(envelope, envelope.GetType().Name, 1000);

                // we got a response, parse if there are any errors
                if (ret == null)
                {
                    throw new ServiceException("AWS.MechanicalTurk.Unkown", "Empty result, unknown error.");
                }

                MTurkLog.Debug("Received response {0}", ret.GetType().Name);

                // check if any errors are included in the response (adapted from Java SDK)            
                OperationRequest opsReq = ReflectionUtil.GetPropertyValue("OperationRequest", ret) as OperationRequest;
                if (opsReq == null)
                {
                    throw new ServiceException("AWS.MechanicalTurk.Unkown", "Empty operation request");
                }

                ErrorsError[] errors = opsReq.Errors;   // AWS.* scoped errors (like auth failures)
                if (errors == null)
                {
                    // check for MTurk specific errors by reflecting the result Property (ie "response->result[]->Request->Errors")
                    object[] resultObject = GetResult(ret);

                    // examine if any of the results have errors
                    for (int i = 0; i < resultObject.Length; i++)
                    {
                        Request resultRequest = ReflectionUtil.GetPropertyValue("Request", resultObject[i]) as Request;
                        if (req != null && resultRequest.Errors != null)
                        {
                            errors = resultRequest.Errors;
                            break;
                        }
                    }
                }

                // if errors were found, parse them and raise them as exceptions
                if (errors != null)
                {
                    ProcessErrors(errors, ret, opsReq);
                }
            }
            catch (Exception ex)
            {
                // log and rethrow;
                MTurkLog.Error("{0}: {1}{2}{3}", 
                    ex.GetType().Name, 
                    ex.Message, 
                    Environment.NewLine, 
                    ex.StackTrace);

                throw;
            }
            finally
            {
                MTurkConfig.CurrentInstance = null;
            }
            	    	
            return ret;	    	
	    }

        #region ITurkOperations implementation
        /// <summary>
        /// The GetAccountBalance operation retrieves the amount of money your 
        /// Amazon Mechanical Turk account, as well as the
        /// amount of money "on hold" pending the completion of transfers from 
        /// your bank account to your Amazon account.
        /// </summary>
        /// <param name="request">A <see cref="GetAccountBalanceRequest"/> instance 
        /// containing the request parameters</param>
        /// <returns>
        /// A <see cref="GetAccountBalanceResult"/> instance
        /// </returns>
        public GetAccountBalanceResult GetAccountBalance(GetAccountBalanceRequest request)
        {
            GetAccountBalanceResponse response = (GetAccountBalanceResponse)(this.SendRequest(request));

            return response.GetAccountBalanceResult[0];
        }

        /// <summary>
        /// The CreateHIT operation creates a new HIT. The new HIT is made available for Workers to 
        /// find and accept on the Mechanical Turk web site.
        /// </summary>
        /// <param name="request">The request parameters for this operation</param>
        /// <returns>A <see cref="HIT"/> instance</returns>
        /// <remarks> Once a HIT has been created, it cannot be deleted. A HIT may be removed from 
        /// the web site using the DisableHIT operation, but Workers that have already accepted the 
        /// HIT will still be allowed to submit results to claim rewards. See DisableHIT for more 
        /// information.</remarks>
        public HIT CreateHIT(CreateHITRequest request)
        {            
            request.Question = EnsureQuestionIsWrappedAndValid(request.Question, true);

            CreateHITResponse response = (CreateHITResponse)(this.SendRequest(request));

            return response.HIT[0];
        }

        /// <summary>
        /// The GetHIT operation retrieves the details of a HIT, using its HIT ID.
        /// </summary>
        /// <param name="request">A <see cref="GetHITRequest"/> instance containing the request 
        /// parameters</param>
        /// <returns>A <see cref="HIT"/> instance</returns>
        public HIT GetHIT(GetHITRequest request)
        {
            GetHITResponse response = (GetHITResponse)(this.SendRequest(request));

            return response.HIT[0];
        }

        /// <summary>
        /// The GetAssignment operation retrieves the details of an Assignment, using its Assignment ID.
        /// </summary>
        /// <param name="request">A <see cref="GetAssignmentRequest"/> instance containing the request 
        /// parameters</param>
        /// <returns>A <see cref="GetAssignmentResult"/> instance, which contains an <see cref="Assignment"/> and optionally the corresponding <see cref="HIT"/> </returns>
        public GetAssignmentResult GetAssignment(GetAssignmentRequest request)
        {
            GetAssignmentResponse response = (GetAssignmentResponse)(this.SendRequest(request));

            return response.GetAssignmentResult[0];
        }

        /// <summary>
        /// The DisableHIT operation removes a HIT from the Mechanical Turk marketplace, 
        /// approves all submitted assignments that have not already been approved or rejected, 
        /// and disposes of the HIT and all assignment data.
        /// </summary>
        /// <param name="request">A <see cref="DisableHITRequest"/> instance containing the 
        /// request parameters</param>
        /// <remarks> Assignments for the HIT that have already been submitted, but not yet 
        /// approved or rejected, will be automatically approved. Assignments in progress at 
        /// the time of the call to DisableHIT will be approved once the assignments are submitted. 
        /// You will be charged for approval of these assignments.</remarks>
        public void DisableHIT(DisableHITRequest request)
        {
            this.SendRequest(request);
        }

        /// <summary>
        /// The ExtendHIT operation increases the maximum number of assignments, or extends the 
        /// expiration date, of an existing HIT.
        /// </summary>
        /// <param name="request">A <see cref="ExtendHITRequest"/> instance containing the 
        /// request parameters</param>
        /// <remarks> If a HIT is not assignable (with a status of Unassignable or Reviewable) 
        /// due to either having reached its maximum number of assignments or having reached 
        /// its expiration date, extending the HIT can make it available again.</remarks>
        public void ExtendHIT(ExtendHITRequest request)
        {
            this.SendRequest(request);
        }

        /// <summary>
        /// The ForceExpireHIT operation causes a HIT to expire immediately, as if the HIT's 
        /// <c>LifetimeInSeconds</c> had elapsed.
        /// </summary>
        /// <param name="request">A <see cref="ForceExpireHITRequest"/> instance containing the 
        /// request parameters</param>
        /// <remarks> The effect is identical to the HIT expiring on its own: The HIT no longer 
        /// appears on the Mechanical Turk web site, and no new Workers are allowed to accept the HIT.
        /// Workers who have accepted the HIT prior to expiration are allowed to complete it or return it,
        /// or allow the assignment duration to elapse (abandon the HIT). Once all remaining assignments 
        /// have been submitted, the expired HIT becomes "reviewable", and will be returned by a call
        /// to GetReviewableHITs.</remarks>
        public void ForceExpireHIT(ForceExpireHITRequest request)
        {
            this.SendRequest(request);
        }

        /// <summary>
        /// The DisposeHIT operation disposes of a HIT that is no longer needed.
        /// </summary>
        /// <param name="request">A <see cref="DisposeHITRequest"/> instance containing the 
        /// request parameters</param>
        /// <remarks> Only HITs in the "reviewable" state, with all submitted assignments 
        /// approved or rejected, can be disposed. A Requester can call GetReviewableHITs
        /// to determine which HITs are reviewable, then call GetAssignmentsForHIT to retrieve
        /// the assignments. Disposing of a HIT removes the HIT from the results of a call to 
        /// GetReviewableHITs. If DisposeHIT is called on a HIT that is not "reviewable"
        /// (that has not expired or has active assignments), or on a HIT that is "reviewable"
        /// but not all of the submitted assignments have been approved or rejected, the service 
        /// will return an error.</remarks>
        public void DisposeHIT(DisposeHITRequest request)
        {
            this.SendRequest(request);
        }

        /// <summary>
        /// The SearchHITs operation returns all of a Requester's HITs, on behalf of the Requester.
        /// </summary>
        /// <param name="request">A <see cref="SearchHITsRequest"/> instance containing the 
        /// request parameters</param>
        /// <returns>
        /// A <see cref="SearchHITsResult"/> instance
        /// </returns>
        /// <remarks> The operation returns HITs of any status, except for HITs that have been 
        /// disposed with the DisposeHIT  operation.</remarks>
        public SearchHITsResult SearchHITs(SearchHITsRequest request)
        {
            SearchHITsResponse response = (SearchHITsResponse)(this.SendRequest(request));

            return response.SearchHITsResult[0];
        }

        /// <summary>
        /// The GetReviewableHITs operation retrieves the HITs that have a status of Reviewable, 
        /// or HITs that have a status of Reviewing, and that belong to the Requester calling 
        /// the operation.
        /// </summary>
        /// <param name="request">A <see cref="GetReviewableHITsRequest"/> instance containing 
        /// the request parameters</param>
        /// <returns>
        /// A <see cref="GetReviewableHITsResult"/> instance
        /// </returns>
        /// <remarks> You can use this operation to determine which of your HITs have results, 
        /// then retrieve those results with the GetAssignmentsForHIT operation. Once a HIT's 
        /// results have been retrieved and the assignments have been approved or rejected 
        /// (with ApproveAssignment or RejectAssignment), you can call DisposeHIT to remove 
        /// the HIT from the results of a call to GetReviewableHITs.</remarks>
        public GetReviewableHITsResult GetReviewableHITs(GetReviewableHITsRequest request)
        {
            GetReviewableHITsResponse response = (GetReviewableHITsResponse)(this.SendRequest(request));

            return response.GetReviewableHITsResult[0];
        }

        /// <summary>
        /// The RegisterHITType operation creates a new HIT type, a set of HIT properties which 
        /// can be used to create new HITs.
        /// </summary>
        /// <param name="request">A <see cref="RegisterHITTypeRequest"/> instance containing 
        /// the request parameters</param>
        /// <returns>The ID of the new HIT type</returns>
        public string RegisterHITType(RegisterHITTypeRequest request)
        {
            RegisterHITTypeResponse response = (RegisterHITTypeResponse)(this.SendRequest(request));

            return response.RegisterHITTypeResult[0].HITTypeId;
        }

        /// <summary>
        /// The SetHITTypeNotification operation creates, updates, disables or re-enables 
        /// notifications for a HIT type.
        /// </summary>
        /// <param name="request">A <see cref="SetHITTypeNotificationRequest"/> instance containing 
        /// the request parameters</param>
        /// <remarks> When a HIT type has an active notification, Mechanical Turk will attempt to 
        /// send a notification message when a HIT of the type changes state, such as when an assignment 
        /// is submitted for the HIT. The state changes to watch and the method of notification are described 
        /// in the notification specification given to SetHITTypeNotification.</remarks>
        public void SetHITTypeNotification(SetHITTypeNotificationRequest request)
        {
            this.SendRequest(request);
        }

        /// <summary>
        /// The SetHITAsReviewing operation updates a HIT with a status of Reviewable to have a  
        /// status of Reviewing, or reverts a Reviewing HIT back to the Reviewable status.
        /// </summary>
        /// <param name="request">A <see cref="SetHITAsReviewingRequest"/> instance containing 
        /// the request parameters</param>
        /// <remarks> The Reviewable and Reviewing status values for a HIT control which HITs are 
        /// returned by the GetReviewableHITs operation. A HIT's status is also returned with a 
        /// HIT's data, such as by a call to the GetHIT operation. Your application can manipulate 
        /// and query these status values as part of the HIT review process. For example, if 
        /// verification for a HIT's results is pending further information, the HIT can be moved 
        /// to the Reviewing status to prevent it from being returned by subsequent calls to 
        /// GetReviewableHITs.</remarks>
        public void SetHITAsReviewing(SetHITAsReviewingRequest request)
        {
            this.SendRequest(request);
        }

        /// <summary>
        /// The GetHITsForQualificationType operation returns the HITs that use the given Qualification 
        /// type for a Qualification requirement.
        /// </summary>
        /// <param name="request">Type of the get HITs for qualification.</param>
        /// <returns>
        /// A <see cref="GetHITsForQualificationTypeResult"/> instance
        /// </returns>
        /// <remarks> The operation returns HITs of any status, except for HITs that have been disposed 
        /// with the DisposeHIT  operation.
        /// <para></para>Only HITs that you created will be returned by the query. </remarks>
        public GetHITsForQualificationTypeResult GetHITsForQualificationType(GetHITsForQualificationTypeRequest request)
        {
            GetHITsForQualificationTypeResponse response = (GetHITsForQualificationTypeResponse)(this.SendRequest(request));

            return response.GetHITsForQualificationTypeResult[0];
        }

        /// <summary>
        /// The SendTestEventNotification operation causes Mechanical Turk to send a notification 
        /// message as if a HIT event occurred, according to the provided notification specification. 
        /// This allows you to test your notification receptor logic without setting up notifications 
        /// for a real HIT type and trying to trigger them using the web site.
        /// </summary>
        /// <param name="request">A <see cref="SendTestEventNotificationRequest"/> instance containing 
        /// the request parameters</param>
        public void SendTestEventNotification(SendTestEventNotificationRequest request)
        {
            this.SendRequest(request);
        }

        /// <summary>
        /// The GetQualificationsForQualificationType operation returns all of the Qualifications 
        /// granted to Workers for a given Qualification type.
        /// </summary>
        /// <param name="request">A <see cref="GetQualificationsForQualificationTypeRequest"/> instance 
        /// containing the request parameters</param>
        /// <returns>
        /// A <see cref="GetQualificationsForQualificationTypeResult"/> instance
        /// </returns>
        /// <remarks> Results are divided into numbered "pages," and a single page of results is 
        /// returned by the operation. Pagination can be controlled with parameters to the operation.</remarks>
        public GetQualificationsForQualificationTypeResult GetQualificationsForQualificationType(GetQualificationsForQualificationTypeRequest request)
        {
            GetQualificationsForQualificationTypeResponse response = (GetQualificationsForQualificationTypeResponse)(this.SendRequest(request));

            return response.GetQualificationsForQualificationTypeResult[0];
        }

        /// <summary>
        /// The ApproveAssignment operation approves the results of a completed assignment.
        /// </summary>
        /// <param name="request">A <see cref="ApproveAssignmentRequest"/> instance containing 
        /// the request parameters</param>
        /// <remarks>
        /// Approving an assignment initiates two payments from the Requester's Amazon.com account:
        /// the Worker that submitted the results is paid the reward specified in the HIT, and
        /// Mechanical Turk fees are debited. If the Requester's account does not have adequate funds
        /// for these payments, the call to ApproveAssignment will return an exception, and the approval
        /// will not be processed.
        /// </remarks>
        public void ApproveAssignment(ApproveAssignmentRequest request)
        {
            this.SendRequest(request);
        }

        /// <summary>
        /// The RejectAssignment operation rejects the results of a completed assignment.
        /// </summary>
        /// <param name="request">A <see cref="RejectAssignmentRequest"/> instance containing 
        /// the request parameters</param>
        /// <remarks> Rejecting an assignment indicates that the Requester believes the results 
        /// submitted by the Worker do not properly answer the question described by the HIT.
        /// The Worker is not paid for a rejected assignment.</remarks>
        public void RejectAssignment(RejectAssignmentRequest request)
        {
            this.SendRequest(request);
        }

        /// <summary>
        /// The ApproveRejectedAssignment operation approves a rejected assignment. 
        /// </summary>
        /// <param name="request">A <see cref="ApproveRejectedAssignmentRequest"/> instance containing 
        /// the request parameters</param>
        /// <remarks>
        /// Approving a rejected assignment reverses the rejection of an assignment and peforms an approval.
        /// If the Requester's account does not have adequate funds for the payments associated with approval, 
        /// the call to ApproveRejectedAssignment will return an exception, and the approval
        /// will not be processed.
        /// </remarks>
        public void ApproveRejectedAssignment(ApproveRejectedAssignmentRequest request)
        {
            this.SendRequest(request);
        }

        /// <summary>
        /// The GetAssignmentsForHIT operation retrieves completed assignments for a HIT. 
        /// You can use this operation to retrieve the results for a HIT.
        /// </summary>
        /// <param name="request">A <see cref="GetAssignmentsForHITRequest"/> instance containing 
        /// the request parameters</param>
        /// <returns>
        /// A <see cref="GetAssignmentsForHITResult"/> instance
        /// </returns>
        /// <remarks> You can get assignments for a HIT at any time, even if the HIT is not 
        /// yet "reviewable". If a HIT requested multiple assignments, and has received some 
        /// results but has not yet become "reviewable", you can still retrieve the partial 
        /// results with GetAssignmentsForHIT.</remarks>
        public GetAssignmentsForHITResult GetAssignmentsForHIT(GetAssignmentsForHITRequest request)
        {
            GetAssignmentsForHITResponse response = (GetAssignmentsForHITResponse)(this.SendRequest(request));

            return response.GetAssignmentsForHITResult[0];
        }

        /// <summary>
        /// The GetFileUploadURL operation generates and returns a temporary URL for the 
        /// purposes of retrieving a file uploaded by a Worker as an answer to a FileUploadAnswer 
        /// question for a HIT.
        /// </summary>
        /// <param name="request">A <see cref="GetFileUploadURLRequest"/> instance containing 
        /// the request parameters</param>
        /// <returns>
        /// A <see cref="String"/> containing the upload URL
        /// </returns>
        /// <remarks> The temporary URL is generated the instant the GetFileUploadURL operation 
        /// is called, and is valid for 60 seconds.</remarks>
        public string GetFileUploadURL(GetFileUploadURLRequest request)
        {
            GetFileUploadURLResponse response = (GetFileUploadURLResponse)(this.SendRequest(request));

            return response.GetFileUploadURLResult[0].FileUploadURL;
        }

        /// <summary>
        /// The GrantBonus operation issues a payment of money from your account to a Worker. 
        /// To be eligible for a bonus, the Worker must have submitted results for one of your 
        /// HITs, and have had those results approved or rejected. This payment happens separately 
        /// from the reward you pay to the Worker when you approve the Worker's assignment.
        /// </summary>
        /// <param name="request">A <see cref="GrantBonusRequest"/> instance containing 
        /// the request parameters</param>
        public void GrantBonus(GrantBonusRequest request)
        {
            this.SendRequest(request);
        }

        /// <summary>
        /// The GetBonusPayments operation retrieves the amounts of bonuses you have paid to 
        /// Workers for a given HIT or assignment.
        /// </summary>
        /// <param name="request">A <see cref="GetBonusPaymentsRequest"/> instance containing 
        /// the request parameters</param>
        /// <returns>
        /// A <see cref="GetBonusPaymentsResult"/> instance
        /// </returns>
        public GetBonusPaymentsResult GetBonusPayments(GetBonusPaymentsRequest request)
        {
            GetBonusPaymentsResponse response = (GetBonusPaymentsResponse)(this.SendRequest(request));

            return response.GetBonusPaymentsResult[0];
        }

        /// <summary>
        /// The CreateQualificationType operation creates a new Qualification type.
        /// </summary>
        /// <param name="request">Type of the create qualification.</param>
        /// <returns>
        /// A <see cref="QualificationType"/> instance
        /// </returns>
        /// <remarks> Every Qualification has a Qualification type. The creator of the type can assign
        /// Qualifications of that type to Workers, and grant requests for Qualifications of the type 
        /// made by Workers. A Qualification can be considered a statement about a Worker made by the 
        /// Qualification type's owner. A Qualification type may include a Qualification test, a set 
        /// of questions a Worker must answer to request the Qualification. The type may also include 
        /// an answer key for the test. Qualification requests for types with answer keys are granted 
        /// automatically by Mechanical Turk, using a value calculated from the answer key and the 
        /// Worker's test answers. If the Qualification type does not have a test, or does
        /// not have an answer key, the type's owner is responsible for polling for and granting 
        /// Qualification requests.</remarks>
        public QualificationType CreateQualificationType(CreateQualificationTypeRequest request)
        {
            request.Test = EnsureQuestionIsWrappedAndValid(request.Test, false);

            CreateQualificationTypeResponse response = (CreateQualificationTypeResponse)(this.SendRequest(request));

            return response.QualificationType[0];
        }

        /// <summary>
        /// The GrantQualification operation grants a user's request for a Qualification.
        /// </summary>
        /// <param name="request">A <see cref="GrantQualificationRequest"/> instance containing 
        /// the request parameters</param>
        /// <remarks> Only the owner of the Qualification type can grant a Qualification request 
        /// for that type.</remarks>
        public void GrantQualification(GrantQualificationRequest request)
        {
            this.SendRequest(request);
        }

        /// <summary>
        /// The AssignQualification operation gives a Worker a Qualification. AssignQualification 
        /// does not require that the Worker submit a Qualification request: It gives the 
        /// Qualification directly to the Worker.
        /// </summary>
        /// <param name="request">The request parameters for this operation</param>
        /// <remarks>
        /// You can assign a Qualification to any Worker that has submitted one of your HITs in the past.
        /// You can only assign a Qualification of a Qualification type that you created.
        /// </remarks>
        public void AssignQualification(AssignQualificationRequest request)
        {
            this.SendRequest(request);
        }

        /// <summary>
        /// The RevokeQualification operation revokes a previously granted Qualification from a user. 
        /// If the user had a Qualification of the given Qualification type, after revoking it, the 
        /// user will no longer have the Qualification, and will not qualify for HITs whose 
        /// Qualification requirements say the user must have the Qualification.
        /// </summary>
        /// <param name="request">A <see cref="RevokeQualificationRequest"/> instance containing 
        /// the request parameters</param>
        /// <remarks> Once a Qualification is revoked from a user, the user cannot be granted the 
        /// Qualification until the user requests the Qualification again. Depending on how the 
        /// Qualification type's retry policy is configured, the user may be restricted from 
        /// requesting the Qualification a second time.</remarks>
        public void RevokeQualification(RevokeQualificationRequest request)
        {
            this.SendRequest(request);
        }

        /// <summary>
        /// The GetQualificationType operation retrieves information about a 
        /// Qualification type using its ID.
        /// </summary>
        /// <param name="request">Type of the get qualification.</param>
        /// <returns>
        /// A <see cref="QualificationType"/> instance
        /// </returns>
        public QualificationType GetQualificationType(GetQualificationTypeRequest request)
        {
            GetQualificationTypeResponse response = (GetQualificationTypeResponse)(this.SendRequest(request));

            return response.QualificationType[0];
        }

        /// <summary>
        /// The GetQualificationRequests operation retrieves requests for Qualifications 
        /// of a particular Qualification type. The Qualification type's owner calls this 
        /// operation to poll for pending requests, and grants Qualifications based on the 
        /// requests using the GrantQualification operation.
        /// </summary>
        /// <param name="request">A <see cref="GetQualificationRequestsRequest"/> instance 
        /// containing the request parameters</param>
        /// <returns>
        /// A <see cref="GetQualificationRequestsResult"/> instance
        /// </returns>
        /// <remarks> Only requests for Qualifications that require the type owner's attention 
        /// are returned by GetQualificationRequests. Requests awaiting Qualification test answers, 
        /// and requests that have already been granted, are not returned.
        /// <para></para>
        /// Only the owner of the Qualification type can retrieve its requests. </remarks>
        public GetQualificationRequestsResult GetQualificationRequests(GetQualificationRequestsRequest request)
        {
            GetQualificationRequestsResponse response = 
                (GetQualificationRequestsResponse)(this.SendRequest(request));

            return response.GetQualificationRequestsResult[0];
        }

        /// <summary>
        /// The RejectQualificationRequest operation rejects a user's request for a Qualification. 
        /// Once a Qualification request is rejected, it will no longer be returned by a call 
        /// to the GetQualificationRequests operation.
        /// </summary>
        /// <param name="request">A <see cref="RejectQualificationRequestRequest"/> instance 
        /// containing the request parameters</param>
        /// <remarks> Rejecting the Qualification request does not change the user's Qualifications: 
        /// If the user already has a Qualification of the corresponding Qualification type, the user 
        /// will continue to have the Qualification with the previously assigned score. If the user 
        /// does not have the Qualification, the user will still not have it after th request is 
        /// rejected.</remarks>
        public void RejectQualificationRequest(RejectQualificationRequestRequest request)
        {
            this.SendRequest(request);
        }

        /// <summary>
        /// The GetRequesterStatistic operation retrieves the value of one of several statistics 
        /// about you (the Requester calling the operation).
        /// </summary>
        /// <param name="request">A <see cref="GetRequesterStatisticRequest"/> instance containing 
        /// the request parameters</param>
        /// <returns>
        /// A <see cref="GetStatisticResult"/> instance
        /// </returns>
        /// <remarks>
        /// Mechanical Turk keeps track of many statistics about users and system activity. 
        /// Statistics are calculated and recorded for each calendar day. GetRequesterStatistic  
        /// can return data points for each of multiple days up to the current day, or an aggregate 
        /// value for a longer time period up to the current day.
        /// <para></para>
        /// A single day's statistic represents the change in an overall value that has resulted 
        /// from the day's activity. For example, the NumberAssignmentsApproved statistic reports 
        /// the number of assignments you have approved in a given day. If you do not approve any
        /// assignments for a day, the value will be 0 for that day. If you approve fifty assignments 
        /// that day, the value will be 50.
        /// </remarks>
        public GetStatisticResult GetRequesterStatistic(GetRequesterStatisticRequest request)
        {
            GetRequesterStatisticResponse response = (GetRequesterStatisticResponse)(this.SendRequest(request));

            return response.GetStatisticResult[0];
        }

        /// <summary>
        /// The GetQualificationScore operation returns the value of a user's Qualification for a 
        /// given Qualification type.
        /// </summary>
        /// <param name="request">A <see cref="GetQualificationScoreRequest"/> instance containing 
        /// the request parameters</param>
        /// <returns>A <see cref="Qualification"/> instance</returns>
        /// <remarks> To get a user's Qualification, you must know the user's ID. A Worker's user ID 
        /// is included in the assignment data returned by the GetAssignmentsForHIT operation.</remarks>
        public Qualification GetQualificationScore(GetQualificationScoreRequest request)
        {
            GetQualificationScoreResponse response = (GetQualificationScoreResponse)(this.SendRequest(request));

            return response.Qualification[0];
        }

        /// <summary>
        /// The SearchQualificationTypes operation searches for Qualification types using the specified 
        /// search query, and returns a list of Qualification types.
        /// </summary>
        /// <param name="request">A <see cref="SearchQualificationTypesRequest"/> instance containing 
        /// the request parameters</param>
        /// <returns>
        /// A <see cref="SearchQualificationTypesResult"/> instance
        /// </returns>
        /// <remarks> Results are sorted and divided into numbered "pages," and a single page of results 
        /// is returned by the operation. Sorting and pagination can be controlled with parameters to the 
        /// operation.</remarks>
        public SearchQualificationTypesResult SearchQualificationTypes(SearchQualificationTypesRequest request)
        {
            SearchQualificationTypesResponse response = (SearchQualificationTypesResponse)(this.SendRequest(request));

            return response.SearchQualificationTypesResult[0];
        }

        /// <summary>
        /// The UpdateQualificationType operation modifies attributes of an existing Qualification type.
        /// </summary>
        /// <param name="request">A <see cref="UpdateQualificationTypeRequest"/> instance containing the 
        /// request parameters</param>
        /// <returns>
        /// A <see cref="QualificationType"/> instance
        /// </returns>
        /// <remarks> Most attributes of a Qualification type can be changed after the type has been created. 
        /// The Name and Keywords fields cannot be modified. If you create a Qualification type and decide 
        /// you do not wish to use it with its name or keywords as they were created, update the type with 
        /// a new QualificationTypeStatus of Inactive, then create a new type using CreateQualificationType 
        /// with the desired values.</remarks>
        public QualificationType UpdateQualificationType(UpdateQualificationTypeRequest request)
        {
            request.Test = EnsureQuestionIsWrappedAndValid(request.Test, false);

            UpdateQualificationTypeResponse response = (UpdateQualificationTypeResponse)(this.SendRequest(request));

            return response.QualificationType[0];
        }

        /// <summary>
        /// The UpdateQualificationScore operation changes the value of a Qualification previously granted 
        /// to a user.
        /// </summary>
        /// <param name="request">A <see cref="UpdateQualificationScoreRequest"/> instance containing the 
        /// request parameters</param>
        /// <remarks> To update a user's Qualification, you must know the user's ID. A Worker's user ID is 
        /// included in the assignment data returned by the GetAssignmentsForHIT operation.</remarks>
        public void UpdateQualificationScore(UpdateQualificationScoreRequest request)
        {
            this.SendRequest(request);
        }

        /// <summary>
        /// The Help operation returns information about the Mechanical Turk Service operations and 
        /// response groups. You can use it to facilitate development and documentation of your web 
        /// site and tools.
        /// </summary>
        /// <param name="request">A <see cref="HelpRequest"/> instance containing 
        /// the request parameters</param>
        /// <returns>A <see cref="Information"/> instance</returns>
        public Information Help(HelpRequest request)
        {
            HelpResponse response = (HelpResponse)(this.SendRequest(request));

            return response.Information[0];
        }

        /// <summary>
        /// The NotifyWorkers operation sends e-mail to one or more Workers, given the recipients' 
        /// Worker IDs.
        /// </summary>
        /// <param name="request">A <see cref="NotifyWorkersRequest"/> instance containing the 
        /// request parameters</param>
        /// <remarks>
        /// Worker IDs are included in the assignment data returned by GetAssignmentsForHIT. You can 
        /// send e-mail to any Worker who has ever submitted results for a HIT you created that you 
        /// have approved or rejected.
        /// <para></para>
        /// The e-mail sent to Workers includes your e-mail address as the "reply-to" address, so 
        /// Workers can respond to the e-mail</remarks>
        public void NotifyWorkers(NotifyWorkersRequest request)
        {
            this.SendRequest(request);
        }

        /// <summary>
        /// Blocks a worker from accepting your HITs
        /// </summary>
        /// <param name="request">A <see cref="BlockWorkerRequest"/> instance containing 
        /// the request parameters</param>
        public void BlockWorker(BlockWorkerRequest request)
        {
            this.SendRequest(request);
        }

        /// <summary>
        /// The GetBlockedWorkers operation returns all of the Workers you have blocked using BlockWorker.
        /// </summary>
        /// <param name="request">A <see cref="GetBlockedWorkersRequest"/> instance 
        /// containing the request parameters</param>
        /// <returns>
        /// A <see cref="BlockWorkerResult"/> instance
        /// </returns>
        /// <remarks> Results are divided into numbered "pages," and a single page of results is 
        /// returned by the operation. Pagination can be controlled with parameters to the operation.</remarks>
        public GetBlockedWorkersResult GetBlockedWorkers(GetBlockedWorkersRequest request)
        {
            GetBlockedWorkersResponse response = (GetBlockedWorkersResponse)(this.SendRequest(request));

            return response.GetBlockedWorkersResult[0];
        }

        /// <summary>
        /// Unblocks a worker who was previously blocked from accepting your HITs
        /// </summary>
        /// <param name="request">A <see cref="UnblockWorkerRequest"/> instance containing 
        /// the request parameters</param>
        public void UnblockWorker(UnblockWorkerRequest request)
        {
            this.SendRequest(request);
        }

        /// <summary>
        /// Changes the HIT type for a HIT
        /// </summary>
        /// <param name="request">A <see cref="UnblockWorkerRequest"/> instance containing 
        /// the request parameters</param>
        public void ChangeHITTypeOfHIT(ChangeHITTypeOfHITRequest request)
        {            
            this.SendRequest(request);
        }

        /// <summary>
        /// Gets the results of any review policies that were applied to a HIT.
        /// </summary>
        /// <param name="request">A <see cref="GetReviewResultsForHITRequest"/> instance containing the request parameters</param>
        /// <returns>A <see cref="GetReviewResultsForHITResult"/> instance containing the review results</returns>
        public GetReviewResultsForHITResult GetReviewResultsForHIT(GetReviewResultsForHITRequest request)
        {
            GetReviewResultsForHITResponse response = (GetReviewResultsForHITResponse)this.SendRequest(request);
            return response.GetReviewResultsForHITResult[0];
        }

        /// <summary>
        /// Gets a worker-specific statistic
        /// </summary>
        /// <param name="request">A <see cref="GetRequesterWorkerStatisticRequest"/> instance containing the request parameters</param>
        /// <returns>A <see cref="GetStatisticResult"/> instance containing the statistic</returns>
        public GetStatisticResult GetRequesterWorkerStatistic(GetRequesterWorkerStatisticRequest request)
        {
            GetRequesterWorkerStatisticResponse response = (GetRequesterWorkerStatisticResponse)this.SendRequest(request);
            return response.GetStatisticResult[0];
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Disposes the Mechanical Turk client and any resources associated with it
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_protocol != null)
                {
                    _protocol.Dispose();
                }
            }
        }

        #endregion
    }
}
