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
    [Activity(Label = "MainActivityTekTokker", Theme = "@android:style/Theme.NoTitleBar", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)] //No titlebar, Portrait
    public class MainActivityTekTokker : Activity
    {
        private bool keepChecking = true;
        public static string[] nameList { get; set; }
        public static bool appIsOnScreen
        {
            get; set;
        }
        
        List<HelpTicket> helpTickets = new List<HelpTicket>();
        

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
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (resultCode == Result.Ok)
            {
                checkForHelpOnce(); //Instantly update the button if we accepted a ticket from ticketListing.cs
            }
            else if (resultCode == Result.Canceled)
            {
                //Nothing lol
            }
        }
        private void displayTicketList()
        {
            //Console.WriteLine("Displaying List...");
            var ticketList = new Intent(this, typeof(ticketListing)); //DIsplay the ticket listing activity
            
            StartActivityForResult(ticketList,1); //ticketListing will return either Result.Ok if ticket accepted or Result.Canceled if no ticket
            
        }
        private void cancelHired()
        {
            //-----------------------------------------SET SELF TO NOT HIRED IN DATABASE--------------------------------------------//
            string query = String.Format("UPDATE `sql6112602`.`Users` SET `Hired` = false WHERE Name = '{0}'",LoginActivity._User.Name); 
            MySql.Data.MySqlClient.MySqlConnection sqlConn;
            string connsqlstring = "Server=sql6.freemysqlhosting.net;Port=3306;database=sql6112602; User Id=sql6112602; Password =id1FFNhByQ; charset =utf8";
            sqlConn = new MySql.Data.MySqlClient.MySqlConnection(connsqlstring);
            sqlConn.Open();
            MySql.Data.MySqlClient.MySqlCommand cQuery = new MySql.Data.MySqlClient.MySqlCommand(query, sqlConn);
            cQuery.ExecuteScalar();
            cQuery.Dispose();
            sqlConn.Close();
            sqlConn.Dispose();
            //---------------------------------------------------------------------------------------------------------------------//
        }
        private void ticketFinish(bool cancel)
        {
            try {
                MySql.Data.MySqlClient.MySqlConnection sqlConn;
                string connsqlstring = "Server=sql6.freemysqlhosting.net;Port=3306;database=sql6112602; User Id=sql6112602; Password =id1FFNhByQ; charset =utf8";
                sqlConn = new MySql.Data.MySqlClient.MySqlConnection(connsqlstring);
                DataRow aTicket;
                sqlConn.Open();
                    string hiredByQuery = "SELECT * FROM Tickets WHERE TekTokker =\"" + LoginActivity._User.Name + "\"";
                    MySql.Data.MySqlClient.MySqlDataAdapter getHiredTicket = new MySql.Data.MySqlClient.MySqlDataAdapter(hiredByQuery, sqlConn);
                    DataSet hiredTicket = new DataSet();
                    getHiredTicket.Fill(hiredTicket);
                    getHiredTicket.Dispose();
                    aTicket = hiredTicket.Tables[0].Rows[0];
                string query = "";
                if (cancel)
                {
                    query = String.Format("UPDATE `sql6112602`.`Tickets` SET `Cancelled` = '1' AND `TekTokker` = '' WHERE `Tickets`.`TeacherName` = '{0}' LIMIT 1 ; UPDATE `sql6112602`.`Users` SET `Hired` = false WHERE Name = '{1}'", aTicket["TeacherName"], LoginActivity._User.Name); //Set Cancelled to tr
                }
                else
                {
                    query = String.Format("UPDATE `sql6112602`.`Tickets` SET `Finished` = '1' AND `TekTokker` = '' WHERE `Tickets`.`TeacherName` = '{0}' LIMIT 1 ; UPDATE `sql6112602`.`Users` SET `Hired` = false WHERE Name = '{1}'", aTicket["TeacherName"], LoginActivity._User.Name); //Set Finished to true
                }
                MySql.Data.MySqlClient.MySqlCommand cQuery = new MySql.Data.MySqlClient.MySqlCommand(query, sqlConn); //submit query through the connection
                cQuery.ExecuteScalar(); //execute command
                cQuery.Dispose();
                sqlConn.Close();
                sqlConn.Dispose();
            }
            catch(Exception noInternet)
            {
                Toast noInternetT = Toast.MakeText(this, "No Internet!", ToastLength.Short); //WE HAS NO INTERWEBS
                noInternetT.Show();
            }
        }
        private void checkForHelpOnce()
        {
            MySql.Data.MySqlClient.MySqlConnection sqlConn;
            ImageView ticketButton = FindViewById<ImageView>(Resource.Id.ticketButton);
            TextView userStatusBox = FindViewById<TextView>(Resource.Id.userStatusBox);
            Android.Widget.Button cancelButton = FindViewById<Android.Widget.Button>(Resource.Id.cancelButton);
            Android.Widget.Button finishButton = FindViewById<Android.Widget.Button>(Resource.Id.FinishButton);
            finishButton.Click += delegate { ticketFinish(false); }; //When finish button is clicked, finish ticket, false means it's not a cancel
            cancelButton.Click += delegate { ticketFinish(true); }; //Same as above but cancel
            string connsqlstring = "Server=sql6.freemysqlhosting.net;Port=3306;database=sql6112602; User Id=sql6112602; Password =id1FFNhByQ; charset =utf8";
            sqlConn = new MySql.Data.MySqlClient.MySqlConnection(connsqlstring); //use the connection string to connect
            string queryHired = "SELECT * from Users WHERE Email = \"" + LoginActivity._User.Email + "\""; //query string to check if hired


            try
            {
                    sqlConn.Open(); //Open a connection
                    MySql.Data.MySqlClient.MySqlDataAdapter getHired = new MySql.Data.MySqlClient.MySqlDataAdapter(queryHired, sqlConn);
                    DataSet UserStatus = new DataSet();
                    getHired.Fill(UserStatus); //fill UserStatus
                    if (!(bool)UserStatus.Tables[0].Rows[0]["Hired"]) //If we're not hired
                    {
                        cancelButton.Visibility = ViewStates.Invisible; //Set ticket buttons to invisible
                        finishButton.Visibility = ViewStates.Invisible;
                        userStatusBox.Text = "Ready to give a helping hand!"; //Set status to ready
                        string queryString = "SELECT * from Tickets"; //Query everything from Tickets table
                        MySql.Data.MySqlClient.MySqlDataAdapter adapter = new MySql.Data.MySqlClient.MySqlDataAdapter(queryString, sqlConn);
                        DataSet tickets = new DataSet();
                        adapter.Fill(tickets);
                        List<HelpTicket> tempHelpTickets = new List<HelpTicket>();
                        try
                        {
                            //ticketButton.Enabled = true;
                            //TODO: variable to store and display on ticketLayout
                            string notif = "";
                            bool empty = true;
                            foreach (DataRow curRow in tickets.Tables[0].Rows)
                            {
                                if (!(bool)curRow["Accepted"]) //If ticket isn't accepted
                                {
                                //Add the ticket to our lists 
                                    empty = false;
                                    notif += curRow["TeacherName"].ToString() + " needs help @ " + curRow["Room"].ToString() + "\n";
                                    HelpTicket curHelpTicket = new HelpTicket();
                                    curHelpTicket.TeacherName = (string)curRow["TeacherName"];
                                    //Console.WriteLine("Requestor Found: {0}", curRow["TeacherName"].ToString());
                                    curHelpTicket.Room = (string)curRow["Room"];
                                    curHelpTicket.Timestamp = (int)curRow["Timestamp"];
                                    curHelpTicket.ExtraInformation = (string)curRow["ExtraInformation"];
                                    tempHelpTickets.Add(curHelpTicket);
                                }
                            }
                            helpTickets = tempHelpTickets;
                            if (tickets.Tables[0].Rows[0][0].ToString() == "" || empty) //If no tickets version 1
                            {

                                ticketButton.SetImageDrawable(GetDrawable(Resource.Drawable.TekTokAwaitingHelpRequest)); //Set picture to awaiting request
                                ticketButton.Enabled = false; //ticketButton not enabled
                                //Console.WriteLine("No tickets!");
                                nameList = new string[] { "" }; //nameList empty, this array is for ticketListing
                            }
                            else {
                                if (!appIsOnScreen) //Only display notif if user is off app
                                    CustomBuilders.notifBuilder("Someone needs help!", notif, typeof(MainActivityTekTokker), GetSystemService(Context.NotificationService) as NotificationManager);
                                ticketButton.SetImageDrawable(GetDrawable(Resource.Drawable.HelpRequestsAvailable));
                                ticketButton.Enabled = true;
                                List<string> names = new List<string>();
                                foreach (HelpTicket help in helpTickets)
                                {
                                    names.Add(help.TeacherName);
                                    //Console.WriteLine("Requestor Fetched: {0}", help.TeacherName);
                                }
                                nameList = names.ToArray();
                            }

                        }
                        catch (Exception getTicketException) //If no tickets version 2
                        {
                            nameList = new string[] { "" };
                            //Console.WriteLine("No tickets!");
                            ticketButton.SetImageDrawable(GetDrawable(Resource.Drawable.TekTokAwaitingHelpRequest));
                            ticketButton.Enabled = false;
                        }
                        adapter.Dispose();
                    }
                    else
                    {
                        //We are hired!
                        cancelButton.Visibility = ViewStates.Visible; //Set ticket buttons to visible
                        finishButton.Visibility = ViewStates.Visible;
                        ticketButton.Enabled = false; 
                        ticketButton.SetImageDrawable(GetDrawable(Resource.Drawable.CannotReceiveRequests)); //Can't receive image as we are on the job
                        DataRow aTicket;
                            string hiredByQuery = "SELECT * FROM Tickets WHERE TekTokker =\"" + LoginActivity._User.Name + "\"";
                            MySql.Data.MySqlClient.MySqlDataAdapter getHiredTicket = new MySql.Data.MySqlClient.MySqlDataAdapter(hiredByQuery, sqlConn);
                            DataSet hiredTicket = new DataSet();
                            getHiredTicket.Fill(hiredTicket);
                            getHiredTicket.Dispose();
                            aTicket = hiredTicket.Tables[0].Rows[0];
                        userStatusBox.Text = String.Format("On the job for: {0} at {1}\nExtra Information: {2}", aTicket["TeacherName"], aTicket["Room"], aTicket["ExtraInformation"]); //Display Ticket Info
                    }
                    sqlConn.Close();
                sqlConn.Dispose();
            }
            catch (Exception noInternet)
            {
                ticketButton.SetImageDrawable(GetDrawable(Resource.Drawable.noInternet)); //Set the nointernet icon
            }
        }
        private async void checkForHelp() //Check asynchronously (on a new thread) if there are help requests
        {
            //Refer to checkForHelpOnce for extended comments
            MySql.Data.MySqlClient.MySqlConnection sqlConn;
            ImageView ticketButton = FindViewById<ImageView>(Resource.Id.ticketButton);
            TextView userStatusBox = FindViewById<TextView>(Resource.Id.userStatusBox);
            Android.Widget.Button cancelButton = FindViewById<Android.Widget.Button>(Resource.Id.cancelButton);
            Android.Widget.Button finishButton = FindViewById<Android.Widget.Button>(Resource.Id.FinishButton);
            finishButton.Click += delegate { ticketFinish(false); };
            cancelButton.Click += delegate { ticketFinish(true); };
            string connsqlstring = "Server=sql6.freemysqlhosting.net;Port=3306;database=sql6112602; User Id=sql6112602; Password =id1FFNhByQ; charset =utf8";
            sqlConn = new MySql.Data.MySqlClient.MySqlConnection(connsqlstring);
            string queryHired = "SELECT * from Users WHERE Email = \"" + LoginActivity._User.Email + "\"";


            try {
                while (keepChecking)
                {
                    //Console.WriteLine("Checking");
                    sqlConn.Open(); //Open a connection
                    MySql.Data.MySqlClient.MySqlDataAdapter getHired = new MySql.Data.MySqlClient.MySqlDataAdapter(queryHired, sqlConn);
                    DataSet UserStatus = new DataSet();
                    getHired.Fill(UserStatus);
                    if (!(bool)UserStatus.Tables[0].Rows[0]["Hired"])
                    {
                        //Console.WriteLine("Noy hired");
                        cancelButton.Visibility = ViewStates.Invisible;
                        finishButton.Visibility = ViewStates.Invisible;
                        userStatusBox.Text = "Ready to give a helping hand!";
                        string queryString = "SELECT * from Tickets"; //Query everything from Tickets table
                        MySql.Data.MySqlClient.MySqlDataAdapter adapter = new MySql.Data.MySqlClient.MySqlDataAdapter(queryString, sqlConn);
                        DataSet tickets = new DataSet();
                        adapter.Fill(tickets);
                        List<HelpTicket> tempHelpTickets = new List<HelpTicket>();
                        try
                        {
                            //ticketButton.Enabled = true;
                            //TODO: variable to store and display on ticketLayout
                            string notif = "";
                            bool empty = true;
                            foreach (DataRow curRow in tickets.Tables[0].Rows)
                            {
                                if (!(bool)curRow["Accepted"])
                                {
                                    empty = false;
                                    notif += curRow["TeacherName"].ToString() + " needs help @ " + curRow["Room"].ToString() + "\n";
                                    HelpTicket curHelpTicket = new HelpTicket();
                                    curHelpTicket.TeacherName = (string)curRow["TeacherName"];
                                    //Console.WriteLine("Requestor Found: {0}", curRow["TeacherName"].ToString());
                                    curHelpTicket.Room = (string)curRow["Room"];
                                    curHelpTicket.Timestamp = (int)curRow["Timestamp"];
                                    curHelpTicket.ExtraInformation = (string)curRow["ExtraInformation"];
                                    tempHelpTickets.Add(curHelpTicket);
                                }
                            }
                            helpTickets = tempHelpTickets;
                            if (tickets.Tables[0].Rows[0][0].ToString() == "" || empty)
                            {

                                ticketButton.SetImageDrawable(GetDrawable(Resource.Drawable.TekTokAwaitingHelpRequest));
                                ticketButton.Enabled = false;
                                //Console.WriteLine("No tickets!");
                                nameList = new string[] { "" };
                            }
                            else {
                                if (!appIsOnScreen) //Only display notif if user is off app
                                    CustomBuilders.notifBuilder("Someone needs help!", notif, typeof(MainActivityTekTokker), GetSystemService(Context.NotificationService) as NotificationManager);
                                ticketButton.SetImageDrawable(GetDrawable(Resource.Drawable.HelpRequestsAvailable));
                                ticketButton.Enabled = true;
                                List<string> names = new List<string>();
                                foreach (HelpTicket help in helpTickets)
                                {
                                    names.Add(help.TeacherName);
                                    //Console.WriteLine("Requestor Fetched: {0}", help.TeacherName);
                                }
                                nameList = names.ToArray();
                            }

                        }
                        catch (Exception getTicketException)
                        {
                            nameList = new string[] { "" };
                            //Console.WriteLine("No tickets!");
                            ticketButton.SetImageDrawable(GetDrawable(Resource.Drawable.TekTokAwaitingHelpRequest));
                            ticketButton.Enabled = false;
                        }
                        adapter.Dispose();
                        await System.Threading.Tasks.Task.Delay(15000); //Poll every 15 seconds (15000 is in millis)
                    }
                    else
                    {
                        //Console.WriteLine("We are hired");
                        //We are hired!
                        cancelButton.Visibility = ViewStates.Visible;
                        finishButton.Visibility = ViewStates.Visible;
                        ticketButton.Enabled = false;
                        ticketButton.SetImageDrawable(GetDrawable(Resource.Drawable.CannotReceiveRequests));
                        DataRow aTicket = null;
                        string hiredByQuery = "SELECT * FROM Tickets WHERE TekTokker =\"" + LoginActivity._User.Name + "\"";
                        MySql.Data.MySqlClient.MySqlDataAdapter getHiredTicket = new MySql.Data.MySqlClient.MySqlDataAdapter(hiredByQuery, sqlConn);
                        DataSet hiredTicket = new DataSet();
                        getHiredTicket.Fill(hiredTicket);
                        getHiredTicket.Dispose();

                        try
                        {
                            aTicket = hiredTicket.Tables[0].Rows[0];
                            userStatusBox.Text = String.Format("On the job for: {0} at {1}\nExtra Information: {2}", aTicket["TeacherName"], aTicket["Room"], aTicket["ExtraInformation"]);
                        }
                        catch
                        {
                            cancelHired(); //Set hired to false
                            if (!appIsOnScreen)
                            {
                                CustomBuilders.notifBuilder("TekTok Help Ticket", "Ticket has been finished/cancelled by requestor!", typeof(MainActivityTekTokker), GetSystemService(Context.NotificationService) as NotificationManager);
                            }
                            else
                            {
                                AlertDialog ticketCancelledByUserDialog = CustomBuilders.DialogBoxCreator("Ticket Cancelled!", "Ticket has been cancelled by requestor", this);
                                ticketCancelledByUserDialog.Show();
                            }
                        }


                        await System.Threading.Tasks.Task.Delay(1000);

                    }
                    sqlConn.Close();
                }
                sqlConn.Dispose();
            }
            catch(Exception noInternet)
            {
                ticketButton.SetImageDrawable(GetDrawable(Resource.Drawable.noInternet)); //Set the nointernet icon
            }
        }
        private void initMainView()
        {
            SetContentView(Resource.Layout.MainTekTokker); //Set the view to tektokker screen
            Android.Widget.Button logoutB = FindViewById<Android.Widget.Button>(Resource.Id.logoutButtonTekTokker);
            logoutB.Click += delegate { LogOut(); }; //Find the logout button and attach LogOut() to it
            TextView nameBox = FindViewById<TextView>(Resource.Id.nameBoxTekTokker);
            nameBox.Text = "Welcome TekTokker!\n" + LoginActivity._User.Name; //Find the nameBox and say hi + name
            ImageView ticketButton = FindViewById<ImageView>(Resource.Id.ticketButton); //Find the ticket button
            ticketButton.SetImageDrawable(GetDrawable(Resource.Drawable.TekTokAwaitingHelpRequest)); //Set the awaiting image to the ticketButton
            ticketButton.Enabled = false; //Not enabled yet
            ticketButton.Click += delegate { displayTicketList(); }; //If it is enabled, run the function ticketList
            keepChecking = true; //Keep checking for queries
            checkForHelp(); //Start checking for help
        }
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
            keepChecking = false; //Stop checking for help
            this.Finish(); //Finish activity, return to login
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            appIsOnScreen = true; //app in on screen because we just started the activity
            base.OnCreate(savedInstanceState);
            //Console.WriteLine("In MainActivity");
            initMainView(); //Initiate the main view
        }
        protected override void OnPause()
        {
            appIsOnScreen = false; //App has gone to BG
            base.OnPause();
        }
        protected override void OnResume()
        {
            appIsOnScreen = true; //App is back on foreground
            base.OnResume();
        }
        protected override void OnRestart()
        {
            appIsOnScreen = true;  //App is ack on foreground
            base.OnRestart();
        }
    }
}