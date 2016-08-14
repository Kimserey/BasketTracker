using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using Xamarin.Android;
using Android.Support.V7.App;
using Android.OS;
using Android.Util;
using Android.App;
using Android.Content;

namespace BasketTracker.Droid
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity: AppCompatActivity
    {
        protected override void OnResume()
        {
            base.OnResume();

            //Task.Run(() =>
            //{
            //    StartActivity(new Intent(Application.Context, typeof(MainActivity)));
            //});
        }
    }
}