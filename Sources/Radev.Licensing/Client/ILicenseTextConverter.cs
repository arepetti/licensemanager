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
    /// Converts a <see cref="License"/> to and from a string.
    /// </summary>
    /// <typeparam name="T">Concrete license type.</typeparam>
    public interface ILicenseTextConverter<T>
        where T : License
    {
        /// <summary>
        /// Converts specified plain text into a new <see cref="License"/>.
        /// </summary>
        /// <param name="content">
        /// String representation of a <c>License</c>. It must be plain
        /// text representation of a license, encrypted and/or encoded text
        /// may be rejected or cause invalid results. No check is required.
        /// </param>
        /// <returns>
        /// A new <c>License</c> created with data from <paramref name="content"/>.
        /// </returns>
        T Parse(string content);

        /// <summary>
        /// Convert specified <c>License</c> to its plain text representation.
        /// </summary>
        /// <param name="license"><c>License</c> to convert.</param>
        /// <returns>
        /// Plain text (not obsufscated nor encrypted) representation of specified license.
        /// </returns>
        string ToString(T license);
    }
}
