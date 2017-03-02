using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvolutionNetwork.NeuroNet;

namespace EvolutionNetwork
{
    public class TestProgram
    {
        static void Main(string []args)
        {

            Random r = new Random(21341);

            int tstIC = 2400;
            int tstOC = 1000;

            int addN = 4000;
            int addC = 100000;

            double[] calcVals = new double[tstIC];
            
            for(int ij = 0;ij<calcVals.Length;++ij)
            {
                calcVals[ij] = r.NextDouble();
            }

            r = new Random(4242421);

            CNeuroNetWrapper net = new CNeuroNetWrapper(tstIC, tstOC);

            for(int i = 0;i < addN;++i)
            {
                net.Nodes.Add(new CNeuroNetWrapper.Node(net.InputsCount + net.OutputsCount + i));
            }
            for(int i = 0; i< addC;++i)
            {
                net.Connections.Add(new CNeuroNetWrapper.Connection(r.Next(net.Nodes.Count), r.Next(net.Nodes.Count), (r.NextDouble() - 0.5) * 4));
            }

            DateTime bcsC = DateTime.Now;

            double[] cRes = net.Compute(calcVals);

            DateTime aftC = DateTime.Now;

            var elapcedC = new TimeSpan(aftC.Ticks) - new TimeSpan(bcsC.Ticks);

            Console.WriteLine(elapcedC);


            r = new Random(4242421);

            CSNeuroNet snet = new CSNeuroNet(tstIC, tstOC);

            for (int i = 0; i < addN; ++i)
            {
                snet.AddNode(snet.Outputs + snet.Inputs + i);
            }
            for (int i = 0; i < addC; ++i)
            {
                snet.AddConnection(new Connection(13124,r.Next(net.Nodes.Count), r.Next(net.Nodes.Count), (r.NextDouble() - 0.5) * 4));
            }

            DateTime bcsCS = DateTime.Now;


            double[] cRes1 = snet.Calculate(calcVals);
            

            DateTime aftCS = DateTime.Now;

            var elapcedCS = new TimeSpan(aftCS.Ticks) - new TimeSpan(bcsCS.Ticks);
            Console.WriteLine(elapcedCS);


            /*for (int k = 0; k<100;++k)
            {
                CNeuroNetWrapper s = new CNeuroNetWrapper(1024, 2048);

                lock (nets)
                {
                    nets.Add(s);
                }
            }

            nets.Clear();
            GC.Collect(); */

            int asd = 0;

            while (true)
            {
                asd = asd + 1 + r.Next();
            }

        }
    }
}
