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

namespace App4
{
    public class GameMain
    {
        public Render render { get; set; }
        public GameControl[] controls { get; set; }
        public Random random { get; set; }
        public double time { get; set; }
        public int score { get; set; }
        public int level { get; set; }

        public GameMain()
        {
            controls = new GameControl[20];
            random = new Random();
            time = 0;
            score = 0;
            level = 0;
        }

        public void Game(Render ren)
        {
            //Тут логика левелов
            if (render != null)
            {
                switch (score)
                {
                    case 0:
                        render.SetTimeToShowLevel(time + 3);
                        controls[0] = new SimpleButton(this);
                        ScoreInc();
                        break;
                    case 5:
                        controls[1] = new SimpleButton(this);
                        ScoreInc();
                        break;
                    case 10:
                        controls[2] = new SimpleButton(this);
                        ScoreInc();
                        break;
                    case 15:
                        controls[3] = new SimpleButton(this);
                        ScoreInc();
                        break;
                    case 20:
                        controls[4] = new SimpleButton(this);
                        ScoreInc();
                        break;
                    case 25:
                        controls[5] = new SimpleButton(this);
                        ScoreInc();
                        break;
                    case 30:
                        controls[6] = new SimpleButton(this);
                        ScoreInc();
                        break;
                    case 35:
                        controls[7] = new SimpleButton(this);
                        ScoreInc();
                        break;
                    case 40:
                        LevelInc();
                        DeactivateAll();
                        SimpleButton.SetRadius(100);
                        SimpleButton.SetTimeToLife(2);
                        render.SetTimeToShowLevel(time + 3);
                        ScoreInc();
                        break;
                    case 45:
                        controls[8] = new SimpleButton(this);
                        ScoreInc();
                        break;
                    case 50:
                        controls[9] = new SimpleButton(this);
                        ScoreInc();
                        break;
                    case 55:
                        controls[10] = new SimpleButton(this);
                        ScoreInc();
                        break;
                    case 60:
                        controls[11] = new SimpleButton(this);
                        ScoreInc();
                        break;
                    case 65:
                        controls[12] = new SimpleButton(this);
                        ScoreInc();
                        break;
                    case 70:
                        controls[13] = new SimpleButton(this);
                        ScoreInc();
                        break;
                    case 75:
                        controls[14] = new SimpleButton(this);
                        ScoreInc();
                        break;
                    case 80:
                        LevelInc();
                        DeactivateAll();
                        render.SetTimeToShowLevel(time + 3);
                        ScoreInc();
                        break;
                    default:
                        break;
                }
            }
        }

        void LevelInc()
        {
            level++;
        }

        public void DeactivateAll()
        {
            if (controls != null)
                for (int i = 0; i < controls.Length; i++)
                {
                    if (controls[i] != null) controls[i].Deactivate();
                }
        }

        public void ScoreInc()
        {
            score++;
        }

        public void ScoreInc(int score)
        {
            this.score = this.score + score;
        }
    }
}