using Ntech.NetFramework.DecryptPluralsight.Model;
using PropertyChanged;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Ntech.NetFramework.DecryptPluralsight.Models
{
    [ImplementPropertyChanged]
    public class HomeModel : ICheckedNode, INotifyPropertyChanged
    {
        private string databasePath;

        private string outputPath;

        private bool isUseDatabase;

        private bool isUseOutputFolder;

        public bool? IsChecked { get; set; }

        public ObservableCollection<ICheckedNode> Items { get; set; }

        public string Name { get; set; }

        public ICheckedNode Parent { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsRemoveFolderAfterDecryption { get; set; } = false;

        public bool IsCreateTranscript { get; set; } = true;

        public string InputPath { get; set; }

        public string DatabasePath
        {
            get
            {
                return this.databasePath;
            }
            set
            {
                this.databasePath = value;
            }
        }

        public string OutputPath
        {
            get
            {
                return this.outputPath;
            }
            set
            {
                this.outputPath = value;
            }
        }

        public bool IsUseDatabase
        {
            get
            {
                if (string.IsNullOrWhiteSpace(databasePath))
                {
                    return false;
                }
                return this.isUseDatabase;
            }
            set
            {
                this.isUseDatabase = value;
            }
        }

        public bool IsUseOutputFolder
        {
            get
            {
                if (string.IsNullOrWhiteSpace(outputPath))
                {
                    return false;
                }
                return this.isUseOutputFolder;
            }
            set
            {
                this.isUseOutputFolder = value;
            }
        }
    }
}
