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
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Radev.Licensing.Tests
{
	public abstract class TestBase
	{
		protected static void CompareDictionaries(IDictionary<string, string> dict1, IDictionary<string, string> dict2, string message)
		{
			// I may use CollectionAssert but I'd like to provide a more detailed description of unmatched element
			Assert.IsTrue(dict1.Count == dict2.Count, message);
			foreach (var entry in dict1)
			{
				string value1 = entry.Value;
				string value2 = dict2[entry.Key];

				Assert.IsTrue(value1 == value2, String.Format("{0}. '{1}' is not the same as '{2}'.", message.TrimEnd('.'), value1, value2));
			}
		}
	}
}
