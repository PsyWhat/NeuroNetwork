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
        double controllerSpeed = 10.0;
        double position;
        public const double PositionLimit = 20.0;
        double grav;
        double avel;
        double rotation2;
        double avel2;
        double length2;
        public double Length
        {
            get { return length; }
        }
        public double Rotation
        {
            get { return rotation; }
        }
        public double Length2
        {
            get { return length2; }
        }
        public double Rotation2
        {
            get { return rotation2; }
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
        public void Update(double timeDiff, double controller)
        {
            if (timeDiff > 0)
            {
                double pp = position;
                double pdiff = controller * controllerSpeed * timeDiff;
                position = Math.Max(-PositionLimit, Math.Min(position + pdiff, PositionLimit));

                double rdiff = Math.Sin(rotation) * timeDiff * grav + Math.Cos(rotation) * (pp - position) * length;
                double rdiff2 = Math.Sin(rotation2) * timeDiff * grav + Math.Cos(rotation2) * (pp - position) * length2;
                avel += rdiff;
                avel2 += rdiff2;
                rotation += avel * timeDiff;
                rotation2 += avel2 * timeDiff;

                totalTime += timeDiff;
            }
        }

        public Tester(double InitialRotation, double Lenght, double InitialRotation2, double Length2, double controllerSpeed, double gravityCoef)
        {
            this.rotation = InitialRotation;
            this.length = Lenght;
            this.totalTime = 0.0;
            this.position = 0.0;
            this.grav = gravityCoef;
            this.controllerSpeed = controllerSpeed;
            this.avel = 0.0;

            this.length2 = Length2;
            this.rotation2 = InitialRotation2;
            this.avel2 = 0.0;

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
                Tester t = new Tester(
                    (r.NextDouble() - r.NextDouble()) * (Math.PI / 3) * (difficultyCoef + 0.1),
                    r.NextDouble() * 5 + 1,

                    (r.NextDouble() - r.NextDouble()) * (Math.PI / 4) * (difficultyCoef + 0.1),
                    r.NextDouble() * 5 + 1,

                    10 * (r.NextDouble() + 1) * (1.1 - difficultyCoef),
                    (difficultyCoef + 1) * (r.NextDouble() + 1) * 5);

                do
                {
                    double[] calcs = gen.Calculate(new double[] { t.rotation, t.rotation2, t.position, });

                    double cont = (calcs[0] - 0.5) * 2;

                    t.Update(timestep, cont);
                } while (t.totalTime < 50.0 && Math.Abs(t.rotation) < (Math.PI / 2) && Math.Abs(t.rotation2) < (Math.PI / 2));
                result += t.totalTime;
            }


            return result;
        }
    }
}
