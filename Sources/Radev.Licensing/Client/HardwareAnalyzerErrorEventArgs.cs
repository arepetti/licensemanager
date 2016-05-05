//
// Copyright (c) 2015 Repetti Adriano.
//
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.  IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using System.Text;

namespace Radev.Licensing.Client
{
	/// <summary>
	/// Additional arguments for <see cref="HardwareAnalyzer.Error"/> event.
	/// </summary>
	public sealed class HardwareAnalyzerErrorEventArgs : EventArgs
	{
		/// <summary>
		/// Constructs a new <see cref="HardwareAnalyzerErrorEventArgs"/> object.
		/// </summary>
		/// <param name="exception">Exception that stopped hardware scanning.</param>
		/// <param name="partialResult">
		/// Partial results, it may contain information collected (by a single query)
		/// until exception occured.
		/// </param>
		public HardwareAnalyzerErrorEventArgs(Exception exception, StringBuilder partialResult)
		{
			Exception = exception;
			PartialResult = partialResult;
		}

		/// <summary>
		/// Gets exception that stopped hardware scanning.
		/// </summary>
		/// <value>
		/// Exception that stopped hardware scanning.
		/// </value>
		public Exception Exception
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets partial results collected until exception occured.
		/// </summary>
		/// <value>
		/// Partial results collected by a single query until exception
		/// specified in <see cref="HardwareAnalyzerErrorEventArgs.Exception"/> occured.
		/// You can modify this result and it will be return value for that query.
		/// </value>
		public StringBuilder PartialResult
		{
			get;
			private set;
		}
	}
}
