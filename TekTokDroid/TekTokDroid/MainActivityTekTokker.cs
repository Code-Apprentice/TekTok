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
    [Activity(Label = "MainActivityTekTokker", Theme = "@android:style/Theme.NoTitleBar", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)] //No titlebar, Portrait
    public class MainActivityTekTokker : Activity
    {
        private string userName;
        private string userEmail;
        private bool keepChecking = true;
        private bool appIsOnScreen;
        public override void OnBackPressed() //When the back button is pressed
        {
            //--------------------ALERT USER IF THEY QUIT USING BACK BUTTON NO NOTIFS------------------------------//
                Android.App.AlertDialog.Builder builder = new AlertDialog.Builder(this);
                AlertDialog alertDialog = builder.Create();
                alertDialog.SetTitle("Quit");
                alertDialog.SetIcon(Android.Resource.Drawable.IcDialogAlert);
                alertDialog.SetMessage("Are you sure you want to quit?\nYour help notifications will be disabled!\nUse the Home Button to exit without disabling notifs");
                alertDialog.SetButton("OK", (s, ev) => { System.Environment.Exit(0); });
                alertDialog.SetButton2("Cancel", (s, ev) => { });
                alertDialog.Show();
            //-----------------------------------------------------------------------------------------------------//
        }
        
        private async void checkForHelp() //Check asynchronously (on a new thread) if there are help requests
        {
            MySql.Data.MySqlClient.MySqlConnection sqlConn; 
            string connsqlstring = "Server=sql6.freemysqlhosting.net;Port=3306;database=sql6112602; User Id=sql6112602; Password =id1FFNhByQ; charset =utf8";
            sqlConn = new MySql.Data.MySqlClient.MySqlConnection(connsqlstring);
            sqlConn.Open(); //Open a connection
            while (keepChecking)
            {
                await System.Threading.Tasks.Task.Delay(15000); //Poll every 15 seconds (15000 is in millis)

                string queryString = "SELECT * from Tickets"; //Query everything from Tickets table
                MySql.Data.MySqlClient.MySqlDataAdapter adapter = new MySql.Data.MySqlClient.MySqlDataAdapter(queryString, sqlConn);
                DataSet tickets = new DataSet();
                adapter.Fill(tickets);
                if (tickets.Tables[0].Rows != null) //If there are tickets
                {
                    //TODO: variable to store and display on ticketLayout
                    int lastTimeTemp = (int)tickets.Tables[0].Rows[0]["Timestamp"];
                    string notif = "";
                    foreach (DataRow curRow in tickets.Tables[0].Rows)
                    {
                        notif += curRow["TeacherName"].ToString() + " needs help @ " + curRow["Room"].ToString() + "\n";
                    }
                    if(!appIsOnScreen) //Only display notif if user is off app
                        CustomBuilders.notifBuilder("Someone needs help!", notif,typeof(MainActivityTekTokker), GetSystemService(Context.NotificationService) as NotificationManager);
                }
            }
            sqlConn.Close();
        }
        private void LogOut()
        {
            Console.WriteLine("Initiating Logout...");
            //---------------------------------DELETE SAVED ACCOUNT--------------------------------//
            var accounts = AccountStore.Create(Android.App.Application.Context).FindAccountsForService(LoginActivity.AppName);
            var account = accounts.FirstOrDefault();

            if (account != null)
            {
                AccountStore.Create(Android.App.Application.Context).Delete(account, LoginActivity.AppName);
            }
            //-------------------------------------------------------------------------------------//
            keepChecking = false; //Stop checking for help
            this.Finish(); //Finish activity, return to login
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            appIsOnScreen = true; //app in on screen because we just started the activity
            base.OnCreate(savedInstanceState);
            Console.WriteLine("In MainActivity");
            userName = Intent.GetStringExtra("userName"); //Request passed on data
            userEmail = Intent.GetStringExtra("userEmail"); //Request passed on Data
            SetContentView(Resource.Layout.MainTekTokker); //Set the view to tektokker screen
            Android.Widget.Button logoutB = FindViewById<Android.Widget.Button>(Resource.Id.logoutButtonTekTokker);
            logoutB.Click += delegate { LogOut(); }; //Find the logout button and attach LogOut() to it
            TextView nameBox = FindViewById<TextView>(Resource.Id.nameBoxTekTokker);
            nameBox.Text = "Welcome TekTokker!\n" + userName; //Find the nameBox and say hi + name
            TextView connectionState = FindViewById<TextView>(Resource.Id.connectionStatusTekTokker);
            connectionState.Text = "Awaiting Help Request...";
            checkForHelp();
        }
        protected override void OnPause()
        {
            appIsOnScreen = false; //App has gone to BG
            base.OnPause();
        }
        protected override void OnResume()
        {
            appIsOnScreen = true; //App is back on foreground
            base.OnRestart();
        }
    }
}