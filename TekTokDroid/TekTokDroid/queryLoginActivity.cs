using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TekTokDroid
{
    [Activity(Label = "queryLoginActivity", Theme = "@android:style/Theme.NoTitleBar", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)] //No titlebar, Portrait
    public class queryLoginActivity : Activity
    {
        private async void databaseQueryLogin()
        {
            ProgressBar progBar = FindViewById<ProgressBar>(Resource.Id.progressBar1);
            TextView textProg = FindViewById<TextView>(Resource.Id.progressText);
            try {
                
                //---------------------------QUERY SQL DATABASE FOR USER INFO--------------------------------//
                textProg.Text = "Establishing connection with the database...";
                MySql.Data.MySqlClient.MySqlConnection sqlConn;
                string connsqlstring = "Server=sql6.freemysqlhosting.net;Port=3306;database=sql6112602; User Id=sql6112602; Password =id1FFNhByQ; charset =utf8"; //DO NOT CHANGE, DATABASE SERVER & CREDENTIALS & DO NOT SHARE!
                sqlConn = new MySql.Data.MySqlClient.MySqlConnection(connsqlstring);
                sqlConn.Open(); //Open connection to the database
                string queryString = "SELECT * from Users WHERE Email=\"" + LoginActivity._User.Email + "\""; //Query all information for logged user's email in the Users table
                //Console.WriteLine("Requesting data for email: {0}", LoginActivity._User.Email);
                MySql.Data.MySqlClient.MySqlDataAdapter adapter = new MySql.Data.MySqlClient.MySqlDataAdapter(queryString, sqlConn);
                DataSet authUserCred = new DataSet();
                progBar.Progress = 25;
                await System.Threading.Tasks.Task.Delay(1000);
                adapter.Fill(authUserCred); //Fill dataset with User info
                textProg.Text = "Requesting User Info...";
                progBar.Progress = 50;
                await System.Threading.Tasks.Task.Delay(1000);
                //-------------------------------------------------------------------------------------------//
                try {
                    if (authUserCred.Tables[0].Rows[0]["Name"].ToString() == "")
                    {
                        textProg.Text = "No user info, creating an account...";
                        progBar.Progress = 75;
                        await System.Threading.Tasks.Task.Delay(1000);
                        string query = String.Format("INSERT INTO Users(Email,Name,TekTokker,Hired) VALUES (\"{0}\",\"{1}\",false,false)", LoginActivity._User.Email, LoginActivity._User.Name);
                        MySql.Data.MySqlClient.MySqlCommand cQuery = new MySql.Data.MySqlClient.MySqlCommand(query, sqlConn);
                        cQuery.ExecuteScalar();
                        cQuery.Dispose();
                        adapter.Fill(authUserCred);
                    }
                    LoginActivity._User.Name = authUserCred.Tables[0].Rows[0]["Name"].ToString(); //Set their name from the Name column
                    
                    if ((bool)authUserCred.Tables[0].Rows[0]["TekTokker"]) //If user is a TekTokker
                    {
                        textProg.Text = "Authenticated as a TekTokker...";
                        progBar.Progress = 100;
                        await System.Threading.Tasks.Task.Delay(1000);
                        //Start the TekTokker page
                        var mainTekTokker = new Intent(this, typeof(MainActivityTekTokker));
                        mainTekTokker.PutExtra("userName", LoginActivity._User.Name);
                        mainTekTokker.PutExtra("userEmail", LoginActivity._User.Email);
                        mainTekTokker.PutExtra("TekTokker", (bool)authUserCred.Tables[0].Rows[0]["TekTokker"]);
                        StartActivity(mainTekTokker);
                        this.Finish();
                    }
                    else //If user is a TekTokkee
                    {
                        textProg.Text = "Authenticated as a TekTokkee...";
                        progBar.Progress = 100;
                        await System.Threading.Tasks.Task.Delay(1000);
                        //Start The TekTokkee page
                        var mainTekTokkee = new Intent(this, typeof(MainActivityTekTokkee));
                        mainTekTokkee.PutExtra("userName", LoginActivity._User.Name);
                        mainTekTokkee.PutExtra("userEmail", LoginActivity._User.Email);
                        StartActivity(mainTekTokkee);
                        this.Finish();
                    }
                }
                catch (Exception noUser)
                {
                    string query = String.Format("INSERT INTO Users(Email,Name,TekTokker,Hired) VALUES (\"{0}\",\"{1}\",false,false)", LoginActivity._User.Email, LoginActivity._User.Name);
                    MySql.Data.MySqlClient.MySqlCommand cQuery = new MySql.Data.MySqlClient.MySqlCommand(query, sqlConn);
                    cQuery.ExecuteScalar();
                    cQuery.Dispose();
                    adapter.Fill(authUserCred);
                    textProg.Text = "No user info, creating an account...";
                    progBar.Progress = 75;
                    await System.Threading.Tasks.Task.Delay(1000);
                    textProg.Text = "Authenticated as a TekTokkee...";
                    progBar.Progress = 100;
                    await System.Threading.Tasks.Task.Delay(1000);
                    //Start The TekTokkee page
                    var mainTekTokkee = new Intent(this, typeof(MainActivityTekTokkee));
                    mainTekTokkee.PutExtra("userName", LoginActivity._User.Name);
                    mainTekTokkee.PutExtra("userEmail", LoginActivity._User.Email);
                    StartActivity(mainTekTokkee);
                    this.Finish();
                }
            }
            catch(Exception noInternet)
            {
                textProg.Text = noInternet.Message;
                await System.Threading.Tasks.Task.Delay(1500);
                this.Finish();
            }
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.queryLoginLayout);
            databaseQueryLogin();
        }
    }
}