// <copyright file="Form.cs" company="13th Parish">
// Copyright (c) 13th Parish 2013 All Rights Reserved
// </copyright>
// <author>George Kozlov (george.kozlov@outlook.com)</author>
// <date>03/30/2013</date>
// <summary>BenchmarkHelper class</summary>

using System;
using System.Diagnostics;

namespace SemantAPI.Common
{
	public delegate object InvokeBenchmarkHandler(object state);

	public sealed class BenchmarkHelper
	{
		public static object Invoke(InvokeBenchmarkHandler method, object state, out TimeSpan execTime)
		{
			Stopwatch watcher = new Stopwatch();
			watcher.Start();

			object result = method.Invoke(state);

			watcher.Stop();
			TimeSpan time = watcher.Elapsed;
			execTime = time;

			return result;
		}
	}
}
