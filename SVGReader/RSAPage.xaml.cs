using Microsoft.Win32;
using SVGReader.RSA;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SVGReader
{
    public partial class RSAPage : Page
    {
        public string PrivateKey { get; set; }
        public string PublicKey { get; set; }
        private RSAKey RsaKey;

        private uint[] keyLengths = new uint[] { 256, 512, 1024, 2048, 4096 };
        public RSAPage()
        {
            InitializeComponent();
            CreateNewKey();
            DataContext = this;
        }

        private void CreateNewKey()
        {
            RsaKey = new RSAKey();
            if (!RsaKey.ReadConfig("config.rsa"))
                RsaKey = RSAKeyGenerator.GenerateKeyPair(1024);
            ReadKeysFromFile();
        }

        private void ReadKeysFromFile()
        {
            try
            {
                FileStream file = new FileStream("id_rsa", FileMode.Open);
                using (StreamReader sr = new StreamReader(file))
                {
                    PrivateKey = sr.ReadToEnd();
                }
            }
            catch
            {
                PrivateKey = "Error";
                SetActiveAllButton(false);
            }

            try
            {
                FileStream file = new FileStream("id_rsa.pub", FileMode.Open);
                using (StreamReader sr = new StreamReader(file))
                {
                    PublicKey = sr.ReadToEnd();
                }
            }
            catch
            {
                PublicKey = "Error";
                SetActiveAllButton(false);
            }
            SetActiveAllButton(true);
        }

        private void SetActiveAllButton(bool state)
        {
            Button1.IsEnabled = state;
            Button2.IsEnabled = state;
            Button3.IsEnabled = state;
            Button4.IsEnabled = state;
        }

        private bool OpenFile(out string fileName)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter += "Scalable Vector Graphics|*.svg|All files (*.*)|*.*";
            if (dialog.ShowDialog() ?? false && RsaKey.isValid())
            {
                fileName = dialog.FileName;
                return true;
            }
            fileName = "";
            return false;
        }

        private void GenerateKey_Handler(object sender, System.Windows.RoutedEventArgs e)
        {
            RsaKey = RSAKeyGenerator.GenerateKeyPair(keyLengths[KeyLengthBox.SelectedIndex]);
            ReadKeysFromFile();
        }

        private void DecryptButton_Handler(object sender, System.Windows.RoutedEventArgs e)
        {
            if (OpenFile(out string fileName))
            {
                RSAecb.DecryptData(fileName, RsaKey);
                string message = "Odszyfrowano plik: "+fileName;
                string caption = "Informacja";

                var result = MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        private void DecryptButtonCBC_Handler(object sender, System.Windows.RoutedEventArgs e)
        {
            if (OpenFile(out string fileName))
            {
                RSAcbc.DecryptData(fileName, RsaKey);
                string message = "Odszyfrowano plik: " + fileName;
                string caption = "Informacja";

                var result = MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
            }

        }

        private void EncryptButton_Handler(object sender, System.Windows.RoutedEventArgs e)
        {
            if (OpenFile(out string fileName))
            {
                RSAecb.EncryptData(fileName, RsaKey);
                string message = "Zaszyfrowano plik: " + fileName;
                string caption = "Informacja";

                var result = MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void EncryptButtonCBC_Handler(object sender, System.Windows.RoutedEventArgs e)
        {
            if (OpenFile(out string fileName))
            {
                RSAcbc.EncryptData(fileName, RsaKey);
                string message = "Zaszyfrowano plik: " + fileName;
                string caption = "Informacja";

                var result = MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
