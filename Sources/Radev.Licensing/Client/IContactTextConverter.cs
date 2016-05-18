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

namespace Radev.Licensing.Client
{
    /// <summary>
    /// Converts a <see cref="Contact"/> to and from a string.
    /// </summary>
    /// <typeparam name="T">Concrete contact type.</typeparam>
    public interface IContactTextConverter<T>
        where T : Contact
    {
        /// <summary>
        /// Converts specified plain text into a new <see cref="Contact"/>.
        /// </summary>
        /// <param name="content">
        /// String representation of a <c>Contact</c>. It must be plain
        /// text representation of a contact, encrypted and/or encoded text
        /// may be rejected or cause invalid results. No check is required.
        /// </param>
        /// <returns>
        /// A new <c>Contact</c> created with data from <paramref name="content"/>.
        /// </returns>
        T Parse(string content);

        /// <summary>
        /// Convert specified <c>Contact</c> to its plain text representation.
        /// </summary>
        /// <param name="contact"><c>Contact</c> to convert.</param>
        /// <returns>
        /// Plain text (not obsufscated nor encrypted) representation of specified contact.
        /// </returns>
        string ToString(T contact);
    }
}
