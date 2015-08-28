using TyrosLayout.Model;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TyrosLayout
{
    /// <summary>
    /// Interaction logic for Pending.xaml
    /// </summary>
    public partial class Pending : Page
    {
        public Pending()
        {
            InitializeComponent();
        }

        Blob transfer;

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    CloudStorageAccount account = new CloudStorageAccount(new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials("accountname", "accountkey"), false);
        //    CloudBlobClient client = account.CreateCloudBlobClient();
        //    CloudBlobContainer container = client.GetContainerReference("container");
        //    CloudBlockBlob blob = container.GetBlockBlobReference("file");

        //    transfer = new Blob();
        //    transfer.TransferProgressChanged += transfer_TransferProgressChanged;
        //    transfer.TransferCompleted += transfer_TransferCompleted;
        //    transfer.DownloadBlobAsync(blob, @"C:\temp\file");
        //}

        //private void button2_Click(object sender, EventArgs e)
        //{
        //    transfer.CancelAsync();
        //}

        //void transfer_TransferCompleted(object sender, AsyncCompletedEventArgs e)
        //{
        //    System.Diagnostics.Debug.WriteLine("Completed. Cancelled = " + e.Cancelled);
        //}

        //void transfer_TransferProgressChanged(object sender, BlobTransfer.BlobTransferProgressChangedEventArgs e)
        //{
        //    System.Diagnostics.Debug.WriteLine("Changed - " + e.BytesSent + " / " + e.TotalBytesToSend + " = " + e.ProgressPercentage + "% " + e.Speed);
        //}
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string content = (sender as Button).Content.ToString();
            // Tab Logic
            if (content.Equals("Home"))
            {
                Home home = new Home();
                this.NavigationService.Navigate(home);
            }
            else if (content.Equals("Upload"))
            {
                Upload upload = new Upload();
                this.NavigationService.Navigate(upload);
            }
            else
            {
                Pending pending = new Pending();
                this.NavigationService.Navigate(pending);
            }
        }
    }
}
