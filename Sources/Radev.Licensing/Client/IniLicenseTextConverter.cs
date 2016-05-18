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
    /// Implements <see cref="ILicenseTextConverter{T}"/> to store a license
    /// into an INI-like text structure.
    /// </summary>
    /// <typeparam name="T">Concrete license type.</typeparam>
    public sealed class IniLicenseTextConverter<T> : ILicenseTextConverter<T>
        where T : License, new()
    {
        public IniLicenseTextConverter()
        {
            _data = new IniFile();
        }

        T ILicenseTextConverter<T>.Parse(string content)
        {
            _data.Parse(content);

            CheckFileFormat();

            var license = new T()
            {
                Id = _data.GetString(LicenseSection, IdKey, ""),
                CreationTime = _data.GetDateTime(LicenseSection, CreationTimeKey)
            };

            ParseProductAndValidity(license);
            ParseEndUser(license);
            ParseRequiredHardwareConfiguration(license);
            ParseFeatures(license);

            _data.Clear();
            return license;
        }

        string ILicenseTextConverter<T>.ToString(T license)
        {
            // You may use conditional compilation to leave out this function
            // (throwing an exception) for non PRIVATE_BUILD builds.
            _data.Clear();

            _data.SetString(LicenseSection, FormatKey, Format);
            _data.SetString(LicenseSection, IdKey, license.Id);
            _data.SetDateTime(LicenseSection, CreationTimeKey, license.CreationTime);
            _data.SetVersion(LicenseSection, MinimumVersionKey, license.MinimumVersion);
            _data.SetVersion(LicenseSection, MaximumVersionKey, license.MaximumVersion);
            _data.SetDateTime(LicenseSection, ValidFromKey, license.Validity.From);
            _data.SetDateTime(LicenseSection, ValidToKey, license.Validity.To);

            if (license.EndUser != null)
            {
                _data.SetString(LicenseSection, EndUserFullNameKey, license.EndUser.FullName);
                _data.SetString(LicenseSection, EndUserOrganizationKey, license.EndUser.Organization);
                _data.SetString(LicenseSection, EndUserAddressKey, license.EndUser.Address);
                _data.SetString(LicenseSection, EndUserPhoneNumberKey, license.EndUser.PhoneNumber);
                _data.SetString(LicenseSection, EndUserEMailAddressKey, license.EndUser.EMailAddress);
                _data.SetString(LicenseSection, EndUserNotesKey, license.EndUser.Notes);
            }

            foreach (var entry in license.RequiredHardwareConfiguration)
                _data.SetString(HardwareSection, entry);

            foreach (var entry in license.Features)
            {
                _data.SetString(FeaturesSection,
                    entry.Key.ToString(CultureInfo.InvariantCulture),
                    entry.Value.ToString(CultureInfo.InvariantCulture));
            }

            return _data.ToString();
        }

        private IniFile _data;

        private static readonly string Format = "1";
        private const string LicenseSection = "License";
        private const string HardwareSection = "Hardware";
        private const string FeaturesSection = "Features";
        private const string FormatKey = "License.Format";
        private const string IdKey = "License.Id";
        private const string CreationTimeKey = "License.Creation";
        private const string MinimumVersionKey = "Product.Version.Minimum";
        private const string MaximumVersionKey = "Product.Version.Maximum";
        private const string ValidFromKey = "Validity.From";
        private const string ValidToKey = "Validity.To";
        private const string EndUserFullNameKey = "EndUser.FullName";
        private const string EndUserOrganizationKey = "EndUser.Organization";
        private const string EndUserAddressKey = "EndUser.Address";
        private const string EndUserPhoneNumberKey = "EndUser.PhoneNumber";
        private const string EndUserEMailAddressKey = "EndUser.EMailAddress";
        private const string EndUserNotesKey = "EndUser.Notes";

        private void CheckFileFormat()
        {
            if (!_data.GetString(LicenseSection, FormatKey, "").Equals(Format, StringComparison.InvariantCultureIgnoreCase))
                throw new LicenseException("Unknown license format.");
        }

        private void ParseProductAndValidity(T license)
        {
            license.MinimumVersion = _data.GetVersion(LicenseSection, MinimumVersionKey, null);
            license.MaximumVersion = _data.GetVersion(LicenseSection, MaximumVersionKey, null);
            license.Validity = new Interval(
                _data.GetDateTime(LicenseSection, ValidFromKey),
                _data.GetDateTime(LicenseSection, ValidToKey));
        }

        private void ParseEndUser(T license)
        {
            license.EndUser = new EndUser
            {
                FullName = _data.GetString(LicenseSection, EndUserFullNameKey, ""),
                Organization = _data.GetString(LicenseSection, EndUserOrganizationKey, ""),
                Address = _data.GetString(LicenseSection, EndUserAddressKey, ""),
                PhoneNumber = _data.GetString(LicenseSection, EndUserPhoneNumberKey, ""),
                EMailAddress = _data.GetString(LicenseSection, EndUserEMailAddressKey, ""),
                Notes = _data.GetString(LicenseSection, EndUserNotesKey, ""),
            };
        }

        private void ParseRequiredHardwareConfiguration(T license)
        {
            foreach (var entry in _data.EnumerateValueNames(HardwareSection))
            {
                license.RequiredHardwareConfiguration.Add(entry,
                    _data.GetString(HardwareSection, entry, ""));
            }
        }

        private void ParseFeatures(T license)
        {
            foreach (var entry in _data.EnumerateValueNames(FeaturesSection))
            {
                license.Features.Add(Convert.ToInt32(entry, CultureInfo.InvariantCulture),
                    _data.GetInt32(FeaturesSection, entry, 0));
            }
        }

    }
}
