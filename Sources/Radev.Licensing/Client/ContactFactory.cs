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
    /// Factory class to create a new <see cref="Contact"/>.
    /// </summary>
    public static class ContactFactory
    {
        /// <summary>
        /// Create a new <see cref="Contact"/>.
        /// </summary>
        /// <typeparam name="T">Effective type of created contact.</typeparam>
        /// <returns>
        /// New contact initialized with all properties required in <code>Contact</code>
        /// base class and containing gathered system configuration (as indicated in
        /// <see cref="Contact.CreateHardwareAnalyzer"/>).
        /// </returns>
        /// <remarks>
        /// If you use a custom derived class you have to manually fill all extra
        /// properties you wish to save. Derived classes must also
        /// define their own <see cref="IContactTextConverter{TContact}"/> and
        /// use generic version of string conversion methods.
        /// </remarks>
        public static T Create<T>()
            where T : Contact, new()
        {
            var contact = new T();

            contact.Id = Guid.NewGuid().ToString();
            contact.CreationTime = DateTime.UtcNow;
            contact.RequiredHardwareConfiguration = contact.CreateHardwareAnalyzer().Query();
            contact.SoftwareVersion = contact.ResolveSoftwareVersion();

            return contact;
        }
    }
}
