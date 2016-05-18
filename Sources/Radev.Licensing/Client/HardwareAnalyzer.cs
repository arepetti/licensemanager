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
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Radev.Licensing.Client
{
    /// <summary>
    /// Base class for hardware analyzers in charge to collect information about
    /// hardware configuration to uniquely identify a single computer to which
    /// license is tied.
    /// </summary>
    public abstract class HardwareAnalyzer
    {
        /// <summary>
        /// Gests/sets collection of queries used to collect hardware information.
        /// </summary>
        /// <value>
        /// Collection of queries (with syntax specific for each concrete implementation)
        /// used to collect hardware information which will tie a license to a specific computer.
        /// Each query must be unique in the sense that same text can not be repeated more than once.
        /// </value>
        /// <exception cref="ArgumentNullException">
        /// Specified value is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Specified value is an empty collection.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// This collection has been already assigned, its value cannot be changed twice.
        /// </exception>
        public IEnumerable<string> Queries
        {
            get { return _queries; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                if (!value.Any())
                    throw new ArgumentException("Queries to perform can not be empty.");

                if (_queries != null)
                    throw new InvalidOperationException("Cannot change queries to perform after it has been initialized once.");

                _queries = value;
            }
        }

        /// <summary>
        /// Performs queries specified in <see cref="Queries"/>.
        /// </summary>
        /// <returns>
        /// Result of performed queries. Keys in this dictionary are unmodified input queries
        /// and each value is result (specific of each concrete implementation) of that query.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// <see cref="Queries"/> has not been initialized (it's <see langword="null"/>)
        /// or it contains a duplicated query.
        /// </exception>
        public IDictionary<string, string> Query()
        {
            if (Queries == null || !Queries.Any())
                throw new InvalidOperationException("Queries to perform can not be null or empty.");

#if FEATURE_PARALLEL
            var result = new ConcurrentDictionary<string, string>();
            Parallel.ForEach(Queries, x => result.TryAdd(x, Query(x)));

            return result;
#else
			return Queries.Aggregate(new Dictionary<string, string>(), (result, query) =>
			{
				result.Add(query, Query(query));
				return result;
			});
#endif
        }

        /// <summary>
        /// Event generated in case of error while performing hardware scanning.
        /// </summary>
        public event EventHandler<HardwareAnalyzerErrorEventArgs> Error;

        /// <summary>
        /// Performs specified query.
        /// </summary>
        /// <param name="query">Query to perform; meaning and syntax depend on concrete implementation.</param>
        /// <returns>Query result, meaning and syntax depend on concrete implementation.</returns>
        protected abstract string Query(string query);

        /// <summary>
        /// Raises <see cref="Error"/> event.
        /// </summary>
        /// <param name="ea">Additional arguments for <code>Error</code> event.</param>
        protected virtual void OnError(HardwareAnalyzerErrorEventArgs ea)
        {
            var error = Error;
            if (error != null)
                error(this, ea);
        }

        private IEnumerable<string> _queries;
    }
}
