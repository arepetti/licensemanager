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
using System.Text;

namespace Radev.Licensing.Client
{
	/// <summary>
	/// Factory class to convert a <see cref="Contact"/> to a secure string representation.
	/// </summary>
	public static class ContactWriter
	{
		/// <summary>
		/// Converts specified contact into its safe string representation.
		/// </summary>
		/// <typeparam name="TContact">Effective type of contact class.</typeparam>
		/// <typeparam name="TConverter">Converter to encode contact string representation.</typeparam>
		/// <param name="contact">
		/// Contact to convert into a safe string representation. 
		/// </param>
		/// <returns>
		/// Safe string representation for specified contact. After contact has been encoded
		/// it can not be decoded in <em>public builds</em>, a <em>private build</em> (with access
		/// to private encryption key) is required.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="contact"/> is <see langword="null"/>.
		/// </exception>
		/// <remarks>
		/// When using custom <code>Contact</code> classes you should also
		/// provide your own <see cref="IContactTextConverter"/> parser implementation
		/// otherwise extra properties will not be serialized. If you do not have
		/// a custom implementation you should alwasy use non generic <see cref="ToString"/>
		/// overloaded version.
		/// </remarks>
		public static string ToString<TContact, TConverter>(TContact contact)
			where TContact : Contact, new()
			where TConverter : IContactTextConverter<TContact>, new()
		{
			if (contact == null)
				throw new ArgumentNullException("contact");

			return ClientSecureCommunication.EncodeMessageFromClient(new TConverter().ToString(contact));
		}

		/// <summary>
		/// Converts specified contact into its safe string representation.
		/// </summary>
		/// <param name="contact">
		/// Contact to convert into a safe string representation. 
		/// </param>
		/// <returns>
		/// Safe string representation for specified contact. After contact has been encoded
		/// it can not be decoded in <em>public builds</em>, a <em>private build</em> (with access
		/// to private encryption key) is required.
		/// </returns>
		public static string ToString(Contact contact)
		{
			return ToString<Contact, IniContactTextConverter<Contact>>(contact);
		}

		/// <summary>
		/// Saves <see cref="Contact"/> safe string representation into a file.
		/// </summary>
		/// <typeparam name="TContact">Effective type of contact class.</typeparam>
		/// <typeparam name="TConverter">Converter to encode contact string representation.</typeparam>
		/// <param name="path">Destination file full path.</param>
		/// <param name="contact">Contact you wish to save into specified file.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="path"/> is <see langword="null"/>.
		/// <br/>-or-<br/>
		/// <paramref name="contact"/> is <see langword="null"/>.
		/// </exception>
		/// <exception cref="IOException">
		/// In case of errors writing file to disk, if path is not accessible or file it's
		/// in use by another process/thread.
		/// </exception>
		/// <exception cref="UnauthorizedAccessException">
		/// You do not have enough privileges to write into specified file.
		/// </exception>
		/// <remarks>
		/// When using custom <code>Contact</code> classes you should also
		/// provide your own <see cref="IContactTextConverter"/> parser implementation
		/// otherwise extra properties will not be serialized. If you do not have
		/// a custom implementation you should alwasy use non generic <see cref="ToString"/>
		/// overloaded version.
		/// </remarks>
		public static void ToFile<TContact, TConverter>(string path, TContact contact)
			where TContact : Contact, new()
			where TConverter : IContactTextConverter<TContact>, new()
		{
			File.WriteAllText(path, ToString<TContact, TConverter>(contact), Encoding.UTF8);
		}

		/// <summary>
		/// Saves <see cref="Contact"/> safe string representation into a file.
		/// </summary>
		/// <param name="path">Destination file full path.</param>
		/// <param name="contact">Contact you wish to save into specified file.</param>
		/// <exception cref="ArgumentNullException">
		/// <paramref name="path"/> is <see langword="null"/>.
		/// <br/>-or-<br/>
		/// <paramref name="contact"/> is <see langword="null"/>.
		/// </exception>
		/// <exception cref="IOException">
		/// In case of errors writing file to disk, if path is not accessible or file it's
		/// in use by another process/thread.
		/// </exception>
		/// <exception cref="UnauthorizedAccessException">
		/// You do not have enough privileges to write into specified file.
		/// </exception>
		public static void ToFile(string path, Contact contact)
		{
			ToFile<Contact, IniContactTextConverter<Contact>>(path, contact);
		}
	}
}
