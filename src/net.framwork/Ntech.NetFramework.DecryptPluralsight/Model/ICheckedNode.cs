namespace Ntech.NetFramework.DecryptPluralsight.Model
{
    using System.Collections.ObjectModel;

    public interface ICheckedNode
    {
        bool? IsChecked { get; set; }

        string Name { get; set; }

        ICheckedNode Parent { get; set; }

        ObservableCollection<ICheckedNode> Items { get; set; }
    }
}
