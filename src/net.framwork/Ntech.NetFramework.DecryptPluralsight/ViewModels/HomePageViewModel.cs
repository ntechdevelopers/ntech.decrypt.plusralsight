using System.Collections.Generic;
using System.ComponentModel.Composition;
using PropertyChanged;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;
using MaterialDesignThemes.Wpf;
using System.Threading.Tasks;
using Caliburn.Micro;
using System.Text;
using System.Linq;
using System;
using Ntech.NetFramework.DecryptPluralsight.Models;
using System.Windows.Forms;
using Ntech.NetFramework.DecryptPluralsight.Views;
using System.IO;
using Ntech.NetStandard.Utilities.DecryptPluralSight;

namespace Ntech.NetFramework.DecryptPluralsight.ViewModels
{
    [Export]
    [ImplementPropertyChanged]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class HomePageViewModel : ContentBase<HomePageViewModel, HomeModel>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(HomePageViewModel));

        Decryptor decryptor = new Decryptor();

        public HomeModel Model { get; set; } = new HomeModel();

        public HomePageViewModel()
        {
            string userName = System.Security.Principal.WindowsIdentity.GetCurrent()?.Name?.Split('\\')?.Last() ?? string.Empty;

            this.Model.DatabasePath = Path.Combine(@"C:\Users", userName, @"AppData\Local\Pluralsight\pluralsight.db");
            this.Model.InputPath = Path.Combine(@"C:\Users", userName, @"AppData\Local\Pluralsight\courses");
            this.Model.OutputPath = @"E:\"; //Path.Combine(@"C:\Users", userName, @"Desktop");
        }

        public void BrowserFile()
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Database file (*.db) | *.db";
                var result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    this.Model.DatabasePath = dialog.FileName;
                    this.NotifyOfPropertyChange(nameof(this.Model.DatabasePath));
                }
            }
        }

        public void BrowserDirectory()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                var result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    this.Model.InputPath = dialog.SelectedPath;
                    this.NotifyOfPropertyChange(nameof(this.Model.DatabasePath));
                }
            }
        }

        public void BrowserOutputDirectory()
        {
            using (var dialog = new FolderBrowserDialog())
            {
                var result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    this.Model.OutputPath = dialog.SelectedPath;
                    this.NotifyOfPropertyChange(nameof(this.Model.DatabasePath));
                }
            }
        }

        public async void ExecuteDecrypt()
        {
            var busyView = new BusyIndicator();
            await DialogHost.Show(busyView, "RootDialog", (sender, args) =>
            {
                Task.Run<IList<string>>(() => this.execute())
                .ContinueWith(task =>
                {
                    Execute.OnUIThreadAsync(() =>
                    {
                        DialogHost.CloseDialogCommand.Execute(true, busyView);
                    });

                    var errors = task.Result;
                    if (errors.Any())
                    {
                        var errorStr = new StringBuilder();
                        foreach (var error in errors)
                        {
                            errorStr.AppendLine(error);
                        }

                        Execute.OnUIThreadAsync(() =>
                        {
                            this.ShowErrorDialog(errorStr.ToString());
                        });
                    }
                    else
                    {
                        Execute.OnUIThreadAsync(() =>
                        {
                            this.ShowDialog("RootDialog", "Decrypt Video Sussfully.");
                        });
                    }
                });
            }, null);
        }

        private IList<string> execute()
        {
            var errors = new List<string>();
            try
            {
                decryptor.DatabasePath = this.Model.DatabasePath;
                decryptor.DecryptAllFolders(this.Model.InputPath, this.Model.IsCreateTranscript, this.Model.OutputPath, false);

                if (this.Model.IsRemoveFolderAfterDecryption)
                {
                    foreach (string coursePath in Directory.GetDirectories(this.Model.InputPath, "*", SearchOption.TopDirectoryOnly))
                    {
                        decryptor.RemoveCourse(coursePath);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Logger.Error("Execute - Fail to decrypt.", ex);
                errors.Add($"Fail to decrypt: {ex.Message}");
            }
            return errors;
        }

        protected override IList<HomeModel> FilterItemsByText(string text, out bool isChangedData)
        {
            isChangedData = false;
            return new List<HomeModel>();
        }
    }
}
