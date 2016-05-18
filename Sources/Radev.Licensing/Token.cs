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
using System.Reflection;
using System.Runtime.CompilerServices;
using Radev.Licensing.Client;

namespace Radev.Licensing
{
	/// <summary>
	/// Represents a token exchanged between client and server containing
	/// secured information.
	/// </summary>
	public abstract class Token
	{
		/// <summary>
		/// Creates a new instance of <see cref="Token"/> object.
		/// </summary>
		protected Token()
		{
			Id = "";
			_requiredHardwareConfiguration = new Dictionary<string, string>();
		}

		/// <summary>
		/// Gets/sets an unique identifier for this token.
		/// </summary>
		/// <value>
		/// Unique identifier used to identify this token. If this property is
		/// a null or empty string then this token must be considered invalid.
		/// </value>
		public string Id
		{
			get;
			set;
		}

		/// <summary>
		/// Gets/sets creation time of this token.
		/// </summary>
		/// <value>
		/// Creation time of this token, expressed in UTC. If not available
		/// it may be <see langword="null"/> (default value).
		/// </value>
		public DateTime? CreationTime
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets hardware configuration related to this token.
		/// </summary>
		/// <value>
		/// Hardware configuration related to this token, for a <see cref="Contact"/>
		/// it's the gathered configuration that uniquely identify a specific computer;
		/// for <see cref="License"/> it is the required hardware configuration for
		/// the destination client (it matches <see cref="RequiredHardwareConfiguration"/>
		/// contained in <c>Contact</c> from which <c>License</c> has been generated).
		/// </value>
		public IDictionary<string, string> RequiredHardwareConfiguration
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get { return _requiredHardwareConfiguration; }

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			protected internal set { _requiredHardwareConfiguration = value; }
		}

		/// <summary>
		/// Resolves product version.
		/// </summary>
		/// <returns>
		/// Version of the product where this assembly has been loaded.
		/// </returns>
		/// <remarks>
		/// It's used to fill <c>SoftwareVersion</c> for <c>Contact</c> and to
		/// validate version requirements for a given <c>License</c>. You may override
		/// this method in a derived class to provide a different version searching
		/// algorithm (by default it uses <see cref="Assemblies.GetProductVersion"/>).
		/// <br/>
		/// This method should not be used server-side.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected internal virtual Version ResolveSoftwareVersion()
		{
			return Assemblies.GetProductVersion();
		}

		/// <summary>
		/// Creates an instance of a <see cref="HardwareAnalyzer"/>.
		/// </summary>
		/// <returns>
		/// An instance of a new <see cref="HardwareAnalyzer"/> object, configured
		/// to gather all required information to uniquely identify machine where
		/// program is running.
		/// </returns>
		/// <remarks>
		/// This method should not be used server-side.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected internal virtual HardwareAnalyzer CreateHardwareAnalyzer()
		{
			return new WmiHardwareAnalyzer { Queries = WmiQueriesForHardwareConfiguration };
		}

		/// <summary>
		/// WMI queries to uniquely identify a system. Each WMI query is made by
		/// a class, a dot and a list of properties (separated with ampersand).
		/// </summary>
		private static readonly string[] WmiQueriesForHardwareConfiguration = new string[]
		{
			"Win32_ComputerSystem.Manufacturer&Model",
			"Win32_BaseBoard.Manufacturer&Model&SerialNumber",
			"Win32_BIOS.SerialNumber",
			"Win32_Processor.ProcessorId",
			"Win32_OperatingSystem.SerialNumber",
			"Win32_OperatingSystem.InstallDate"
		};

		private IDictionary<string, string> _requiredHardwareConfiguration;
	}
}
