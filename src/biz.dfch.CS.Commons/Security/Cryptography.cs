/**
 * Copyright 2014-2016 d-fens GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Diagnostics.Contracts;
using System.Security.Cryptography;
using System.Text;

namespace biz.dfch.CS.Commons.Security
{
    public class Cryptography
    {
        private static readonly UTF8Encoding _utf8 = new UTF8Encoding();

        internal static string Password { get; set; }

        internal Cryptography(string password)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(password));

            Password = password;
        }

        public static string Encrypt(string plainText)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(plainText));
            Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));

            return Encrypt(plainText, Password);
        }

        public static string Encrypt(string plainText, string password)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(plainText));
            Contract.Requires(!string.IsNullOrWhiteSpace(password));
            Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));

            var hashProvider = new SHA256CryptoServiceProvider();
            var algorithm = new AesManaged
            {
                Key = hashProvider.ComputeHash(_utf8.GetBytes(password)),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            try
            {
                using (var encryptor = algorithm.CreateEncryptor())
                {
                    var abData = _utf8.GetBytes(plainText);
                    var abResult = encryptor.TransformFinalBlock(abData, 0, abData.Length);
                    var result = System.Convert.ToBase64String(abResult);
                    return result;
                }
            }
            finally
            {
                algorithm.Clear();
                algorithm.Dispose();

                hashProvider.Clear();
                hashProvider.Dispose();
            }
        }

        public static string Decrypt(string encryptedData)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(encryptedData));
            Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));

            return Decrypt(encryptedData, Password);
        }

        public static string Decrypt(string encryptedData, string password)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(encryptedData));
            Contract.Requires(!string.IsNullOrWhiteSpace(password));
            Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));

            var hashProvider = new SHA256CryptoServiceProvider();
            var algorithm = new AesCryptoServiceProvider
            {
                Key = hashProvider.ComputeHash(_utf8.GetBytes(password)),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            string result;
            try
            {
                var abEncryptedData = System.Convert.FromBase64String(encryptedData);
                using (var decryptor = algorithm.CreateDecryptor())
                {
                    var abResult = decryptor.TransformFinalBlock(abEncryptedData, 0, abEncryptedData.Length);
                    result = _utf8.GetString(abResult);
                }
            }
            catch (Exception)
            {
                result = encryptedData;
            }
            finally
            {
                algorithm.Clear();
                algorithm.Dispose();
                hashProvider.Clear();
                hashProvider.Dispose();
            }

            return result;
        }
    }
}
