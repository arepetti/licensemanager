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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Security;

namespace Radev.Licensing
{
    /// <summary>
    /// Represents a license.
    /// </summary>
    [SecurityCritical]
    public class License : Token
    {
        /// <summary>
        /// Constructs a new and empty <see cref="License"/> object.
        /// </summary>
        public License()
        {
            _features = new Dictionary<int, int>();
        }

        /// <summary>
        /// Gets/sets time validity for this license.
        /// </summary>
        /// <value>
        /// Time validity for this license, before its beginning or after its end this license
        /// is considered expried (and then not valid). Default value is <see langword="null"/>
        /// (it represents a license without any time constraint and then without expiration).
        /// </value>
        /// <exception cref="InvalidOperationException">
        /// This property has been already assigned (it can not be assigned twice) or license
        /// has been <em>frozen</em> and can not be further modified.
        /// </exception>
        public Interval Validity
        {
            get { return _validity ?? new Interval(); }
            set
            {
                if (_validity != null || _frozen)
                    throw new InvalidOperationException("Cannot change license validity interval after it has been initialized once.");

                _validity = value;
            }
        }

        /// <summary>
        /// Gets/sets license end-user information.
        /// </summary>
        /// <value>
        /// Information about end-user to whom this license is associated. Default value is
        /// <see langword="null"/> (no information available).
        /// </value>
        /// <exception cref="InvalidOperationException">
        /// This property has been already assigned (it can not be assigned twice) or license
        /// has been <em>frozen</em> and can not be further modified.
        /// </exception>
        public EndUser EndUser
        {
            get { return _endUser; }
            set
            {
                if (_endUser != null || _frozen)
                    throw new InvalidOperationException("Cannot change license end-user after it has been initialized once.");

                _endUser = value;
            }
        }

        /// <summary>
        /// Gets/sets minimum required software version required for this license.
        /// </summary>
        /// <value>
        /// License is valid only if software version is at least value specified in
        /// this property. If omitted (then <see langword="null"/>) an older software version may
        /// reuse a license issued for a newer version (it applies in both directions,
        /// see also <see cref="MaximumVersion"/>). Default value is <see langword="null"/>.
        /// </value>
        /// <exception cref="InvalidOperationException">
        /// This property has been already assigned (it can not be assigned twice) or license
        /// has been <em>frozen</em> and can not be further modified.
        /// </exception>
        /// <seealso cref="MaximumVersion"/>
        public Version MinimumVersion
        {
            get { return _minimumVersion; }
            set
            {
                if (_minimumVersion != null || _frozen)
                    throw new InvalidOperationException("Cannot change product minimum version after it has been initialized once.");

                _minimumVersion = value;
            }
        }

        /// <summary>
        /// Gets/sets maximum allowed software version required for this license.
        /// </summary>
        /// <value>
        /// License is valid only if software version is at most value specified in this property.
        /// If omitted (then <see langword="null"/>) then versions newer that what specified
        /// will not be allowed to run. Default value is <see langword="null"/>.
        /// </value>
        /// <exception cref="InvalidOperationException">
        /// This property has been already assigned (it can not be assigned twice) or license
        /// has been <em>frozen</em> and can not be further modified.
        /// </exception>
        /// <seealso cref="MinimumVersion"/>
        public Version MaximumVersion
        {
            get { return _maximumVersion; }
            set
            {
                if (_maximumVersion != null || _frozen)
                    throw new InvalidOperationException("Cannot change product maximum version after it has been initialized once.");

                _maximumVersion = value;
            }
        }

        /// <summary>
        /// Gets a dictionary of <em>features</em> attached to this license.
        /// </summary>
        /// <value>
        /// Features dictionary is made by integer <see cref="Int32"/> tuples. It may be used
        /// to store flags (for example if a given feature is available or not) or to store integer
        /// values directly (for example maximum number of concurrent users). This class exposes
        /// the raw implementation <see cref="IDictionary{Int32,Int32}"/> but it should not be accessed
        /// directly, a wrapper should hide this (see, for example, reference <see cref="LicenseManager"/>
        /// implementation).
        /// </value>
        public IDictionary<int, int> Features
        {
            get { return _features; }
        }

        /// <summary>
        /// Freezes this license with its current configuration. No further changes are possible.
        /// </summary>
        /// <remarks>
        /// Note that freezing is a weak protection, your code must be secured and protected
        /// against malicious/untrusted code injection. Malicious code may still change license
        /// fields at run-time through Reflection and JIT compiled code may still be tampered
        /// by a malicious external program.
        /// </remarks>
        public void Freeze()
        {
            _frozen = true;
            RequiredHardwareConfiguration = new ReadOnlyDictionary<string, string>(RequiredHardwareConfiguration);
            _features = new ReadOnlyDictionary<int, int>(_features);
        }

        /// <summary>
        /// Verifies if license is expired.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if license is expired, <see langword="false"/> if it's still
        /// valid or it has no expiration. Note that an invalid license (for example because
        /// issued to another machine or a different software version) may return <see langword="true"/>
        /// for this: only validity is checked.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsExpired()
        {
            // UTC check is because license may be issued with an UTC interval, to avoid
            // silly errors we relax validity for these few hours.
            return !Validity.Contains(DateTime.Now) && !Validity.Contains(DateTime.UtcNow);
        }

        /// <summary>
        /// Verifies if license is valid and it is not expired.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if license is valid (it has been issued for this
        /// machine and for this software version) and it is not expired.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsValid()
        {
            if (IsExpired())
                return false;

            return IsValidForThisSoftwareVersion() && IsAssociatedWithThisMachine();
        }

        /// <summary>
        /// Search for a feature with specified ID and return its value.
        /// </summary>
        /// <param name="id">ID of the feature to search for, see <see cref="Features"/>.</param>
        /// <returns>
        /// Value for the feature with specified ID or <see langword="null"/> if that feature
        /// has not been specified/included.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int? ReadFeature(int id)
        {
            if (_features.ContainsKey(id))
                return _features[id];

            return null;
        }

        // Maximum number of allowed hardware changes before license is considered
        // obsolete/invalid and has to be renewed according to new hardware configuration.
        private const int MaximumNumberOfHardwareChanges = 2;

        private Interval? _validity;
        private EndUser _endUser;
        private Version _minimumVersion, _maximumVersion;
        private IDictionary<int, int> _features;
        private bool _frozen;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsValidForThisSoftwareVersion()
        {
            var currentVersion = ResolveSoftwareVersion();

            if (currentVersion == null)
            {
                if (MaximumVersion != null || MinimumVersion != null)
                    return false;

                return true;
            }

            if (MaximumVersion != null && currentVersion > MaximumVersion)
                return false;

            if (MinimumVersion != null && currentVersion < MinimumVersion)
                return false;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool IsAssociatedWithThisMachine()
        {
            int differences = 0;

            // If parallel WMI queries execution is enabled (FEATURE_PARALLEL) then
            // we must not rely on order, same entry may be on a different position each time.
            var currentHardwareConfiguration = CreateHardwareAnalyzer().Query();
            foreach (var required in RequiredHardwareConfiguration)
            {
                if (currentHardwareConfiguration.ContainsKey(required.Key))
                {
                    var current = currentHardwareConfiguration[required.Key];
                    if (String.Equals(required.Value, current, StringComparison.CurrentCultureIgnoreCase))
                        continue;
                }

                ++differences;
            }

            return differences <= MaximumNumberOfHardwareChanges;
        }
    }
}


