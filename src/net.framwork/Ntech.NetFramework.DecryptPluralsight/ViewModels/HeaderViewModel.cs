namespace Ntech.NetFramework.DecryptPluralsight.ViewModels
{
    using System;
    using System.ComponentModel.Composition;
    using System.Windows;
    using Caliburn.Micro;
    using Interface;
    using PropertyChanged;
    using ILog = log4net.ILog;

    [Export]
    [Export(typeof(IHeaderBase))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    [ImplementPropertyChanged]
    public class HeaderViewModel : IHeaderBase
    {
        #region Variables

        private IContentManagement contentManagement => IoC.Get<IContentManagement>();

        private string filterText;

        private static readonly ILog Logger = log4net.LogManager.GetLogger(typeof(HeaderViewModel));

        private ShellViewModel shellViewModel => IoC.Get<ShellViewModel>();

        #endregion

        #region Properties

        [AlsoNotifyFor("HasText")]
        public string FilterText
        {
            get { return this.filterText; }
            set
            {
                this.filterText = value;
                this.SearchTextChangedEvent?.Invoke(this, value);
            }
        }

        public Visibility HasText => string.IsNullOrWhiteSpace(this.FilterText) ? Visibility.Collapsed : Visibility.Visible;

        public event EventHandler<string> SearchTextChangedEvent;

        #endregion

        #region Commands handling

        public void ClearText()
        {
            this.FilterText = string.Empty;
        }

        public void HomeManager()
        {
            Logger.Debug("HomeManager... - Show HomeManager");
            this.contentManagement.ShowContent(IoC.Get<HomePageViewModel>());
            this.shellViewModel.FooterLog = "Project management";
            Logger.Debug("HomeManager... - Show HomeManager..DONE");
        }

        public void Exit()
        {
            Logger.Debug("Exit...");
            Application.Current.MainWindow.Close();
            Logger.Debug("Exit...DONE");
        }

        #endregion
    }
}
