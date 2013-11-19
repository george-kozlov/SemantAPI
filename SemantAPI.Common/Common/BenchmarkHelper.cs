//
// SemantAPI.Robot, SemantAPI.Human
// Copyright (C) 2013 George Kozlov
// These programs are free software: you can redistribute them and/or modify them under the terms of the GNU General Public License as published by the Free Software Foundation. either version 3 of the License, or any later version.
// These programs are distributed in the hope that they will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License for more details. You should have received a copy of the GNU General Public License along with this program. If not, see http://www.gnu.org/licenses/.
// For further questions or inquiries, please contact semantapi (at) gmail (dot) com
//

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
