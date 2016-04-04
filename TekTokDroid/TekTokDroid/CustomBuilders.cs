using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TekTokDroid
{
    /*
        CUSTOM BUILDERS CLASS
        For easy dialog box, notifications building etc.
        DO NOT CHANGE!!! (Unless you know what you're doing)
    */
    class CustomBuilders
    {
        public static void notifBuilder(string title, string content, Type activity, NotificationManager notificationManager)
        {

            Intent resultIntent = new Intent(Android.App.Application.Context, activity);

            // Construct a back stack for cross-task navigation:
            TaskStackBuilder stackBuilder = TaskStackBuilder.Create(Android.App.Application.Context);
            stackBuilder.AddParentStack(Java.Lang.Class.FromType(activity));
            stackBuilder.AddNextIntent(resultIntent);

            // Create the PendingIntent with the back stack:            
            PendingIntent resultPendingIntent =
                stackBuilder.GetPendingIntent(0, PendingIntentFlags.UpdateCurrent);



            Notification.Builder builder = new Notification.Builder(Android.App.Application.Context)
            .SetContentTitle(title)
            .SetContentText(content)
            .SetSmallIcon(Resource.Drawable.notification_template_icon_bg)
            .SetAutoCancel(true)
            .SetContentIntent(resultPendingIntent);
            builder.SetWhen(Java.Lang.JavaSystem.CurrentTimeMillis());
            builder.SetDefaults(NotificationDefaults.Sound | NotificationDefaults.Vibrate);
            // Build the notification:
            builder.SetPriority(2);
            Notification notification = builder.Build();

            // Publish the notification:
            const int notificationId = 0;
            notificationManager.Notify(notificationId, notification);
        }
        public static AlertDialog DialogBoxCreator(string title, string message, Context context) //For creating a dialog box, DO NOT CHANGE UNLESS YOU KNOW WHAT YOU'RE DOING!!!
        {
            Android.App.AlertDialog.Builder builder = new AlertDialog.Builder(context);
            AlertDialog alertDialog = builder.Create();
            alertDialog.SetTitle(title);
            alertDialog.SetIcon(Android.Resource.Drawable.IcDialogAlert);
            alertDialog.SetMessage(message);
            alertDialog.SetButton("OK", (s, ev) => { });
            return alertDialog;
        }
    }
}