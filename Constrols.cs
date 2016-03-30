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

        public virtual void DrawControl(Render render, Canvas canvas, Paint paint) { }

        public virtual void GenerateEvent() { }

        public virtual void Deactivate() { }

        public virtual void Touch(MotionEvent e) { }
    }

    class SimpleButton : GameControl
    {
        public GameMain game { get; set; }        

        static int radius = 200;
        static int textureId = 0;
        static int textureIdChecked = 1;
        static double chanceToCheck = 0.2;
        static double timeToLife = 3;
        
        double timeToEnd;        
        bool check;

        public static void SetRadius(int r)
        {
            radius = r;
        }

        public static void SetTimeToLife(double t)
        {
            timeToLife = t;
        }

        public SimpleButton(GameMain game)
        {
            this.game = game;

            pos.X = game.random.Next(radius, game.render.screenSize.X - radius);
            pos.Y = game.random.Next(radius, game.render.screenSize.Y - radius);

            int i = 0;

            while (!SimpleButton_Colision(pos.X, pos.Y) && i <= 200)
            {
                pos.X = game.random.Next(radius, game.render.screenSize.X - radius);
                pos.Y = game.random.Next(radius, game.render.screenSize.Y - radius);
                i++;
            }

            if (i <= 200)
            {
                enable = true;
            }
        }

        public override void DrawControl(Render render, Canvas canvas, Paint paint)
        {
            if (enable)
            {
                if (check)
                {
                    paint.SetStyle(Paint.Style.Stroke);
                    paint.Color = Color.Blue;
                    paint.StrokeWidth = 20;
                    double grad = 360 * (timeToEnd - game.time) / timeToLife;
                    canvas.DrawArc(new RectF(pos.X - radius - 2, pos.Y - radius - 2, pos.X + radius + 2, pos.Y + radius + 2), 270, (int)Math.Round(grad), true, paint);
                    render.TextureDraw(canvas, new Rect(pos.X - radius, pos.Y - radius, pos.X + radius, pos.Y + radius), textureIdChecked);
                    SimpleButton_Check();
                }
                else
                {
                    render.TextureDraw(canvas, new Rect(pos.X - radius, pos.Y - radius, pos.X + radius, pos.Y + radius), textureId);
                }
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
            int proc = 1 / (game.random.Next(99) + 1);
            if (proc >= chanceToCheck && !check)
            {
                check = true;
                timeToEnd = game.time + timeToLife;
            }
        }

        public override void Deactivate()
        {
            check = false;
        }

        void SimpleButton_Check()
        {
            if (game.time > timeToEnd)
            {
                //Проигрышь
                check = false;
                var intent = new Intent(game.render.Context, typeof(LoseActivity));
                game.render.Context.StartActivity(intent);
            }
        }

        void SimpleButton_TouchUp(float x, float y)
        {
            if (
                    Math.Sqrt(
                        (pos.X - x) * (pos.X - x) +
                        (pos.Y - y) * (pos.Y - y)) <= radius && check
                    )
            {
                //Попадание по кнопке
                check = false;
                if (radius < 60) enable = false;
                game.ScoreInc();
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
                            (game.controls[i].pos.Y - y) * (game.controls[i].pos.Y - y)) <= radius * 2)
                        return false;
                }
            }
            return true;
        }
    }
}