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
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radev.Licensing.Client;
using Radev.Licensing.Server;

namespace Radev.Licensing.Tests
{
	// Be careful to add more tests here: we're testing a static class,
	// even if license itself is local per thread we rely on a physical file on
	// a known location then tests may randomly fail. These tests should
	// be refactored, see LicenseManagerTests.IsValidIfLocalLicenseDoesNotAlreadyExist().

	[TestClass]
	public class LicenseManagerTests : TestBase
	{
		[TestMethod]
		public void CanReadValidLicense()
		{
			IsValidIfLocalLicenseDoesNotAlreadyExist();

			// Assuming test environment can write in one of folders license file may be.
			using (var path = new TemporaryFile(LicenseManager.GetLicenseFilePath()))
			{
				// In test environment there isn't license file/storage then this test must fail.
				// After this we must discard cached (and empty) license to load the newly created one.
				Assert.IsNull(LicenseManager.License);
				Assert.IsFalse(LicenseManager.IsLicenseValid);

				LicenseManager.Renew();

				// Now we prepare a fake license file to check few basics in
				// LicenseManager class.
				var contact = ContactFactory.Create<Contact>();
				var license = LicenseFactory.Create<License>(contact);
				license.Features.Add((int)Feature.Example1, 1);

				LicenseWriter.ToFile(path, license);

				Assert.IsTrue(LicenseManager.IsLicenseValid);
				Assert.IsNotNull(LicenseManager.License);
				Assert.AreEqual(LicenseManager.License.Id, license.Id);
				Assert.IsTrue(LicenseManager.IsFeatureAvailable(Feature.Example1));
			}
		}

		private static void IsValidIfLocalLicenseDoesNotAlreadyExist()
		{
			// On development machines we probably have our "local" license file, I don't
			// want to overwrite it (and then delete it) with license generated for this test.
			// It would be better to write a proper mock (static class is bad to test)
			// but it's a compromise to keep "surface attack" as low as possible.
			// Every single time you change LicenseManager class you should (manually...) delete
			// your old license file and run all these tests again.
			var licenseFilePath = LicenseManager.GetLicenseFilePath();
			if (File.Exists(licenseFilePath))
				Assert.Inconclusive("Test skipped because '{0}' already exists. This test may not run on development machines.", licenseFilePath);
		}
	}
}
