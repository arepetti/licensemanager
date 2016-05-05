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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Radev.Licensing.Client
{
	/// <summary>
	/// Helper functions to collect information about assemblies.
	/// </summary>
	public static class Assemblies
	{
		/// <summary>
		/// Gets the product version associated with application entry point.
		/// </summary>
		/// <returns>
		/// Product version associated with application (host) entry point. It may be
		/// a managed or unamanged host. For managed hosts version number is searched in this order:
		/// <list type="number">
		/// <item><description>
		/// Deduced from qualified assembly name itself. 
		/// </description></item>
		/// <item><description>
		/// Version is searched in <see cref="AssemblyFileVersionAttribute"/>.
		/// </description></item>
		/// <item><description>
		/// In unmanaged resources via <see cref="FileVersionInfo"/> (first in
		/// <see cref="FileVersionInfo.ProductVersion"/> then <code>ProductXyzPart</code>
		/// and then in <see cref="FileVersionInfo.FileVersion"/>).
		/// </description></item>
		/// </list>
		/// For unmanaged hosts search is performed directly in unmanaged resources (with
		/// same order described before).
		/// <br/>
		/// Note that returned version number is unrelated to informational product version
		/// and it is always a valid <see cref="Version"/> version number.
		/// <br/>
		/// Return value may be <see langword="null"/> it's impossible to determine
		/// entry-point assembly or host path. In case of error or if there is not
		/// any version information this function returns <code>0.0</code> as version number.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Version GetProductVersion()
		{
			var productVersion = GetProductVersion(Assembly.GetEntryAssembly());
			if (productVersion != null)
				return productVersion;

			string entryPointModulePath = UnsafeNativeMethods.GetModuleFileName(NativeMethods.NullHandle);
			return GetProductVersion(entryPointModulePath);
		}

		/// <summary>
		/// Gets full path of application executable file.
		/// </summary>
		/// <returns>
		/// Full path of application executable file, it may be a managed or
		/// an unmanaged host. Managed host may return any valid URI (not only a file path).
		/// </returns>
		public static string ResolveEntryPointPath()
		{
			var entryPointAssembly = Assembly.GetEntryAssembly();
			if (entryPointAssembly != null)
				return ResolvePath(entryPointAssembly);

			return UnsafeNativeMethods.GetModuleFileName(NativeMethods.NullHandle);
		}

		private static Version GetProductVersion(Assembly assembly)
		{
			if (assembly == null)
				return null;

			var name = assembly.GetName();
			if (name != null && name.Version.Major > 0 && name.Version.Minor >= 0)
				return name.Version;

			try
			{
				var fv = assembly.GetCustomAttribute<AssemblyFileVersionAttribute>();
				if (fv != null)
					return new Version(fv.Version);

				var assemblyPath = ResolvePath(assembly);
				if (assemblyPath == null)
					return null;

				return GetProductVersion(assemblyPath);
			}
			catch (OverflowException)
			{
				// Too high part number (maximum is 65534) however you may
				// build with AssemblyFileVersionInfoAttribute and greater values.
			}

			return new Version(0, 0);
		}

		private static Version GetProductVersion(string path)
		{
			try
			{
				var fvi = FileVersionInfo.GetVersionInfo(path);
				if (!String.IsNullOrWhiteSpace(fvi.ProductVersion))
					return Version.Parse(fvi.ProductVersion);

				if (fvi.ProductMajorPart > 0 && fvi.ProductMinorPart >= 0)
					return new Version(fvi.ProductMajorPart, fvi.ProductMinorPart, fvi.ProductBuildPart);

				if (!String.IsNullOrWhiteSpace(fvi.FileVersion))
					return new Version(fvi.FileVersion);
			}
			catch (ArgumentException)
			{
				// Invalid version format or invalid assembly path.
			}
			catch (FormatException)
			{
				// Invalid FileVersionInfo.ProductVersion (maybe textual content)
			}
			catch (IOException)
			{
				// Cannot access assembly (or assembly does not exist)
			}
			catch (UnauthorizedAccessException)
			{
				// Not enough privileges to read/access assembly
			}

			return new Version(0, 0);
		}
		
		private static string ResolvePath(Assembly assembly)
		{
			if (assembly == null)
				return null;

			var uri = new Uri(assembly.CodeBase);
			if (uri.IsFile)
				return uri.LocalPath + Uri.UnescapeDataString(uri.Fragment);

			return uri.ToString();
		}
	}
}
