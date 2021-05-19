using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Text;
using System.Linq;

namespace SVGReader.RSA
{
    public static class RSAcbc
    {

        private static BitArray m_initializationVector;
        private static int byteCount=0;

        public static void DecryptData(string fileName, RSAKey key)
        {
            string[] temp = fileName.Split('.');
            string saveFileName = temp[0] + "_decryptedCBC";
            if (temp.Length == 2)
                saveFileName = temp[0] + "_decryptedCBC." + temp[1];

            FileStream readStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            FileStream writeStream = new FileStream(saveFileName, FileMode.Create, FileAccess.Write);
            int fileBytesToRead = (int)readStream.Length;
            BitArray CBC_seed = m_initializationVector;

            using (StreamWriter writer = new StreamWriter(writeStream))
            {
                using (StreamReader sr = new StreamReader(readStream))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        byte[] bytes;
                        int readCount = line.Length;
                        fileBytesToRead -= readCount;
                        bytes = Encoding.UTF8.GetBytes(line);
                        string testowy = Encoding.UTF8.GetString(bytes);

                        BigInteger decryptedFormString = new BigInteger(Convert.FromBase64String(testowy));
                        BigInteger modPowValue = BigInteger.ModPow(decryptedFormString, key.d, key.n);
                        BitArray decryptedData = new BitArray(modPowValue.ToByteArray());
                        if (decryptedData.Length != CBC_seed.Length)
                        {
                            BitArray seedCopy = new BitArray(CBC_seed);
                            CBC_seed = new BitArray(decryptedData.Length);
                            for (int i = 0; i < decryptedData.Length; i++)
                            {
                                CBC_seed.Set(i, seedCopy.Get(i));
                            }
                        }
                        decryptedData.Xor(CBC_seed);
                        bytes = new byte[decryptedData.Length / 8];
                        decryptedData.CopyTo(bytes, 0);
                        CBC_seed = new BitArray(Convert.FromBase64String(testowy));
                        if(bytes[bytes.Length-1] == 0)
                        {
                            int lastNonZeroIndex = Array.FindLastIndex(bytes, x => x != 0);
                            Array.Resize(ref bytes, lastNonZeroIndex + 1);
                        }
                        writer.Write(Encoding.UTF8.GetString(bytes));
                    }
                }
            }
            readStream.Close();
            writeStream.Close();
        }

        public static void EncryptData(string fileName, RSAKey key)
        {
            string[] temp = fileName.Split('.');
            string saveFileName = temp[0] + "_encryptedCBC";
            if (temp.Length == 2)
                saveFileName = temp[0] + "_encryptedCBC." + temp[1];
            SetByteCount(key);

            FileStream readStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            FileStream writeStream = new FileStream(saveFileName, FileMode.Create, FileAccess.Write);

            int fileBytesToRead = (int)readStream.Length;
            m_initializationVector = GenerateInitializationVector((uint)byteCount);
            BitArray CBC_seed = m_initializationVector;
            using (StreamWriter writer = new StreamWriter(writeStream))
            {
                while (fileBytesToRead > 0)
                {
                    byte[] bytes = new byte[byteCount];
                    int readCount = readStream.Read(bytes, 0, byteCount);//przeczytaj n-bytów
                    fileBytesToRead -= readCount; //pomiejsz liczbę do przeczytania
                    BitArray array = new BitArray(bytes);
                    if (CBC_seed.Length != array.Length)
                    {
                        BitArray seedCopy = new BitArray(CBC_seed);
                        CBC_seed = new BitArray(array.Length);
                        for (int i = 0; i < array.Length; i++)
                        {
                            CBC_seed.Set(i, seedCopy.Get(i));
                        }
                    }
                    array.Xor(CBC_seed);
                    array.CopyTo(bytes, 0);
                    BigInteger numberFromBytes = new BigInteger(bytes);
                    BigInteger encryptedData = BigInteger.ModPow(numberFromBytes, key.e, key.n);
                    byte[] bytesToSave = encryptedData.ToByteArray();
                    CBC_seed = new BitArray(bytesToSave);
                    string base64 = Convert.ToBase64String(bytesToSave);
                    writer.Write(base64 + "\n");
                }
                writer.Close();
            }
            readStream.Close();
            writeStream.Close();
        }
        private static BitArray GenerateInitializationVector(uint bytesCount)
        {
            BitArray arr = new BitArray(RSAKeyGenerator.GenerateRandomBigInteger(bytesCount));
            return arr;
        }

        private static void SetByteCount(RSAKey key)
        {
            var rand = new Random();
            byteCount = rand.Next(25, key.n.GetByteCount() - 1);
        }

    }
}
