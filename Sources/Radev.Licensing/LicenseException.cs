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

namespace Radev.Licensing
{
    /// <summary>
    /// Represents errors related to licensing.
    /// </summary>
    public sealed class LicenseException : Exception
    {
        /// <summary>
        /// Constructs a new <see cref="LicenseException"/> object with
        /// a specified error message.
        /// </summary>
        /// <param name="message">Error messages that describes an error.</param>
        public LicenseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Constructs a new <see cref="LicenseException"/> object with
        /// a specified error message an an inner exception that caused this issue.
        /// </summary>
        /// <param name="message">Error messages that describes an error.</param>
        /// <param name="exception">Inner exception that caused this error.</param>
        public LicenseException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}
