﻿using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace DataEncryptor.Serialization
{
    public static class Serializer
    {
        public static void SerializeToFile<T>(string fileFullName, T obj, CryptoKey key)
        {
            if (string.IsNullOrEmpty(fileFullName))
                throw new ArgumentException(Resources.FileInfoNull);

            try
            {
                using (var triploDesProvider = new TripleDESCryptoServiceProvider())
                {
                    triploDesProvider.Key = key.KeyBytes;
                    triploDesProvider.IV = key.IVBytes;

                    var encryptor = triploDesProvider.CreateEncryptor(triploDesProvider.Key, triploDesProvider.IV);

                    using (var stream = new FileStream(fileFullName, FileMode.Create))
                    using (var cryptoStream = new CryptoStream(stream, encryptor, CryptoStreamMode.Write))
                    {
                        var formatter = new BinaryFormatter();
                        formatter.Serialize(cryptoStream, obj);
                    }
                }
            }
            catch (CryptographicException ex)
            {
                throw new ApplicationException(Resources.CryptographyError);
            }
            catch
            {
                throw;
            }
        }

        public static T DeserializeFromFile<T>(string fileFullName, CryptoKey key)
        {
            if (string.IsNullOrEmpty(fileFullName))
                throw new ArgumentException(Resources.FileInfoNull);

            try
            {
                using (var triploDesProvider = new TripleDESCryptoServiceProvider())
                {
                    triploDesProvider.Key = key.KeyBytes;
                    triploDesProvider.IV = key.IVBytes;

                    var decryptor = triploDesProvider.CreateDecryptor(triploDesProvider.Key, triploDesProvider.IV);

                    using (var stream = new FileStream(fileFullName, FileMode.Open))
                    using (var cryptoStream = new CryptoStream(stream, decryptor, CryptoStreamMode.Read))
                    {
                        var formatter = new BinaryFormatter();
                        return (T)formatter.Deserialize(cryptoStream);
                    }
                }
            }
            catch (CryptographicException ex)
            {
                throw new ApplicationException(Resources.CryptographyError);
            }
            catch
            {
                throw;
            }
        }
    }
}