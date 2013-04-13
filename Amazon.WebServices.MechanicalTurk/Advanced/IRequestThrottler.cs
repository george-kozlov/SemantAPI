using System;
using System.Collections.Generic;
using System.Text;

namespace Amazon.WebServices.MechanicalTurk.Advanced
{
    /// <summary>
    /// Throttles requests to avoid HTTP issues when testing SDK software against the sandbox environment
    /// (See <a href="http://sandbox.mturk.com">"guidelines and policies"</a>)
    /// </summary>
    /// <remarks>You can set a specific throttler for a service endpoint via the <c>Throttler</c>
    /// property of the <see cref="MTurkClient"/> class.</remarks>
    public interface IRequestThrottler
    {
        /// <summary>
        /// Notify the throttler that a request is about to begin. This method will block if the request should be throttled.
        /// </summary>
        void StartRequest();
    }
}
