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
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radev.Licensing.Client;
using Radev.Licensing.Server;

namespace Radev.Licensing.Tests
{
	[TestClass]
	public class ContactTests : TestBase
	{
		[TestMethod, TestCategory("Client")]
		public void CreateNewContact()
		{
			Assert.IsNotNull(CreateContact());
		}

		[TestMethod, TestCategory("Client")]
		public void SaveToFile()
		{
			using (var tempFile = new TemporaryFile())
			{
				ContactWriter.ToFile(tempFile, CreateContact());
			}
		}

		[TestMethod, TestCategory("Server")]
		public void LoadFromFile()
		{
			using (var tempFile = new TemporaryFile())
			{
				var contact = CreateContact();
				ContactWriter.ToFile(tempFile, contact);

				CompareContacts(contact, ContactReader.FromFile(tempFile));

			}
		}

		[TestMethod, TestCategory("Client")]
		public void AppendCustomBase64Text()
		{
			var contact = CreateContact();
			var encodedContact = new StringBuilder(ContactWriter.ToString(contact));
			encodedContact.Append('.');
			encodedContact.Append(Convert.ToBase64String(Encoding.UTF8.GetBytes("This is a test string.")));

			CompareContacts(contact, ContactReader.FromString(encodedContact.ToString()));
		}

		[TestMethod, TestCategory("Client")]
		public void IsBase64Encoded()
		{
			var contact = CreateContact();
			var encodedContact = ContactWriter.ToString(contact);

			Assert.IsNotNull(Convert.FromBase64String(encodedContact));
		}

		[TestMethod, TestCategory("Client")]
		public void EachNewContactHasUniqueId()
		{
			// Different IDs (and timestamps) will produce different encoded strings and
			// also you will get different encrypted values.
			Contact contact1 = CreateContact(), contact2 = CreateContact();

			Assert.IsTrue(ContactWriter.ToString(contact1) != ContactWriter.ToString(contact2));
			Assert.IsTrue(ContactWriter.ToString(contact1) != ContactWriter.ToString(contact1));
		}

		[TestMethod, TestCategory("Client")]
		public void HardwareConfigurationIsStableOverTime()
		{
			var contact1 = ContactFactory.Create<Contact>();
			var contact2 = ContactFactory.Create<Contact>();

			CompareDictionaries(contact1.RequiredHardwareConfiguration, contact2.RequiredHardwareConfiguration, "Hardware configuration changed.");
		}

		private static Contact CreateContact()
		{
			return ContactFactory.Create<Contact>();
		}

		private static string CreateEncodedContact()
		{
			return ContactWriter.ToString(CreateContact());
		}

		private static void CompareContacts(Contact contact1, Contact contact2)
		{
			// Check field by field to give better error messages (instead of raw binary comparison).
			Assert.IsTrue(contact1.Id == contact2.Id, "ID changed.");
			Assert.IsTrue(contact1.SoftwareVersion == contact2.SoftwareVersion, "Software version changed.");
			Assert.IsTrue((contact1.CreationTime ?? DateTime.MinValue) == (contact2.CreationTime ?? DateTime.MinValue), "Creation time changed.");
			CompareDictionaries(contact1.RequiredHardwareConfiguration, contact2.RequiredHardwareConfiguration, "Hardware configuration changed.");
		}
	}
}
