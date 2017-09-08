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
        public MainPage()
        {
            this.InitializeComponent();
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            try
            {
                StorageFile package = (StorageFile)e.Parameter;
                packageInContext = package;
                //Update UI For installation
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

        }



        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        private async void installButton_Click(object sender, RoutedEventArgs e)
        {
            loadFileButton.Visibility = Visibility.Collapsed;
            installButton.Visibility = Visibility.Collapsed;
            cancelButton.Visibility = Visibility.Collapsed;
            PackageManager pkgManager = new PackageManager();
            Progress<DeploymentProgress> progressCallback = new Progress<DeploymentProgress>(installProgress);
            var result = await pkgManager.AddPackageAsync(new Uri(packageInContext.Path), null, DeploymentOptions.RequiredContentGroupOnly).AsTask(progressCallback);
            cancelButton.Content = "Exit";
            cancelButton.Visibility = Visibility.Visible;
            if (!result.IsRegistered)
            {
                resultTextBlock.Text = result.ErrorText;

            }
        }

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

        private async void loadFileButton_Click(object sender, RoutedEventArgs e)
        {
            var picker = new Windows.Storage.Pickers.FileOpenPicker();
            picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
            picker.FileTypeFilter.Add(".appx");
            picker.FileTypeFilter.Add(".appxbundle");

            Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // Application now has read/write access to the picked file
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
    }
}
