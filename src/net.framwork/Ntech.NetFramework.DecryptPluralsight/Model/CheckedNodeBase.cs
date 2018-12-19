namespace Ntech.NetFramework.DecryptPluralsight.Model
{
    using System.Collections.ObjectModel;

    public class CheckedNodeBase : ICheckedNode
    {
        #region Implementation of ICheckedNode

         public bool? IsChecked { get; set; }

        public string Name { get; set; }

        public ICheckedNode Parent { get; set; }

        public ObservableCollection<ICheckedNode> Items { get; set; }

        #endregion
    }
}
