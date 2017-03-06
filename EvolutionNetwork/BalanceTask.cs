using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EvolutionNetwork.Teacher;

namespace EvolutionNetwork
{
    public partial class BalanceTask : Control
    {

        System.Threading.Timer timer = null;
        Tester t = null;
        NeuroNetGenome gen = null;
        Random r = null;

        Pen pen = null;

        public BalanceTask()
        {
            InitializeComponent();
            r = new Random();
            pen = new Pen(Brushes.Red);
            pen.Width = 4.0f;
        }

        const double DrawingLengthCoeff = 5.0;

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            if (t != null && gen != null)
            {
                Size s1 = new Size(this.Width / 10, 10);
                Size sh = new Size(s1.Width/2,s1.Height);
                Point p1 = new Point((int)(((t.Position + Tester.PositionLimit) / (Tester.PositionLimit * 2)) * this.Width), this.Height - 100);


                Rectangle rect = new Rectangle(p1-sh,s1);
                pen.Color = Color.Pink;
                pe.Graphics.DrawRectangle(pen, rect);

                //TODO: Fix render.

                pen.Color = Color.Red;

                p1.X = p1.X + sh.Width;
                p1.Y = p1.Y - sh.Height; 
                Point p2 = new Point((int)(p1.X + (Math.Sin(t.Rotation)*t.Length* DrawingLengthCoeff)),(int)(p1.Y - (Math.Cos(t.Rotation)*t.Length* DrawingLengthCoeff)));
                pe.Graphics.DrawLine(pen,p1 , p2);

                p1.X = p1.X - sh.Width*2;
                p2 = new Point((int)(p1.X + (Math.Sin(t.Rotation2) * t.Length2 * DrawingLengthCoeff)), (int)(p1.Y - (Math.Cos(t.Rotation2) * t.Length2 * DrawingLengthCoeff)));
                pe.Graphics.DrawLine(pen, p1, p2);
            }
        }

        public void SetAGenome(NeuroNetGenome g)
        {
            if (gen == null)
            {
                gen = new NeuroNetGenome(g);
                gen.Flush();
            }
            else
            {
                lock (gen)
                {
                    gen = new NeuroNetGenome(g);
                    gen.Flush();
                }
            }
            GenerateNewTest();
            if (timer == null)
            {
                timer = new System.Threading.Timer(TimerTick, null, 0, 20);
            }
        }

        void GenerateNewTest()
        {
            t = new Tester(
                (r.NextDouble() - r.NextDouble()) * (Math.PI / 3),
                (r.NextDouble() + 1)*5 + 1,

                (r.NextDouble() - r.NextDouble()) * (Math.PI / 3),
                (r.NextDouble() + 1)*5 + 1,

                (r.NextDouble() + 1) * 5,
                5);
            gen.Flush();
        }

        void TimerTick(object state)
        {
            lock (gen)
            {
                double[] r = gen.Calculate(new double[] { t.Rotation, t.Position });
                double cont = (r[0] - 0.5) * 2;
                t.Update(0.02, cont);
                if (t.TotalTime > 10.0)
                {
                    GenerateNewTest();
                }
            }

            this.Invoke(
                new Action(delegate ()
                {
                    this.Refresh();
                })
            );

        }

    }
}
