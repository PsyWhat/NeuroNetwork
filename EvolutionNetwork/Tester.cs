using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvolutionNetwork.Teacher;
using EvolutionNetwork.NeuroNet;

namespace EvolutionNetwork.Teacher
{
    public class Tester
    {
        double rotation;
        double length;
        double totalTime;
        double controllerSpeed;
        double position;
        public const double PositionLimit = 20.0;
        double grav;

        public double Rotation
        {
            get { return rotation; }
        }
        public double Position
        {
            get { return position; }
            set { position = value; }
        }
        public double TotalTime
        {
            get { return totalTime; }
            set { totalTime = value; }
        }
        public double Length
        {
            get { return length; }
        }
        public void Update(double timeDiff, double controller)
        {
            if (timeDiff > 0)
            {
                double pp = position;
                position = Math.Max(-PositionLimit, Math.Min(position + (controller * controllerSpeed) * timeDiff, PositionLimit));

                rotation += Math.Sin(rotation) * timeDiff * grav - Math.Cos(rotation) * (pp - position);

                totalTime += timeDiff;
            }
        }

        public Tester(double InitialRotation, double Lenght, double controllerSpeed, double gravityCoef)
        {
            this.rotation = InitialRotation;
            this.length = Lenght;
            this.totalTime = 0.0;
            this.position = 0.0;
            this.grav = gravityCoef;
        }


        public static double TestFunction(NeuroNetGenome gen)
        {
            double result = 0.0;
            const int maxTests = 10;
            const double timestep = 0.02;
            for (int i = 0; i < maxTests; ++i)
            {
                gen.Flush();
                Random r = new Random(123454321 + i);
                double difficultyCoef = i / (maxTests - 1.0);
                Tester t = new Tester((r.NextDouble() - r.NextDouble()) * (Math.PI / 3) * (difficultyCoef + 0.1), r.NextDouble() + 1,
                    10 * (r.NextDouble()+1) *(1.1-difficultyCoef), (difficultyCoef + 1) * r.NextDouble());

                do
                {
                    double[] calcs = gen.Calculate(new double[] { t.rotation, t.position});

                    double cont = (calcs[0] - 0.5) * 2;

                    t.Update(timestep, cont);
                } while (t.totalTime < 50.0 && Math.Abs(t.rotation) < (Math.PI / 2));
                result += t.totalTime;
            }


            return result;
        }
    }
}
