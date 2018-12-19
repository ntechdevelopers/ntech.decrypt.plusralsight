namespace Ntech.NetFramework.DecryptPluralsight.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Caliburn.Micro;
    using Interface;
    using MaterialDesignThemes.Wpf;
    using PropertyChanged;
    using System.Windows.Forms;
    using Views;
    using Ntech.NetFramework.DecryptPluralsight.Model;

    [ImplementPropertyChanged]
    public abstract class ContentBase<TViewModel, TDataContext> : ValidatingScreen<TViewModel> where TDataContext : ICheckedNode where TViewModel : ValidatingScreen<TViewModel>
    {

        private bool? isSelectedAll;

        private readonly IHeaderBase headerBase = IoC.Get<IHeaderBase>();

        public IList<TDataContext> OriginItems { get; set; }

        public ObservableCollection<TDataContext> Items { get; set; } = new ObservableCollection<TDataContext>();

        public bool? IsAllItemsSelected
        {
            set
            {
                this.isSelectedAll = value;
                this.CheckedAll(value);
            }

            get { return this.isSelectedAll; }
        }

        protected ContentBase()
        {
            this.IsAllItemsSelected = false;
            this.headerBase.SearchTextChangedEvent += this.OnSearchTextChangedEvent;
            this.OriginItems = new List<TDataContext>();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
            this.headerBase.SearchTextChangedEvent -= this.OnSearchTextChangedEvent;
        }

        private void OnSearchTextChangedEvent(object sender, string text)
        {
            this.HandleSearchChanged(text);
        }

        protected virtual void HandleSearchChanged(string text)
        {
            bool needReloadData;
            var filteredItems = this.FilterItemsByText(text, out needReloadData);

            // Determine should be reload data on GUI or NOT
            if (needReloadData)
            {
                Execute.OnUIThreadAsync(() =>
                {
                    this.ClearDataSource();
                    foreach (var item in filteredItems)
                    {
                        this.AddItem(item);
                    }
                });
            }
        }

        public void DirectoryBrowser()
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.Description = @"Direct to Implementation folder of Project";
                var result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    this.NotifyOfPropertyChange("RootDir");
                }
            }
        }

        protected abstract IList<TDataContext> FilterItemsByText(string text, out bool isChangedData);

        public void CheckedAll(bool? isChecked)
        {
            foreach (var it in this.Items)
            {
                it.IsChecked = isChecked == true;
            }
        }

        protected IList<TDataContext> GetSelectedItems()
        {
            return this.Items.Where(pr => pr.IsChecked != false).ToList();
        }

        protected void ClearDataSource()
        {
            if (this.Items.Count > 0)
            {
                Execute.OnUIThread(() =>
                {
                    this.Items.Clear();
                });
            }
        }

        protected void AddItem(TDataContext item)
        {
            this.Items.Add(item);
        }

        protected virtual bool IsChangeData(IList<TDataContext> filteringItems)
        {
            if (filteringItems.Count != this.Items.Count)
            {
                return true;
            }

            return false;
        }

        public async Task ShowDialog(string mainScreen, string message)
        {
            var dialogView = new SimpleDialog
            {
                Message = { Text = message }
            };
            await DialogHost.Show(dialogView, mainScreen);
        }

        public async Task ShowErrorDialog(string message, string mainScreen = "RootDialog")
        {
            var dialogView = new SimpleDialog
            {
                Message = { Text = message }
            };
            await DialogHost.Show(dialogView, mainScreen);
        }
    }
}
