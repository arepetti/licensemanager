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
using System.Globalization;
using System.Linq;
using System.Text;

namespace Radev.Licensing.Client
{
    /// <summary>
    /// Simple INI-like dictionary.
    /// </summary>
    /// <remarks>
    /// Each line must be separate with <c>\r\n</c>.
    /// </remarks>
    sealed class IniFile
    {
        public IniFile(string content = "")
        {
            _values = CreateDictionary<Dictionary<string, string>>();

            if (!String.IsNullOrWhiteSpace(content))
                ParseLines(Split(content));
        }

        public void Parse(string content)
        {
            ParseLines(Split(content));
        }

        public IEnumerable<string> EnumerateValueNames(string sectionName)
        {
            var values = GetSection(sectionName);
            if (values != null)
            {
                foreach (KeyValuePair<string, string> entry in values)
                    yield return entry.Key;
            }
        }

        public void SetString(string sectionName, string valueName, string value)
        {
            CheckIsValidValueName(valueName);

            var values = GetSection(sectionName);
            if (values == null)
            {
                values = CreateDictionary<string>();
                _values.Add(sectionName, values);
            }

            if (values.ContainsKey(valueName))
                values[valueName] = value;
            else
                values.Add(valueName, value);
        }

        public void SetString(string sectionName, KeyValuePair<string, string> entry)
        {
            SetString(sectionName, entry.Key, entry.Value);
        }

        public void SetDateTime(string sectionName, string valueName, DateTime? value)
        {
            if (value != null)
                SetString(sectionName, valueName, value.Value.ToString("o", CultureInfo.InvariantCulture));
            else
                SetString(sectionName, valueName, "");
        }

        public void SetVersion(string sectionName, string valueName, Version value)
        {
            if (value != null)
                SetString(sectionName, valueName, value.ToString());
            else
                SetString(sectionName, valueName, "");
        }

        public string GetString(string sectionName, string valueName, string defaultValue)
        {
            var values = GetSection(sectionName);
            if (values != null && values.ContainsKey(valueName))
                return values[valueName];

            return defaultValue;
        }

        public Version GetVersion(string sectionName, string valueName, Version defaultValue)
        {
            var values = GetSection(sectionName);
            if (values != null && values.ContainsKey(valueName))
            {
                string value = values[valueName];
                if (String.IsNullOrWhiteSpace(value))
                    return defaultValue;

                return Version.Parse(value);
            }

            return defaultValue;
        }

        public DateTime? GetDateTime(string sectionName, string valueName)
        {
            var values = GetSection(sectionName);
            if (values != null && values.ContainsKey(valueName))
            {
                string value = values[valueName];
                if (String.IsNullOrWhiteSpace(value))
                    return null;

                DateTime result;
                if (DateTime.TryParseExact(value, "o", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out result))
                    return result;
            }

            return null;
        }

        public int GetInt32(string sectionName, string valueName, int defaultValue)
        {
            var values = GetSection(sectionName);
            if (values != null && values.ContainsKey(valueName))
                return Convert.ToInt32(values[valueName], CultureInfo.InvariantCulture);

            return defaultValue;
        }

        public override string ToString()
        {
            var content = new StringBuilder();

            foreach (var section in _values)
            {
                if (content.Length > 0)
                    content.AppendLine();

                content.AppendFormat("[{0}]{1}", section.Key, Environment.NewLine);

                foreach (KeyValuePair<string, string> value in section.Value)
                    content.AppendFormat("{0}={1}{2}", value.Key, value.Value, Environment.NewLine);
            }

            return content.ToString();
        }

        public void Clear()
        {
            _values.Clear();
        }

        private readonly Dictionary<string, Dictionary<string, string>> _values;

        private static string[] Split(string content)
        {
            return content.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
        }

        private static Dictionary<string, T> CreateDictionary<T>()
        {
            return new Dictionary<string, T>(StringComparer.InvariantCultureIgnoreCase);
        }

        private void ParseLines(string[] lines)
        {
            string currentSection = "";
            foreach (string line in lines.Where(IsNotCommentOrEmpty))
                ParseLine(ref currentSection, line.Trim());
        }

        private static bool IsNotCommentOrEmpty(string line)
        {
            if (String.IsNullOrWhiteSpace(line))
                return true;

            return !line.TrimStart().StartsWith(";", StringComparison.InvariantCulture);
        }

        private void ParseLine(ref string currentSection, string line)
        {
            if (IsSectionName(line))
                currentSection = ExtractSectionName(line);
            else
                SetString(currentSection, line.SplitKeyValuePair('='));
        }

        private static bool IsSectionName(string line)
        {
            return line.StartsWith("[", StringComparison.InvariantCulture)
                && line.EndsWith("]", StringComparison.InvariantCulture)
                && line.Length > 2;
        }

        private static string ExtractSectionName(string line)
        {
            return line.Substring(1, line.Length - 2);
        }

        private static void CheckIsValidValueName(string valueName)
        {
#if DEBUG
            if (valueName == null)
                throw new ArgumentNullException("valueName");

            if (String.IsNullOrWhiteSpace(valueName))
                throw new ArgumentException("Value name can not be empty or made of white spaces.", valueName);

            if (valueName.Contains("="))
                throw new ArgumentException("Name part can not contain character '='.", valueName);

            if (valueName.StartsWith("[", StringComparison.InvariantCulture))
                throw new ArgumentException("Name part can not start with '['.", valueName);

            if (valueName.StartsWith(";", StringComparison.InvariantCulture))
                throw new ArgumentException("Name part can not start with ';'.", valueName);
#endif
        }

        private object GetValue(string sectionName, string valueName, object defaultValue)
        {
            var values = GetSection(sectionName);
            if (values == null)
                return defaultValue;

            if (values.ContainsKey(valueName))
                return values[valueName];

            return defaultValue;
        }

        private Dictionary<string, string> GetSection(string sectionName)
        {
            if (_values.ContainsKey(sectionName))
                return _values[sectionName];

            return null;
        }
    }
}
