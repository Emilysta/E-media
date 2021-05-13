using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SVGReader.RSA
{
    class RSA
    {
        public static void EncryptData(string fileName)
        {
            RSAKey key = RSAKeyGenerator.GenerateKeyPair(1024);
            int byteCount = key.n.GetByteCount(true)-1;
            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            string[] temp = fileName.Split('.');
            string saveFileName = temp[0] + "_encrypted.svg";
            FileStream writeStream = new FileStream(saveFileName, FileMode.Create, FileAccess.Write);
            int fileBytesToRead = (int)stream.Length;
            while (fileBytesToRead > 0)
            {
                byte[] bytes = ReadBytes(stream, byteCount); //przeczytaj n-bytów
                fileBytesToRead -= bytes.Length; //pomiejsz liczbę do przeczytania
                byte[] encryptedBytes = BigInteger.ModPow(new BigInteger(bytes), key.e, key.n).ToByteArray();
                writeStream.WriteAsync(encryptedBytes);
            }
            stream.Close();
            writeStream.Close();
            DecryptData(key, saveFileName);
        }
        public static void DecryptData(RSAKey key,string fileName)
        {
            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            string[] temp = fileName.Split('.');
            string saveFileName = temp[0] + "_decrypted.svg";
            int byteCount = key.n.GetByteCount(true) - 1;
            FileStream writeStream = new FileStream(saveFileName, FileMode.Create, FileAccess.Write);
            int fileBytesToRead = (int)stream.Length;
            while (fileBytesToRead > 0)
            {
                byte[] bytes = ReadBytes(stream, byteCount); //przeczytaj n-bytów
                fileBytesToRead -= bytes.Length; //pomiejsz liczbę do przeczytania
                byte[] encryptedBytes = BigInteger.ModPow(new BigInteger(bytes), key.d, key.n).ToByteArray();
                writeStream.WriteAsync(encryptedBytes);
            }
            stream.Close();
            writeStream.Close();
        }

        public static byte[] ReadBytes(System.IO.Stream stream, int count)
        {
            byte[] buffer = new byte[count];
            int offset = 0;
            while (offset < count)
            {
                int read = stream.Read(buffer, offset, count - offset);
                if (read == 0)
                    throw new System.IO.EndOfStreamException();
                offset += read;
            }
            return buffer;
        }
    }
}
