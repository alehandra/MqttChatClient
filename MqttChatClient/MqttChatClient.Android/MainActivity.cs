using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Net.Mqtt;
using System.Threading.Tasks;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using Android;

namespace MqttChatClient.Droid
{
    [Activity(Label = "MqttChatClient", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        private const int READ_CONTACTS = 1000;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            LoadApplication(new App());
        }

        protected override void OnStart()
        {
            base.OnStart();

            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadContacts) != Permission.Granted)
            {
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.ReadContacts }, READ_CONTACTS);
            }
            else
            {
                App.Instance.InitializeWorkingResources();
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            switch (requestCode)
            {
                case READ_CONTACTS:
                    if (grantResults.Length > 0 && grantResults[0] == Permission.Granted)
                    {
                        App.Instance.InitializeWorkingResources();
                    }
                    else
                    {
                        Finish();
                    }
                    break;
            }
        }
    }
}