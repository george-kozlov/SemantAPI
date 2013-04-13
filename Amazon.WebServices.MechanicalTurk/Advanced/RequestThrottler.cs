#region Copyright & license notice
/*
 * Copyright: Copyright (c) 2007 Amazon Technologies, Inc.
 * License:   Apache License, Version 2.0
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Amazon.WebServices.MechanicalTurk.Advanced
{
    /// <summary>
    /// Throttles requests to avoid HTTP issues when testing SDK software against the sandbox environment
    /// (See <a href="http://sandbox.mturk.com">"guidelines and policies"</a>)
    /// </summary>
    /// <remarks>You can set a specific throttler for a service endpoint via the <c>Throttler</c>
    /// property of the <see cref="MTurkClient"/> class.</remarks>
    public class RequestThrottler : IDisposable
    {
        private int curTokenCount = 0;      // number of tokens currently in the bucket
        private string endpoint;            // endpoint associated with throttler
        private int capacity = 0;           // capacity of the bucket 
        private int ratePerSecond = 0;      // refill rate of tokens per second
        private System.Threading.Timer timerRefill;

        // FIFO list of enqueued threads waiting for tokens to become available
        private List<ManualResetEvent> queue = new List<ManualResetEvent>();    

        // current instances (endpoint->throttler)
        private static Dictionary<string, RequestThrottler> instances = new Dictionary<string, RequestThrottler>();

        /// <summary>
        /// Returns a throttler for a specific service endpoint URL
        /// </summary>
        /// <param name="serviceEndpoint">URL of the mechanical turk service endpoint</param>
        /// <param name="capacity">Number of requests the throttler permits all at once 
        /// (bucket capacity)</param>
        /// <param name="rate">Number of requests the throttler allows per second 
        /// (average long term)</param>
        /// <returns>A <see cref="RequestThrottler"/> instance</returns>
        public static RequestThrottler GetInstance(string serviceEndpoint, int capacity, int rate)
        {
            if (serviceEndpoint == null)
            {
                throw new ArgumentNullException("serviceEndpoint", "Endpoint URL may not be null");
            }

            if (capacity <= 0)
            {
                throw new ArgumentException("Capacity must be bigger than zero", "capacity");
            }

            if (rate > capacity)
            {
                throw new ArgumentException("Rate must be bigger than capacity", "rate");
            }

            RequestThrottler ret = null;
            string key = string.Format("{0}{1}{2}", serviceEndpoint, capacity, rate);

            if (instances.ContainsKey(key))
            {
                ret = instances[key];
            }
            else
            {
                lock (instances)
                {
                    if (instances.ContainsKey(key))
                    {
                        ret = instances[key];
                    }
                    else
                    {
                        MTurkLog.Debug("Throttling requests to {0} (Capacity: {1}. Rate: {2}/sec)", serviceEndpoint, capacity, rate);
                        ret = new RequestThrottler(serviceEndpoint, capacity, rate);
                        instances[key] = ret;
                    }
                }
            }            

            return ret;
        }

        /// <summary>
        /// Throttles requests to the service endpoint (for sandbox)
        /// </summary>
        /// <param name="capacity">Number of requests permitted all at once (bucket capacity)</param>
        /// <param name="rate">Number of requests allowed per second (average long term)</param>
        /// <param name="serviceEndpoint">Web service endpoint for this throttler</param>
        private RequestThrottler(string serviceEndpoint, int capacity, int rate)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException("Capacity must be bigger than zero", "capacity");
            }

            if (rate <= 0)
            {
                throw new ArgumentException("Rate must be bigger than zero", "rate");
            }

            this.capacity = (int)capacity;
            this.ratePerSecond = (int)rate;
            this.curTokenCount = this.capacity;
            this.endpoint = serviceEndpoint;

            timerRefill = new Timer(new TimerCallback(OnRefill), null, 1000, 1000);
        }

        /// <summary>
        /// Refills the bucket and works the backlog in the order the requests came in
        /// </summary>
        private void OnRefill(object o)
        {
            // add tokens to the bucket
            if (Add(ratePerSecond) < capacity)
            {
                MTurkLog.Debug("Refilled {0} tokens to throttle bucket (Current size: {1})", ratePerSecond, curTokenCount);
            }

            // work backlog in order            
            lock (queue)
            {
                if (queue.Count > 0)
                {
                    int num = Math.Min(queue.Count, curTokenCount);
                    MTurkLog.Debug("Processing {0} throttled requests from backlog (Size: {1})", num, queue.Count);
                    for (int i = 0; i < num; i++)
                    {
                        if (Add(-1) != null)
                        {
                            // signal waiting thread to resume sending FIFO
                            queue[0].Set();
                            queue.RemoveAt(0);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Returns null, if no tokens are available and throttling must start
        /// </summary>
        private int? Add(int i)
        {
            lock (queue)
            {
                curTokenCount += i;
                if (curTokenCount < 0)
                {
                    curTokenCount = 0;

                    return null;    // indicate that throttling
                }
                else if (curTokenCount > capacity)
                {
                    // cap to avoid overflow of bucket through refill timer
                    curTokenCount = capacity;    
                }

                return curTokenCount;
            }
        }

        /// <summary>
        /// Starts a throttled request. If it can get a slice from the bucket, then it
        /// can run immediately. Otherwise enqueue it and notify it once a slice becomes available.
        /// </summary>
        internal void StartRequest()
        {
            if (Add(-1) == null)
            {
                // No more tokens available: enqueue thread
                MTurkLog.Debug("Throttling request");

                ManualResetEvent evt = new ManualResetEvent(false);
                lock (queue)
                {                                                           
                    queue.Add(evt);                    
                }
                evt.WaitOne();
                
                //Thread.CurrentThread.Suspend();
                MTurkLog.Debug("Released throttle on request");                
            }
        }

        /// <summary>
        /// Returns pertinent information about the trottler configuration 
        /// (burst/capacity and refresh rate)
        /// </summary>
        public override string ToString()
        {
            return string.Format("RequestThrottler for '{2}' (Burst: {0}, Rate {1})", this.capacity, this.ratePerSecond, this.endpoint);
        }

        #region IDisposable Members

        /// <summary>
        /// Disposes the throttler instance and any resources associated with it
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
                lock (instances)
                {
                    string key = string.Format("{0}{1}{2}", endpoint, capacity, ratePerSecond);
                    RequestThrottler throttler = instances[key];

                    MTurkLog.Debug("Disposing {0}", throttler);
                    if (timerRefill != null)
                    {
                        timerRefill.Dispose();
                    }
                    
                    instances.Remove(key);
                }
            }
        }

        #endregion
    }
}
