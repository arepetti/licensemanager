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
using Radev.Licensing.Client;

namespace Radev.Licensing.Tests
{
	sealed class LicenseWithChangedHardware : License
	{
		protected override HardwareAnalyzer CreateHardwareAnalyzer()
		{
			return new FakeHardwareAnalyzer();
		}

		private sealed class FakeHardwareAnalyzer : HardwareAnalyzer
		{
			public FakeHardwareAnalyzer()
			{
				Queries = new string[] { "new hardware 1", "new hardware 2", "new hardware 3" }; // Just fake values
			}

			protected override string Query(string query)
			{
				// Result of each query is query itself, it will
				// make tests fail when license is generated with default analyzer.
				return query;
			}
		}
	}

	sealed class ContactForSlightlyChangedHardware : Contact
	{
		protected override HardwareAnalyzer CreateHardwareAnalyzer()
		{
			return new FakeHardwareAnalyzer();
		}

		private sealed class FakeHardwareAnalyzer : HardwareAnalyzer
		{
			public FakeHardwareAnalyzer()
			{
				Queries = new string[] { "1", "2", "to change" }; // Just fake values
			}

			protected override string Query(string query)
			{
				// Result of each query is query itself, it will
				// make tests fail when license is generated with default analyzer.
				return query;
			}
		}
	}

	sealed class LicenseWithSlightlyChangedHardware : License
	{
		protected override HardwareAnalyzer CreateHardwareAnalyzer()
		{
			return new FakeHardwareAnalyzer();
		}

		private sealed class FakeHardwareAnalyzer : HardwareAnalyzer
		{
			public FakeHardwareAnalyzer()
			{
				Queries = new string[] { "1", "2", "changed" }; // Just fake values
			}

			protected override string Query(string query)
			{
				// Result of each query is query itself, it will
				// make tests fail when license is generated with default analyzer.
				return query;
			}
		}
	}
}
