using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using TekTokDroid;
using Xamarin.Forms;
using Xamarin.Auth;
using Newtonsoft.Json;
using Xamarin.Forms.Platform.Android;
using System.Linq;
using Android.Net;
using System.Data;
namespace TekTokDroid
{
    [Activity(Label = "TekTokDroid", MainLauncher = true, Theme = "@android:style/Theme.NoTitleBar", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)] //Set no title bar, and screen orientation portrait
    public class LoginActivity : Activity
    {
        public static string AppName { get { return "TekTokDroid"; } } //The AppName for cross file referencing
        public static User _User { get; set; } //The class/struct to hold logged user's data, class in User.cs
        
        public void AuthenticateUser()
        {
            
            ConnectivityManager connectivityManager = (ConnectivityManager)GetSystemService(ConnectivityService);
            NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo; //Internet information
            if (activeConnection != null){ //If we're connected
                //-------------------LOAD ACCOUNTSTORE FOR SAVED ACCOUNTS-----------------//
                var accounts = AccountStore.Create(this).FindAccountsForService(AppName);
                var account = accounts.FirstOrDefault();
                //------------------------------------------------------------------------//

                if (account == null) //No account stored
                {
                    //-------------AUTHENTICATE USER--------------//
                    var auth = new OAuth2Authenticator( //DO NOT CHANGE!!!
                       Constants.ClientId,
                       Constants.ClientSecret,
                       Constants.Scope,
                       new System.Uri(Constants.AuthorizeUrl),
                       new System.Uri(Constants.RedirectUrl),
                       new System.Uri(Constants.AccessTokenUrl));

                    auth.Completed += OnAuthenticationCompleted; //OnAuthenticationCompleted is the event handler for when user is authenticated
                    auth.Error += Auth_Error; //Auth_Error is the event handler for when and AuthenticationError has occured
                    StartActivity(auth.GetUI(this)); //Start the authentication UI                                                     //--------------------------------------------//
                }
                else //User does exist
                {
                    _User = new User(); //Instantiate a user struct
                    
                    _User.Email = account.Username; //Set the email because we know this already
                    databaseQueryLogin(); //Query the email for their user data
                }
            }
            else //We're not connected
            {
                AlertDialog noInternet= CustomBuilders.DialogBoxCreator("No Internet!","An internet connection is required to login!",this);
                noInternet.Show(); //Alert user we have no internet
            }
        }

        private void databaseQueryLogin()
        {
            //Open the progress bar activity, just to let user know it is logging in
            var loginAct = new Intent(this, typeof(queryLoginActivity));
            StartActivity(loginAct);
        }

     
        private void Auth_Error(object sender, AuthenticatorErrorEventArgs e)
        {
            //Console.WriteLine("!!!ERROR-START!!!"); //Just debug stuff
            //Console.WriteLine(e.Message);
            //Console.WriteLine("!!!ERROR-END!!!");

        }



        async void OnAuthenticationCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            if (e.IsAuthenticated) //Only run the below if the user was successfully authenticated
            {
                
                // If the user is authenticated, request their basic user data from Google
                // UserInfoUrl = https://www.googleapis.com/oauth2/v2/userinfo
                var request = new OAuth2Request("GET", new System.Uri(Constants.UserInfoUrl), null, e.Account);
                var response = await request.GetResponseAsync();
                if (response != null)
                {
                    // Deserialize the data and store it in the account store
                    // The users email address will be used to identify data in MySQL
                    string userJson = response.GetResponseText();
                    _User = JsonConvert.DeserializeObject<User>(userJson);
                    e.Account.Username = _User.Email;
                    
                    if (_User.Email.Contains("@alice-smith.edu.my")) 
                    {
                        AccountStore.Create(Android.App.Application.Context).Save(e.Account, AppName);
                        databaseQueryLogin();
                    }
                    else
                    {
                        AlertDialog dialog = CustomBuilders.DialogBoxCreator("Authentication Failure!", "Email is not an Alice Smith email!",this); //If user was not a KLASS member, do not continue logging in
                        dialog.Show();
                    }
                }
            }

        }



        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            // Set our view from the "main" layout resource
            _User = null;
            SetContentView(Resource.Layout.loginLayout);
            ImageView gLogin = FindViewById<ImageView>(Resource.Id.loginButton);
            gLogin.Click += delegate {  AuthenticateUser(); }; //Find the login button and attach the button to AuthenticateUser();
            
        }
    }
}

