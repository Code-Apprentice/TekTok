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
    [Activity(Label = "Available Tickets")]
    public class ticketListing : ListActivity
    {
        private IList<string> names;
        public static DataSet acceptedTicket { get; set; }
        private bool keepUpdate = true;
        public override void OnBackPressed()
        {
            SetResult(Result.Canceled); //We haven't accepted a ticket
            keepUpdate = false;
            this.Finish();           
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            
            
            MainActivityTekTokker.appIsOnScreen = true;
            update();               
        }
        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if(resultCode == Result.Ok) //If we have accepted a ticket
            {
                keepUpdate = false;
                SetResult(Result.Ok); //We have accepted a ticket
                this.Finish();
            }
            else if (resultCode == Result.Canceled)
            {
                //Nothing lol
            }
        }
        private async void update()
        {
            
            while (keepUpdate)
            {
                await System.Threading.Tasks.Task.Delay(1000);
                names = MainActivityTekTokker.nameList.ToList<string>();
                ArrayAdapter adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleListItem1, MainActivityTekTokker.nameList);
                ListAdapter = adapter;
            }
        }
            
        protected override void OnPause()
        {
            MainActivityTekTokker.appIsOnScreen = false; //App has gone to BG
            base.OnPause();
        }
        protected override void OnResume()
        {
            MainActivityTekTokker.appIsOnScreen = true; //App is back on foreground
            base.OnResume();
        }
        protected override void OnRestart()
        {
            MainActivityTekTokker.appIsOnScreen = true;
            base.OnRestart();
        }
        
        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            MySql.Data.MySqlClient.MySqlConnection sqlConn;
            Android.Widget.Button ticketButton = FindViewById<Android.Widget.Button>(Resource.Id.ticketButton);
            string connsqlstring = "Server=sql6.freemysqlhosting.net;Port=3306;database=sql6112602; User Id=sql6112602; Password =id1FFNhByQ; charset =utf8";
            sqlConn = new MySql.Data.MySqlClient.MySqlConnection(connsqlstring);
            sqlConn.Open(); //Open a connection
            string queryString = "SELECT * from Tickets WHERE TeacherName=\"" + names[position] + "\""; //Query help ticket
            MySql.Data.MySqlClient.MySqlDataAdapter adapter = new MySql.Data.MySqlClient.MySqlDataAdapter(queryString, sqlConn);
            DataSet ticket = new DataSet();
            adapter.Fill(ticket);
            sqlConn.Close();
            DataSet ticket2 = ticket;
            try {
                if ((bool)ticket.Tables[0].Rows[0]["Accepted"]) //If ticket has already been accepted
                {
                    AlertDialog alreadyProcessed = CustomBuilders.DialogBoxCreator("Error", "Whoops! It looks like this ticket has already been accepted by another TekTokker", this);
                    alreadyProcessed.Show();
                }
                else if (ticket.Tables[0].Rows[0][0].ToString() == "") //If ticket is empty
                {
                    AlertDialog alreadyProcessed = CustomBuilders.DialogBoxCreator("Error", "Whoops! It looks like this ticket no longer exists!", this);
                    alreadyProcessed.Show();
                }
                else
                {
                    //Show ticket information to user in a seperate activity
                    acceptedTicket = ticket2;
                    var showTicketLayout = new Intent(this, typeof(ticketActivity));
                    StartActivityForResult(showTicketLayout, 1);
                }
            }
            catch(Exception e) //If ticket is empty 2
            {
                AlertDialog alreadyProcessed = CustomBuilders.DialogBoxCreator("Error", "Whoops! It looks like this ticket no longer exists!", this);
                alreadyProcessed.Show();
            }
        }
    }
}