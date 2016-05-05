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

namespace Radev.Licensing.Server
{
	/// <summary>
	/// Factory class to create a new <see cref="License"/>.
	/// </summary>
	public static class LicenseFactory
	{
		/// <summary>
		/// Creates a new <see cref="License"/> from an existing <see cref="Contact"/>.
		/// </summary>
		/// <typeparam name="TLicense">Concrete license class.</typeparam>
		/// <param name="contact">Contact from/for which a new license must be created.</param>
		/// <returns>
		/// A new license tied with hardware configuration described in specified contact.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="contact"/> is <see langword="null"/>.
		/// </exception>
		public static TLicense Create<TLicense>(Contact contact)
			where TLicense : License, new()
		{
			if (contact == null)
				throw new ArgumentNullException("contact");

			var license = new TLicense();
			license.Id = Guid.NewGuid().ToString();
			license.CreationTime = DateTime.UtcNow;

			foreach (var entry in contact.RequiredHardwareConfiguration)
				license.RequiredHardwareConfiguration.Add(entry);

			return license;
		}
	}
}
