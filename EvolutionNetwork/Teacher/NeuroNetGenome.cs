using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EvolutionNetwork.NeuroNet;

namespace EvolutionNetwork.Teacher
{
    public class NeuroNetGenome : IGenome<CNeuroNetWrapper>
    {
        public const double NodeComplexityCoefficient = 0.5;
        public const double ConnectionComplexityCoefficient = 1;
        public static Random GenomeRandom;

        int _nonStructuralMutations;

        CNeuroNetWrapper _neuroNet;

        int _totalnonStructuralMutations;

        int _structuralMutations;

        static NeuroNetGenome()
        {
            GenomeRandom = new Random();
        }

        public NeuroNetGenome(CNeuroNetWrapper network)
        {
            this._neuroNet = network;
            _nonStructuralMutations = 0;
            _totalnonStructuralMutations = 0;
            _structuralMutations = 0;
            LastResult = 0;
        }

        public double[] Calculate(double[] data)
        {
            return _neuroNet.Compute(data);
        }

        public NeuroNetGenome(NeuroNetGenome copy)
        {
            this._neuroNet = new CNeuroNetWrapper(copy._neuroNet);
            this._nonStructuralMutations = copy._nonStructuralMutations;
            this._structuralMutations = copy._structuralMutations;
            this._totalnonStructuralMutations = copy._totalnonStructuralMutations;
            this.LastResult = LastResult;
        }

        public double Complexity
        {
            get
            {
                return _neuroNet.Connections.Count * ConnectionComplexityCoefficient + _neuroNet.Nodes.Count * NodeComplexityCoefficient ;
            }
        }

        public double LastResult
        {
            get;

            set;
        }

        public int NonStructuralMutations
        {
            get
            {
                return _nonStructuralMutations;
            }
        }

        public double ParentResult
        {
            get;

            set;
        }

        public int StructuralMutations
        {
            get
            {
                return _structuralMutations;
            }
        }

        public int TotalNonStructuralMutations
        {
            get
            {
                return _totalnonStructuralMutations;
            }
        }

        public IGenome<CNeuroNetWrapper> Copy()
        {
            return new NeuroNetGenome(this);
        }

        public double GetRandom(int Precision)
        {
            return (GenomeRandom.NextDouble() - GenomeRandom.NextDouble()) / ((Math.Log(Precision + Math.E)+(_totalnonStructuralMutations+1))/2);
        }

        public IGenome<CNeuroNetWrapper> ProceedNonStructuralMutation()
        {
            NeuroNetGenome copy = new NeuroNetGenome(this);
            foreach(var c in copy._neuroNet.Connections)
            {
                do
                {
                    c.Weight += GetRandom(copy._nonStructuralMutations);
                } while (c.Weight > 2 || c.Weight < -2);
            }
            copy._nonStructuralMutations += 1;
            copy._totalnonStructuralMutations += 1;
            return copy;
        }

        public IGenome<CNeuroNetWrapper> ProceedStructuralMutation()
        {
            NeuroNetGenome copy = new NeuroNetGenome(this);
            CNeuroNetWrapper.Connection con = null;

            int maxCon = copy._neuroNet.Nodes.Count * (copy._neuroNet.Nodes.Count - copy._neuroNet.InputsCount);

            double chance = copy._neuroNet.Connections.Count / (double)maxCon;

            if (GenomeRandom.NextDouble() < chance )
            {
                do
                {
                    foreach (var c in copy._neuroNet.Connections)
                    {
                        if (GenomeRandom.NextDouble() < (1.0 / copy._neuroNet.Connections.Count))
                        {
                            con = c;
                            break;
                        }
                    }
                } while (con == null);

                int from = con.From;
                int to = con.To;

                copy._neuroNet.Connections.Remove(con);

                int nodeId = copy._neuroNet.Nodes.Count;
                copy._neuroNet.AddNode(nodeId);

                copy._neuroNet.AddConnection(from, nodeId, (GenomeRandom.NextDouble() - GenomeRandom.NextDouble()) * 2);
                copy._neuroNet.AddConnection(nodeId, to, (GenomeRandom.NextDouble() - GenomeRandom.NextDouble()) * 2);
            }else
            {
                int from = GenomeRandom.Next(copy._neuroNet.Nodes.Count);
                int to = GenomeRandom.Next(copy._neuroNet.InputsCount - 1, copy._neuroNet.Nodes.Count);

                copy._neuroNet.AddConnection(from, to, (GenomeRandom.NextDouble() - GenomeRandom.NextDouble()) * 2);
                
            }

            copy._structuralMutations += 1;
            copy._nonStructuralMutations = 0;

            return copy;
        }

        public void Flush()
        {
            _neuroNet.Flush();
        }
    }
}
