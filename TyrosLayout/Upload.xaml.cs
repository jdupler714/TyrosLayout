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

using TyrosLayout.ViewModel;

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

        private string _SelectedFolder;
        public string SelectedFolder
        {
            get { return _SelectedFolder; }
            set { return; }
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
                var fileInfo = new FileInfo(filename);
                Files.Add(new Model.File()
                    {
                        Name = fileInfo.Name,
                        FullFilePath = filename,
                        SizeInBytes = fileInfo.Length,
                        RelativeToFolder = Path.GetDirectoryName(filename) + Path.DirectorySeparatorChar,
                    });
            }
        }
        //private void Browse(object sender, RoutedEventArgs e)
        //{
        //    // Create OpenFileDialog 
        //    //Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
        //    var dlg = new Microsoft.Win32.OpenFileDialog() { CheckFileExists = true, InitialDirectory = SelectedFolder, Multiselect = true };



        //    // Set filter for file extension and default file extension 
        //    //          dlg.DefaultExt = ".png";
        //    //        dlg.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";


        //    // Display OpenFileDialog by calling ShowDialog method 
        //    Nullable<bool> result = dlg.ShowDialog();


        //    // Get the selected file name and display in a TextBox 
        //    if (result == true)
        //    {
        //        // Open document 
        //        SelectedFolder = Path.GetDirectoryName(dlg.FileNames[0]);
        //        Files.Clear();
        //        foreach (var file in dlg.FileNames)
        //        {
        //            var fileInfo = new FileInfo(file);
        //            Files.Add(new Model.File()
        //            {
        //                Name = Path.GetFileName(file),
        //                FullFilePath = file,
        //                SizeInBytes = fileInfo.Length,
        //                RelativeToFolder = Path.GetDirectoryName(file) + Path.DirectorySeparatorChar,
        //            });
        //        }

        //    }
        //}

        private void Upload_Docs(object sender, RoutedEventArgs e)
        {
           // Add all docs in ListBox to Files so pending can access them
        }

        public ObservableCollection<Model.File> Files
        {
            get
            {
                return _files;
            }
        }

        private ObservableCollection<Model.File> _files = new ObservableCollection<Model.File>();

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

                var fileList = new List<TyrosLayout.Model.File>();


                foreach (var file in files)
                {

                    var fileInfo = new FileInfo(file);
                    fileList.Add(new Model.File()
                    {
                        Name = fileInfo.Name,
                        FullFilePath = file,
                        SizeInBytes = fileInfo.Length,
                        RelativeToFolder = null,
                    });
                    string name = fileInfo.Name;
                   // FileInfo fi = new FileInfo(file);
                  //  Row next = new Row();
                  //  next.Name=fi.Name;
                  //  next.LocalPath=filePath;
                  //  ListBoxData.Add(next);
                  //  _files.Add(name);
     
                }

                UploadFiles(fileList);
            }

            var listbox = sender as ListBox;
            listbox.Background = new SolidColorBrush(Color.FromRgb(226, 226, 226));
        }

        
        private void UploadFiles(List<TyrosLayout.Model.File> files)
        {
            return;
        }

        
    }
}
