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

namespace Amazon.WebServices.MechanicalTurk.Advanced
{
    /// <summary>
    /// Interface to format and parse model objects
    /// </summary>
    internal interface IModelObjectFormatter
    {
        /// <summary>
        /// Formats the specified object.
        /// </summary>
        /// <param name="writer">The output writer.</param>
        /// <param name="obj">The object to format</param>
        void Format(TextWriter writer, object obj);

        /// <summary>
        /// Parses the specified string in an object of the specified type
        /// </summary>
        /// <param name="reader">The input reader</param>
        /// <param name="t">The type of the object that needs to be parsed</param>
        /// <returns>The parsed object</returns>
        object Parse(TextReader reader, Type t);
    }
}
