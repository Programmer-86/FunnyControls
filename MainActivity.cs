using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace App4
{
    [Activity(Label = "App4", MainLauncher = true, Icon = "@drawable/icon", Theme = "@android:style/Theme.NoTitleBar.Fullscreen")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            GameMain game = new GameMain();
            Render view = new Render(this, game);
            view.SetOnTouchListener(view);

            SetContentView(Resource.Layout.Main);

            Button button = FindViewById<Button>(Resource.Id.button3);
            button.Click += delegate { SetContentView(view); };
            button = FindViewById<Button>(Resource.Id.button1);
            button.Click += delegate { Android.OS.Process.KillProcess(Android.OS.Process.MyPid()); };

            //StartActivity(typeof(LoseActivity));

            //SetContentView(view);
            //view.SetOnTouchListener(view);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            //Array.Clear(controls, 0, controls.Length);
        }
    }
}

