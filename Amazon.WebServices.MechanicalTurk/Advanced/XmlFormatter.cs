#region Copyright & license notice
/*
 * Copyright: Copyright (c) 2007 Amazon Technologies, Inc.
 * License:   Apache License, Version 2.0
 */
#endregion

using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Amazon.WebServices.MechanicalTurk;

namespace Amazon.WebServices.MechanicalTurk.Advanced
{
    /// <summary>
    /// Formats and parses objects to and from XML using .Net XMLSerialization
    /// </summary>
    internal class XmlFormatter : IModelObjectFormatter
    {
        #region Singleton
        private static IModelObjectFormatter _instance = new XmlFormatter();
        internal static IModelObjectFormatter Instance
        {
            get { return _instance; }
        }

        private XmlFormatter()
        {
        }
        #endregion

        #region IModelObjectFormatter Members

        /// <summary>
        /// Formats the specified object.
        /// </summary>
        /// <param name="writer">The output writer.</param>
        /// <param name="obj">The object to format</param>
        /// <returns>Formatted string</returns>
        public void Format(TextWriter writer, object obj)
        {
            XmlUtil.SerializeXML(obj, writer);
        }

        /// <summary>
        /// Parses the specified string in an object of the specified type
        /// </summary>
        /// <param name="reader">The input reader</param>
        /// <param name="t">The type of the object that needs to be parsed</param>
        /// <returns>The parsed object</returns>
        public object Parse(TextReader reader, Type t)
        {
            return XmlUtil.DeserializeXML(t, reader);
        }

        #endregion
    }
}
