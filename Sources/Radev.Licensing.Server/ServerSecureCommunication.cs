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

namespace Radev.Licensing.Server
{
	/// <summary>
	/// Exposes simplified methods for a secure client-server communication
	/// through encrypted/signed and obfuscated text messages.
	/// </summary>
	public static class ServerSecureCommunication
	{
		/// <summary>
		/// Encode a text message to be sent from server to client.
		/// </summary>
		/// <param name="message">Message to encode.</param>
		/// <returns>
		/// Encoded and signed message. When <paramref name="message"/>
		/// is a textual serialization of an object then this message representation
		/// is called (in this documentation) <em>safe string representation</em>.
		/// </returns>
		/// <exception cref="NotSupportedException">
		/// Always for non <em>private builds</em>.
		/// </exception>
		public static string EncodeMessageFromServer(string message)
		{
			var rawData = Encoding.UTF8.GetBytes(message);
			return JoinMultipart(rawData, CreateSignature(rawData));
		}

		/// <summary>
		/// Decode a message sent from client to server.
		/// </summary>
		/// <param name="content">Message to decode and validate.</param>
		/// <returns>
		/// Decoded message, in plain text.
		/// </returns>
		/// <exception cref="LicenseException">
		/// If message is not valid or it has been tampered.
		/// </exception>
		public static string DecodeMessageFromClient(string content)
		{
			var data = SplitMultipart(content);
			if (data.Count() < 1)
				throw new LicenseException("Data is not valid (too few data sections).");

			var contactData = data.First();

			using (var rsa = RSA.Create())
			{
				rsa.FromXmlString(Keys.PrivateKey); // This will fail for non-private builds
				using (var sa = SymmetricAlgorithm.Create())
				{
					byte[] keyex = new byte[rsa.KeySize >> 3];
					Buffer.BlockCopy(contactData, 0, keyex, 0, keyex.Length);

					var def = new RSAPKCS1KeyExchangeDeformatter(rsa);
					byte[] key = def.DecryptKeyExchange(keyex);

					byte[] iv = new byte[sa.IV.Length];
					Buffer.BlockCopy(contactData, keyex.Length, iv, 0, iv.Length);

					using (var ct = sa.CreateDecryptor(key, iv))
					{
						var decryptedData = ct.TransformFinalBlock(contactData,
							keyex.Length + iv.Length,
							contactData.Length - (keyex.Length + iv.Length));

						try
						{
							return Encoding.UTF8.GetString(decryptedData);
						}
						catch (DecoderFallbackException exception)
						{
							// Encoded sequence is invalid, message has been
							// compromized or encrypted with an invalid public key
							throw new LicenseException("Data is not valid.", exception);
						}
					}
				}
			}
		}

		private static byte[] CreateSignature(byte[] content)
		{
			// You may use conditional compilation to leave out this function
			// (throwing an exception) for non PRIVATE_BUILD builds.
			using (var rsa = RSA.Create())
			{
				rsa.FromXmlString(Keys.PrivateKey); // This will fail for non-private builds
				using (var rcsp = new RSACryptoServiceProvider())
				using (var scp = new SHA1CryptoServiceProvider())
				{
					rcsp.ImportParameters(rsa.ExportParameters(true));
					return rcsp.SignData(content, scp);
				}
			}
		}

		private static string JoinMultipart(byte[] part1, byte[] part2)
		{
			return new StringBuilder()
				.Append(Convert.ToBase64String(part1))
				.Append(PartsSeparator)
				.Append(Convert.ToBase64String(part2))
				.ToString();
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
