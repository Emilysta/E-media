using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace SVGReader.RSA
{
    class RSA
    {
        public static void EncryptData(string fileName, RSAKey key)
        {
            int byteCount = 4;
            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            string[] temp = fileName.Split('.');
            string saveFileName = temp[0] + "_encrypted.svg";
            FileStream writeStream = new FileStream(saveFileName, FileMode.Create, FileAccess.Write);
            int fileBytesToRead = (int)stream.Length;
            key.e = 11;
            key.n = 701111;
            key.d = 254339;
            byte[] bytesTest = new byte[2];
            bytesTest[0] = 60;
            bytesTest[1] = 63;
            BigInteger temp2 = new BigInteger(bytesTest);
            BigInteger c = BigInteger.ModPow(temp2, key.e, key.n);
            
            Trace.WriteLine("m=" + Encoding.UTF8.GetString(bytesTest));
            Trace.WriteLine("c="+ c);
            BigInteger r = BigInteger.ModPow(c, key.d, key.n);
            Trace.WriteLine("m=" + Encoding.UTF8.GetString(r.ToByteArray()));
            StreamWriter writer = new StreamWriter(writeStream, new UTF8Encoding(false));
            while (fileBytesToRead > 0)
            {
                byte[] bytes = new byte[byteCount];
                int readCount = stream.Read(bytes, 0, byteCount);//przeczytaj n-bytów
                fileBytesToRead -= readCount; //pomiejsz liczbę do przeczytania
                BigInteger temp3 = new BigInteger(bytes);
                BigInteger encryptedData = BigInteger.ModPow(temp3, key.e, key.n);
                //writeStream.Write(encryptedData.ToByteArray());
                byte[] bytesToSave = encryptedData.ToByteArray();
                string base64 = Convert.ToBase64String(bytesToSave);
                writer.Write(base64);
            }
            stream.Close();
            writer.Close();
            writeStream.Close();
            DecryptData(saveFileName, key);
        }
        public static void DecryptData(string fileName, RSAKey key)
        {
            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            string[] temp = fileName.Split('.');
            string saveFileName = temp[0] + "_decrypted.svg";
            int byteCount = 4;
            FileStream writeStream = new FileStream(saveFileName, FileMode.Create, FileAccess.Write);
            BinaryWriter writer = new BinaryWriter(writeStream, Encoding.UTF8);
            int fileBytesToRead = (int)stream.Length;
            while (fileBytesToRead > 0)
            {
                byte[] bytes = new byte[byteCount];
                int readCount = stream.Read(bytes, 0, byteCount);//ReadBytes(stream, byteCount); //przeczytaj n-bytów
                fileBytesToRead -= readCount; //pomiejsz liczbę do przeczytania
                //Array.Reverse(bytes);
                string testowy = Encoding.UTF8.GetString(bytes);
                
                BigInteger decryptedFormString = new BigInteger(Convert.FromBase64String(testowy));
                BigInteger temp2 = BigInteger.ModPow(decryptedFormString, key.d, key.n);
                byte[] decryptedData = temp2.ToByteArray();
                //Encoding.UTF8.ToString()
                writer.Write(decryptedData);
            }
            stream.Close();
            writer.Close();
            writeStream.Close();
        }

        public static byte[] ReadBytes(Stream stream, int count)
        {
            byte[] buffer = new byte[count];
            int offset = 0;
            while (offset < count)
            {
                int read = stream.Read(buffer, offset, count - offset);
                if (read == 0)
                    throw new EndOfStreamException();
                offset += read;
            }
            return buffer;
        }
        private static byte[] addPadding(byte[] array,int length)
        {
            byte[] newArray = new byte[length];
            Array.Copy(array, 0, newArray, length - array.Length, array.Length);
            return newArray;
        }

    }
}
