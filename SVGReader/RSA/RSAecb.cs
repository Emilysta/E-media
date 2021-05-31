using System;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace SVGReader.RSA
{
    public static class RSAecb
    {
        private static int byteCount = 0;
        public static void DecryptData(string fileName, RSAKey key)
        {
            string[] temp = fileName.Split('.');
            string saveFileName = temp[0] + "_decryptedECB";
            if (temp.Length == 2)
                saveFileName = temp[0] + "_decryptedECB." + temp[1];

            FileStream readStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            FileStream writeStream = new FileStream(saveFileName, FileMode.Create, FileAccess.Write);
            using (StreamWriter writer = new StreamWriter(writeStream))
            {
                using (StreamReader sr = new StreamReader(readStream))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(line);
                        string testowy = Encoding.UTF8.GetString(bytes);

                        BigInteger decryptedFormString = new BigInteger(Convert.FromBase64String(testowy));
                        BigInteger modPowValue = BigInteger.ModPow(decryptedFormString, key.d, key.n);
                        byte[] decryptedData = modPowValue.ToByteArray();
                        writer.Write(Encoding.UTF8.GetString(decryptedData));
                    }
                }
            }
            readStream.Close();
            writeStream.Close();
        }

        public static void DecryptDataWithMicrosoft(string fileName, RSAKey key)
        {
            string[] temp = fileName.Split('.');
            string saveFileName = temp[0] + "_decryptedECB";
            if (temp.Length == 2)
                saveFileName = temp[0] + "_decryptedECB." + temp[1];

            FileStream readStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            FileStream writeStream = new FileStream(saveFileName + "microsoft", FileMode.Create, FileAccess.Write);
            using (StreamWriter writer = new StreamWriter(writeStream))
            {
                using (StreamReader sr = new StreamReader(readStream))
                {
                    using (RSACryptoServiceProvider crypto = new RSACryptoServiceProvider())
                    {
                        RSAParameters rsaKey = new RSAParameters();
                        rsaKey.Modulus = key.n.ToByteArray();
                        rsaKey.Exponent = key.e.ToByteArray();
                        rsaKey.D = key.d.ToByteArray();
                        rsaKey.P = key.p.ToByteArray();
                        rsaKey.Q = key.q.ToByteArray();
                        rsaKey.DQ = (key.d % (key.q - 1)).ToByteArray();
                        rsaKey.DP = (key.d % (key.p - 1)).ToByteArray();
                        rsaKey.InverseQ = RSAKeyGenerator.ModularInverse(key.q, key.p).ToByteArray();
                        crypto.ImportParameters(rsaKey);

                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            byte[] bytes = Encoding.UTF8.GetBytes(line);
                            string testowy = Encoding.UTF8.GetString(bytes);
                            byte[] decryptedData = crypto.Decrypt(Convert.FromBase64String(testowy), RSAEncryptionPadding.Pkcs1);
                            writer.Write(Encoding.UTF8.GetString(decryptedData));
                        }
                    }
                }
            }
            readStream.Close();
            writeStream.Close();
        }

        public static void EncryptData(string fileName, RSAKey key)
        {
            string[] temp = fileName.Split('.');
            string saveFileName = temp[0] + "_encryptedECB";
            if (temp.Length == 2)
                saveFileName = temp[0] + "_encryptedECB." + temp[1];

            SetByteCount(key);

            FileStream readStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            FileStream writeStream = new FileStream(saveFileName, FileMode.Create, FileAccess.Write);
            FileStream writeStream2 = new FileStream(saveFileName + "microsoft", FileMode.Create, FileAccess.Write);


            int fileBytesToRead = (int)readStream.Length;

            using (StreamWriter writer = new StreamWriter(writeStream))
            {
                using (StreamWriter writer2 = new StreamWriter(writeStream2))
                {
                    using (RSACryptoServiceProvider crypto = new RSACryptoServiceProvider())
                    {
                        RSAParameters rsaKey = new RSAParameters();
                        rsaKey.Modulus = key.n.ToByteArray();
                        rsaKey.Exponent = key.e.ToByteArray();
                        crypto.ImportParameters(rsaKey);

                        while (fileBytesToRead > 0)
                        {
                            byte[] bytes = new byte[byteCount];
                            int readCount = readStream.Read(bytes, 0, byteCount);//przeczytaj n-bytów
                            fileBytesToRead -= readCount; //pomiejsz liczbę do przeczytania

                            BigInteger numberFromBytes = new BigInteger(bytes);
                            BigInteger encryptedData = BigInteger.ModPow(numberFromBytes, key.e, key.n);
                            byte[] bytesToSave = encryptedData.ToByteArray();
                            if (bytesToSave.Length != key.n.ToByteArray().Length)
                                bytesToSave = new byte[key.n.ToByteArray().Length - bytesToSave.Length].Concat(bytesToSave).ToArray();
                            string base64 = Convert.ToBase64String(bytesToSave);
                            writer.Write(base64 + "\n");

                            bytesToSave = crypto.Encrypt(bytes, RSAEncryptionPadding.Pkcs1);
                            base64 = Convert.ToBase64String(bytesToSave);
                            writer2.Write(base64 + "\n");
                        }
                    }
                }
            }
            readStream.Close();
            writeStream.Close();
            writeStream2.Close();
        }

        private static void SetByteCount(RSAKey key)
        {
            var rand = new Random();
            byteCount = rand.Next(25, key.n.GetByteCount() - 1);
        }
    }
}