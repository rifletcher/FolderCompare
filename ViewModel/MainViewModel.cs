using FolderCompare.Models;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FolderCompare.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private RelayCommand _callGetSourceFolderCommand;

        public RelayCommand CallGetSourceFolderCommand
        {
            get
            {
                return _callGetSourceFolderCommand ?? (_callGetSourceFolderCommand = new RelayCommand(
                           async () =>
                           {
                               await ExecuteCallGetSourceFolderCommand();
                           }));
            }
        }

        private RelayCommand _callGetDestinationFolderCommand;

        public RelayCommand CallGetDesinationFolderCommand
        {
            get
            {
                return _callGetDestinationFolderCommand ?? (_callGetDestinationFolderCommand = new RelayCommand(
                           async () =>
                           {
                               await ExecuteCallGetDestinationFolderCommand();
                           }));
            }
        }

        private RelayCommand _callProcessCommand;

        public RelayCommand CallProcessCommand
        {
            get
            {
                return _callProcessCommand ?? (_callProcessCommand = new RelayCommand(
                           async () =>
                           {
                               await ExecuteCallProcessCommand();
                           }));
            }
        }

        /// <summary>
        /// The source folder
        /// </summary>
        private string _sourceFolder = "";
        public string SourceFolder
        {
            get => _sourceFolder;
            set
            {
                _sourceFolder = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// The dstination folder
        /// </summary>
        private string _destinationFolder = "";
        public string DestinationFolder
        {
            get => _destinationFolder;
            set
            {
                _destinationFolder = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// DisplayFiles
        /// </summary>
        private ObservableCollection<FileRecord> _displayFiles = new ObservableCollection<FileRecord>();
        public ObservableCollection<FileRecord> DisplayFiles
        {
            get => _displayFiles;
            set
            {
                _displayFiles = value;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
        }

        public async Task ExecuteCallGetSourceFolderCommand()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();
                SourceFolder = dialog.SelectedPath;
            }
            return;
        }

        public async Task ExecuteCallGetDestinationFolderCommand()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();
                DestinationFolder = dialog.SelectedPath;
            }
            return;
        }

        private void DisplayMessage(string message)
        {
            MessageBoxButtons buttons = MessageBoxButtons.OK;
            MessageBox.Show(message, "Message", buttons);

        }
        public async Task ExecuteCallProcessCommand()
        {
            if (string.IsNullOrEmpty(_sourceFolder) || string.IsNullOrEmpty(_destinationFolder))
            {
                DisplayMessage("Source Folder or Desintation Folder is missing or invalid");
            }
            else
            {
                var sourceFiles = (from file in Directory.EnumerateFiles(SourceFolder, "*.*", SearchOption.AllDirectories)
                                  select new FileRecord
                                  {
                                      Path = file,
                                      Filename = Path.GetFileName(file)
                                  }).ToList();
                var DesinationFiles = (from file in Directory.EnumerateFiles(DestinationFolder, "*.*", SearchOption.AllDirectories)
                                      select new FileRecord
                                      {
                                          Path = file,
                                          Filename = Path.GetFileName(file)
                                      }).ToList();

                var difference = sourceFiles
                    .Where(x => !DesinationFiles.Any(y => y.Filename == x.Filename))
                    .ToList();

                DisplayFiles = new ObservableCollection<FileRecord>(difference.OrderBy(x => x.Filename));
            }

            return;
        }
    }
}