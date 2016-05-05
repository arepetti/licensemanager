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
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radev.Licensing.Client;
using Radev.Licensing.Server;

namespace Radev.Licensing.Tests
{
	// Most of these test applies also to "public" builds but we
	// need private key to generate a license then we perform
	// these test on private builds and they apply unchanged to
	// public builds too.

	[TestClass]
	public class LicenseTests : TestBase
	{
		[TestMethod, TestCategory("Server")]
		public void CreateFromContact()
		{
			Assert.IsNotNull(CreateLicenseFromNewContact());
		}

		[TestMethod, TestCategory("Client")]
		public void CanSetSoftwareVersion()
		{
			var softwareVersion = Assemblies.GetProductVersion();

			var license = CreateLicenseFromNewContact();
			license.MinimumVersion = new Version(softwareVersion.Major - 1, 0);
			license.MaximumVersion = new Version(softwareVersion.Major + 1, 0);

			license = SaveAndReloadFromDisk(license);
			Assert.IsTrue(license.IsValid());
		}

		[TestMethod, TestCategory("Client")]
		public void CanSetMinimumSoftwareVersion()
		{
			var softwareVersion = Assemblies.GetProductVersion();

			var license = CreateLicenseFromNewContact();
			license.MinimumVersion = new Version(softwareVersion.Major + 1, 0);
			
			license = SaveAndReloadFromDisk(license);
			Assert.IsFalse(license.IsValid());
		}

		[TestMethod, TestCategory("Client")]
		public void CanSetMaximumSoftwareVersion()
		{
			var softwareVersion = Assemblies.GetProductVersion();

			var license = CreateLicenseFromNewContact();
			license.MaximumVersion = new Version(softwareVersion.Major - 1, 0);

			license = SaveAndReloadFromDisk(license);
			Assert.IsFalse(license.IsValid());
		}

		[TestMethod, TestCategory("Client")]
		public void CanSetValidity()
		{
			var license = CreateLicenseFromNewContact();
			license.Validity = new Interval(DateTime.Now.AddDays(-2), DateTime.Now.AddDays(2));

			license = SaveAndReloadFromDisk(license);
			Assert.IsTrue(license.IsValid());
		}

		[TestMethod, TestCategory("Client")]
		public void CanSetValidityStart()
		{
			var license = CreateLicenseFromNewContact();
			license.Validity = new Interval(DateTime.Now.AddDays(2), null);

			license = SaveAndReloadFromDisk(license);
			Assert.IsFalse(license.IsValid());
		}

		[TestMethod, TestCategory("Client")]
		public void CanSetValidityEnd()
		{
			var license = CreateLicenseFromNewContact();
			license.Validity = new Interval(null, DateTime.Now.AddDays(-2));

			license = SaveAndReloadFromDisk(license);
			Assert.IsFalse(license.IsValid());
		}

		[TestMethod, TestCategory("Client")]
		public void ReadAndWriteFeatures()
		{
			var license = CreateLicenseFromNewContact();
			license.Features.Add(1977, 1977);

			license = SaveAndReloadFromDisk(license);
			Assert.AreEqual(1977, license.ReadFeature(1977));
			Assert.AreEqual((int?)null, license.ReadFeature(1978));
		}

		[TestMethod, TestCategory("Client")]
		public void ReadAndWriteEndUserData()
		{
			const string endUserFullName = "Chuck Norris";

			var license = CreateLicenseFromNewContact();
			license.EndUser = new EndUser { FullName = endUserFullName };

			license = SaveAndReloadFromDisk(license);
			Assert.AreEqual(endUserFullName, license.EndUser.FullName);
		}

		[TestMethod, TestCategory("Client")]
		[ExpectedException(typeof(LicenseException))]
		public void TamperedLicenseIsRejected()
		{
			var license = CreateLicenseFromNewContact();
			var encodedLicense = new StringBuilder(LicenseWriter.ToString(license));
			encodedLicense[0] = 'A'; // I'd assume we dont already have this sequence...
			encodedLicense[1] = 'B';
			encodedLicense[2] = 'C';
			encodedLicense[3] = 'D';

			LicenseReader.FromString(encodedLicense.ToString());
		}

		[TestMethod, TestCategory("Client")]
		[ExpectedException(typeof(LicenseException))]
		public void NotSignedLicenseIsRejected()
		{
			var license = CreateLicenseFromNewContact();
			var encodedLicense = LicenseWriter.ToString(license);

			LicenseReader.FromString(encodedLicense.Split('.').First());
		}

		[TestMethod, TestCategory("Client")]
		[ExpectedException(typeof(LicenseException))]
		public void InvalidLicenseIsRejected1()
		{
			LicenseReader.FromString("This is an invalid license text");
		}

		[TestMethod, TestCategory("Client")]
		[ExpectedException(typeof(LicenseException))]
		public void InvalidLicenseIsRejected2()
		{
			LicenseReader.FromString(
				Convert.ToBase64String(Encoding.UTF8.GetBytes("This is an invalid license text")));
		}

		private static License CreateLicenseFromNewContact(Action<Contact> customizeContact = null)
		{
			var contact = ContactFactory.Create<Contact>();

			if (customizeContact != null)
				customizeContact(contact);

			return LicenseFactory.Create<License>(contact);
		}

		private static License SaveAndReloadFromDisk(License license)
		{
			using (var path = new TemporaryFile())
			{
				LicenseWriter.ToFile(path, license);
				return LicenseReader.FromFile(path);
			}
		}
	}
}
