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


        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            if (t != null && gen != null)
            {
                double wc = (Tester.PositionLimit * 2) / this.Width;
                double mid = (this.Width / 2.0) * wc;
                Rectangle rect = new Rectangle((int)(((t.Position + Tester.PositionLimit) / (Tester.PositionLimit * 2)) * this.Width), this.Height - 10, this.Width / 10, 10);
                pen.Color = Color.Pink;
                pe.Graphics.DrawRectangle(pen, rect);

                //TODO: Fix render.

                pen.Color = Color.Red;
                Point p1 = new Point((int)(((t.Position + Tester.PositionLimit) / (Tester.PositionLimit * 2)) * this.Width) + this.Width / 20, this.Height - 10);
                Point p2 = new Point((int)(((t.Position + Tester.PositionLimit) / (Tester.PositionLimit * 2)) * this.Width) + this.Width / 20 + (int)( Math.Sin(t.Rotation) * t.Length * 10), this.Height - (int)(Math.Cos(t.Rotation) * t.Length * 10));
                pe.Graphics.DrawLine(pen,p1 , p2); jksadhfkjsadlfkjgaskjhfgksjadghkfajsgfkjh
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
            if (timer == null)
            {
                GenerateNewTest();
                timer = new System.Threading.Timer(TimerTick, null, 0, 20);
            }
        }

        void GenerateNewTest()
        {
            t = new Tester((r.NextDouble() - r.NextDouble()) * (Math.PI / 3),
                (r.NextDouble() + 1), (r.NextDouble() + 1) * 5, 1);
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
