#region Copyright & license notice
/*
 * Copyright: Copyright (c) 2007 Amazon Technologies, Inc.
 * License:   Apache License, Version 2.0
 */
#endregion

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace Amazon.WebServices.MechanicalTurk.Advanced
{
    /// <summary>
    /// Stream used to log the soap request and response to the <see cref="ILog"/> interface
    /// </summary>
    internal class LogStream : Stream
    {
        private static string END_OF_ENVELOPE = "</soap:Envelope>";

        private Stream _sink;
        private StringBuilder sb = new StringBuilder();

        public LogStream(Stream orgStream)
        {
            _sink = orgStream;
        }

        public override bool CanRead
        {
            get { return _sink.CanRead; }
        }

        public override bool CanSeek
        {
            get { return _sink.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return _sink.CanWrite; }
        }

        public override long Length
        {
            get { return _sink.Length; }
        }

        public override long Position
        {
            get { return _sink.Position; }
            set { _sink.Position = value; }
        }

        public override long Seek(long offset, System.IO.SeekOrigin direction)
        {
            return _sink.Seek(offset, direction);
        }

        public override void SetLength(long length)
        {
            _sink.SetLength(length);
        }

        public override void Close()
        {
            // write buffer to ILog (to the end of the soap envelope)
            string s = sb.ToString().Trim();
            int i = s.IndexOf(END_OF_ENVELOPE);
                
            MTurkLog.Debug(s.Substring(0, i+END_OF_ENVELOPE.Length));
            sb = null;

            _sink.Close();
        }

        public override void Flush()
        {
            _sink.Flush();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _sink.Write(buffer, offset, count);

            AppendBuffer(buffer, offset, count);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {            
            AppendBuffer(buffer, offset, count);

            return _sink.Read(buffer, offset, count); ;
        }

        private void AppendBuffer(byte[] buffer, int offset, int count)
        {
            if (buffer.Length > 0 && buffer[0] != 0)
            {
                sb.Append(System.Text.Encoding.UTF8.GetChars(buffer, offset, count));
            }
        }
    }

    /// <summary>
    /// WebResponse wrapper to log the soap request to the <see cref="ILog"/> interface
    /// </summary>
    internal class LoggedWebReqest : WebRequest
    {
        private WebRequest wr;
        private Stream request_stream;

        public LoggedWebReqest(WebRequest org)
        {
            wr = org;
        }

        public override string Method
        {
            get { return wr.Method; }
            set { wr.Method = value; }
        }

        public override Uri RequestUri
        {
            get { return wr.RequestUri; }
        }

        public override WebHeaderCollection Headers
        {
            get { return wr.Headers; }
            set { wr.Headers = value; }
        }

        public override long ContentLength
        {
            get { return wr.ContentLength; }
            set { wr.ContentLength = value; }
        }

        public override string ContentType
        {
            get { return wr.ContentType; }
            set { wr.ContentType = value; }
        }

        public override ICredentials Credentials
        {
            get { return wr.Credentials; }
            set { wr.Credentials = value; }
        }

        public override bool PreAuthenticate
        {
            get { return wr.PreAuthenticate; }
            set { wr.PreAuthenticate = value; }
        }        

        public override System.IO.Stream GetRequestStream()
        {
            return WrappedRequestStream(wr.GetRequestStream());
        }

        public override IAsyncResult BeginGetRequestStream(AsyncCallback callback, object state)
        {
            return wr.BeginGetRequestStream(callback, state);
        }

        public override System.IO.Stream EndGetRequestStream(IAsyncResult asyncResult)
        {
            return WrappedRequestStream(wr.EndGetRequestStream(asyncResult));
        }

        private Stream WrappedRequestStream(Stream streamToWrap)
        {
            if (request_stream == null)
            {
                request_stream = new LogStream(streamToWrap);
            }

            return request_stream;
        }

        public override WebResponse GetResponse()
        {
            return new LoggedWebResponse(wr.GetResponse());
        }

        public override IAsyncResult BeginGetResponse(AsyncCallback callback, object state)
        {
            return wr.BeginGetResponse(callback, state);
        }

        public override WebResponse EndGetResponse(IAsyncResult asyncResult)
        {
            return new LoggedWebResponse(wr.EndGetResponse(asyncResult));
        }
    }

    /// <summary>
    /// WebResponse wrapper to log the soap response to the <see cref="ILog"/> interface
    /// </summary>
    internal class LoggedWebResponse : WebResponse
    {
        private WebResponse wr;
        private Stream response_stream;

        public LoggedWebResponse(WebResponse org)
        {
            wr = org;
        }

        public override Stream GetResponseStream()
        {
            if (response_stream == null)
            {
                response_stream = new LogStream(wr.GetResponseStream());
            }

            return response_stream;
        }

        public override long ContentLength
        {
            get { return wr.ContentLength; }
            set { wr.ContentLength = value; }
        }

        public override string ContentType
        {
            get { return wr.ContentType; }
            set { wr.ContentType = value; }
        }

        public override Uri ResponseUri
        {
            get { return wr.ResponseUri; }
        }

        public override WebHeaderCollection Headers
        {
            get { return wr.Headers; }
        }
    }
}
