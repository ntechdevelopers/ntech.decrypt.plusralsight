namespace Ntech.NetFramework.DecryptPluralsight.ViewModels
{
    using System.ComponentModel.Composition;
    using Caliburn.Micro;
    using Interface;
    using PropertyChanged;

    [Export]
    [Export(typeof(IContentManagement))]
    [ImplementPropertyChanged]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class BodyViewModel : Conductor<Screen> , IContentManagement
    {
        public BodyViewModel()
        {
        }

        public void ShowContent(Screen content)
        {
            if ( ((Screen) this.ActiveItem) != content )
            {
                this.ActivateItem(content);
            }
        }
    }
}
