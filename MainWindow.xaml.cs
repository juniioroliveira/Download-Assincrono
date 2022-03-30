using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DownloadArquivos
{
    public MainWindow()
        {
            InitializeComponent();

            spLocal.IsEnabled = true;
            tFile.Focus();

        }
        string fileName;        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.ShowDialog();

            fileName = open.SafeFileName;
            tFile.Text = open.FileName;

        }

        private void RadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            spLocal.IsEnabled = false;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (spLocal != null)
            {
                spLocal.IsEnabled = true;
                tFile.Focus();
                tWeb.Clear();
            }

        }

        private void RadioButton_Checked_1(object sender, RoutedEventArgs e)
        {
            if (spWeb != null)
            {
                spWeb.IsEnabled = true;
                tWeb.Focus();
                tFile.Clear();
            }
        }

        private void RadioButton_Unchecked_1(object sender, RoutedEventArgs e)
        {
            spWeb.IsEnabled = false;
        }

        public void Start(string textBox)
        {
            Thread thread = new Thread(() => {
                WebClient client = new WebClient();
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);

                //https://docs.google.com/uc?export=download&id=19vkiH9m42eHcGXd1FFfLpUhcdfaNGxG3
                if(!Directory.Exists(Directory.GetCurrentDirectory() + @"\Downloaded"))
                {
                    Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Downloaded");
                }

                client.DownloadFileAsync(new Uri(textBox), Directory.GetCurrentDirectory() + @"\Downloaded\" + fileName);
                
            });
            thread.Start();
        }
        decimal mbDown;
        decimal mbTotal;
        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = bytesIn / totalBytes * 100;
                mbDown = e.BytesReceived / 1000000;
                mbTotal = e.TotalBytesToReceive / 1000000;

                label2.Content = "Download " + mbDown.ToString("n0") + " mb de " + mbTotal.ToString("n0") + " mb";
                progressBar1.Value = int.Parse(Math.Truncate(percentage).ToString());

                label1.Content = progressBar1.Value.ToString() + "%";

                if (progressBar1.Value == 100)
                {
                    MessageBox.Show("Download concluido com sucesso!");
                    label1.Content = "";
                    progressBar1.Value = 0;
                }

            });
        }

        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                label1.Content = "Completo";
            });
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (cbFile.IsChecked == true)
            {
                Start(tFile.Text);
            }
            else
            {
                ///Para download da web é necessário definir nome e extensão do arquivo baixado.
                fileName = "arquivoDownload.zip";
                Start(tWeb.Text);
            }
        }
}
