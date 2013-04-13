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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Xml.XPath;
using System.Xml.Xsl;
using Amazon.WebServices.MechanicalTurk.Advanced;

namespace Amazon.WebServices.MechanicalTurk
{
    /// <summary>
    /// XML utility for xml validation, transformation and serialization
    /// </summary>
    public sealed class XmlUtil
    {
        private static UTF8Encoding encoder = new UTF8Encoding(false);

        private static Dictionary<string, XmlReaderSettings> xslSettingsCache = 
            new Dictionary<string, XmlReaderSettings>();
        private static Dictionary<string, XslCompiledTransform> xslCache = 
            new Dictionary<string, XslCompiledTransform>();

        private XmlUtil()
        {
        }

        #region Validation
        private static XmlReaderSettings GetCachedXmlReaderSettings(string schemaName)
        {
            XmlReaderSettings options = null;

            if (xslSettingsCache.ContainsKey(schemaName))
            {
                options = xslSettingsCache[schemaName];
            }
            else
            {
                lock (xslSettingsCache)
                {
                    if (xslSettingsCache.ContainsKey(schemaName))
                    {
                        options = xslSettingsCache[schemaName];
                    }
                    else
                    {
                        // load schema resource
                        string resSchema = typeof(XmlUtil).Assembly.GetName().Name + ".Resources." + schemaName;
                        using (Stream s = typeof(XmlUtil).Assembly.GetManifestResourceStream(resSchema))
                        {
                            XmlSchema resolvedSchema = XmlSchema.Read(s, new ValidationEventHandler(RaiseValidationError));
                            XmlSchemaSet schemaColl = new XmlSchemaSet();
                            schemaColl.Add(resolvedSchema);
                            schemaColl.Compile();

                            options = new XmlReaderSettings();
                            options.Schemas = schemaColl;
                            options.ValidationType = ValidationType.Schema;
                            options.ValidationEventHandler += new ValidationEventHandler(RaiseValidationError);

                            xslSettingsCache[schemaName] = options;
                        }
                    }
                }
            }

            return options;
        }

        /// <summary>
        /// Validates a XML file or fragment against an XSL resource (such as "Question.xsd");
        /// </summary>
        /// <param name="schemaName">Name of the schema resource.</param>
        /// <param name="xmlFileOrFragment">The XML file or fragment to validate.</param>
        public static void ValidateXML(string schemaName, string xmlFileOrFragment)
        {
            if (string.IsNullOrEmpty(schemaName))
            {
                throw new ArgumentException("Invalid (empty) schema name. Can't validate.", "schemaName");
            }

            if (string.IsNullOrEmpty(xmlFileOrFragment))
            {
                throw new ArgumentException("Empty or null xml. Can't validate.", "xmlFileOrFragment");
            }

            XmlReaderSettings options = GetCachedXmlReaderSettings(schemaName);
            TextReader textReader = null;
            if (File.Exists(xmlFileOrFragment))
            {
                textReader = new StreamReader(xmlFileOrFragment);
            }                
            else
            {
                textReader = new StringReader(xmlFileOrFragment);
            }

            using (XmlReader reader = XmlReader.Create(textReader, options))
            {
                while (reader.Read()) { }
            }
        }

        private static void RaiseValidationError(object sender, ValidationEventArgs args)
        {
            XmlSchemaException ex = args.Exception;
            MTurkLog.Error("XML validation error at line {1} position {2}: '{0}'", ex.Message, ex.LineNumber, ex.LinePosition);

            throw ex;
        }
        #endregion

        #region Serialization
        /// <summary>
        /// Escapes reserved XML chars in a string with the spec'd escape sequence
        /// </summary>
        public static string XmlEncode(string s)
        {
            string ret = s;
            if (!string.IsNullOrEmpty(s))
            {                
                ret = System.Web.HttpUtility.HtmlEncode(s);
            }

            return ret;
        }

        /// <summary>
        /// Unescapes reserved XML chars into a string representation
        /// </summary>
        public static string XmlDecode(string s)
        {
            string ret = s;
            if (!string.IsNullOrEmpty(s))
            {
                ret = System.Web.HttpUtility.HtmlDecode(s);
            }

            return ret;
        }

        /// <summary>
        /// Serializes an object to XML
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>XML string</returns>
        public static string SerializeXML(object obj)
        {
            string ret = null;
            if (obj != null)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    using (XmlTextWriter writer = new XmlTextWriter(stream, encoder))
                    {
                        XmlSerializer serializer = new XmlSerializer(obj.GetType());
                        serializer.Serialize(writer, obj);

                        ret = encoder.GetString(stream.GetBuffer(), 0, (int)stream.Length);
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Deserializes XML to an instance of a given type
        /// </summary>
        /// <param name="obj">The object to serialize</param>
        /// <param name="writer">The writer to write the serialized XML to</param>
        /// <returns>Deserialized object</returns>
        public static void SerializeXML(object obj, TextWriter writer)
        {
            if (obj != null)
            {
                XmlSerializer serializer = new XmlSerializer(obj.GetType());
                serializer.Serialize(writer, obj);
            }
        }

        /// <summary>
        /// Deserializes XML to an instance of a given type
        /// </summary>
        /// <param name="typeOfTargetObject">The type of target object.</param>
        /// <param name="xmlToDeserialize">The XML to deserialize.</param>
        /// <returns>Deserialized object</returns>
        public static object DeserializeXML(Type typeOfTargetObject, string xmlToDeserialize)
        {
            object ret = null;

            using (MemoryStream ms = new MemoryStream(encoder.GetBytes(xmlToDeserialize)))
            {
                XmlSerializer serializer = new XmlSerializer(typeOfTargetObject);

                byte[] rawBytes = ms.ToArray();
                using (StringReader sr = new StringReader(encoder.GetString(rawBytes, 0, rawBytes.Length)))
                {
                    ret = serializer.Deserialize(sr);
                }
            }

            return ret;
        }

        /// <summary>
        /// Deserializes XML to an instance of a given type
        /// </summary>
        /// <param name="typeOfTargetObject">The type of target object.</param>
        /// <param name="reader">Reader containing the XML to deserialize.</param>
        /// <returns>Deserialized object</returns>
        public static object DeserializeXML(Type typeOfTargetObject, TextReader reader)
        {
            XmlSerializer serializer = new XmlSerializer(typeOfTargetObject);

            return serializer.Deserialize(reader);
        }
        #endregion

        #region Transformation
        private static XslCompiledTransform GetCachedXmlTransformer(string xslFile)
        {
            XslCompiledTransform transformer = null;
            if (xslCache.ContainsKey(xslFile))
            {
                transformer = xslCache[xslFile];
            }
            else
            {
                lock (xslCache)
                {
                    if (xslCache.ContainsKey(xslFile))
                    {
                        transformer = xslCache[xslFile];
                    }
                    else
                    {
                        // load XSLT from file
                        FileInfo fi = new FileInfo(xslFile);
                        using (Stream s = fi.OpenRead())
                        {
                            transformer = new XslCompiledTransform();
                            transformer.Load(new XmlTextReader(s));

                            xslCache[xslFile] = transformer;
                        }
                    }
                }
            }

            return transformer;
        }

        /// <summary>
        /// Transforms the specified XML fragment or file with a XSL file
        /// </summary>
        /// <param name="xslFile">XSL file used for transformation</param>
        /// <param name="xmlFileOrFragment">The XML file or fragment to transform.</param>
        /// <returns>Result of the transformation</returns>
        public static string Transform(string xslFile, string xmlFileOrFragment)
        {
            if (string.IsNullOrEmpty(xslFile))
            {
                throw new ArgumentException("Invalid (empty) XSL name. Can't transform.", "xslFile");
            }

            if (string.IsNullOrEmpty(xmlFileOrFragment))
            {
                throw new ArgumentException("Empty or null xml. Can't transform.", "xmlFileOrFragment");
            }

            // get the transformer
            XslCompiledTransform transformer = GetCachedXmlTransformer(xslFile);

            // load source XML
            XmlDocument doc = new XmlDocument();
            if (File.Exists(xmlFileOrFragment))
            {
                doc.Load(xmlFileOrFragment);
            }
            else
            {
                doc.LoadXml(xmlFileOrFragment);
            }

            // Create an XPathNavigator to use for the transform.
            XPathNavigator nav = doc.DocumentElement.CreateNavigator();

            // Transform the file into output string            
            StringBuilder sb = new StringBuilder();
            StringWriter sw = new StringWriter(sb);
            XmlTextWriter writer = new XmlTextWriter(sw);
            transformer.Transform(nav, writer);

            return sb.ToString();
        }
        #endregion
    }
}
