// <copyright file="IExecutor.cs" company="13th Parish">
// Copyright (c) 13th Parish 2013 All Rights Reserved
// </copyright>
// <author>George Kozlov (george.kozlov@outlook.com)</author>
// <date>03/30/2013</date>
// <summary>IExecutor interface</summary>

using System.Collections.Generic;

namespace SemantAPI.Common
{
	public interface IExecutor
	{
		void Execute(AnalysisExecutionContext context);
		bool IsLanguageSupported(string language);

		AnalysisExecutionContext Context { get; }
	}
}
