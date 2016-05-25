using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace App4
{
    [Activity(Label = "App4", MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.NoTitleBar.Fullscreen", ConfigurationChanges = Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class MainActivity : Activity
    {
        Render view;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);            

            this.RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
            SetContentView(Resource.Layout.Miniature);

            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 5000;
            timer.AutoReset = false;
            timer.Elapsed += OnTimedEvent;
            timer.Enabled = true;


            GameMain game = new GameMain();
            view = new Render(this, game);
            view.SetOnTouchListener(view);

            //SetContentView(view);


            //
            ///button.Click += delegate { Android.OS.Process.KillProcess(Android.OS.Process.MyPid()); };
        }

        private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            RunOnUiThread(() => SetContentView(view));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            //Array.Clear(controls, 0, controls.Length);
        }
    }
}

