using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;

namespace SVGReader.RSA
{
    class RSA
    {
        public static void EncryptData(string fileName, RSAKey key)
        {
            int byteCount = 16;
            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            string[] temp = fileName.Split('.');
            string saveFileName = temp[0] + "_encrypted.svg";
            FileStream writeStream = new FileStream(saveFileName, FileMode.Create, FileAccess.Write);
            int fileBytesToRead = (int)stream.Length;
            key.e = 11;
            key.n = 701111;
            key.d = 254339;
            BigInteger test = BigInteger.ModPow(4,key.e,key.n);
            Trace.WriteLine("m=" + 4);
            Trace.WriteLine("c="+test);
            while (fileBytesToRead > 0)
            {
                byte[] bytes = new byte[byteCount];
                int readCount = stream.Read(bytes, 0, byteCount);//ReadBytes(stream, byteCount); //przeczytaj n-bytów
                fileBytesToRead -= readCount; //pomiejsz liczbę do przeczytania
                byte[] encryptedBytes = BigInteger.ModPow(new BigInteger(bytes), key.e, key.n).ToByteArray();
                writeStream.Write(encryptedBytes);
            }
            stream.Close();
            writeStream.Close();
            DecryptData(saveFileName, key,test);
        }
        public static void DecryptData(string fileName, RSAKey key,BigInteger test)
        {
            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            string[] temp = fileName.Split('.');
            string saveFileName = temp[0] + "_decrypted.svg";
            int byteCount = 16;
            FileStream writeStream = new FileStream(saveFileName, FileMode.Create, FileAccess.Write);
            int fileBytesToRead = (int)stream.Length;
            Trace.WriteLine("m="+BigInteger.ModPow(test,key.d,key.n));
            while (fileBytesToRead > 0)
            {
                byte[] bytes = new byte[byteCount];
                int readCount = stream.Read(bytes, 0, byteCount);//ReadBytes(stream, byteCount); //przeczytaj n-bytów
                fileBytesToRead -= readCount; //pomiejsz liczbę do przeczytania
                byte[] encryptedBytes = BigInteger.ModPow(new BigInteger(bytes), key.d, key.n).ToByteArray();
                writeStream.Write(encryptedBytes);
            }
            stream.Close();
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
    }
}
