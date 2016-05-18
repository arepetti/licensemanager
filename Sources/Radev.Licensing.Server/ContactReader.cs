//
// Copyright (c) 2016 Repetti Adriano.
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
using Radev.Licensing.Client;

namespace Radev.Licensing.Server
{
    /// <summary>
    /// Factory class to create a new <see cref="Contact"/> from a secure string representation.
    /// </summary>
    public static class ContactReader
    {
        /// <summary>
        /// Converts a <see cref="Contact"/> from its secure string representation
        /// into a live object.
        /// </summary>
        /// <typeparam name="TContact">Effective type of contact class.</typeparam>
        /// <typeparam name="TParser">Parser to decode contact string representation.</typeparam>
        /// <param name="content">Contact safe string representation.</param>
        /// <returns>A new <see cref="Contact"/> filled with information from specified string.</returns>
        /// <exception cref="InvalidOperationException">
        /// Always for public builds. this method requires <code>PRIVATE_BUILD</code>
        /// because to decrypt contact private key is required.
        /// </exception>
        /// <exception cref="LicenseException">
        /// <paramref name="content"/> does not contain a valid contact (it may be empty,
        /// corrupted or of an unknown format).
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="content"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        /// When using custom <code>Contact</code> classes you should also
        /// provide your own <see cref="IContactTextConverter"/> parser implementation
        /// otherwise extra properties will not be deserialized. If you do not have
        /// a custom implementation you should alwasy use non generic <see cref="FromString"/>
        /// overloaded version.
        /// <br/><strong>This function is available only on <em>private builds</em></strong>
        /// with access to encryptor private key.
        /// </remarks>
        public static TContact FromString<TContact, TParser>(string content)
            where TContact : Contact, new()
            where TParser : IContactTextConverter<TContact>, new()
        {
            if (content == null)
                throw new ArgumentNullException("content");

            return new TParser().Parse(ServerSecureCommunication.DecodeMessageFromClient(content));
        }

        /// <summary>
        /// Converts a <see cref="Contact"/> from its secure string representation
        /// into a live object.
        /// </summary>
        /// <param name="content">Contact safe string representation.</param>
        /// <returns>A new <see cref="Contact"/> filled with information from specified string.</returns>
        /// <exception cref="InvalidOperationException">
        /// Always for public builds. this method requires <code>PRIVATE_BUILD</code>
        /// because to decrypt contact private key is required.
        /// </exception>
        /// <exception cref="LicenseException">
        /// <paramref name="content"/> does not contain a valid contact (it may be empty,
        /// corrupted or of an unknown format).
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="content"/> is <see langword="null"/>.
        /// </exception>
        /// <remarks>
        /// <strong>This function is available only on <em>private builds</em></strong>
        /// with access to encryptor private key.
        /// </remarks>
        public static Contact FromString(string content)
        {
            return FromString<Contact, IniContactTextConverter<Contact>>(content);
        }

        /// <summary>
        /// Loads a <see cref="Contact"/> from an UTF8 encoded text file into a live object.
        /// </summary>
        /// <typeparam name="TContact">Effective type of contact class.</typeparam>
        /// <typeparam name="TParser">Parser to decode contact string representation.</typeparam>
        /// <param name="path">File to load full path.</param>
        /// <returns>A new <see cref="Contact"/> filled with information from specified file.</returns>
        /// <exception cref="InvalidOperationException">
        /// Always for public builds. this method requires <code>PRIVATE_BUILD</code>
        /// because to decrypt contact private key is required.
        /// </exception>
        /// <exception cref="LicenseException">
        /// <paramref name="content"/> does not contain a valid contact (it may be empty,
        /// corrupted or of an unknown format).
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="path"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="IOException">
        /// If specified file can not be read or any error occurs during reading.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// If you do not have enough privileges to read specified file.
        /// </exception>
        /// <remarks>
        /// When using custom <code>Contact</code> classes you should also
        /// provide your own <see cref="IContactTextConverter"/> parser implementation
        /// otherwise extra properties will not be deserialized. If you do not have
        /// a custom implementation you should alwasy use non generic <see cref="FromString"/>
        /// overloaded version.
        /// <br/><strong>This function is available only on <em>private builds</em></strong>
        /// with access to encryptor private key.
        /// </remarks>
        public static TContact FromFile<TContact, TParser>(string path)
            where TContact : Contact, new()
            where TParser : IContactTextConverter<TContact>, new()
        {
            return FromString<TContact, TParser>(File.ReadAllText(path, Encoding.UTF8));
        }

        /// <summary>
        /// Loads a <see cref="Contact"/> from an UTF8 encoded text file into a live object.
        /// </summary>
        /// <param name="path">File to load full path.</param>
        /// <returns>A new <see cref="Contact"/> filled with information from specified file.</returns>
        /// <exception cref="LicenseException">
        /// <paramref name="content"/> does not contain a valid contact (it may be empty,
        /// corrupted or of an unknown format).
        /// </exception>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="path"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="IOException">
        /// If specified file can not be read or any error occurs during reading.
        /// </exception>
        /// <exception cref="UnauthorizedAccessException">
        /// If you do not have enough privileges to read specified file.
        /// </exception>
        /// <remarks>
        /// <strong>This function is available only on <em>private builds</em></strong>
        /// with access to encryptor private key.
        /// </remarks>
        public static Contact FromFile(string path)
        {
            return FromString(File.ReadAllText(path, Encoding.UTF8));
        }
    }
}
