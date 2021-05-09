using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Threading;
using BigPrimeNumber;
using BigPrimeNumber.Primality.Heuristic;

namespace SVGReader.RSA
{
    public class RSAKeyPair
    {
        public string privateKey;
        public string publicKey;
    }
    class RSAKeyGenerator
    {
        private static List<uint> m_validKeyLengths = new List<uint>() { 512, 1024, 2048, 4096 };
        public static async void GenerateKeyPair(uint keyLength)
        {
            if (!m_validKeyLengths.Any(x => x == keyLength))
                throw new Exception("Key length is not supported.\nSupported Lengths: 512,1024,2048,4096");

            uint offset = keyLength / 64;
            uint length1 =  (keyLength/8) / 2 + offset;
            uint length2 = (keyLength/8) / 2 - offset;

            BigInteger p = new BigInteger();
            BigInteger q = new BigInteger();


            Thread thread1 = new Thread(async () =>
            {
                var isPrime = false;
                while (!isPrime)
                {
                    p = new BigInteger(GenerateRandomBigInteger(length1));
                    if (p.Sign == -1)
                        p = -p;
                    isPrime = await p.IsPrime(new RobinMillerTest(50));
                }
                Trace.WriteLine(p);
            });

            Thread thread2 = new Thread(async () =>
            {
                var isPrime = false;
                while (!isPrime)
                {
                    q = new BigInteger(GenerateRandomBigInteger(length2));
                    if (q.Sign == -1)
                        q = -q;
                    isPrime = await q.IsPrime(new RobinMillerTest(50));
                }
                Trace.WriteLine(q);
            });

            thread1.Start();
            thread2.Start();

            thread1.Join();
            thread2.Join();
            p = new BigInteger(1123);
            q = new BigInteger(1237);

            BigInteger n = p * q;
            BigInteger h = LeastCommonMultiple(p - 1, q - 1);
            BigInteger e = GenerateEValue(n,p,q);
            BigInteger d = BigInteger.ModPow(1/e, 1, h);
            Trace.WriteLine(d);

        }

        private static BigInteger GenerateEValue(BigInteger n, BigInteger p, BigInteger q)
        {
            while (true)
            {
                byte[] bytes = new byte[n.GetByteCount(true)];
                using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(bytes);
                }
                BigInteger dupa = new BigInteger(bytes);
                if (dupa.Sign == -1)
                    dupa = -dupa;
                if (dupa < n && dupa > 1)
                    if (GreatestCommonDivisor(dupa, (p - 1) * (q - 1)) == 1)
                        return dupa;
            }
        }

        private static byte[] GenerateRandomBigInteger(uint bytesLength)
        {
            byte[] bytes = new byte[bytesLength];
            using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }
            return bytes;
        }

        private static BigInteger LeastCommonMultiple(BigInteger value1, BigInteger value2)
        {
            return (value1 * value2) / GreatestCommonDivisor(value1, value2);
        }

        private static BigInteger GreatestCommonDivisor(BigInteger value11, BigInteger value22)
        {
            var value1 = value11;
            var value2 = value22;
            while (value1 != value2)
            {
                if (value1 > value2)
                    value1 -= value2;
                else
                    value2 -= value1;
            }
            return value1;
        }
    }
}
