using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Management.Deployment;

namespace installTask
{
    public sealed class install : IBackgroundTask
    {
        BackgroundTaskDeferral _deferral;
        string resultText = "Nothing";
        bool pkgRegistered = false;

        /// <summary>
        /// Pretty much identical to showProgressInApp() in MainPage.xaml.cs
        /// </summary>
        /// <param name="taskInstance"></param>
        public async void Run(IBackgroundTaskInstance taskInstance)
        {

            _deferral = taskInstance.GetDeferral();
            ApplicationTriggerDetails details = (ApplicationTriggerDetails)taskInstance.TriggerDetails;
            string packagePath = "";

            packagePath = (string)details.Arguments["packagePath"];
            PackageManager pkgManager = new PackageManager();
            Progress<DeploymentProgress> progressCallback = new Progress<DeploymentProgress>(installProgress);
            notification.SendUpdatableToastWithProgress(0);
            if ((int)details.Arguments["installType"] == 1)
            {
                List<Uri> dependencies = new List<Uri>();
                var dependencyPairs = details.Arguments.Where(p => p.Key.Contains("d")).ToList();
                foreach (var dependencyPair in dependencyPairs)
                {
                    string dependencyAsString = (string)dependencyPair.Value;
                    dependencies.Add(new Uri(dependencyAsString));
                    
                }

                try
                {
                    var result = await pkgManager.AddPackageAsync(new Uri(packagePath), dependencies, DeploymentOptions.ForceTargetApplicationShutdown).AsTask(progressCallback);
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
                    var result = await pkgManager.AddPackageAsync(new Uri(packagePath), null, DeploymentOptions.ForceTargetApplicationShutdown).AsTask(progressCallback);
                    checkIfPackageRegistered(result, resultText);
                }

                catch (Exception e)
                {
                    resultText = e.Message;
                }


            }


            if (pkgRegistered == true)
            {
                notification.showInstallationHasCompleted();
            }
            else
            {
                notification.showError(resultText);
            }


            _deferral.Complete();
        }


        private void installProgress(DeploymentProgress installProgress)
        {
            double installPercentage = installProgress.percentage;
            notification.UpdateProgress(installPercentage);
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




    }
}
