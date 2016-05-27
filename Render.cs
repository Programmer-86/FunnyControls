using System;
using Android.Content;
using Android.Graphics;
using Android.Views;
using System.Timers;
using System.Diagnostics;
using Android.Media;

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

        public bool showMenu;
        bool hideMenu;
        public Color _backgroundColor = Color.Rgb(34, 44, 54);
        public Color _whiteButton = Color.Rgb(255, 255, 255);
        PointF[] _playButton;

        Color _animationColor;

        public MediaPlayer _buttonClickSound;
        MediaPlayer _backgroundMusic;

        public Render(Context context, GameMain game) : base(context)
        {
            _buttonClickSound = MediaPlayer.Create(context, Resource.Raw.ddd);
            _backgroundMusic = MediaPlayer.Create(context, Resource.Raw.background);
            _backgroundMusic.Completion += delegate { _backgroundMusic.Start(); };
            //_backgroundMusic.Start();

            var metrics = Resources.DisplayMetrics;
            //screenSize = new Point(metrics.HeightPixels, metrics.WidthPixels);
            screenSize = new Point(Math.Max(metrics.HeightPixels, metrics.WidthPixels), Math.Min(metrics.HeightPixels, metrics.WidthPixels));
            game.render = this;

            fps = 60;
            frameCount = 0;
            fpsCounter = new Timer(1000);
            fpsCounter.Elapsed += CountFPS;
            fpsCounter.Start();
            
            this.game = game;

            ShowMenu();

            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InPreferredConfig = Bitmap.Config.Rgb565;
            options.InSampleSize = 1;

            //bitmaps[0] = BitmapFactory.DecodeResource(Resources, Resource.Drawable.SimpleButton, options);
            //bitmaps[1] = BitmapFactory.DecodeResource(Resources, Resource.Drawable.SimpleButton_checked, options);
            //bitmaps[2] = BitmapFactory.DecodeResource(Resources, Resource.Drawable.Background, options);

            _playButton = new[]
                {
                    new PointF((float)(screenSize.X / 2 + Math.Min(screenSize.X, screenSize.Y) * 0.18 * Math.Sin(Math.PI * (-30) / 180)), (float)(screenSize.Y / 2 - Math.Min(screenSize.X, screenSize.Y) * 0.18 * Math.Cos(Math.PI * (-30) / 180))),
                    new PointF((float)(screenSize.X / 2 + Math.Min(screenSize.X, screenSize.Y) * 0.18 * Math.Sin(Math.PI * (90) / 180)), (float)(screenSize.Y / 2 - Math.Min(screenSize.X, screenSize.Y) * 0.18 * Math.Cos(Math.PI * (90) / 180))),
                    new PointF((float)(screenSize.X / 2 + Math.Min(screenSize.X, screenSize.Y) * 0.18 * Math.Sin(Math.PI * (210) / 180)), (float)(screenSize.Y / 2 - Math.Min(screenSize.X, screenSize.Y) * 0.18 * Math.Cos(Math.PI * (210) / 180)))
                };

            levelsImg[0] = BitmapFactory.DecodeResource(Resources, Resource.Drawable.Level1, options);
        }

        public void ShowMenu()
        {
            showMenu = true;
            hideMenu = false;
            _animationColor = _backgroundColor;
        }

        public void HideMenu()
        {
            hideMenu = true;
        }

        public void StartGame()
        {
            showMenu = false;
            game.ResumeValue();
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


            if (!showMenu && game.controls != null)
            {

                //BackGround
                //canvas.DrawBitmap(bitmaps[2], new Rect(0, 0, 10000, 10000), new Rect(0, 0, canvas.Width, canvas.Height), null);
                paint.SetStyle(Paint.Style.Fill);
                paint.Color = _backgroundColor;
                canvas.DrawRect(new Rect(0, 0, canvas.Width, canvas.Height), paint);

                game.controls[0].GenerateEvent();

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
                    game.DeactivateAll();
                    canvas.DrawBitmap(levelsImg[0], new Rect(0, 0, 10000, 10000), new Rect(0, 0, canvas.Width, canvas.Height), null);
                }
                else
                {
                    //GenerateEvent
                    //for (int i = 0; i < game.controls.Length; i++)
                    //{
                    //    if (game.controls[i] != null && game.controls[i].enable)
                    //        game.controls[i].GenerateEvent();
                    //}
                }

                //DEBUG
                //paint.Color = Color.Blue;
                //paint.SetStyle(Paint.Style.Fill);
                //paint.StrokeWidth = 1;
                //paint.TextSize = 15;
                //for (int i = 0; i < game.controls.Length; i++)
                //{
                //    if (game.controls[i] != null && game.controls[i].enable)
                //        canvas.DrawText("controls[" + i + "] = " + game.controls[i].pos.X + "_" + game.controls[i].pos.Y, 100, 90 + i * 40, paint);
                //}

                paint.Color = _whiteButton;
                paint.TextSize = 100;
                canvas.DrawText(game.score.ToString(), canvas.Width / 2, 100, paint);

            } else
            {
                //Draw menu

                //Фон
                paint.SetStyle(Paint.Style.Fill);
                paint.Color = _backgroundColor;
                canvas.DrawRect(new Rect(0, 0, canvas.Width, canvas.Height), paint);

                paint.TextSize = Convert.ToInt32(Math.Min(canvas.Width, canvas.Height) * 0.09);
                paint.Color = _animationColor;
                if (game.score > 1)
                    canvas.DrawText("Score = " + game.score, canvas.Width / 3, canvas.Height / 6, paint);
                else
                    canvas.DrawText("Arithmetic run", canvas.Width / 3, canvas.Height / 6, paint);

                //Главная кнопка
                paint.SetStyle(Paint.Style.Fill);
                if (!hideMenu)
                    paint.Color = _animationColor;
                else
                    paint.Color = _whiteButton;
                canvas.DrawCircle(canvas.Width/2, canvas.Height/2, (float)(Math.Min(canvas.Width, canvas.Height) * 0.28), paint);
                paint.Color = _backgroundColor;
                canvas.DrawCircle(canvas.Width / 2, canvas.Height / 2, (float)(Math.Min(canvas.Width, canvas.Height) * 0.23), paint);

                var path = new Path();
                path.MoveTo(_playButton[0].X, _playButton[0].Y);
                for (var i = 1; i < _playButton.Length; i++)
                {
                    path.LineTo(_playButton[i].X, _playButton[i].Y);
                }
                paint.SetStyle(Paint.Style.Fill);
                paint.Color = _animationColor;
                canvas.DrawPath(path, paint);

                //Дополнительные кнопки (4 штуки)
                for (int i = 0; i < 5; i++)  
                {
                    if (i != 2)
                    {
                        paint.Color = _animationColor;
                        canvas.DrawCircle((canvas.Width / 5) * i + (canvas.Width / 10), canvas.Height - canvas.Height / 5, (float)(Math.Min(canvas.Width, canvas.Height) * 0.11), paint);
                        paint.Color = _backgroundColor;
                        canvas.DrawCircle((canvas.Width / 5) * i + (canvas.Width / 10), canvas.Height - canvas.Height / 5, (float)(Math.Min(canvas.Width, canvas.Height) * 0.09), paint);
                    }
                    if (i == 4)
                    {
                        paint.TextSize = Convert.ToInt32(Math.Min(canvas.Width, canvas.Height) * 0.09);
                        paint.Color = _animationColor;
                        paint.TextAlign = Paint.Align.Center;
                        canvas.DrawText("Exit", (canvas.Width / 5) * i + (canvas.Width / 10), canvas.Height - canvas.Height / 5 + paint.TextSize / 2, paint);
                    }
                }

                //Плавное появление
                if (!hideMenu) AnimateColor(ref _animationColor, _whiteButton, 2);
                else
                    if (AnimateColor(ref _animationColor, _backgroundColor, 4)) StartGame();

            }


            //FPS
            paint.Color = Color.White;
            paint.StrokeWidth = 1;
            frameCount++;
            paint.TextSize = 15;
            //canvas.DrawText("FPS = " + fps + " (" + canvas.Width + " x " + canvas.Height + ")", 10, 10, paint);


            //Time
            timeStep = 1.0 / fps;
            if (timeStep <= 1) { game.time += timeStep; } else { game.time += 0.0001; }
            //canvas.DrawText("Time = " + game.time, 10, 50, paint);

            //Проверка score
            game.Game(this);

            //ReDraw
            PostInvalidate();
        }

        bool AnimateColor(ref Color animateColor, Color color, int speed)
        {
            for (int i = 0; i < speed; i++)
            {
                if (animateColor != color)
                {
                    if (animateColor.R < color.R) animateColor.R++;
                    else
                        if (animateColor.R > color.R) animateColor.R--;
                    if (animateColor.G < color.G) animateColor.G++;
                    else
                        if (animateColor.G > color.G) animateColor.G--;
                    if (animateColor.B < color.B) animateColor.B++;
                    else
                        if (animateColor.B > color.B) animateColor.B--;
                }
                else
                    return true;
            }            
            return false;
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

            //Обрабатываем клики по меню
            if (showMenu && Math.Sqrt(
                        (e.GetX() - ((screenSize.X / 5) * 4 + (screenSize.X / 10))) * (e.GetX() - ((screenSize.X / 5) * 4 + (screenSize.X / 10))) +
                        (e.GetY() - (screenSize.Y - screenSize.Y / 5)) * (e.GetY() - (screenSize.Y - screenSize.Y / 5))) <= (Math.Min(screenSize.X, screenSize.Y) * 0.11)
                        )
            {
                 Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
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
