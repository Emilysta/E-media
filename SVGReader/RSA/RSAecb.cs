using System;
using System.IO;
using System.Numerics;
using System.Text;

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

        public static void EncryptData(string fileName, RSAKey key)
        {
            string[] temp = fileName.Split('.');
            string saveFileName = temp[0] + "_encryptedECB";
            if (temp.Length == 2)
                saveFileName = temp[0] + "_encryptedECB." + temp[1];

            SetByteCount(key);

            FileStream readStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            FileStream writeStream = new FileStream(saveFileName, FileMode.Create, FileAccess.Write);

            int fileBytesToRead = (int)readStream.Length;

            using (StreamWriter writer = new StreamWriter(writeStream))
            {
                while (fileBytesToRead > 0)
                {
                    byte[] bytes = new byte[byteCount];
                    int readCount = readStream.Read(bytes, 0, byteCount);//przeczytaj n-bytów
                    fileBytesToRead -= readCount; //pomiejsz liczbę do przeczytania
                    BigInteger numberFromBytes = new BigInteger(bytes);
                    BigInteger encryptedData = BigInteger.ModPow(numberFromBytes, key.e, key.n);
                    byte[] bytesToSave = encryptedData.ToByteArray();
                    string base64 = Convert.ToBase64String(bytesToSave);
                    writer.Write(base64 + "\n");
                }
                writer.Close();
            }
            readStream.Close();
            writeStream.Close();
        }

        private static void SetByteCount(RSAKey key)
        { 
            var rand = new Random();
            byteCount = rand.Next(25, key.n.GetByteCount() - 1);
        }
    }
}