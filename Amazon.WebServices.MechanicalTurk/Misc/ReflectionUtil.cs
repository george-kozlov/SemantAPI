#region Copyright & license notice
/*
 * Copyright: Copyright (c) 2007 Amazon Technologies, Inc.
 * License:   Apache License, Version 2.0
 */
#endregion

using System;
using System.Reflection;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using Amazon.WebServices.MechanicalTurk.Advanced;

namespace Amazon.WebServices.MechanicalTurk
{


    /// <summary>
    /// Various reflection utility functions used in the SDK
    /// </summary>
    internal class ReflectionUtil
    {
        private static int MAX_PAGE_SIZE = 100;
        private static string token = "lock";
        private static string REQUEST_POSTFIX="Request";
        private static Type[] EMPTY_PARAMS = new Type[] { };        
        private static Type[] SINGLE_INT_PARAM = new Type[] { typeof(System.Int32) };
        private static object[] SINGLE_INT_ARGUMENT = new object[] { 1 };

        private static Dictionary<Type, ReflectionCache> htTypeCache 
            = new Dictionary<Type, ReflectionCache>();
        private static Dictionary<Type, Dictionary<PropertyInfo, PropertyInfo>> htTypeCacheForOptionalProperties 
            = new Dictionary<Type, Dictionary<PropertyInfo, PropertyInfo>>();
        private static Dictionary<Type, ConstructorInfo> htCtorCache 
            = new Dictionary<Type, ConstructorInfo>();  // cache for default constructors
        private static Dictionary<Type, ConstructorInfo> htCtorArrayCache 
            = new Dictionary<Type, ConstructorInfo>();  // cache for array constructors
        private static Dictionary<Type, int> htPageDefaultsCache 
            = new Dictionary<Type, int>();              // cache to store page size defaults

        private ReflectionUtil()
        {
        }

        #region Cache accessors
        public static ReflectionCache GetCache(Type t)
        {
            ReflectionCache ret = null;
            if (htTypeCache.ContainsKey(t))
            {
                ret = htTypeCache[t];
            }
            else
            {
                lock (token)
                {
                    if (htTypeCache.ContainsKey(t))
                    {
                        ret = htTypeCache[t];
                    }
                    else
                    {
                        ret = new ReflectionCache(t);
                        htTypeCache[t] = ret;
                    }
                }
            }

            return ret;
        }

        private static ConstructorInfo GetCachedCtor(Type itemType)
        {
            ConstructorInfo ci = null;

            if (htCtorCache.ContainsKey(itemType))
            {
                ci = htCtorCache[itemType];
            }
            else
            {
                lock (token)
                {
                    if (htCtorCache.ContainsKey(itemType))
                    {
                        ci = htCtorCache[itemType];
                    }
                    else
                    {
                        Type envType = itemType.Assembly.GetType(itemType.FullName.Substring(0,
                            itemType.FullName.Length - REQUEST_POSTFIX.Length), true);

                        // get default constructor
                        ci = envType.GetConstructor(EMPTY_PARAMS);
                        htCtorCache[itemType] = ci;
                    }
                }
            }

            return ci;
        }

        private static ConstructorInfo GetCachedArrayCtor(Type itemType)
        {
            ConstructorInfo ciArr = null;
            if (htCtorArrayCache.ContainsKey(itemType))
            {
                ciArr = htCtorArrayCache[itemType];
            }
            else
            {
                lock (token)
                {
                    if (htCtorArrayCache.ContainsKey(itemType))
                    {
                        ciArr = htCtorArrayCache[itemType];
                    }
                    else
                    {
                        Type arrType = itemType.Assembly.GetType(itemType.FullName + "[]", true);
                        ciArr = arrType.GetConstructor(SINGLE_INT_PARAM);
                    }
                }
            }

            return ciArr;
        }

        private static Dictionary<PropertyInfo, PropertyInfo> GetCachedOptionalProperties(Type itemType)
        {
            Dictionary<PropertyInfo, PropertyInfo> htOptionalProps = null;

            // get the optional properties for the type (If not in cache, use reflection)
            if (htTypeCacheForOptionalProperties.ContainsKey(itemType))
            {
                htOptionalProps = htTypeCacheForOptionalProperties[itemType];
            }
            else
            {
                lock (token)
                {
                    if (htTypeCacheForOptionalProperties.ContainsKey(itemType))
                    {
                        htOptionalProps = htTypeCacheForOptionalProperties[itemType];
                    }
                    else
                    {
                        htOptionalProps = new Dictionary<PropertyInfo, PropertyInfo>();
                        // find all public properties that have the XmlIgnore attribute 
                        // set and end with "Specified" and build a map<PropertyInfo, PropertyInfo> 
                        // for it. This map will be evaluated prior to sending the request
                        PropertyInfo[] props = itemType.GetProperties();
                        foreach (PropertyInfo pi in props)
                        {
                            if (pi.Name.EndsWith("Specified") && pi.GetCustomAttributes(typeof(System.Xml.Serialization.XmlIgnoreAttribute), false) != null)
                            {
                                PropertyInfo piRef = itemType.GetProperty(pi.Name.Replace("Specified", string.Empty));
                                if (piRef != null)
                                {
                                    htOptionalProps[piRef] = pi;
                                }
                            }
                        }

                        // cache result for subsequent calls
                        htTypeCacheForOptionalProperties[itemType] = htOptionalProps;
                    }
                }
            }

            return htOptionalProps;
        }

        private static int GetCachedPageSize(Type itemType, object curRequestItem)
        {
            int pageSizeDefault = -1;
            if (htPageDefaultsCache.ContainsKey(itemType))
            {
                pageSizeDefault = htPageDefaultsCache[itemType];
            }
            else
            {
                lock (token)
                {
                    if (htPageDefaultsCache.ContainsKey(itemType))
                    {
                        pageSizeDefault = htPageDefaultsCache[itemType];
                    }
                    else
                    {
                        if (GetCache(curRequestItem.GetType()).GetProperty("PageSize", false) == null)
                        {
                            // not a paged request
                            pageSizeDefault = -1;
                        }
                        else
                        {
                            // find it scoped to the request first
                            string defaultValue = MTurkConfig.CurrentInstance.GetConfig("MechanicalTurk.Defaults.PageSize." + itemType.Name,
                                                  MTurkConfig.CurrentInstance.GetConfig("MechanicalTurk.Defaults.PageSize", null));

                            if (defaultValue == null)
                            {
                                pageSizeDefault = -1; // no client defaults configured, use service defaults
                            }
                            else
                            {
                                pageSizeDefault = int.Parse(defaultValue,
                                    System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
                            }
                        }

                        if (pageSizeDefault > MAX_PAGE_SIZE)
                        {
                            // cap to max
                            pageSizeDefault = MAX_PAGE_SIZE;
                        }

                        htPageDefaultsCache[itemType] = pageSizeDefault;
                    }
                }
            }

            return pageSizeDefault;
        }
        #endregion



        public static object CreateInstance(Type t)
        {
            return GetCache(t).DefaultConstructor.Invoke(EMPTY_PARAMS);
        }

        public static PropertyInfo GetProperty(Type t, string propName)
        {
            return GetCache(t).GetProperty(propName);
        }

        public static object GetPropertyValue(string propName, object target)
        {
            return GetProperty(target.GetType(), propName).GetValue(target, null);
        }

        public static void SetPropertyValue(string propName, object target, object newValue)
        {
            GetProperty(target.GetType(), propName).SetValue(target, newValue, null);
        }

        public static MethodInfo GetMethod(Type t, string methodName)
        {
            return GetCache(t).GetMethod(methodName);
        }

        public static object InvokeMethod(object target, string methodName, object[] arguments)
        {
            return GetMethod(target.GetType(), methodName).Invoke(target, arguments);
        }



        /// <summary>
        /// Creates the MTurk request envelope for a specific itemType 
        /// (e.g. "CreateHIT" envelope for the "CreateHITRequest" item).
        /// The envelope is an object that contains the common AWS properties, 
        /// such as access key ID etc.
        /// </summary>
        public static object CreateRequestEnvelope(object requestItem)
        {
            object ret = null;
            ConstructorInfo ci = null;
            Type itemType = null;
            object[] requestItems = null;

            if (requestItem.GetType().IsArray)
            {
                requestItems = (object[])requestItem;
                itemType = requestItems.GetValue(0).GetType();
            }
            else
            {
                itemType = requestItem.GetType();
            }

            ci = GetCachedCtor(itemType);

            // create a new envelope instance
            ret = ci.Invoke(null);

            if (requestItems == null)
            {
                // wrap the single item in an array
                ConstructorInfo ciArr = GetCachedArrayCtor(itemType);                      

                // create an array for this type (length 1)
                requestItems = (object[])ciArr.Invoke(SINGLE_INT_ARGUMENT);

                // wrap item
                requestItems[0] = requestItem;
            }                                         

            // set the request property for the newly created envelope
            ReflectionUtil.SetPropertyValue("Request", ret, requestItems);                 

            return ret;
        }

        /// <summary>
        /// Evaluates whether any optional properties in a request item have been set. If this
        /// is the case, then the correlating "Specified" property will be set to, otherwise
        /// the property does not get serialized in the soap request.
        /// 
        /// See e.g. http://blogs.msdn.com/eugeneos/archive/2007/02/05/solving-the-disappearing-data-issue-when-using-add-web-reference-or-wsdl-exe-with-wcf-services.aspx
        /// for more details (using WCF is out of question for the API)
        /// </summary>
        /// <param name="request">Array of request items passed in an MTurk request 
        /// (such as CreateItemRequest[])</param>
        public static void PreProcessOptionalRequestProperties(object request)
        {
            if (request != null)
            {
                Type requestType = request.GetType();

                if (requestType.IsArray)
                {
                    // caches the optional property to its "Specified" member (e.g. "PageSize->PageSizeSpecified")
                    Dictionary<PropertyInfo, PropertyInfo> htOptionalProps = null;                    

                    object[] requestItems = request as object[];
                    Type itemType = null;
                    object curRequestItem = null;

                    for (int i = 0; i < requestItems.Length; i++)
                    {
                        curRequestItem = requestItems[i];
                        itemType = curRequestItem.GetType();
                        htOptionalProps = GetCachedOptionalProperties(itemType);

                        // evaluate optional properties and set the "Specified" property to true, 
                        // if the reference property was modified. From a client perspective, this
                        // alleviates the pain of setting the "Specified" properties to true explicitly
                        if (htOptionalProps.Count > 0)
                        {
                            PropertyInfo piSpecified = null;
                            object valRef = null;
                            bool valSpecified = false;
                            Type propTypeRef = null;
                            // flag indicating, whether the "xxxSpecified" property needs to be set
                            bool requiresSpecSet = false;   
                            foreach (PropertyInfo piRef in htOptionalProps.Keys)
                            {
                                // check first, if it was explicitly set to true
                                valSpecified = false;
                                piSpecified = htOptionalProps[piRef];
                                valSpecified = (bool)piSpecified.GetValue(curRequestItem, null);

                                if (!valSpecified)
                                {
                                    valRef = piRef.GetValue(curRequestItem, null);
                                    if (valRef != null)
                                    {
                                        // Generally, value types require to be explicitly set,
                                        // otherwise they can'typeOfTargetObject be set back to their default
                                        // values easily (e.g. setting a value back from "true" to "false")
                                        // The following code will set the "Specified" property to true
                                        // if the reference value is different from the default value
                                        // for the reference value type
                                        requiresSpecSet = false;

                                        propTypeRef = valRef.GetType();
                                        if (propTypeRef.IsValueType)
                                        {
                                            if (valRef is bool)
                                            {
                                                requiresSpecSet = (bool)valRef;     
                                            }
                                            else if (valRef is DateTime)
                                            {
                                                requiresSpecSet = !DateTime.MinValue.Equals(valRef);
                                            }
                                            else if (propTypeRef.IsEnum)
                                            {
                                                requiresSpecSet = 
                                                    !(valRef.Equals(Enum.GetValues(propTypeRef).GetValue(0)));
                                            }
                                            else
                                            {
                                                requiresSpecSet = !(Decimal.Parse(valRef.ToString(), System.Globalization.CultureInfo.InvariantCulture.NumberFormat).Equals(Decimal.Zero));
                                            }
                                        }
                                        else if (propTypeRef.IsArray)
                                        {
                                            requiresSpecSet = ((object[])valRef).Length > 0;
                                        }                                        
                                        else if (propTypeRef.IsClass)
                                        {
                                            requiresSpecSet = true;
                                        }
                                        else
                                        {
                                            MTurkLog.Warn("Cannot preprocess optional property {0} for type {1} (No handler for property type {2}).", 
                                                piRef.Name, 
                                                itemType.FullName, 
                                                propTypeRef.FullName);
                                        }

                                        // set the "xxSpecified" property, so the referenced member gets soap serialized
                                        if (requiresSpecSet)
                                        {
                                            piSpecified.SetValue(curRequestItem, true, null);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }                
            }
        }

        /// <summary>
        /// Sets the common list properties (pagenumber/size) to defaults if they
        /// were not explicitly set. Applies only to list requests
        /// </summary>
        public static void SetPagingProperties(object request)
        {
            if (request.GetType().IsArray)
            {
                object[] requestItems = (object[])request;
                object curRequestItem = null;
                Type itemType = null;

                for (int i = 0; i < requestItems.Length; i++)
                {
                    curRequestItem = requestItems[i];
                    itemType = curRequestItem.GetType();
                    int pageSizeDefault = GetCachedPageSize(itemType, curRequestItem);

                    if (pageSizeDefault == -1)
                    {
                        break;  // not a paged request
                    }
                    else
                    {
                        // if it was not explicitly set, then use default size 
                        int pageSize = (int)GetPropertyValue("PageSize", curRequestItem);
                        if (pageSize == 0)
                        {
                            SetPropertyValue("PageSize", curRequestItem, pageSizeDefault);

                            int pageNum = (int)GetPropertyValue("PageNumber", curRequestItem);
                            if (pageNum == 0)
                            {
                                SetPropertyValue("PageNumber", curRequestItem, 1);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Shallow "clone": Clones the public properties from a source to a target object (of the same type)
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="target">The target.</param>
        public static void Clone(object source, object target)
        {
            if (source == null)
            {
                throw new ArgumentException("Can't clone from null source", "source");
            }

            if (target == null)
            {
                throw new ArgumentException("Can't clone to null target", "target");
            }

            if (!source.GetType().Equals(target.GetType()))
            {
                throw new ArgumentException("Can't clone. Type mismatch between source and target object", "source");
            }

            ReflectionCache cacheSrc = GetCache(source.GetType());
            ReflectionCache cacheTarget = GetCache(target.GetType());
           
            PropertyInfo[] propsSrc = cacheSrc.PublicProperties;
            PropertyInfo propTarget = null;
            foreach (PropertyInfo propSrc in propsSrc)
            {
                propTarget = cacheTarget.GetProperty(propSrc.Name);
                

                if (propTarget != null && propSrc.CanRead && propTarget.CanWrite)
                {
                    propTarget.SetValue(target, propSrc.GetValue(source, null), null);
                }
            }
        }

        /// <summary>
        /// Returns the value for a property path.
        /// </summary>
        /// <param name="objRoot">The root object for the path. Needs to be of the cached type</param>
        /// <param name="propertyPath">The property path (e.g. "LocaleValue.Country")</param>
        /// <returns>The result for the property path or null, if any of the values in the 
        /// path returned null</returns>
        public static object GetPropertyPathValue(object objRoot, string propertyPath)
        {
            if (objRoot == null)
            {
                throw new ArgumentException("Cannot resolve value for path. Root object is null.", "objRoot");
            }

            return GetCache(objRoot.GetType()).GetPropertyPathValue(objRoot, propertyPath);
        }

        /// <summary>
        /// Sets the property path value. If one of the properties in the path is null, a new instance 
        /// for its type is created.
        /// </summary>
        /// <param name="objRoot">The root object for the path. Needs to be of the cached type</param>
        /// <param name="propertyPath">The property path (e.g. "LocaleValue.Country")</param>
        /// <param name="newValueForProperty">New value for the last property in the path</param>
        public static void SetPropertyPathValue(object objRoot, string propertyPath, object newValueForProperty)
        {
            if (objRoot == null)
            {
                throw new ArgumentException("Cannot resolve value for path. Root object is null.", "objRoot");
            }

            GetCache(objRoot.GetType()).SetPropertyPathValue(objRoot, propertyPath, newValueForProperty);
        }
    }
}
