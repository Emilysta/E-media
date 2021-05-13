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
    public class RSAKey
    {
        public BigInteger n;
        public BigInteger e;
        public BigInteger d;
    }
    class RSAKeyGenerator
    {
        private static List<uint> m_validKeyLengths = new List<uint>() { 256, 512, 1024, 2048, 4096 };
        public static RSAKey GenerateKeyPair(uint keyLength)
        {
            if (!m_validKeyLengths.Any(x => x == keyLength))
                throw new Exception("Key length is not supported.\nSupported Lengths:256,512,1024,2048,4096");

            uint offset = 1;
            uint length1 = (keyLength / 8) / 2 + offset;
            uint length2 = (keyLength / 8) / 2 - offset;

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
                    isPrime = await p.IsPrime(new RobinMillerTest(200));
                }
            });
            thread1.Start();
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
            });
            thread2.Start();
            thread1.Join();
            thread2.Join();

            BigInteger n = p * q;
            BigInteger h = LeastCommonMultiple(p - 1, q - 1);
            BigInteger e = 65537;// GenerateEValue(n, p, q);
            BigInteger d = ModularInverse(e, (p - 1) * (q - 1));

            Trace.WriteLine("p=" + p);
            Trace.WriteLine("q=" + q);
            Trace.WriteLine("n=" + n);
            Trace.WriteLine("e=" + e);
            Trace.WriteLine("d=" + d);
            return new RSAKey
            {
                e = e,
                d = d,
                n = n
            };

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

        private static BigInteger GreatestCommonDivisor(BigInteger value1, BigInteger value2)
        {
            while (value1 != value2)
            {
                if (value1 > value2)
                    value1 -= value2;
                else
                    value2 -= value1;
            }
            return value1;
        }

        private static BigInteger ModularInverse(BigInteger value, BigInteger modulus)
        {
            BigInteger inv, u1, u3, v1, v3, t1, t3, q, iter;
            /* Step X1. Initialise */
            u1 = 1;
            u3 = value;
            v1 = 0;
            v3 = modulus;
            /* Remember odd/even iterations */
            iter = 1;
            /* Step X2. Loop while v3 != 0 */
            while (v3 != 0)
            {
                /* Step X3. Divide and "Subtract" */
                q = u3 / v3;
                t3 = u3 % v3;
                t1 = u1 + q * v1;
                /* Swap */
                u1 = v1; v1 = t1; u3 = v3; v3 = t3;
                iter = -iter;
            }
            /* Make sure u3 = gcd(u,v) == 1 */
            if (u3 != 1)
                return 0;   /* Error: No inverse exists */
            /* Ensure a positive result */
            if (iter < 0)
                inv = modulus - u1;
            else
                inv = u1;
            return inv;
        }
    }
}
