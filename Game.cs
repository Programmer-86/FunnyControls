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

        public int sum { get; set; }
        public int showSum;

        public int minValue;
        public int maxValue;
        public int maxClick;
        public int lastScore;



        public GameMain()
        {
            random = new Random();
            controls = new GameControl[4];
            time = 0;
            ResumeValue();
        }

        public void ResumeValue()
        {
            sum = 0;
            score = 0;
            level = 0;
            lastScore = 0;
            RecountVar();
            for (int i = 1; i < controls.Length; i++)
            {
                controls[i] = null;
            }
            Game(render);
        }

        void RecountVar()
        {
            minValue = 1 + level * 5;
            maxValue = 10 + level * level;
            maxClick = 5 + level / 2;
        }

        int GetNullControlIndex()
        {
            for (int i = 1; i < controls.Length; i++)
            {
                if (controls[i] == null || !controls[i].enable) return i;
            }

            int index = 1;
            for (int i = 2; i < controls.Length; i++)
            {
                if (controls[index].GetValue() > controls[i].GetValue()) index = i;
            }
            return index;
        }

        public void Game(Render ren)
        {
            //Тут логика левелов
            if (render != null)
            {
                switch (score)
                {
                    case 0:
                        controls[0] = new MainButton(this);
                        controls[GetNullControlIndex()] = new SimpleButton(this);
                        controls[GetNullControlIndex()] = new SimpleButton(this);
                        ScoreInc();
                        break;
                    default:
                        break;
                }

                if (score >= lastScore + 100)
                {
                    level++;
                    RecountVar();
                    lastScore = score;
                    controls[GetNullControlIndex()] = new SimpleButton(this);
                    controls[0].SetValue(0);
                    controls[0].GenerateEvent();
                }
            }
        }

        void LevelInc()
        {
            level++;
            render.SetTimeToShowLevel(time + 3);
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