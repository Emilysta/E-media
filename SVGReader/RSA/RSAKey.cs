using System.Diagnostics;
using System.IO;
using System.Numerics;

namespace SVGReader.RSA
{
    public class RSAKey
    {
        public BigInteger d = 0;
        public BigInteger e = 0;
        public BigInteger n = 0;
        public BigInteger p = 0;
        public BigInteger q = 0;
        public bool ReadConfig(string fileName)
        {
            try
            {
                FileStream stream = new FileStream(fileName, FileMode.Open);
                using (StreamReader sr = new StreamReader(stream))
                {
                    string line;
                    do
                    {
                        line = sr.ReadLine();
                    } while (!line.StartsWith("modulus"));
                    n = BigInteger.Parse(line.Split(':')[1]);
                    line = sr.ReadLine();
                    e = BigInteger.Parse(line.Split(':')[1]);
                    line = sr.ReadLine();
                    d = BigInteger.Parse(line.Split(':')[1]);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        public void SaveConfig()
        {
            FileStream stream = new FileStream("config.rsa", FileMode.OpenOrCreate);
            using (StreamWriter sw = new StreamWriter(stream))
            {
                sw.WriteLine("asn1 = SEQUENCE:rsa_key");
                sw.WriteLine("");
                sw.WriteLine("[rsa_key]");
                sw.WriteLine("version=INTEGER:0");
                sw.WriteLine($"modulus=INTEGER:{n}");
                sw.WriteLine($"pubExp=INTEGER:{e}");
                sw.WriteLine($"privExp=INTEGER:{d}");
                sw.WriteLine($"p=INTEGER:{p}");
                sw.WriteLine($"q=INTEGER:{q}");
                sw.WriteLine($"e1=INTEGER:{d % (p - 1)}");
                sw.WriteLine($"e2=INTEGER:{d % (q - 1)}");
                sw.WriteLine($"coeff=INTEGER:{RSAKeyGenerator.ModularInverse(q, p)}");
            }
            string cmdText = $"/C openssl asn1parse -genconf config.rsa -out newkey.der";
            Process.Start("CMD.exe", cmdText);
            Process proc = new Process();
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.FileName = "CMD.exe";
            proc.StartInfo.Arguments = "/C openssl rsa -in newkey.der -inform der -text -check";
            proc.EnableRaisingEvents = true;
            proc.OutputDataReceived += Proc_OutputDataReceived;
            proc.Start();
            proc.BeginOutputReadLine();
            proc.WaitForExit();
            cmdText = $"/C openssl rsa -in newkey.der -inform DER -outform PEM -out id_rsa";
            proc = Process.Start("CMD.exe", cmdText);
            proc.WaitForExit();
            cmdText = $"/C openssl rsa -in id_rsa -pubout > id_rsa.pub";
            Process.Start("CMD.exe", cmdText);
        }

        private void Proc_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                Trace.WriteLine(e.Data);
                if (e.Data.Contains("RSA key error"))
                    throw new System.Exception("RSA Key is not valid!");
            }
        }
    }
}
