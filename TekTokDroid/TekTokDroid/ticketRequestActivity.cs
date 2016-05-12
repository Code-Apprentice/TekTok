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
using Android.Media;

namespace TekTokDroid
{
    [Activity(Label = "ticketRequestActivity", Theme = "@android:style/Theme.NoTitleBar", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class ticketRequestActivity : Activity
    {
        private bool OnScreen = true; //Is app on screen currently?
        public class VideoLoop : Java.Lang.Object, Android.Media.MediaPlayer.IOnPreparedListener
        {
            public void OnPrepared(MediaPlayer mp)
            {
                mp.Looping = true;
            }
        }
        public override void OnBackPressed() //When the back button is pressed
        {
            AlertDialog quitDialog = CustomBuilders.DialogBoxCreator("Cancel request?", "Do you want to cancel your ticket request?", this);
            quitDialog.SetButton("Yes", (s, ev) => { this.Finish(); });
            quitDialog.SetButton2("No", (s, ev) => { });
            quitDialog.Show();
            
        }
        private void SendRequest()
        {
            EditText roomBox = FindViewById<EditText>(Resource.Id.roomTextTicket);
            EditText infoBox = FindViewById<EditText>(Resource.Id.infoTextTicket);
            string roomText = roomBox.Text;
            string infoText = infoBox.Text;
            if(String.IsNullOrWhiteSpace(roomBox.Text)|| String.IsNullOrWhiteSpace(infoBox.Text))
            {
                AlertDialog formError = CustomBuilders.DialogBoxCreator("Form Invalid!","You haven't completed the form properly!",this);
                formError.Show();
            }
            else
            {
                SetContentView(Resource.Layout.ticketSendPingLayout);
                VideoView anim = FindViewById<VideoView>(Resource.Id.pingAnimation);
                anim.SetOnPreparedListener(new VideoLoop());
                String uriPath = "android.resource://" + PackageName + "/" + Resource.Drawable.HelpBeaconGif;
                Android.Net.Uri uri = Android.Net.Uri.Parse(uriPath);
                anim.SetVideoURI(uri);
                anim.Start();
                MySql.Data.MySqlClient.MySqlConnection sqlConn;
                string connsqlstring = "Server=sql6.freemysqlhosting.net;Port=3306;database=sql6112602; User Id=sql6112602; Password =id1FFNhByQ; charset =utf8";
                sqlConn = new MySql.Data.MySqlClient.MySqlConnection(connsqlstring); //use the connection string to connect
                sqlConn.Open();
                string query = String.Format("INSERT INTO Tickets (TeacherName,Room,ExtraInformation) VALUES (\"{0}\",\"{1}\",\"{2}\")",LoginActivity._User.Name,roomText,infoText); //query string to check if hired
                Console.WriteLine("Query: "+query);
                MySql.Data.MySqlClient.MySqlCommand cQuery = new MySql.Data.MySqlClient.MySqlCommand(query, sqlConn);
                cQuery.ExecuteScalar();
                sqlConn.Close();
                sqlConn.Dispose();
                cQuery.Dispose();
                checkForHelper();
            }
        }
        private async void checkForHelper()
        {
            System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
            timer.Start();
            
            while (true)
            {
                MySql.Data.MySqlClient.MySqlConnection sqlConn;
                string connsqlstring = "Server=sql6.freemysqlhosting.net;Port=3306;database=sql6112602; User Id=sql6112602; Password =id1FFNhByQ; charset =utf8";
                sqlConn = new MySql.Data.MySqlClient.MySqlConnection(connsqlstring);
                string queryString = String.Format("SELECT * FROM Tickets WHERE TeacherName = \"{0}\"", LoginActivity._User.Name); 
                MySql.Data.MySqlClient.MySqlDataAdapter adapter = new MySql.Data.MySqlClient.MySqlDataAdapter(queryString, sqlConn);
                DataSet ticket = new DataSet();
                adapter.Fill(ticket);
                if ((bool)ticket.Tables[0].Rows[0]["Accepted"]) //Accepted
                {
                    gotoTicketLayout(ticket);
                    break;
                }
                    
                if(timer.Elapsed.Seconds >= 60) //Timeout
                {
                    Toast timeoutToast = Toast.MakeText(this, "Request Timeout, no TekTokkers have responded in time. Please try again later.",ToastLength.Long);
                    timeoutToast.Show();
                    if(!OnScreen)
                        CustomBuilders.notifBuilder("Ticket Timeout!", "Sorry but no TekTokkers have responded in time. Please try again later.", typeof(MainActivityTekTokker), GetSystemService(Context.NotificationService) as NotificationManager);

                    this.Finish();
                    break;
                }
                await System.Threading.Tasks.Task.Delay(1000);
            }
        }
        private void gotoTicketLayout(DataSet ticket) //Ticket has been accepted by TekTokker so display info etc.
        {
            var ticketActivity = new Intent(this, typeof(tekTokkeeTicket));
            ticketActivity.PutExtra("TekTokkerName", ticket.Tables[0].Rows[0]["TekTokker"].ToString());
            StartActivity(ticketActivity);
            this.Finish();
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.ticketForm);    
            Android.Widget.Button sendRequestB = FindViewById<Android.Widget.Button>(Resource.Id.sendRequestButton);
            sendRequestB.Click += delegate { SendRequest(); };
        }
        protected override void OnPause()
        {
            OnScreen = false; //App has gone to BG
            base.OnPause();
        }
        protected override void OnResume()
        {
            OnScreen = true; //App is back on foreground
            base.OnResume();
        }
        protected override void OnRestart()
        {
            OnScreen = true;  //App is back on foreground
            base.OnRestart();
        }
    }
}