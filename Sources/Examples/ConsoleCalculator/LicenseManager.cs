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
using System.Runtime.CompilerServices;
using Radev.Licensing.Client;

namespace Radev.Licensing
{
    static class LicenseManager
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsLicenseValid()
        {
            return _license != null && _license.IsValid();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static bool IsFeatureAvailable(Feature feature)
        {
            if (!IsLicenseValid())
                return false;

            return (_license.ReadFeature((int)feature) ?? -1) >= 0;
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void ExitIfNotSatisfied(Func<bool> action)
        {
            if (!action())
                Environment.Exit(1);
        }

        private const string LicenseFileName = "ConsoleCalculator.lic";
        private static readonly License _license = ReadCurrentLicense();

        private static License ReadCurrentLicense()
        {
            var path = GetLicenseFilePath();
            if (File.Exists(path))
                return LicenseReader.FromFile(path);

            return null;
        }

        private static string GetLicenseFilePath()
        {
            // 1st attempt is license file located in application folder
            var executablePath = Assemblies.ResolveEntryPointPath();
            if (String.IsNullOrWhiteSpace(executablePath))
                return null;

            var path = Path.Combine(Path.GetDirectoryName(executablePath), LicenseFileName);
            if (File.Exists(path))
                return path;

            try
            {
                // 2nd attempt is license file located in shared documents folder
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),
                    LicenseFileName);
            }
            catch (PlatformNotSupportedException)
            {
                // 3rd attempt...something weird, I can't find SpecialFolder.CommonDocuments
                return Path.Combine(Environment.CurrentDirectory, LicenseFileName);
            }
        }
    }
}