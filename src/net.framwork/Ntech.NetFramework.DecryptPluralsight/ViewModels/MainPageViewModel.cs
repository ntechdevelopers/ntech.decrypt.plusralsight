namespace Ntech.NetFramework.DecryptPluralsight.ViewModels
{
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.Windows;
    using Caliburn.Micro;
    using Helper;
    using PropertyChanged;
    using ILog = log4net.ILog;

    /// <summary>
    /// Class MainPageViewModel.
    /// </summary>
    /// <seealso cref="Caliburn.Micro.Screen" />
    [Export]
    [ImplementPropertyChanged]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class MainPageViewModel : Screen
    {
        #region Varialbes

        /// <summary>
        /// The logger
        /// </summary>
        private static readonly ILog Logger = log4net.LogManager.GetLogger(typeof (MainPageViewModel));

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        /// <value>The header.</value>
        public HeaderViewModel Header { get; set; } = IoC.Get<HeaderViewModel>();

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>The body.</value>
        public BodyViewModel Body { get; set; } = IoC.Get<BodyViewModel>();

        #endregion

        #region CTOR

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPageViewModel" /> class.
        /// </summary>
        public MainPageViewModel()
        {
            Logger.Debug("MainPageViewModel...");

            Logger.Debug("MainPageViewModel...DONE");
            Application.Current.MainWindow.Closing += this.MainWindowOnClosing;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Mains the window on closing.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="cancelEventArgs">The <see cref="CancelEventArgs"/> instance containing the event data.</param>
        private void MainWindowOnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
        }

        #endregion
    }
}
