//
// Copyright (c) 2015 Repetti Adriano.
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
	/// Represents a time interval.
	/// </summary>
	/// <remarks>
	/// This class is simply a conventient representation for <code>Tuple{DateTime, DateTime}</code>
	/// and it doesn't add any other meaning or purpose.
	/// <br/>
	/// <strong>
	/// It's not intended to be used to properly represent a time interval in any other scenario.
	/// </strong>
	/// </remarks>
	public struct Interval
	{
		/// <summary>
		/// Constructs a new <see cref="Interval"/> with given boundary.
		/// </summary>
		/// <param name="from">
		/// Interval beginning, <see langword="null"/> if beginning is unspecified.
		/// </param>
		/// <param name="to">
		/// Interval end, <see langword="null"/> if end is unspecified.
		/// </param>
		/// <exception cref="ArgumentException">
		/// Both <paramref name="from"/> and <paramref name="to"/> are not
		/// <see langword="null"/> and <paramref name="to"/> is earlier (in time)
		/// than <paramref name="from"/>.
		/// </exception>
		public Interval(DateTime? from, DateTime? to)
		{
			if (from != null && to != null && to < from)
				throw new ArgumentException("Interval can not end before its beginning.");

			_from = from;
			_to = to;
		}

		/// <summary>
		/// Gets interval beginning.
		/// </summary>
		/// <value>
		/// Interval beginning or <see langword="null"/> if this interval
		/// has no specified beginning.
		/// </value>
		public DateTime? From
		{
			get { return _from; }
		}

		/// <summary>
		/// Gets interval end.
		/// </summary>
		/// <value>
		/// Interval end or <see langword="null"/> if this interval
		/// has no specified end.
		/// </value>
		public DateTime? To
		{
			get { return _to; }
		}

		/// <summary>
		/// Gets interval duration.
		/// </summary>
		/// <value>
		/// Interval duration (from its beginning to its end, inclusive) if
		/// both boundaries are specified, <see langword="null"/> otherwise.
		/// </value>
		public TimeSpan? Duration
		{
			get
			{
				if (From == null || To == null)
					return null;

				return To.Value - From.Value;
			}
		}

		/// <summary>
		/// Checks if specified instant is contained inside this interval.
		/// </summary>
		/// <param name="value">Instant to check.</param>
		/// <returns>
		/// <see langword="true"/> if specified instant is contained inside this
		/// interval: it's not less than <see cref="From"/> (if specified) and it's not
		/// more than <see cref="To"/> (if specified).
		/// </returns>
		public bool Contains(DateTime value)
		{
			if (From != null && value < From.Value)
				return false;

			if (To != null && value > To.Value)
				return false;

			return true;
		}

		private readonly DateTime? _from, _to;
	}
}
