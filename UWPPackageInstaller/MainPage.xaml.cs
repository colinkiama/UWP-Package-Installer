using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Management.Deployment;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPPackageInstaller
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        StorageFile packageInContext;
        List<Uri> dependencies = new List<Uri>();
        //ValueSet cannot contain values of the URI class which is why there is another list below.
        //This is required to update the progress in a notification using a background task.
        List<string> dependenciesAsString = new List<string>();

        bool pkgRegistered = false;
        public MainPage()
        {
            this.InitializeComponent();

        }

        /// <summary>
        /// Attempts to get appx/appxbundle from the OnFileActivated event in App.xaml.cs
        ///If the cast fails in the try statement then the catch statement will change
        ///the UI so the user can load the required files themselves.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            base.OnNavigatedTo(e);
            try
            {
                StorageFile package = (StorageFile)e.Parameter;
                packageInContext = package;
                updateUIForPackageInstallation();
            }
            catch (Exception x)
            {
                Debug.WriteLine(x.Message);
                permissionTextBlock.Text = "Load an .appx/.appxbundle file to install";
                installProgressBar.Visibility = Visibility.Collapsed;
                installValueTextBlock.Visibility = Visibility.Collapsed;
                installButton.Visibility = Visibility.Collapsed;
                cancelButton.Content = "Exit";
                packageNameTextBlock.Text = "No package Selected";

            }
        }

        private void updateUIForPackageInstallation()
        {
            packageNameTextBlock.Text = packageInContext.DisplayName;
            loadFileButton.Content = "Load a different file";

        }



        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        /// <summary>
        /// <para>
        /// Installs the the package with or without it's dependencies depending on whether the user loads their dependecies or not.
        /// The AddPackageAsync method uses the Uri of the files used to install the packages and dependencies.
        /// </para>
        /// <para>
        /// WARNING: In order to use some PackageManager class' methods, restricted capabilities need to be added to 
        /// the appxmanifest. In this case, the restricted capability that has been added is the "packageManagement".
        /// </para>
        /// If they are not added, to your app and you use certain methods, your app will crash unexpectedly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void installButton_Click(object sender, RoutedEventArgs e)
        {
            loadFileButton.Visibility = Visibility.Collapsed;
            loadDependenciesButton.Visibility = Visibility.Collapsed;
            installButton.Visibility = Visibility.Collapsed;
            cancelButton.Visibility = Visibility.Collapsed;

            //Modern Test:
            //showProgressInNotification();

            //Legacy Test:
            //showProgressInApp();

            //Normal Code:
            //If the device is on the creators update or later, install progress is shown in the action center and App UI
            //Otherwise, all progress is shown in the App's UI.
            if (ApiInformation.IsApiContractPresent("Windows.Foundation.UniversalApiContract", 4))
            {
                showProgressInNotification();
            }
            else
            {
                showProgressInApp();
            }




        }


        private async void showProgressInApp()
        {
            installProgressBar.Visibility = Visibility.Visible;
            installValueTextBlock.Visibility = Visibility.Visible;
            PackageManager pkgManager = new PackageManager();
            Progress<DeploymentProgress> progressCallback = new Progress<DeploymentProgress>(installProgress);
            string resultText = "Nothing";

            notification.showInstallationHasStarted(packageInContext.Name);
            if (dependencies != null && dependencies.Count > 0)
            {
                try
                {
                    var result = await pkgManager.AddPackageAsync(new Uri(packageInContext.Path), dependencies, DeploymentOptions.ForceTargetApplicationShutdown).AsTask(progressCallback);
                    checkIfPackageRegistered(result, resultText);

                }
                catch (Exception e)
                {
                    resultText = e.Message;
                }

            }
            else
            {
                try
                {

                    var result = await pkgManager.AddPackageAsync(new Uri(packageInContext.Path), null, DeploymentOptions.ForceTargetApplicationShutdown).AsTask(progressCallback);
                    checkIfPackageRegistered(result, resultText);
                }

                catch (Exception e)
                {
                    resultText = e.Message;
                }

            }

            cancelButton.Content = "Exit";
            cancelButton.Visibility = Visibility.Visible;
            if (pkgRegistered == true)
            {
                permissionTextBlock.Text = "Completed";
                notification.ShowInstallationHasCompleted(packageInContext.Name);



            }
            else
            {
                resultTextBlock.Text = resultText;
                notification.sendError(resultText);
            }
        }

        private void checkIfPackageRegistered(DeploymentResult result, string resultText)
        {
            if (result.IsRegistered)
            {
                pkgRegistered = true;
            }
            else
            {
                resultText = result.ErrorText;
            }
        }

        /// <summary>
        /// Passes package file path and of file paths dependencies into the backgroundTask
        /// using a ValueSet.
        /// </summary>
        private async void showProgressInNotification()
        {
            permissionTextBlock.Text = "Check Your Notifications/Action Center 😉";
            var thingsToPassOver = new ValueSet();
            thingsToPassOver.Add("packagePath", packageInContext.Path);
            if (dependenciesAsString != null & dependenciesAsString.Count > 0)
            {
                int count = dependenciesAsString.Count();
                for (int i = 0; i < count; i++)
                {
                    thingsToPassOver.Add($"dependencies{i}", dependenciesAsString[i]);
                }
                thingsToPassOver.Add("installType", 1);
            }
            else
            {
                thingsToPassOver.Add("installType", 0);
            }

            PackageManager pkgManager = new PackageManager();
            ApplicationTrigger appTrigger = new ApplicationTrigger();
            var backgroundTask = RegisterBackgroundTask("installTask.install", "installTask", appTrigger);
            //backgroundTask.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
            backgroundTask.Progress += new BackgroundTaskProgressEventHandler(OnProgress);
            var result = await appTrigger.RequestAsync(thingsToPassOver);

            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                if (task.Value.Name == "installTask")
                {
                    AttachCompletedHandler(task.Value);

                }
            }
            installProgressBar.Visibility = Visibility.Visible;
            installValueTextBlock.Visibility = Visibility.Visible;
        }

        private async void OnProgress(BackgroundTaskRegistration sender, BackgroundTaskProgressEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {

            installProgressBar.Value = args.Progress;
            installValueTextBlock.Text = $"{args.Progress}%";
            });
        }

        private void AttachCompletedHandler(IBackgroundTaskRegistration task)
        {
            task.Completed += new BackgroundTaskCompletedEventHandler(OnCompleted);
        }


        private async void OnCompleted(IBackgroundTaskRegistration task, BackgroundTaskCompletedEventArgs args)
        {
            //UpdateUI;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                cancelButton.Content = "Exit";
                cancelButton.Visibility = Visibility.Visible;
                permissionTextBlock.Text = "Insall Task Complete, check notifications for results";


            });
        }



        public static BackgroundTaskRegistration RegisterBackgroundTask(string taskEntryPoint,
                                                                            string taskName,
                                                                            IBackgroundTrigger trigger)
        {
            //
            // Check for existing registrations of this background task.
            //

            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {

                if (cur.Value.Name == taskName)
                {
                    //
                    // The task is already registered.
                    //

                    return (BackgroundTaskRegistration)(cur.Value);
                }
            }

            //
            // Register the background task.
            //

            var builder = new BackgroundTaskBuilder();

            builder.Name = taskName;
            builder.TaskEntryPoint = taskEntryPoint;
            builder.SetTrigger(trigger);

            BackgroundTaskRegistration task = builder.Register();

            return task;
        }



        /// <summary>
        /// Updates the progress bar and status of the installation in the app's UI.
        /// </summary>
        /// <param name="installProgress"></param>
        private void installProgress(DeploymentProgress installProgress)
        {

            double installPercentage = installProgress.percentage;
            permissionTextBlock.Text = "Installing...";
            installProgressBar.Value = installPercentage;
            string percentageAsString = String.Format($"{installPercentage}%");
            installValueTextBlock.Text = percentageAsString;

        }

        /// <summary>
        /// Retreives an appx/appxbundle file using the file picker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void loadFileButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            picker.FileTypeFilter.Add(".appx");
            picker.FileTypeFilter.Add(".appxbundle");
            picker.FileTypeFilter.Add(".msix");
            picker.FileTypeFilter.Add(".msixbundle");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                //UI changes to allow the user to install the package
                packageInContext = file;
                permissionTextBlock.Text = "Do you want to install this package?";
                installButton.Visibility = Visibility.Visible;
                cancelButton.Content = "Cancel";
                packageNameTextBlock.Text = packageInContext.DisplayName;
                loadFileButton.Content = "Load a different file";
            }
        }

        /// <summary>
        /// Retrieves one OR MORE dependencies using the file picker
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void loadDependenciesButton_Click(object sender, RoutedEventArgs e)
        {
            dependencies = new List<Uri>();
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            picker.FileTypeFilter.Add(".appx");
            picker.FileTypeFilter.Add(".appxbundle");
            picker.FileTypeFilter.Add(".msix");
            picker.FileTypeFilter.Add(".msixbundle");

            var files = await picker.PickMultipleFilesAsync();
            if (files != null)
            {

                foreach (var dependency in files)
                {
                    dependencies.Add(new Uri(dependency.Path));
                }


                foreach (var dependency in files)
                {
                    dependenciesAsString.Add(dependency.Path);
                }

                loadDependenciesButton.Content = "Load different dependencies";
            }
        }
    }
}
