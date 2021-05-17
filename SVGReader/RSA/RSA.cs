using System;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Text;

namespace SVGReader.RSA
{
    public static class RSA
    {
        public static void EncryptData(string fileName, RSAKey key)
        {
            string[] temp = fileName.Split('.');
            string saveFileName = temp[0] + "_encrypted.svg";

            FileStream readStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            FileStream writeStream = new FileStream(saveFileName, FileMode.Create, FileAccess.Write);

            int fileBytesToRead = (int)readStream.Length;
            int byteCount = 4;

            using (StreamWriter writer = new StreamWriter(writeStream))
            {
                while (fileBytesToRead > 0)
                {
                    byte[] bytes = new byte[byteCount];
                    int readCount = readStream.Read(bytes, 0, byteCount);//przeczytaj n-bytów
                    fileBytesToRead -= readCount; //pomiejsz liczbę do przeczytania
                    BigInteger temp3 = new BigInteger(bytes);
                    BigInteger encryptedData = BigInteger.ModPow(temp3, key.e, key.n);
                    //writeStream.Write(encryptedData.ToByteArray());
                    byte[] bytesToSave = encryptedData.ToByteArray();
                    string base64 = Convert.ToBase64String(bytesToSave);
                    writer.Write(base64 + "\n");
                }
                writer.Close();
            }
            readStream.Close();
            writeStream.Close();
            DecryptData(saveFileName, key);
        }
        public static void DecryptData(string fileName, RSAKey key)
        {
            string[] temp = fileName.Split('.');
            string saveFileName = temp[0] + "_decrypted.svg";

            FileStream readStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            FileStream writeStream = new FileStream(saveFileName, FileMode.Create, FileAccess.Write);

            int byteCount = 4;
            int fileBytesToRead = (int)readStream.Length;
            using (StreamWriter writer = new StreamWriter(writeStream))
            {
                using (StreamReader sr = new StreamReader(readStream))
                {
                    //while (fileBytesToRead > 0)
                    {
                        string line;
                        while ((line = sr.ReadLine()) != null)
                        {
                            byte[] bytes = new byte[byteCount];
                            int readCount = line.Length;
                            fileBytesToRead -= readCount;
                            bytes = Encoding.UTF8.GetBytes(line);
                            string testowy = Encoding.UTF8.GetString(bytes);

                            BigInteger decryptedFormString = new BigInteger(Convert.FromBase64String(testowy));
                            BigInteger temp2 = BigInteger.ModPow(decryptedFormString, key.d, key.n);
                            byte[] decryptedData = temp2.ToByteArray();
                            //Encoding.UTF8.ToString()
                            writer.Write(Encoding.UTF8.GetString(decryptedData));
                        }
                    }
                }
            }
            readStream.Close();
            writeStream.Close();
        }
    }
}
