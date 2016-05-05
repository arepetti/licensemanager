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
using System.IO;
using System.Threading;

namespace Radev.Licensing.Tests
{
	/// <summary>
	/// Helper class to create and automatically delete (with <em>Dispose pattern</em>)
	/// a temporary file.
	/// </summary>
	sealed class TemporaryFile : IDisposable
	{
		public TemporaryFile(string path = null)
		{
			_path = path ?? Path.GetTempFileName();
		}

		~TemporaryFile()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public static implicit operator string(TemporaryFile file)
		{
			return file._path;
		}

		private string _path;

		private void Dispose(bool disposing)
		{
			if (!File.Exists(_path))
				return;

			for (int i = 0; i < 3; ++i)
			{
				try
				{
					File.Delete(_path);
					return;
				}
				catch (IOException)
				{
				}
				catch (UnauthorizedAccessException)
				{
				}

				Thread.Sleep(500);
			}

			// Last attempt, it will propagate exception up to the caller
			File.Delete(_path);
		}
	}
}
