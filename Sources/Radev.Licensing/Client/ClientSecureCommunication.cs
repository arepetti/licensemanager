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
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Radev.Licensing.Client
{
	/// <summary>
	/// Exposes simplified methods for a secure client-server communication
	/// through encrypted/signed and obfuscated text messages.
	/// </summary>
	public static class ClientSecureCommunication
	{
		/// <summary>
		/// Decode a message sent from server to client.
		/// </summary>
		/// <param name="content">Message to decode and validate.</param>
		/// <returns>
		/// Decoded message, in plain text.
		/// </returns>
		/// <exception cref="LicenseException">
		/// If message is not valid or it has been tampered.
		/// </exception>
		public static string DecodeMessageFromServer(string content)
		{
			var dataSections = SplitMultipart(content).ToArray();
			if (dataSections.Length < 2)
				throw new LicenseException("Data is not valid (too few data sections).");

			var data = dataSections[0];
			var signature = dataSections[1];

			using (var rsa = RSA.Create())
			{
				rsa.FromXmlString(Keys.PublicKey);

				using (var rcsp = new RSACryptoServiceProvider())
				using (var scp = new SHA1CryptoServiceProvider())
				{
					rcsp.ImportParameters(rsa.ExportParameters(false));
					if (!rcsp.VerifyData(data, scp, signature))
						throw new LicenseException("Data is not valid (corrupted or changed).");
				}

				try
				{
					return Encoding.UTF8.GetString(data);
				}
				catch (DecoderFallbackException exception)
				{
					// Encoded sequence is invalid, message has been
					// compromized or encrypted with an invalid public key
					throw new LicenseException("Data is not valid.", exception);
				}
			}
		}

		/// <summary>
		/// Encode a text message to be sent from client to server.
		/// </summary>
		/// <param name="message">Message to encode.</param>
		/// <returns>
		/// Encoded and signed message. When <paramref name="message"/>
		/// is a textual serialization of an object then this message representation
		/// is called (in this documentation) <em>safe string representation</em>.
		/// </returns>
		public static string EncodeMessageFromClient(string content)
		{
			using (var rsa = RSA.Create())
			{
				rsa.FromXmlString(Keys.PublicKey);
				using (var algorithm = SymmetricAlgorithm.Create())
				using (var transform = algorithm.CreateEncryptor())
				{
					var unencrypted = Encoding.UTF8.GetBytes(content);
					byte[] encrypted = transform.TransformFinalBlock(unencrypted, 0, unencrypted.Length);

					var fmt = new RSAPKCS1KeyExchangeFormatter(rsa);
					byte[] keyex = fmt.CreateKeyExchange(algorithm.Key);

					byte[] result = new byte[keyex.Length + algorithm.IV.Length + encrypted.Length];
					Buffer.BlockCopy(keyex, 0, result, 0, keyex.Length);
					Buffer.BlockCopy(algorithm.IV, 0, result, keyex.Length, algorithm.IV.Length);
					Buffer.BlockCopy(encrypted, 0, result, keyex.Length + algorithm.IV.Length, encrypted.Length);

					return Convert.ToBase64String(result);
				}
			}
		}

		private static IEnumerable<byte[]> SplitMultipart(string text)
		{
			try
			{
				// I call ToArray() to materialize results here and
				// catch FormatException, if deferred it'll be wrapped
				// in TargetInvocationException and it has to be catched elsewhere.
				return text.Split(PartsSeparator)
					.Select(x => Convert.FromBase64String(x))
					.ToArray();
			}
			catch (FormatException exception)
			{
				throw new LicenseException("Invalid data token.", exception);
			}
		}

		private const char PartsSeparator = '.';
	}
}
