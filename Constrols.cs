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

using Android.Graphics;

namespace App4
{

    public class GameControl
    {

        public bool enable { get; set; }
        public Point pos { get; set; }

        public GameControl()
        {
            pos = new Point();
        }

        public virtual float ReturnSize() { return 0; }

        public virtual int GetValue() { return -10; }
        public virtual void SetValue(int a) { }

        public virtual void DrawControl(Render render, Canvas canvas, Paint paint) { }

        public virtual void GenerateEvent() { }

        public virtual void Deactivate() { }

        public virtual void Touch(MotionEvent e) { }
    }

    class ExampleButton : GameControl
    {
        public GameMain game { get; set; }

        public ExampleButton(GameMain game)
        {
            this.game = game;
            pos = new Point(0, 0);
        }

        public override void DrawControl(Render render, Canvas canvas, Paint paint)
        {
            if (enable)
            {
                //3443
            }
        }

        public override void Touch(MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Up:
                    break;
                case MotionEventActions.Down:
                    break;
                case MotionEventActions.Move:
                    break;
                default:
                    break;
            }
        }
    }//Class for copy

    class MainButton : GameControl
    {
        public GameMain game { get; set; }
        Color _backgroundColor;
        Color _mainColor;
        float radius;
        int value = 0;
        string ansver = "";

        double timeToEnd;
        double timeToLife = 7;

        public override float ReturnSize()
        {
            return radius;
        }

        public MainButton(GameMain game)
        {
            this.game = game;
            enable = true;
            pos = new Point(Math.Max(game.render.screenSize.X, game.render.screenSize.Y)/2, Math.Min(game.render.screenSize.X, game.render.screenSize.Y)/2);
            radius = (float)(Math.Min(game.render.screenSize.X, game.render.screenSize.Y) * 0.28);

            GenerateEvent();
        }

        void MainButton_TouchUp(float x, float y)
        {
            if (
                    Math.Sqrt(
                        (pos.X - x) * (pos.X - x) +
                        (pos.Y - y) * (pos.Y - y)) <= (float)(Math.Min(game.render.screenSize.X, game.render.screenSize.Y) * 0.28)
                    )
            {
                //Попадание по кнопке
                if (!game.render.showMenu)
                {
                    if (value == game.sum)
                    {
                        game.ScoreInc(game.sum);
                        value = 0;
                        GenerateEvent();
                    }
                    else
                    {
                        game.render.ShowMenu();
                    }
                    game.sum = 0;
                } else
                {
                    game.render.HideMenu();
                }
                //game.ScoreInc();
            }
        }

        public override void GenerateEvent()
        {
            if (value == 0 && game.controls != null && game.controls[1] != null && !game.render.showMenu)
            {
                game.sum = 0;
                ansver = "";
                for (int i = 0; i < game.random.Next(1, game.maxClick + 1); i++)
                {
                    int b = game.random.Next(game.controls.Length - 1) + 1;
                    while (game.controls[b] == null)
                    {
                        b = game.random.Next(game.controls.Length - 1) + 1;
                    }
                    if (game.controls[b].enable)
                    {
                        value += game.controls[b].GetValue();
                        ansver += game.controls[b].GetValue() + " ";
                        game.showSum = value;
                    }
                    else
                        game.controls[b] = null;
                }
                timeToEnd = game.time + timeToLife;
            }

            //Не успел во время
            if (game.controls != null && game.controls[1] != null && !game.render.showMenu && game.time > timeToEnd)
                game.render.ShowMenu();

        }

        public override void SetValue(int a)
        {
            value = a;
        }

        public override void DrawControl(Render render, Canvas canvas, Paint paint)
        {
            if (enable)
            {

                paint.SetStyle(Paint.Style.Stroke);
                paint.Color = new Color(game.render._whiteButton.R, game.render._whiteButton.G - (byte)(game.render._whiteButton.G * (game.time - timeToEnd) / timeToLife), game.render._whiteButton.B - (byte)(game.render._whiteButton.B * (game.time - timeToEnd) / timeToLife));
                paint.StrokeWidth = 40;
                double grad = 360 * (timeToEnd - game.time) / timeToLife;
                canvas.DrawArc(new RectF(pos.X - radius - 2, pos.Y - radius - 2, pos.X + radius + 2, pos.Y + radius + 2), 270, (int)Math.Round(grad), true, paint);

                _backgroundColor = render._backgroundColor;
                _mainColor = render._whiteButton;

                paint.SetStyle(Paint.Style.Fill);
                paint.Color = _mainColor;

                canvas.DrawCircle(canvas.Width / 2, canvas.Height / 2, (float)(Math.Min(canvas.Width, canvas.Height) * 0.28), paint);
                paint.Color = render._backgroundColor;
                canvas.DrawCircle(canvas.Width / 2, canvas.Height / 2, (float)(Math.Min(canvas.Width, canvas.Height) * 0.23), paint);

                paint.Color = game.render._whiteButton;
                paint.TextSize = 100;
                paint.TextAlign = Paint.Align.Center;

                game.showSum = value - game.sum;
                canvas.DrawText(game.showSum.ToString(), pos.X, pos.Y + paint.TextSize / 2 - 10, paint);
                //paint.TextSize = 30;
                //canvas.DrawText(ansver, pos.X, pos.Y + radius / 2, paint);

            }
        }

        public override void Touch(MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Up:
                    MainButton_TouchUp(e.GetX(), e.GetY());
                    break;
                default:
                    break;
            }
        }
    }//Central big button

    class SimpleButton : GameControl
    {
        public GameMain game { get; set; }        

        static int radius = 100;
        int value;

        public static void SetRadius(int r)
        {
            radius = r;
        }

        public override float ReturnSize()
        {
            return radius;
        }

        public override int GetValue()
        {
            return value;
        }

        public SimpleButton(GameMain game)
        {
            this.game = game;

            SetRadius(Math.Min(game.render.screenSize.X, game.render.screenSize.Y) / 10 );

            pos.X = game.random.Next(radius, game.render.screenSize.X - radius);
            pos.Y = game.random.Next(radius, game.render.screenSize.Y - radius);

            int i = 0; //i - попытки создать кнопки(на случай если для кнопке не найдется место) 

            while (!SimpleButton_Colision(pos.X, pos.Y) && i <= 200)
            {
                pos.X = game.random.Next(radius, game.render.screenSize.X - radius);
                pos.Y = game.random.Next(radius, game.render.screenSize.Y - radius);
                i++;
            }

            value = game.random.Next(game.minValue, game.maxValue+1);
            if (value % 10 == 0) value++;

            if (i <= 200)
            {
                enable = true;
            }
        }

        public override void DrawControl(Render render, Canvas canvas, Paint paint)
        {
            if (enable)
            {
                    //Отрисовка времени
                    //paint.SetStyle(Paint.Style.Stroke);
                    //paint.Color = Color.Rgb(255, 255, 255);
                    //paint.StrokeWidth = 20;
                    //double grad = 360 * (timeToEnd - game.time) / timeToLife;
                    //canvas.DrawArc(new RectF(pos.X - radius - paint.StrokeWidth/2, pos.Y - radius - paint.StrokeWidth/2, pos.X + radius + paint.StrokeWidth/2, pos.Y + radius + paint.StrokeWidth/2), 270, (int)Math.Round(grad), true, paint);
                    //render.TextureDraw(canvas, new Rect(pos.X - radius, pos.Y - radius, pos.X + radius, pos.Y + radius), textureIdChecked);

                paint.SetStyle(Paint.Style.Fill);
                paint.Color = game.render._whiteButton;
                canvas.DrawCircle(pos.X, pos.Y, radius, paint);
                paint.Color = game.render._backgroundColor;
                canvas.DrawCircle(pos.X, pos.Y, (float)(radius - radius*0.20), paint);

                paint.Color = game.render._whiteButton;
                paint.TextSize = 100;
                paint.TextAlign = Paint.Align.Center;
                canvas.DrawText(value.ToString(), pos.X, pos.Y + paint.TextSize/2-10, paint);  //Поправить!!!!
            }
        }

        public override void Touch(MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Up:
                    SimpleButton_TouchUp(e.GetX(), e.GetY());
                    break;
                default:
                    break;
            }
        }

        public override void GenerateEvent()
        {
            //int proc = 1 / (game.random.Next(99) + 1);
            //if (proc >= chanceToCheck && !check)
            //{
            //    check = true;
            //    timeToEnd = game.time + timeToLife;
            //}
        }

        void SimpleButton_TouchUp(float x, float y)
        {
            if (
                    Math.Sqrt(
                        (pos.X - x) * (pos.X - x) +
                        (pos.Y - y) * (pos.Y - y)) <= radius
                    )
            {
                //Попадание по кнопке                
                game.render._buttonClickSound.Start();
                game.sum += value;
                if (game.showSum < 0) game.render.ShowMenu();
                //game.ScoreInc();
            }
        }

        bool SimpleButton_Colision(int x, int y)
        {
            if (game.controls != null)
            {
                for (int i = 0; i < game.controls.Length; i++)
                {

                    if (game.controls[i] != null && game.controls[i].enable &&
                        Math.Sqrt(
                            (game.controls[i].pos.X - x) * (game.controls[i].pos.X - x) +
                            (game.controls[i].pos.Y - y) * (game.controls[i].pos.Y - y)) <= game.controls[i].ReturnSize() * 2)
                        return false;
                }
            }
            return true;
        }
    }
}