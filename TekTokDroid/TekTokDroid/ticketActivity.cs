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
    [Activity(Label = "ticketActivity", Theme = "@android:style/Theme.NoTitleBar", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)] //No titlebar, Portrait
    public class ticketActivity : Activity
    {
        private bool queryBool = true;
        public override void OnBackPressed() //When the back button is pressed
        {
            SetResult(Result.Canceled);
            queryBool = false;
            this.Finish();
        }
        private void acceptTicket(string name)
        {
            try {
                string queryString = "UPDATE `sql6112602`.`Tickets` SET `Accepted` = '1', `TekTokker` = '" + LoginActivity._User.Name + "'" + " WHERE `Tickets`.`TeacherName` = '" + name + "'" + " LIMIT 1 ;UPDATE `sql6112602`.`Users` SET `Hired` = '1' WHERE `Users`.`Email` = '" + LoginActivity._User.Email + "'" + " LIMIT 1 ;"; //Set ticket accepted to true and notify ticket requestor of their helper. Also set self as "hired"
                MySql.Data.MySqlClient.MySqlConnection sqlConn;
                string connsqlstring = "Server=sql6.freemysqlhosting.net;Port=3306;database=sql6112602; User Id=sql6112602; Password =id1FFNhByQ; charset =utf8";
                sqlConn = new MySql.Data.MySqlClient.MySqlConnection(connsqlstring);
                sqlConn.Open(); //Open a connection
                MySql.Data.MySqlClient.MySqlCommand command = new MySql.Data.MySqlClient.MySqlCommand(queryString, sqlConn);
                command.ExecuteScalar();
                sqlConn.Close();
                sqlConn.Dispose();
                SetResult(Result.Ok);
                queryBool = false;
                this.Finish(); //We have accepted a ticket, close the activity
            }
            catch(Exception noInternet)
            {
                Toast noInternetT = Toast.MakeText(this, "No Internet!", ToastLength.Short);
                noInternetT.Show();
            }
        }
        private async void queryTicket()
        {
            while (queryBool) //Keep querying the selected ticket
            {
                try {
                    MySql.Data.MySqlClient.MySqlConnection sqlConn;
                    string connsqlstring = "Server=sql6.freemysqlhosting.net;Port=3306;database=sql6112602; User Id=sql6112602; Password =id1FFNhByQ; charset =utf8";
                    sqlConn = new MySql.Data.MySqlClient.MySqlConnection(connsqlstring);
                    string queryString = String.Format("SELECT * from Tickets WHERE TeacherName = \"{0}\"", ticketListing.acceptedTicket.Tables[0].Rows[0]["TeacherName"].ToString()); //Query everything from Tickets table
                    MySql.Data.MySqlClient.MySqlDataAdapter adapter = new MySql.Data.MySqlClient.MySqlDataAdapter(queryString, sqlConn);
                    DataSet ticket = new DataSet();
                    adapter.Fill(ticket);
                    try
                    {
                        DataRow ticketRow = ticket.Tables[0].Rows[0];
                        if ((bool)ticketRow["Accepted"]) //Has ticket been accepted?
                        {
                            Toast acceptedToast = Toast.MakeText(this, "Too late! Ticket has been accepted by someone else!", ToastLength.Long);
                            queryBool = false;
                            acceptedToast.Show();
                            this.Finish();
                        }
                    }
                    catch (Exception noTicket) //No more ticket :(
                    {
                        Toast ticketToast = Toast.MakeText(this, "Ticket no longer exists!", ToastLength.Long);
                        ticketToast.Show();
                        queryBool = false;
                        this.Finish();
                    }
                    await System.Threading.Tasks.Task.Delay(1000);
                }
                catch(Exception noInternet) //Internet lost :(
                {
                    Toast noInternetT = Toast.MakeText(this, "No Internet!", ToastLength.Short);
                    noInternetT.Show();
                    queryBool = false;
                    this.Finish();
                }
            }
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            //Show user ticket info
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ticketLayout);
            TextView ticketHolderBox = FindViewById<TextView>(Resource.Id.ticketHolderBox);
            ticketHolderBox.Text = "Ticket Holder: " + ticketListing.acceptedTicket.Tables[0].Rows[0]["TeacherName"].ToString();
            TextView roomBox = FindViewById<TextView>(Resource.Id.locationBox);
            roomBox.Text = "Room: " + ticketListing.acceptedTicket.Tables[0].Rows[0]["Room"].ToString();
            TextView extraInfoBox = FindViewById<TextView>(Resource.Id.extraInfoBox);
            extraInfoBox.Text = ticketListing.acceptedTicket.Tables[0].Rows[0]["ExtraInformation"].ToString();
            Button acceptButton = FindViewById<Button>(Resource.Id.acceptButton);
            acceptButton.Click += delegate { acceptTicket(ticketListing.acceptedTicket.Tables[0].Rows[0]["TeacherName"].ToString()); };
            Button declineButton = FindViewById<Button>(Resource.Id.declineButton);
            declineButton.Click += delegate { this.Finish();};
            queryTicket();
        }
        protected override void OnPause()
        {
            queryBool = false; //Stop querying
            base.OnPause();
        }
        protected override void OnRestart()
        {
            queryBool = true;
            queryTicket();
            base.OnRestart();
        }
        protected override void OnResume()
        {
            queryBool = true;
            queryTicket();
            base.OnResume();
        }
    }
}