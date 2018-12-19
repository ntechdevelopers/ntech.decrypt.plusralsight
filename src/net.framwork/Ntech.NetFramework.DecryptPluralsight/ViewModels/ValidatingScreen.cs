namespace Ntech.NetFramework.DecryptPluralsight.ViewModels
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel;
    using Caliburn.Micro;
    using PropertyChanged;
    using ILog = log4net.ILog;

    public class ValidatingScreen<TViewModel> : Screen, INotifyDataErrorInfo where TViewModel : ValidatingScreen<TViewModel>
    {
        #region Variables

        public readonly ConcurrentDictionary<string, List<string>> errorDic;

        protected readonly HashSet<string> validatorList = new HashSet<string>();

        private static readonly ILog Logger = log4net.LogManager.GetLogger(typeof(ValidatingScreen<TViewModel>));

        #endregion

        #region Constructor

        public ValidatingScreen()
        {
            this.errorDic = new ConcurrentDictionary<string, List<string>>();
        }

        #endregion

        #region Override of Screen

        protected override void OnViewLoaded(object view)
        {
            Logger.Debug("OnViewLoaded...");
            base.OnViewLoaded(view);
            Logger.Debug("OnViewLoaded...");
        }

        #endregion

        #region Implementation of INotifyDataErrorInfo

        [DoNotNotify]
        public virtual bool HasErrors => !this.errorDic.IsEmpty;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        #endregion

        #region Internal methods

        protected virtual void OnProperyErrors(string propertyName, IEnumerable<string> errors)
        {
        }

        public void OnPropertyErrorsChanged(string property)
        {
            this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(property));
        }

        protected bool ValidateProperty(string propertyName, object value)
        {
            return false;
        }

        public void OnNotifyErrorChanged(string propertyName)
        {
            var handler = this.ErrorsChanged;
            handler?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        public IEnumerable GetErrors(string propertyName)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
