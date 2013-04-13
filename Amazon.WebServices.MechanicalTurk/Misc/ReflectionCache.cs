#region Copyright & license notice
/*
 * Copyright: Copyright (c) 2007 Amazon Technologies, Inc.
 * License:   Apache License, Version 2.0
 */
#endregion

using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using Amazon.WebServices.MechanicalTurk.Advanced;

namespace Amazon.WebServices.MechanicalTurk
{
    /// <summary>
    /// A reflection cache for a specific type to avoid cost of repetitive reflection
    /// </summary>
    internal class ReflectionCache
    {
        private static Type[] EMPTY_PARAMS = new Type[] { };
        private static object syncToken = new Object();
        private Type _type;
        private ConstructorInfo _defaultConstructor;
        private Dictionary<string, PropertyInfo> htPropertyCache = new Dictionary<string, PropertyInfo>();
        private Dictionary<string, MethodInfo> htMethodCache = new Dictionary<string, MethodInfo>();
        private Dictionary<string, PropertyInfo[]> htPathCache = new Dictionary<string, PropertyInfo[]>();
        private System.Collections.Specialized.StringCollection excludeList = new System.Collections.Specialized.StringCollection();
        private PropertyInfo[] _publicProperties;

        public ReflectionCache(Type cachedType)
        {
            _type = cachedType;
            _defaultConstructor = _type.GetConstructor(EMPTY_PARAMS);
        }

        public PropertyInfo GetProperty(string propName)
        {
            return GetProperty(propName, true);
        }

        public PropertyInfo GetProperty(string propName, bool throwOnError)
        {
            PropertyInfo ret = null;

            try
            {
                if (htPropertyCache.ContainsKey(propName))
                {
                    ret = htPropertyCache[propName];
                }
                else
                {
                    lock (syncToken)
                    {
                        if (htPropertyCache.ContainsKey(propName))
                        {
                            ret = htPropertyCache[propName];
                        }
                        else
                        {
                            // check only if the property is not yet on the exclude list
                            if (!excludeList.Contains(propName))
                            {
                                // look for public property;
                                ret = _type.GetProperty(propName);
                                if (ret == null)
                                {
                                    ret = _type.GetProperty(propName, BindingFlags.Instance | BindingFlags.NonPublic);
                                    if (ret == null)
                                    {
                                        if (throwOnError)
                                        {
                                            throw new ArgumentException("No such property for type " + _type.FullName, "propName");
                                        }
                                        else
                                        {
                                            // add to exclude list
                                            excludeList.Add(propName);
                                        }
                                    }
                                }
                                else
                                {
                                    htPropertyCache[propName] = ret;
                                }
                            }
                        }
                    }
                }
            }
            catch (ArgumentException ex)
            {
                MTurkLog.Warn("Cannot find property {0} for type {1}: {2}", propName, _type.FullName, ex.Message);
            }

            return ret;
        }

        public MethodInfo GetMethod(string methodName)
        {
            MethodInfo ret = null;
            if (htMethodCache.ContainsKey(methodName))
            {
                ret = htMethodCache[methodName];
            }
            else
            {
                lock (syncToken)
                {
                    if (htMethodCache.ContainsKey(methodName))
                    {
                        ret = htMethodCache[methodName];
                    }
                    else
                    {
                        // look for public method;
                        ret = _type.GetMethod(methodName);
                        if (ret == null)
                        {
                            ret = _type.GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
                            if (ret == null)
                            {
                                throw new ArgumentException("No such method for type " + _type.FullName, "propName");
                            }
                        }
                        else
                        {
                            htMethodCache[methodName] = ret;
                        }
                    }
                }
            }

            return ret;
        }

        public PropertyInfo[] PublicProperties
        {
            get
            {
                if (_publicProperties == null)
                {
                    _publicProperties = _type.GetProperties();
                }

                return _publicProperties;
            }
        }

        public ConstructorInfo DefaultConstructor
        {
            get
            {
                return _defaultConstructor;
            }
        }

        /// <summary>
        /// Returns all properties for a property path starting with the cached type
        /// 
        /// </summary>
        private PropertyInfo[] GetPropertyPath(string propertyPath)
        {
            if (string.IsNullOrEmpty(propertyPath))
            {
                throw new ArgumentException("Cannot resolve properties for empty or null resolvedPath", "propertyPath");
            }

            PropertyInfo[] ret = null;

            if (htPathCache.ContainsKey(propertyPath))
            {
                ret = htPathCache[propertyPath];
            }
            else
            {
                lock (syncToken)
                {
                    if (htPathCache.ContainsKey(propertyPath))
                    {
                        ret = htPathCache[propertyPath];
                    }
                    else
                    {
                        string[] parts = propertyPath.Split('.');
                        ret = new PropertyInfo[parts.Length];
                        PropertyInfo pi;
                        Type curType = _type;

                        for (int i = 0; i < parts.Length; i++)
                        {
                            pi = curType.GetProperty(parts[i]);
                            if (pi == null)
                            {
                                throw new InvalidOperationException(string.Format("Cannot resolve property resolvedPath '{0}'. No such property: {1}", propertyPath, parts[i]));
                            }
                            ret[i] = pi;

                            curType = pi.PropertyType;
                        }
                    }
                }
            }

            return ret;
        }

        /// <summary>
        /// Returns the value for a property path.
        /// </summary>
        /// <param name="objRoot">The root object for the path. Needs to be of the cached type</param>
        /// <param name="propertyPath">The property path (e.g. "LocaleValue.Country")</param>
        /// <returns>The result for the property path or null, if any of the values in the path returned null</returns>
        public object GetPropertyPathValue(object objRoot, string propertyPath)
        {
            object ret = objRoot;
            PropertyInfo[] props = GetPropertyPath(propertyPath);

            foreach (PropertyInfo pi in props)
            {
                ret = pi.GetValue(ret, null);

                if (ret == null)
                {
                    // object not set, so return null
                    break;
                }
            }

            return ret;
        }

        /// <summary>
        /// Sets the property path value. If one of the properties in the path is null, a new instance for its type is created.
        /// </summary>
        /// <param name="objRoot">The root object for the path. Needs to be of the cached type</param>
        /// <param name="propertyPath">The property path (e.g. "LocaleValue.Country")</param>
        /// <param name="newValueForProperty">New value for the last property in the path</param>
        public void SetPropertyPathValue(object objRoot, string propertyPath, object newValueForProperty)
        {
            Type curType = null;
            PropertyInfo pi = null;
            PropertyInfo[] resolvedPath = GetPropertyPath(propertyPath);
            object curParent = objRoot;
            object cur = objRoot;

            for (int i = 0; i < resolvedPath.Length - 1; i++)
            {
                curParent = cur;
                pi = resolvedPath[i];
                curType = pi.PropertyType;
                cur = pi.GetValue(cur, null);

                if (cur == null)
                {
                    if (curType.IsClass || curType.IsValueType)
                    {
                        // Create a new instance (requires default constructor)
                        ConstructorInfo ctor = curType.GetConstructor(EMPTY_PARAMS);
                        if (ctor == null)
                        {
                            throw new InvalidOperationException(string.Format("Cannot execute property resolvedPath '{0}'. No default constructor found for type '{1}'", propertyPath, curType.FullName));
                        }
                        else
                        {
                            // create a new instance and set it on the curParent
                            cur = ctor.Invoke(null);

                            ReflectionUtil.SetPropertyValue(pi.Name, curParent, cur);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException(string.Format("Cannot execute property resolvedPath '{0}'. Property '{1}' is neither class nor value type", propertyPath, pi.Name));
                    }
                }
            }

            // now set the value for the last element of the resolvedPath
            pi = resolvedPath[resolvedPath.Length - 1];
            curType = pi.PropertyType;
            if (newValueForProperty != null)
            {
                if (!curType.Equals(newValueForProperty.GetType()))
                {
                    // check if it is nullable and use underlying typ
                    if (curType.IsGenericType && curType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        curType = Nullable.GetUnderlyingType(curType);
                    }

                    if (curType.IsEnum)
                    {
                        newValueForProperty = Enum.Parse(curType, newValueForProperty.ToString(), true);
                    }
                    else
                    {
                        TypeConverter tc = TypeDescriptor.GetConverter(newValueForProperty.GetType());
                        if (tc.CanConvertTo(curType))
                        {
                            newValueForProperty = tc.ConvertTo(newValueForProperty, curType);
                        }
                        else
                        {
                            // no builtin converter found: let's look if we have a static Parse method for this type)
                            MethodInfo mi = curType.GetMethod("Parse", new Type[] { newValueForProperty.GetType() });
                            if (mi != null)
                            {
                                object[] param = { newValueForProperty };
                                newValueForProperty = mi.Invoke(null, param);
                            }
                            else
                            {
                                MTurkLog.Warn("No type conversion/parser found for {0}. Defaulting to {1} ({2})", pi.Name, newValueForProperty, newValueForProperty.GetType().Name);
                            }
                        }
                    }
                }
            }

            pi.SetValue(cur, newValueForProperty, null);
        }
    }
}
