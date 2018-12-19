namespace Ntech.NetFramework.DecryptPluralsight.Extension
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public static class CollectionExtension
    {
        public static void AddRange<T>(this ObservableCollection<T> target, IList<T> source)
        {
            foreach (var item in source)
            {
                target.Add(item);
            }
        }
    }
}
