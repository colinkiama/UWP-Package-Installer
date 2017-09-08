using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
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

namespace mobilePackageInstaller
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        StorageFile packageInContext;
        List<Uri> dependencies;
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
        private async void installButton_Click(object sender, RoutedEventArgs e)
        {
            loadFileButton.Visibility = Visibility.Collapsed;
            installButton.Visibility = Visibility.Collapsed;
            cancelButton.Visibility = Visibility.Collapsed;
            PackageManager pkgManager = new PackageManager();

            Progress<DeploymentProgress> progressCallback = new Progress<DeploymentProgress>(installProgress);
            DeploymentResult result;
            if (dependencies != null && dependencies.Count > 0)
            {
                result = await pkgManager.AddPackageAsync(new Uri(packageInContext.Path), dependencies, DeploymentOptions.RequiredContentGroupOnly).AsTask(progressCallback);
            }
            else
            {
                result = await pkgManager.AddPackageAsync(new Uri(packageInContext.Path), null, DeploymentOptions.RequiredContentGroupOnly).AsTask(progressCallback);
            }

            cancelButton.Content = "Exit";
            cancelButton.Visibility = Visibility.Visible;
            if (!result.IsRegistered)
            {
                resultTextBlock.Text = result.ErrorText;

            }
        }

        /// <summary>
        /// Updates the progress bar and status of the installation
        /// </summary>
        /// <param name="installProgress"></param>
        private void installProgress(DeploymentProgress installProgress)
        {
            
            double installPercentage = installProgress.percentage;
            permissionTextBlock.Text = "Installing...";
            installProgressBar.Value = installPercentage;
            string percentageAsString = String.Format($"{installPercentage}%");
            installValueTextBlock.Text = percentageAsString;
            if (installProgressBar.Value >= 100)
            {
                permissionTextBlock.Text = "Completed";
            }
            
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

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                //UI changes to allow the user to install the package
                packageInContext = file;
                permissionTextBlock.Text = "Do you want to install this package?";
                installProgressBar.Visibility = Visibility.Visible;
                installValueTextBlock.Visibility = Visibility.Visible;
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

            var files = await picker.PickMultipleFilesAsync();
            if (files != null)
            {
                
                foreach (var dependency in files)
                {
                    dependencies.Add(new Uri(dependency.Path));
                }

                loadDependenciesButton.Content = "Load different dependencies";
            }
        }
    }
}
