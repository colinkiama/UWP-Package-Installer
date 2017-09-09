using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using Microsoft.Toolkit.Uwp.Notifications;

namespace installTask
{
    public sealed class notification
    {

        //Note: You can update or replace a toast notification by updating/sending another toast using
        //The same tag and group as the original toast.

        public static void SendUpdatableToastWithProgress(double progress)
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
                        Text = new BindableString("changingText4Mobile")
                    },

                    new AdaptiveProgressBar()
                    {
                        Title = "Progress",
                        Value = new BindableProgressBarValue("progressValue"),
                        ValueStringOverride = new BindableString("progressString"),
                        Status = new BindableString("progressStatus")
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
            toast.Data = new NotificationData();
            toast.Data.Values["progressValue"] = String.Format("{0}", progress / 100);
            toast.Data.Values["progressString"] = String.Format("{0}%", progress);
            toast.Data.Values["progressStatus"] = "Installing";
            toast.Data.Values["changingText4Mobile"] = String.Format("Install Progress at {0}%", progress);

            // Provide sequence number to prevent updating out-of-order or assign it with value 0 to indicate "always update";
            toast.Data.SequenceNumber = 1;

            // Show the toast notification to the user;
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        public static void UpdateProgress(double progress)
        {
            // Construct a NotificationData object;
            string tag = "appInstall";
            string group = "Install1";

            // Create NotificationData with new values;
            // Make sure that sequence number is incremented since last update, or assign with value 0 for updating regardless of order;
            var data = new NotificationData { SequenceNumber = 2 };
            data.Values["changingText4Mobile"] = String.Format("Install Progress at {0}%", progress);
            data.Values["progressValue"] = String.Format("{0}", progress / 100);
            data.Values["progressString"] = String.Format("{0}%", progress);

            // Updating a previously sent toast with tag, group, and new data;
            NotificationUpdateResult updateResult = ToastNotificationManager.CreateToastNotifier().Update(data, tag, group);
        }

        public static void showInstallationHasCompleted()
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
                        Text=$"The Package has finished Installing!"
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

        public static void showError(string errorText)
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
    }
}




