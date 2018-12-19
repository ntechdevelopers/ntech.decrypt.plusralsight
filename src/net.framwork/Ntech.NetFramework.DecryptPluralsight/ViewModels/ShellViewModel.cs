namespace Ntech.NetFramework.DecryptPluralsight.ViewModels
{
    using System.ComponentModel.Composition;
    using Caliburn.Micro;
    using PropertyChanged;
    using ILog = log4net.ILog;

    [Export]
    [ImplementPropertyChanged]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ShellViewModel : Conductor<Screen>
    {
        private static readonly ILog Logger = log4net.LogManager.GetLogger(typeof(ShellViewModel));

        public string FooterLog { get; set; }

        public ShellViewModel()
        {
        }

        #region Overrides of ViewAware

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            this.ActiveMainPage();
        }

        #endregion

        private void ActiveMainPage()
        {
            Logger.Debug("ActiveMainPage...");
            var mainPage = IoC.Get<MainPageViewModel>();
            this.ActivateItem(mainPage);
            Logger.Debug("ActiveMainPage...Done");
        }
    }
}