using BigPrimeNumber;
using BigPrimeNumber.Primality.Heuristic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Threading;

namespace SVGReader.RSA
{
    class RSAKeyGenerator
    {
        private static List<uint> m_validKeyLengths = new List<uint>() { 256, 512, 1024, 2048, 4096 };
        public static RSAKey GenerateKeyPair(uint keyLength)
        {
            if (!m_validKeyLengths.Any(x => x == keyLength))
                throw new Exception("Key length is not supported.\nSupported Lengths:256,512,1024,2048,4096");

            uint offset = 1;
            uint length1 = (keyLength / 8) / 2;
            uint length2 = (keyLength / 8) - length1;

            BigInteger p = new BigInteger(0);
            BigInteger q = new BigInteger(0);


            Thread thread1 = new Thread(() =>
            {
                var isPrime = false;
                while (!isPrime)
                {
                    p = new BigInteger(GenerateRandomBigInteger(length1));
                    if (p.Sign == -1)
                        p = -p;
                    isPrime = p.IsPrime(new RobinMillerTest(20000)).Result;
                }
                Trace.WriteLine("Watek p koniec");
            });
            thread1.Start();
            Thread thread2 = new Thread(() =>
            {
                var isPrime = false;
                while (!isPrime)
                {
                    q = new BigInteger(GenerateRandomBigInteger(length2));
                    if (q.Sign == -1)
                        q = -q;
                    isPrime = q.IsPrime(new RobinMillerTest(20000)).Result;
                }
                Trace.WriteLine("Watek q koniec");
            });
            thread2.Start();
            thread1.Join();
            thread2.Join();

            BigInteger n = p * q;
            BigInteger h = (p - 1) * (q - 1);
            BigInteger e = 65537;
            BigInteger d = ModularInverse(e, (p - 1) * (q - 1));

            Trace.WriteLine("p=" + p.ToString());
            Trace.WriteLine("q=" + q.ToString());
            Trace.WriteLine("n=" + n.ToString());
            Trace.WriteLine("e=" + e.ToString());
            Trace.WriteLine("d=" + d.ToString());
            RSAKey key = new RSAKey
            {
                n = n,
                e = e,
                d = d,
                p = p,
                q = q
            };
            try
            {
                key.SaveConfig();
            }
            catch
            {
                key = GenerateKeyPair(keyLength);
            }
            return key;
        }

        public static byte[] GenerateRandomBigInteger(uint bytesLength)
        {
            byte[] bytes = new byte[bytesLength];
            do
            {
                using (RandomNumberGenerator rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(bytes);
                }
            } while (bytes[0] == 0);
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

        public static BigInteger ModularInverse(BigInteger value, BigInteger modulus)
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

        private static bool IsPrime(BigInteger value)
        {
            if (value == 2 || value == 3)
                return true;
            else if (value <= 1 || (value % 2) == 0 || value % 3 == 0)
                return false;

            BigInteger i = 5;
            while ((i ^ 2) <= value)
            {
                if ((value % i) == 0 || (value % (i + 2) == 0))
                    return false;
                i += 6;
            }
            return true;
        }
    }
}
