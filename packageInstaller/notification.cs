using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;

namespace packageInstaller
{
    public class notification
    {
        
        //Note: You can update or replace a toast notification by updating/sending another toast using
        //The same tag and group as the original toast.

        public static void showInstallationHasStarted(string packageName)
        {
            // Define a tag value and a group value to uniquely identify a notification, in order to target it to apply the update later;
            string toastTag = "appInstall";
            string toastGroup = "Install1";

            // Construct the toast content with updatable data fields inside;
            var content = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                {
                    new AdaptiveText()
                    {
                        Text = "Install Status"
                    },

                    new AdaptiveText()
                    {
                        Text=$"{packageName} is installing..."
                    }

                  
                }
                    }
                }
            };

            // Generate the toast notification;
            var toast = new ToastNotification(content.GetXml());

            // Assign the tag and group properties;
            toast.Tag = toastTag;
            toast.Group = toastGroup;

            // Define NotificationData property and add it to the toast notification to bind the initial data;
            // Data.Values are assigned with string values;
            

            // Show the toast notification to the user;
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        public static void sendError(string errorText)
        {
            string toastTag = "appInstall";
            string toastGroup = "Install1";

            // Construct the toast content with updatable data fields inside;
            var content = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                {
                    new AdaptiveText()
                    {
                        Text = "Install Status"
                    },

                    new AdaptiveText()
                    {
                        Text=$"Installation has failed"
                    },

                    new AdaptiveText()
                    {
                        Text=$"{errorText}"
                    }


                }
                    }
                }
            };

            // Generate the toast notification;
            var toast = new ToastNotification(content.GetXml());

            // Assign the tag and group properties;
            toast.Tag = toastTag;
            toast.Group = toastGroup;

            // Define NotificationData property and add it to the toast notification to bind the initial data;
            // Data.Values are assigned with string values;


            // Show the toast notification to the user;
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        public static void ShowInstallationHasCompleted(string packageName)
        {
            string toastTag = "appInstall";
            string toastGroup = "Install1";

            // Construct the toast content with updatable data fields inside;
            var content = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                {
                    new AdaptiveText()
                    {
                        Text = "Install Status"
                    },

                    new AdaptiveText()
                    {
                        Text=$"Installation of {packageName} Is Complete!"
                    }

                   


                }
                    }
                }
            };

            // Generate the toast notification;
            var toast = new ToastNotification(content.GetXml());

            // Assign the tag and group properties;
            toast.Tag = toastTag;
            toast.Group = toastGroup;

            // Define NotificationData property and add it to the toast notification to bind the initial data;
            // Data.Values are assigned with string values;


            // Show the toast notification to the user;
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}




