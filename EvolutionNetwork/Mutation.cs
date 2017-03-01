using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EvolutionNetwork
{
    public enum MutationType
    {
        WeightChange,
        AddNode,
        AddConnection,
    }

    public struct Mutation
    {
        public MutationType Type;

        public double PrevParentFun;
        public int PreviousAge;
        public Connection RemovedConnection;
        public Node AdditionalNode;
        public List<Tuple<Connection,double>> WeightChanges;
        public Connection NewConnection;
        public Connection NewConnection2;
    }
}
