using Android.Widget;
using ClubStat.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClubStatUI.Platforms.Android
{
    class AndroidMessage : IMessageDialog
    {
        public void ShowMessage(string message)
        {
            Toast.MakeText(Platform.CurrentActivity, message, ToastLength.Short);
        }
    }
}
