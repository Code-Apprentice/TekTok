using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TekTokDroid
{
    [Activity(Label = "tekTokkeeTicket", Theme = "@android:style/Theme.NoTitleBar", ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait)]
    public class tekTokkeeTicket : Activity
    {
        private bool OnScreen = true; //Is app on screen currently?
        public override void OnBackPressed() //When the back button is pressed
        {
            finishTicket();
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.tekTokkeeTicket);
            TextView tekTokker = FindViewById<TextView>(Resource.Id.tekTokkerHelperLabel);
            tekTokker.Text = "TekTokker: " + Intent.GetStringExtra("TekTokkerName");
            Button finishBtn = FindViewById<Button>(Resource.Id.tttFinishBtn);
            finishBtn.Click += delegate { finishTicket(); };
            checkTicketAsync();
        }
        private async void checkTicketAsync()
        {
            while (true)
            {
                MySql.Data.MySqlClient.MySqlConnection sqlConn;
                string connsqlstring = "Server=sql6.freemysqlhosting.net;Port=3306;database=sql6112602; User Id=sql6112602; Password =id1FFNhByQ; charset =utf8";
                sqlConn = new MySql.Data.MySqlClient.MySqlConnection(connsqlstring);
                string queryString = String.Format("SELECT * from Tickets WHERE TeacherName = \"{0}\"", LoginActivity._User.Name);
                MySql.Data.MySqlClient.MySqlDataAdapter dataAdapter = new MySql.Data.MySqlClient.MySqlDataAdapter(queryString, sqlConn);
                DataSet ticket = new DataSet();
                dataAdapter.Fill(ticket);
                dataAdapter.Dispose();

                if ((bool)ticket.Tables[0].Rows[0]["Cancelled"])
                {
                    AlertDialog cancelledDialogAlert = CustomBuilders.DialogBoxCreator("Ticket Cancelled by TekTokker", "Ticket has been cancelled by the TekTokker, request another ticket if you still require help", this);
                    cancelledDialogAlert.Show();
                    cancelledDialogAlert.CancelEvent += delegate { this.Finish(); };
                    cancelledDialogAlert.SetButton("Ok", (s, ev) => { this.Finish(); });
                }
                if ((bool)ticket.Tables[0].Rows[0]["Finished"])
                {
                    AlertDialog cancelledDialogAlert = CustomBuilders.DialogBoxCreator("Ticket Finished by TekTokker", "Ticket has been finished by the TekTokker, request another ticket if you still require help", this);
                    cancelledDialogAlert.Show();
                    cancelledDialogAlert.CancelEvent += delegate { this.Finish(); };
                    cancelledDialogAlert.SetButton("Ok", (s, ev) => { this.Finish(); });
                }
                await System.Threading.Tasks.Task.Delay(1000);
            }
        }
        private void deleteTicket()
        {
            MySql.Data.MySqlClient.MySqlConnection sqlConn;
            string connsqlstring = "Server=sql6.freemysqlhosting.net;Port=3306;database=sql6112602; User Id=sql6112602; Password =id1FFNhByQ; charset =utf8";
            sqlConn = new MySql.Data.MySqlClient.MySqlConnection(connsqlstring);
            sqlConn.Open();
            string queryString = String.Format("DELETE FROM Tickets WHERE TeacherName = {0}", LoginActivity._User.Name);
            MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(queryString);
            cmd.ExecuteScalar();
            this.Finish();
        }
        private void finishTicket()
        {
            AlertDialog quitDialog = CustomBuilders.DialogBoxCreator("Cancel request?", "Do you wish to cancel your ticket request?", this);
            quitDialog.SetButton("Yes", (s, ev) => { deleteTicket(); });
            quitDialog.SetButton2("No", (s, ev) => { });
            quitDialog.Show();
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