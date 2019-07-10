using Android.App;
using Android.Widget;
using Android.OS;

namespace TaiwanPetroUWP.Android
{
    [Activity(Label = "台灣油價查詢", MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsApplicationActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new MainPage());
        }
    }
}

