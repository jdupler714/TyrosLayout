using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using TyrosLayout.Model;
using TyrosLayout.ViewModel;

namespace TyrosLayout.ViewModel
{
    public class OnErrorEventArgs : EventArgs
    {
        public string Message { get; set; }
    }

    class MainPageViewModel : ModelBase
    {
        public event EventHandler<OnErrorEventArgs> OnError;
        private void OnOnError(string message)
        {
            if (OnError != null)
                ExecuteOnUI(() => OnError(this, new OnErrorEventArgs() { Message = message }));
        }

        public WorkerManager WorkerManager { get; private set; }
        public ObservableCollection<TyrosLayout.Model.File> Files { get; private set; }
        public ObservableCollection<TyrosLayout.Model.Blob> Blobs { get; private set; }

        public MainPageViewModel()
        {
            Files = new ObservableCollection<TyrosLayout.Model.File>();
            Blobs = new ObservableCollection<TyrosLayout.Model.Blob>();
            WorkerManager = new WorkerManager();
            WorkerManager.OnError += OnErrorHandler;
        }

        void OnErrorHandler(object sender, OnErrorEventArgs e)
        {
            OnOnError(e.Message);
        }



        private string _SelectedFolder;
        public string SelectedFolder
        {
            get { return _SelectedFolder; }
            set { SetField(ref _SelectedFolder, value, () => SelectedFolder); }
        }

        private ICommand _SelectFilesCommand;
        public ICommand SelectFilesCommand
        {
            get
            {
                if (_SelectFilesCommand == null)
                    _SelectFilesCommand = new ActionCommand(OnSelectFiles);
                return _SelectFilesCommand;
            }
        }

        private void OnSelectFiles(object param)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog() { CheckFileExists = true, InitialDirectory = SelectedFolder, Multiselect = true };
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                SelectedFolder = Path.GetDirectoryName(dialog.FileNames[0]);
                Files.Clear();
                foreach (var file in dialog.FileNames)
                {
                    var fileInfo = new FileInfo(file);
                    Files.Add(new Model.File()
                    {
                        Name = Path.GetFileName(file),
                        FullFilePath = file,
                        SizeInBytes = fileInfo.Length,
                        RelativeToFolder = Path.GetDirectoryName(file) + Path.DirectorySeparatorChar,
                    });
                }
            }
        }
    }
}
