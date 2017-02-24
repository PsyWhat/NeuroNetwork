using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.Linq;

namespace EvolutionNetwork
{
    public class Teacher
    {

        public EvolutionTeaching ET;

        float record = 0.0f;

        float Test(NeuroNet nn)
        {
            Random r = new Random();

            Vector2 pos = new Vector2(0.0f, 10.0f);
            Vector2 vel = new Vector2((float)((r.NextDouble() - 0.5) * 5), 0.0f);
            Vector2 player = new Vector2(0.0f, 0.0f);
            Vector2 grav = new Vector2(0.0f, -30.0f);
            Vector2 controller = new Vector2(20.0f, 0.0f);
            float time = 0.05f;

            nn.Flush();

            float overall = 0.0f;

            while (true)
            {

                var res = nn.Calculate(new float[] { player.x, player.y, pos.x, pos.y, vel.x, vel.y, time });


                if (res[0] > 0.6f)
                {
                    player -= controller * time;
                }
                if (res[1] > 0.6f)
                {
                    player += controller * time;
                }
                if (player.x > 10.0f)
                {
                    player.x = 10.0f;
                }
                else if (player.x < -10.0f)
                {
                    player.x = -10.0f;
                }

                vel = vel + grav * time;
                pos += vel * time;
                if (pos.x > 10.0f)
                {
                    pos.x = 10.0f;
                    vel.x = -vel.x;
                }
                else if (pos.x < -10.0f)
                {
                    pos.x = -10.0f;
                    vel.x = -vel.x;
                }

                if (pos.y < 0.0f)
                {
                    if ((pos.x < player.x + 0.5f) && (pos.x > player.x - 0.5f))
                    {
                        pos.y = 0.0f;
                        vel.y = -vel.y;
                        vel.x = vel.x + (pos.x - player.x) + (float)((r.NextDouble() - 0.5) * 2);
                    }
                    else
                    {
                        break;
                    }
                }



                overall += time;
                if ((pos.x < player.x + 0.5f) && (pos.x > player.x - 0.5f))
                {
                    overall += time * 2;
                }


                if (overall > 100000.0f)
                {
                    Console.WriteLine("############################New record!#################################");
                    break;
                }
            }

            pos = new Vector2(0.0f, 10.0f);
            vel = new Vector2((float)((r.NextDouble() - 0.5) * 5), 0.0f);
            player = new Vector2(0.0f, 0.0f);
            grav = new Vector2(0.0f, -30.0f);
            controller = new Vector2(20.0f, 0.0f);

            while (true)
            {

                var res = nn.Calculate(new float[] { player.x, player.y, pos.x, pos.y, vel.x, vel.y, time });


                if (res[0] > 0.6f)
                {
                    player -= controller * time;
                }
                if (res[1] > 0.6f)
                {
                    player += controller * time;
                }
                if (player.x > 10.0f)
                {
                    player.x = 10.0f;
                }
                else if (player.x < -10.0f)
                {
                    player.x = -10.0f;
                }

                vel = vel + grav * time;
                pos += vel * time;
                if (pos.x > 10.0f)
                {
                    pos.x = 10.0f;
                    vel.x = -vel.x;
                }
                else if (pos.x < -10.0f)
                {
                    pos.x = -10.0f;
                    vel.x = -vel.x;
                }

                if (pos.y < 0.0f)
                {
                    if ((pos.x < player.x + 0.5f) && (pos.x > player.x - 0.5f))
                    {
                        pos.y = 0.0f;
                        vel.y = -vel.y;
                        vel.x = vel.x + (pos.x - player.x) + (float)((r.NextDouble() - 0.5) * 2);
                    }
                    else
                    {
                        break;
                    }
                }



                overall += time;
                if ((pos.x < player.x + 0.5f) && (pos.x > player.x - 0.5f))
                {
                    overall += time * 2;
                }
                if (overall > 10000000.0f)
                {
                    Console.WriteLine("############################New record!#################################");
                    break;
                }
            }


            if (overall > record)
            {
                record = overall;
                Console.WriteLine("New record! {0} by net: {1}", record, nn);
            }

            return overall;
        }
        public NeuroNet neuro;

        public bool pause;

        // Use this for initialization
        public void Start(int seed, int passes, int individuals, float dispersion, float a, float b)
        {
            this.ET = new EvolutionTeaching(2 + 2 + 2 + 1, 2, seed, individuals, Test, dispersion,a,b);

            ET.Init();

            for (int i = 0; i < passes; ++i)
            {
                bool wasP = false;
                Console.WriteLine("Starting pass {0}", i);
                ET.PassGeneration();
                Console.WriteLine("Minimum is: {0}, Maximum: {1}, medium: {2}", ET.LastResult.Min(x => x.a), ET.LastResult.Max(x => x.a), ET.LastResult.ConvertAll(x => x.a).Average(x => x));
                Console.WriteLine("Pass {0} have been passed", i);

                if (pause)
                {
                    Console.WriteLine("Pausing...");
                    wasP = true;
                }
                while (pause)
                {
                    Thread.Sleep(100);
                }
                if (wasP)
                {
                    Console.WriteLine("Continueing");
                }
            }

            neuro = ET[ET.Individuals - 1];
            while (true)
            {

            }
        }

    }

}