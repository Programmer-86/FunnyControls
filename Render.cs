using System;
using Android.Content;
using Android.Graphics;
using Android.Views;
using System.Timers;
using System.Diagnostics;

namespace App4
{
    public class Render : View, View.IOnTouchListener
    {
        int fps, frameCount;
        Timer fpsCounter;
        public Point screenSize { get; set; }
        //int screenWidth, screenHeight;
        double timeStep;

        double timeToShowLevel;
        GameMain game;

        Bitmap[] levelsImg = new Bitmap[1];
        Bitmap[] bitmaps = new Bitmap[10];

        public Render(Context context, GameMain game) : base(context)
        {
            var metrics = Resources.DisplayMetrics;
            screenSize = new Point(metrics.WidthPixels, metrics.HeightPixels);
            game.render = this;

            fps = 60;
            frameCount = 0;
            fpsCounter = new Timer(1000);
            fpsCounter.Elapsed += CountFPS;
            fpsCounter.Start();
            
            this.game = game;

            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InPreferredConfig = Bitmap.Config.Rgb565;
            options.InSampleSize = 1;

            bitmaps[0] = BitmapFactory.DecodeResource(Resources, Resource.Drawable.SimpleButton, options);
            bitmaps[1] = BitmapFactory.DecodeResource(Resources, Resource.Drawable.SimpleButton_checked, options);
            bitmaps[2] = BitmapFactory.DecodeResource(Resources, Resource.Drawable.Background, options);

            levelsImg[0] = BitmapFactory.DecodeResource(Resources, Resource.Drawable.Level1, options);
        }

        public void TextureDraw(Canvas canvas, Rect rect, int TextureId)
        {
            //123
            canvas.DrawBitmap(bitmaps[TextureId], new Rect(0, 0, 10000, 10000), rect, null);
        }

        ~Render()
        {
            fpsCounter.Dispose();
            for (int i = 0; i < bitmaps.Length; i++)
            {
                bitmaps[i].Dispose();
            }
            for (int i = 0; i < levelsImg.Length; i++)
            {
                levelsImg[i].Dispose();
            }
        }

        protected override void OnDraw(Canvas canvas)
        {

            base.OnDraw(canvas);

            //Paint
            var paint = new Paint();
            paint.Color = Color.Black;
            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeWidth = 1;

            if (game.controls != null)
            {
                //BackGround
                canvas.DrawBitmap(bitmaps[2], new Rect(0, 0, 10000, 10000), new Rect(0, 0, canvas.Width, canvas.Height), null);

                //Draw game controls
                for (int i = 0; i < game.controls.Length; i++)
                {
                    if (game.controls[i] != null && game.controls[i].enable)
                    {
                        game.controls[i].DrawControl(this, canvas, paint);
                    }

                }

                if (game.time <= timeToShowLevel)
                {
                    //canvas.DrawBitmap(levelsImg[game.GetLevel()], new Rect(0, 0, 10000, 10000), new Rect(0, 0, canvas.Width, canvas.Height), null);
                    canvas.DrawBitmap(levelsImg[0], new Rect(0, 0, 10000, 10000), new Rect(0, 0, canvas.Width, canvas.Height), null);
                }
                else
                {
                    //GenerateEvent
                    for (int i = 0; i < game.controls.Length; i++)
                    {
                        if (game.controls[i] != null && game.controls[i].enable)
                            game.controls[i].GenerateEvent();
                    }
                }

                //DEBUG
                paint.Color = Color.Blue;
                paint.SetStyle(Paint.Style.Stroke);
                paint.StrokeWidth = 1;
                for (int i = 0; i < game.controls.Length; i++)
                {
                    if (game.controls[i] != null && game.controls[i].enable)
                        canvas.DrawText("controls[" + i + "] = " + game.controls[i].pos.X + "_" + game.controls[i].pos.Y, 10, 90 + i * 40, paint);
                }

            }

            //FPS
            paint.Color = Color.White;
            paint.StrokeWidth = 1;
            frameCount++;
            canvas.DrawText("FPS = " + fps + " (" + canvas.Width + " x " + canvas.Height + ")", 10, 10, paint);


            //Time
            timeStep = 1.0 / fps;
            if (timeStep <= 1) { game.time += timeStep; } else { game.time += 0.0001; }
            canvas.DrawText("Time = " + game.time, 10, 50, paint);

            //Проверка score
            game.Game(this);

            //ReDraw
            PostInvalidate();
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            for (int i = 0; i < game.controls.Length; i++)
            {
                if (game.controls[i] != null && game.controls[i].enable)
                {
                    game.controls[i].Touch(e);
                }
            }
            return true;
        }

        public void SetTimeToShowLevel(double time)
        {
            timeToShowLevel = time;
        }

        private void CountFPS(object sender, EventArgs e)
        {
            fps = frameCount;
            frameCount = 0;
        }
    }
}
