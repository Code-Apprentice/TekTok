using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;
using Android.OS;
using TekTokDroid;
using Xamarin.Forms;
using Xamarin.Auth;
using Newtonsoft.Json;
using Xamarin.Forms.Platform.Android;
using System.Linq;
using System.Data;

namespace TekTokDroid
{
    [Activity(Label = "MainActivityTekTokkee", Theme = "@android:style/Theme.NoTitleBar", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class MainActivityTekTokkee : Activity
    {
        private void LogOut()
        {
            //Console.WriteLine("Initiating Logout...");
            //---------------------------------DELETE SAVED ACCOUNT--------------------------------//
            var accounts = AccountStore.Create(Android.App.Application.Context).FindAccountsForService(LoginActivity.AppName);
            var account = accounts.FirstOrDefault();

            if (account != null)
            {
                AccountStore.Create(Android.App.Application.Context).Delete(account, LoginActivity.AppName);
            }
            //-------------------------------------------------------------------------------------//
            this.Finish(); //Finish activity, return to login
        }
        private void requestATicket()
        {
            StartActivity(new Intent(this,typeof(ticketRequestActivity)));
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.MainTekTokkee);
            Android.Widget.Button logoutB = FindViewById<Android.Widget.Button>(Resource.Id.logoutButtonTekTokkee);
            logoutB.Click += delegate { LogOut(); };
            TextView nameBox = FindViewById<TextView>(Resource.Id.nameBoxTekTokkee);
            nameBox.Text = "Welcome TekTokkee,\n" + LoginActivity._User.Name;
            ImageView ticketRequest = FindViewById<ImageView>(Resource.Id.helpPingButton);
            ticketRequest.Click += delegate { requestATicket(); };
        }
    }
}