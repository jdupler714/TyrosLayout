using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TyrosLayout
{
    /// <summary>
    /// Interaction logic for Upload.xaml
    /// </summary>
    public partial class Upload : Page
    {
        public Upload()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public List<Row> ListBoxData { get; set; }

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

        private void Browse(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();



            // Set filter for file extension and default file extension 
  //          dlg.DefaultExt = ".png";
    //        dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                _files.Add(filename);
            }
        }

        private void Upload_Docs(object sender, RoutedEventArgs e)
        {
           // Add all docs in ListBox to Files so pending can access them
        }

        public ObservableCollection<string> Files
        {
            get
            {
                return _files;
            }
        }

        private ObservableCollection<string> _files = new ObservableCollection<string>();

        private void DropBox_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
                var listbox = sender as ListBox;
                listbox.Background = new SolidColorBrush(Color.FromRgb(155, 155, 155));
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }


        private void DropBox_DragLeave(object sender, DragEventArgs e)
        {
            var listbox = sender as ListBox;
            listbox.Background = new SolidColorBrush(Color.FromRgb(226, 226, 226));
        }

        private void DropBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                _files.Clear();
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string filePath in files)
                {
                    FileInfo fi = new FileInfo(filePath);
                    
                    _files.Add(fi.Name);

                    
                }

                UploadFiles(files);
            }

            var listbox = sender as ListBox;
            listbox.Background = new SolidColorBrush(Color.FromRgb(226, 226, 226));
        }

        private void UploadFiles(string[] files)
        {
            return;
        }

        public class Row
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }
        
    }
}
