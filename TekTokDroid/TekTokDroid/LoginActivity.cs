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
                   new Uri(Constants.AuthorizeUrl),
                   new Uri(Constants.RedirectUrl),
                   new Uri(Constants.AccessTokenUrl));

                auth.Completed += OnAuthenticationCompleted; //OnAuthenticationCompleted is the event handler for when user is authenticated
                auth.Error += Auth_Error; //Auth_Error is the event handler for when and AuthenticationError has occured
                StartActivity(auth.GetUI(this)); //Start the authentication UI
                //--------------------------------------------//
            }
            else //User does exist
            {
                _User = new User(); //Instantiate a user struct

                _User.Email = account.Username; //Set the email because we know this already
                databaseQueryLogin(); //Query the email for their user data
            }
        }

        private void databaseQueryLogin()
        {
            //---------------------------QUERY SQL DATABASE FOR USER INFO--------------------------------//
            MySql.Data.MySqlClient.MySqlConnection sqlConn;
            string connsqlstring = "Server=sql6.freemysqlhosting.net;Port=3306;database=sql6112602; User Id=sql6112602; Password =id1FFNhByQ; charset =utf8"; //DO NOT CHANGE, DATABASE SERVER & CREDENTIALS & DO NOT SHARE!
            sqlConn = new MySql.Data.MySqlClient.MySqlConnection(connsqlstring);
            sqlConn.Open(); //Open connection to the database
            string queryString = "SELECT * from Users WHERE Email=\"" + _User.Email + "\""; //Query all information for logged user's email in the Users table
            Console.WriteLine("Requesting data for email: {0}", _User.Email);
            MySql.Data.MySqlClient.MySqlDataAdapter adapter = new MySql.Data.MySqlClient.MySqlDataAdapter(queryString, sqlConn);
            DataSet authUserCred = new DataSet();
            adapter.Fill(authUserCred); //Fill dataset with User info
            sqlConn.Close(); //Close connection & release memory
            //-------------------------------------------------------------------------------------------//
            //TODO: ADD USER
            _User.Name = authUserCred.Tables[0].Rows[0]["Name"].ToString(); //Set their name from the Name column
            if ((bool)authUserCred.Tables[0].Rows[0]["TekTokker"]) //If user is a TekTokker
            {
                //Start the TekTokker page
                var mainTekTokker = new Intent(this, typeof(MainActivityTekTokker)); 
                mainTekTokker.PutExtra("userName", _User.Name);
                mainTekTokker.PutExtra("userEmail", _User.Email);
                mainTekTokker.PutExtra("TekTokker", (bool)authUserCred.Tables[0].Rows[0]["TekTokker"]);
                StartActivity(mainTekTokker);
            }
            else //If user is a TekTokkee
            {
                //Start The TekTokkee page
                var mainTekTokkee = new Intent(this, typeof(MainActivityTekTokkee));
                mainTekTokkee.PutExtra("userName", _User.Name);
                mainTekTokkee.PutExtra("userEmail", _User.Email);
                StartActivity(mainTekTokkee);
            }
        }

        

        

        

        

        private void Auth_Error(object sender, AuthenticatorErrorEventArgs e)
        {
            Console.WriteLine("!!!ERROR-START!!!"); //Just debug stuff
            Console.WriteLine(e.Message);
            Console.WriteLine("!!!ERROR-END!!!");

        }

        public void DialogBoxCreator(string title, string message) //For creating a dialog box, DO NOT CHANGE UNLESS YOU KNOW WHAT YOU'RE DOING!!!
        {
            Android.App.AlertDialog.Builder builder = new AlertDialog.Builder(this);
            AlertDialog alertDialog = builder.Create();
            alertDialog.SetTitle(title);
            alertDialog.SetIcon(Android.Resource.Drawable.IcDialogAlert);
            alertDialog.SetMessage(message);
            alertDialog.SetButton("OK", (s, ev) => { });
            alertDialog.Show();
        }

        async void OnAuthenticationCompleted(object sender, AuthenticatorCompletedEventArgs e)
        {
            if (e.IsAuthenticated) //Only run the below if the user was successfully authenticated
            {
                // If the user is authenticated, request their basic user data from Google
                // UserInfoUrl = https://www.googleapis.com/oauth2/v2/userinfo
                var request = new OAuth2Request("GET", new Uri(Constants.UserInfoUrl), null, e.Account);
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
                        DialogBoxCreator("Authentication Failure!", "Email is not an Alice Smith email!"); //If user was not a KLASS member, do not continue logging in
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
            ImageButton gLogin = FindViewById<ImageButton>(Resource.Id.loginButton);
            gLogin.Click += delegate { AuthenticateUser(); }; //Find the login button and attach the button to AuthenticateUser();

        }
    }
}

