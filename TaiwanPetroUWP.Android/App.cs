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
using Xamarin.Forms;

namespace TaiwanPetroUWP.Android
{
    class App : Xamarin.Forms.Application
    {
        public static MasterDetail MD { get; set; }
        MD = new MasterDetail();
        MD.Master = new PageMaster();
        MD.Detail = new PageDetail();

        Application.Current.MainPage = MD;
    }
}