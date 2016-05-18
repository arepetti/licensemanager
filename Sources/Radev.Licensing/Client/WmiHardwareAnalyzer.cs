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
using System.Linq;
using System.Globalization;
using System.Management;
using System.Text;

namespace Radev.Licensing.Client
{
    /// <summary>
    /// Implements <see cref="HardwareAnalyzer"/> gathering
    /// hardware information using WMI queries.
    /// </summary>
    /// <remarks>
    /// See <see cref="Constraints.WmiQueriesForHardwareConfiguration"/>
    /// for list and syntax of supported WMI queries.
    /// </remarks>
    public class WmiHardwareAnalyzer : HardwareAnalyzer
    {
        protected override string Query(string query)
        {
            var parts = query.SplitKeyValuePair(ClassNameAndPropertiesSeparator);
            return Query(parts.Key, parts.Value.Split(PropertiesSeparator));
        }

        private const char PropertiesSeparator = '&';
        private const char ClassNameAndPropertiesSeparator = '.';
        private const string EntriesValueSeparator = ",";
        private const string PropertiesValueSeparator = " ";

        private string Query(string queryOrClassName, string[] properties)
        {
            // Each query may produce multiple results and for each result
            // all required properties are joined. Each property is separated with
            // PropertiesValueSeparator and each result is separated with EntriesValueSeparator.
            // Leading and trailing spaces are trimmed everywhere.
            var result = new StringBuilder();

            try
            {
                var query = new SelectQuery(queryOrClassName);
                using (var searcher = new ManagementObjectSearcher(query))
                {
                    var entries = searcher.Get()
                        .OfType<ManagementBaseObject>()
                        .Select(x => WmiObjectToString(x, properties))
                        .Where(x => !String.IsNullOrWhiteSpace(x));

                    result.Append(String.Join(EntriesValueSeparator, entries));
                }
            }
            catch (ManagementException exception)
            {
                OnError(new HardwareAnalyzerErrorEventArgs(exception, result));
            }

            return result.RemoveNewLinesAndTabs().ToString();
        }

        private static string WmiObjectToString(ManagementBaseObject obj, string[] properties)
        {
            var values = properties
                .Select(x => WmiPropertyToString(obj[x]))
                .Where(x => !String.IsNullOrWhiteSpace(x));

            return String.Join(PropertiesValueSeparator, values);
        }

        private static string WmiPropertyToString(object obj)
        {
            return Convert.ToString(obj, CultureInfo.InvariantCulture).Trim();
        }
    }
}
