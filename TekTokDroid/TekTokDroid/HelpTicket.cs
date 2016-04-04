using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TekTokDroid
{
    class HelpTicket
    {
        public string TeacherName { get; set; }

        public string Room { get; set; }

        public int Timestamp { get; set; }

        public string ExtraInformation { get; set; }
    }
}