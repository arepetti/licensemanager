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
using System.Globalization;

namespace Radev.Licensing.Client
{
    /// <summary>
    /// Implements <see cref="IContactTextConverter{T}"/> to store a contact
    /// into an INI-like text structure.
    /// </summary>
    /// <typeparam name="T">Concrete contact type.</typeparam>
    public sealed class IniContactTextConverter<T> : IContactTextConverter<T>
        where T : Contact, new()
    {
        /// <summary>
        /// Creates a new <see cref="IniContactTextConverter"/> object.
        /// </summary>
        public IniContactTextConverter()
        {
            _data = new IniFile();
        }

        T IContactTextConverter<T>.Parse(string content)
        {
            _data.Clear();
            _data.Parse(content);
            CheckFileFormat();

            var license = new T()
            {
                Id = _data.GetString(ContactSection, IdKey, ""),
                CreationTime = _data.GetDateTime(ContactSection, CreationTimeKey),
                SoftwareVersion = _data.GetVersion(ContactSection, VersionKey, null)
            };

            ParseRequiredHardwareConfiguration(license);

            return license;
        }

        string IContactTextConverter<T>.ToString(T contact)
        {
            _data.Clear();

            _data.SetString(ContactSection, FormatKey, Format);
            _data.SetString(ContactSection, IdKey, contact.Id);
            _data.SetDateTime(ContactSection, CreationTimeKey, contact.CreationTime);
            _data.SetVersion(ContactSection, VersionKey, contact.SoftwareVersion);

            foreach (var entry in contact.RequiredHardwareConfiguration)
                _data.SetString(HardwareSection, entry);

            return _data.ToString();
        }

        private IniFile _data;

        private static readonly string Format = "1";
        private const string ContactSection = "Contact";
        private const string HardwareSection = "Hardware";
        private const string FormatKey = "Contact.Format";
        private const string IdKey = "Contact.Id";
        private const string CreationTimeKey = "Contact.Creation";
        private const string VersionKey = "Product.Version";

        private void CheckFileFormat()
        {
            if (!_data.GetString(ContactSection, FormatKey, "").Equals(Format, StringComparison.InvariantCultureIgnoreCase))
                throw new LicenseException("Unknown contact format.");
        }

        private void ParseRequiredHardwareConfiguration(T license)
        {
            foreach (var entry in _data.EnumerateValueNames(HardwareSection))
            {
                license.RequiredHardwareConfiguration.Add(entry,
                    _data.GetString(HardwareSection, entry, ""));
            }
        }
    }
}
