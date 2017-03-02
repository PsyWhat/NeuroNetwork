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

        double record = 0.0;

        public double Test(CSNeuroNet nn)
        {

            Vector2 pos = new Vector2(0.0, 10.0);
            Vector2 vel = new Vector2((double)(( 0.5) * 5), 0.0);
            Vector2 player = new Vector2(0.0, 0.0);
            Vector2 grav = new Vector2(0.0, -30.0);
            Vector2 controller = new Vector2(10.0, 0.0);
            double time = 0.04;

            nn.Flush();

            double overall = 0.0;




            int hj = 0;

            int max = 75;

            while (hj++ < max)
            {
                Random r = new Random(hj + 4723);
                Random rad = new Random(hj + 1234);
                Func<double, double, double> randF = (x, y) =>
                  {
                      return (double)(rad.NextDouble() * ((double)y - x) + x);
                  };

                double fr = randF(-2,2);
                double fr2 = randF(-2, 2);

                controller = new Vector2(17.0, 0.0);
                pos = new Vector2( randF(-5,5) , randF(3,20) );

                int diff = hj / 4;
                int maxDiff = max / 4;
                double diffC = (diff / (double)maxDiff);


                vel = new Vector2( randF(-5,5) + randF(-1,1)* diffC*controller.x, randF(-1,1) + randF(-4,0)*diffC );
                player = new Vector2(0.0, 0.0);
                grav = new Vector2(0.0, -35.0);
                double curTim = 0;
                while (true)
                {
                    var res = nn.Calculate(new double[] { player.x, player.y, pos.x, pos.y, vel.x, vel.y, time, controller.x , grav.y, -10.0,10.0, -0.5,0.5 });


                    if (res[0] > 0.6)
                    {
                        player -= controller * time;
                    }
                    if (res[1] > 0.6)
                    {
                        player += controller * time;
                    }
                    if (player.x > 10.0)
                    {
                        player.x = 10.0;
                    }
                    else if (player.x < -10.0)
                    {
                        player.x = -10.0;
                    }

                    vel = vel + grav * time;
                    pos += vel * time;
                    if (pos.x > 10.0)
                    {
                        pos.x = 10.0;
                        vel.x = -vel.x;
                    }
                    else if (pos.x < -10.0)
                    {
                        pos.x = -10.0;
                        vel.x = -vel.x;
                    }

                    if (pos.y < 0.0)
                    {
                        if ((pos.x < player.x + 0.5) && (pos.x > player.x - 0.5))
                        {
                            pos.y = 0.0;
                            vel.y = -vel.y;
                            vel.x = vel.x + (pos.x - player.x) + (double)((r.NextDouble() - 0.5) * 2);
                        }
                        else
                        {
                            break;
                        }
                    }



                    curTim += time;
                    if ((pos.x < player.x + 0.5) && (pos.x > player.x - 0.5))
                    {
                        curTim += time * 2;
                    }
                    if (curTim > 40.0)
                    {
                        break;
                    }
                }
                overall += curTim * diffC;
            }


            if (overall > record)
            {
                record = overall;
                Console.WriteLine("New record! {0:F5} by net: {1}", record, nn);
            }

            return overall;
        }

        public CSNeuroNet neuro;

        public bool pause;


        public Teacher(int seed, int individuals, double dispersion, double a, double b)
        {
            this.ET = new EvolutionTeaching(13, 2, seed, individuals, Test, dispersion, a, b);
        }

        public Dictionary<int, Tuple<double, CSNeuroNet>> BestFromPasses;

        // Use this for initialization
        public void Start(int passes)
        {
            BestFromPasses = new Dictionary<int, Tuple<double, CSNeuroNet>>();
            ET.Init();

            for (int i = 0; i < passes; ++i)
            {
                bool wasP = false;
                Console.WriteLine("Starting pass {0}", i);
                ET.PassGeneration();

                ET.LastResult.Sort((x, y) =>
                {
                    if (x.a > y.a)
                    {
                        return 1;
                    }
                    else if (x.a < y.a)
                    {
                        return -1;
                    }
                    else
                    {
                        return 0;
                    }
                });
                var max = ET.LastResult[ET.LastResult.Count-1];

                BestFromPasses.Add(i, new Tuple<double, CSNeuroNet>(max.a,max.b));
                Console.WriteLine("Minimum is: {0:F5}, Maximum: {1:F5}, medium: {2:F5}", ET.LastResult.Min(x => x.a), max.a, ET.LastResult.ConvertAll(x => x.a).Average(x => x));
                Console.WriteLine("Best net: {0}", max.b);
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