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

        CNeuroNetWrapper _neuroNet;

        public NeuroNetGenome(CNeuroNetWrapper network)
        {
            this._neuroNet = network;
        }
        public NeuroNetGenome(NeuroNetGenome copy)
        {
            this._neuroNet = new CNeuroNetWrapper(copy._neuroNet);
            this._nonStructuralMutations = copy._nonStructuralMutations;
            this._structuralMutations = copy._structuralMutations;
            this._totalnonStructuralMutations = copy._totalnonStructuralMutations;
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

        int _nonStructuralMutations;

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

        int _structuralMutations;

        public int TotalNonStructuralMutations
        {
            get
            {
                return _totalnonStructuralMutations;
            }
        }

        int _totalnonStructuralMutations;

        public IGenome<CNeuroNetWrapper> Copy()
        {
            return new NeuroNetGenome(this);
        }

        public IGenome<CNeuroNetWrapper> ProceedNonStructuralMutation()
        {
            throw new NotImplementedException();
        }

        public IGenome<CNeuroNetWrapper> ProceedStructuralMutation()
        {
            throw new NotImplementedException();
        }
    }
}
