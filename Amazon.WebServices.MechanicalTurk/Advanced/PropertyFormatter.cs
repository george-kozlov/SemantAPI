#region Copyright & license notice
/*
 * Copyright: Copyright (c) 2007 Amazon Technologies, Inc.
 * License:   Apache License, Version 2.0
 */
#endregion

using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using Amazon.WebServices.MechanicalTurk;

namespace Amazon.WebServices.MechanicalTurk.Advanced
{
    /// <summary>
    /// Basic formatter and parser to serialize/deserialize objects to and from Mechanical Turk property format
    /// </summary>
    internal class PropertyFormatter : IModelObjectFormatter
    {
        private static char SEPARATOR_CHAR = ':';
        private static char COMMENT_CHAR = '#';
        private static char INDEX_START_CHAR = '[';
        private static char INDEX_END_CHAR = ']';

        #region Singleton
        private static IModelObjectFormatter _instance = new PropertyFormatter();
        internal static IModelObjectFormatter Instance
        {
            get { return _instance; }
        }

        private PropertyFormatter()
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
            StringBuilder sb = new StringBuilder();

            Format(obj, sb, string.Empty);

            writer.Write(sb.ToString().Replace(".{0}", string.Empty));
        }

        /// <summary>
        /// Parses the specified string in an object of the specified type
        /// </summary>
        /// <param name="reader">The input reader</param>
        /// <param name="t">The type of the object that needs to be parsed</param>
        /// <returns>The parsed object</returns>
        public object Parse(TextReader reader, Type t)
        {
            object ret = null;

            List<KeyValuePair<string, string>> singleValues = new List<KeyValuePair<string, string>>();
            Dictionary<string, List<List<KeyValuePair<string, string>>>> arrayValues = 
                new Dictionary<string,List<List<KeyValuePair<string,string>>>>();
            List<List<KeyValuePair<string, string>>> curArrayList;

            // parse properties into single keyvalue pairs and the indexed properties into
            // the array Values
            #region Parse properties
            string curLine = reader.ReadLine();
            int idxSeparator, idxIndex1, idxIndex2;
            string key, val, keyIndex, keyIndexBase, keyIndexProp;
            int curIndex;
            while (curLine != null)
            {
                curLine = curLine.Trim();
                if (!string.IsNullOrEmpty(curLine) && curLine[0] != COMMENT_CHAR)
                {
                    idxSeparator = curLine.IndexOf(SEPARATOR_CHAR);
                    if (idxSeparator != -1)
                    {
                        key = curLine.Substring(0, idxSeparator);
                        val = curLine.Substring(idxSeparator + 1);

                        // check if the key is indexed/array
                        idxIndex1 = key.IndexOf(INDEX_START_CHAR);
                        if (idxIndex1 == -1)
                        {
                            singleValues.Add(new KeyValuePair<string, string>(key, val));
                        }
                        else
                        {
                            idxIndex2 = key.IndexOf(INDEX_END_CHAR);
                            if (idxIndex2 < idxIndex1)
                            {
                                throw new InvalidDataException("Cannot read index for property: " + key);
                            }
                            else
                            {
                                keyIndex = key.Substring(idxIndex1 + 1, idxIndex2 - idxIndex1 - 1);
                                if (int.TryParse(keyIndex, out curIndex))
                                {
                                    keyIndexBase = key.Substring(0, idxIndex1);
                                    keyIndexProp = key.Substring(idxIndex2 + 2);

                                    if (!arrayValues.ContainsKey(keyIndexBase))
                                    {
                                        arrayValues[keyIndexBase] = new List<List<KeyValuePair<string, string>>>();
                                    }
                                    curArrayList = arrayValues[keyIndexBase];
                                    if (curArrayList == null || curArrayList.Count <= curIndex)
                                    {
                                        curArrayList.Add(new List<KeyValuePair<string,string>>());
                                    }
                                    curArrayList[curIndex].Add(new KeyValuePair<string, string>(keyIndexProp, val));
                                }
                                else
                                {
                                    throw new InvalidDataException("Not a valid indexed property: " + key);
                                }
                            }
                        }
                    }
                }
                curLine = reader.ReadLine();
            }            
            #endregion

            // create a new object for type
            ret = ReflectionUtil.CreateInstance(t);

            ReflectionCache typeCache = ReflectionUtil.GetCache(t);
            PropertyInfo piSpecified = null;

            // now reflect through the specified type and set parsed values
            foreach (KeyValuePair<string, string> kvp in singleValues)
            {
                ReflectionUtil.SetPropertyPathValue(ret, kvp.Key, kvp.Value);

                // also set the "Specified"-flag, if any
                piSpecified = typeCache.GetProperty(kvp.Key + "Specified", false);
                if (piSpecified != null)
                {
                    piSpecified.SetValue(ret, true, null);
                }

            }

            object curRootObject;
            List<KeyValuePair<string, string>> curRootObjectProperties;            
            PropertyInfo pi;
            ConstructorInfo ci;

            foreach (string arrayPropertyName in arrayValues.Keys)
            {
                curArrayList = arrayValues[arrayPropertyName];

                pi = ReflectionUtil.GetProperty(t, arrayPropertyName);
                ci = pi.PropertyType.GetConstructor(new Type[] { typeof(int) });
                object[] o = (object[])ci.Invoke(new object[] { curArrayList.Count });

                Type elementType = pi.PropertyType.Assembly.GetType(pi.PropertyType.FullName.Replace("[]", string.Empty), false);
                
                for (int i = 0; i < curArrayList.Count; i++ )
                {
                    curRootObject = ReflectionUtil.CreateInstance(elementType);
                    curRootObjectProperties = curArrayList[i];

                    typeCache = ReflectionUtil.GetCache(elementType);

                    foreach (KeyValuePair<string, string> kvp in curRootObjectProperties)
                    {
                        ReflectionUtil.SetPropertyPathValue(curRootObject, kvp.Key, kvp.Value);

                        // also set the "Specified"-flag, if any
                        piSpecified = typeCache.GetProperty(kvp.Key + "Specified", false);
                        if (piSpecified != null)
                        {
                            piSpecified.SetValue(curRootObject, true, null);
                        }
                    }

                    o[i] = curRootObject;
                }

                ReflectionUtil.SetPropertyPathValue(ret, arrayPropertyName, o);
            }

            return ret;
        }

        #endregion

        private void Format(object obj, StringBuilder sb, string keyFormat)
        {
            if (obj == null)
            {
                return;
            }

            if (obj is string)
            {
                sb.Append(keyFormat);
                sb.Append(SEPARATOR_CHAR);
                sb.AppendLine(obj.ToString());
            }
            else
            {
                PropertyInfo[] allProps = ReflectionUtil.GetCache(obj.GetType()).PublicProperties;
                object curValue = null;
                string curKey = null;
                bool isCurPropSpecified = false;
                string refPropName = null;

                // filter out all optional props which have a "xxxSpecified" that evaluates to false
                Dictionary<string, PropertyInfo> htProps = new Dictionary<string, PropertyInfo>();
                foreach (PropertyInfo pi in allProps)
                {
                    htProps.Add(pi.Name, pi);
                }

                foreach (PropertyInfo pi in allProps)
                {
                    if (pi.PropertyType.Equals(typeof(System.Boolean)) && pi.Name.EndsWith("Specified"))
                    {
                        // if value is false, remove both from list, otherwise just the Specified property
                        // we don't want these serialized
                        isCurPropSpecified = (bool)pi.GetValue(obj, null);
                        if (!isCurPropSpecified)
                        {
                            refPropName = pi.Name.Replace("Specified", string.Empty);
                            if (htProps.ContainsKey(refPropName)) 
                            {
                                htProps.Remove(refPropName);
                            }                            
                        }
                        htProps.Remove(pi.Name);
                    }
                }

                foreach (PropertyInfo pi in htProps.Values)
                {
                    if (pi.CanRead && pi.CanWrite)
                    {
                        curKey = (keyFormat.Length == 0) ? pi.Name : string.Format(keyFormat, pi.Name);

                        if (pi.PropertyType.IsArray)
                        {
                            curValue = pi.GetValue(obj, null);
                            if (curValue != null)
                            {
                                Array arr = (Array)curValue;
                                for (int i = 0; i < arr.Length; i++)
                                {
                                    // resolve array
                                    Format(arr.GetValue(i), sb, string.Format("{0}[{1}].{2}", pi.Name, i, "{0}"));
                                }
                            }
                        }
                        else
                        {
                            curValue = pi.GetValue(obj, null);
                            if (curValue != null)
                            {


                                if (pi.PropertyType.Equals(typeof(string)) 
                                    || pi.PropertyType.IsPrimitive 
                                    || pi.PropertyType.IsEnum 
                                    || pi.PropertyType.IsValueType)
                                {
                                    sb.Append(curKey).Append(SEPARATOR_CHAR);
                                    sb.AppendLine(curValue.ToString());
                                }
                                else
                                {
                                    if (pi.GetCustomAttributes(typeof(System.Xml.Serialization.XmlIgnoreAttribute), false).Length==0)
                                    {
                                        // resolve properties of this property
                                        Format(curValue, sb, curKey + ".{0}");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
