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
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Radev.Licensing.Client;

namespace Radev.Licensing
{
	/// <summary>
	/// Factory class to create a new <see cref="License"/> from a secure string representation.
	/// </summary>
	public static class LicenseReader
	{
		public static TLicense FromString<TLicense, TConverter>(string content)
			where TLicense : License, new()
			where TConverter : ILicenseTextConverter<TLicense>, new()
		{
			var licenseContent = ClientSecureCommunication.DecodeMessageFromServer(content);
			var parser = new TConverter();
			var license = parser.Parse(licenseContent);
			license.Freeze();

			return license;
		}

		public static License FromString(string content)
		{
			return FromString<License, IniLicenseTextConverter<License>>(content);
		}

		public static TLicense FromFile<TLicense, TConverter>(string path)
			where TLicense : License, new()
			where TConverter : ILicenseTextConverter<TLicense>, new()
		{
			return FromString<TLicense, TConverter>(File.ReadAllText(path, Encoding.UTF8));
		}

		public static License FromFile(string path)
		{
			return FromString(File.ReadAllText(path, Encoding.UTF8));
		}
	}
}
