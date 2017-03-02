using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections;

namespace EvolutionNetwork.NeuroNet
{
    public class CNeuroNetWrapper : IDisposable
    {


        public struct Node
        {
            public int ID
            {
                get
                {
                    return _id;
                }
            }

            int _id;

            public Node(int ID)
            {
                _id = ID;
            }

        }

        public class NodesCollection : ICollection<Node>,  IEnumerable<Node>
        {
            public class NodeEnumerator : IEnumerator<Node>
            {
                IntPtr _enumPtr = IntPtr.Zero;

                protected NodeEnumerator(IntPtr enumPointer)
                {
                    _enumPtr = enumPointer;
                }

                public Node Current
                {
                    get
                    {
                        return new Node(CNeuroNetWrapperFunctions.__GetNodeID(CNeuroNetWrapperFunctions.__GetEnumeratorCurrent(_enumPtr)));
                    }
                }

                object IEnumerator.Current
                {
                    get
                    {
                        return new Node(CNeuroNetWrapperFunctions.__GetNodeID(CNeuroNetWrapperFunctions.__GetEnumeratorCurrent(_enumPtr)));
                    }
                }

                public void Dispose()
                {
                    Marshal.FreeHGlobal(_enumPtr);
                    _enumPtr = IntPtr.Zero;
                }

                public bool MoveNext()
                {
                    return CNeuroNetWrapperFunctions.__EnumeratorGoNext(_enumPtr) != 0;
                }

                public void Reset()
                {
                    CNeuroNetWrapperFunctions.__ResetEnumerator(_enumPtr);
                }

                ~NodeEnumerator()
                {
                    Dispose();
                }
            }

            private class NEC:NodeEnumerator
            {
                public NEC(IntPtr ptr):base(ptr)
                {

                }
            }


            protected NodesCollection(IntPtr link)
            {
                net = link;
            }
            

            public static NodesCollection GetLinkedConnection(IntPtr link)
            {
                NodesCollection c = null;

                if(link != null)
                {
                    c = new NodesCollection(link);
                }


                return c;
            }


            IntPtr net;
            public int Count
            {
                get
                {
                    return CNeuroNetWrapperFunctions.__GetNodesCount(net);
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            public void Add(Node item)
            {
                CNeuroNetWrapperFunctions.__AddNode(net, item.ID);
            }

            public void Clear()
            {
                CNeuroNetWrapperFunctions.__ClearNodes(net);
            }

            public bool Contains(Node item)
            {
                return CNeuroNetWrapperFunctions.__FindNode(net, item.ID) != IntPtr.Zero;
            }


            public void CopyTo(Node[] array, int arrayIndex)
            {
                int i = 0;
                foreach (var n in this)
                {
                    array[arrayIndex + i++] = n;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new NEC(CNeuroNetWrapperFunctions.__GetEnumerator(CNeuroNetWrapperFunctions.__GetNodes(net)));
            }

            public IEnumerator<Node> GetEnumerator()
            {
                return new NEC(CNeuroNetWrapperFunctions.__GetEnumerator(CNeuroNetWrapperFunctions.__GetNodes(net)));
            }

            public bool Remove(Node item)
            {
                return CNeuroNetWrapperFunctions.__RemoveNode(net, item.ID) != 0;
            }
        }

        public class Connection
        {
            public int From
            {
                get
                {
                    if (_link != IntPtr.Zero)
                    {
                        return CNeuroNetWrapperFunctions.__GetConnectionFrom(_link);
                    }
                    else
                    {
                        return _from;
                    }
                }
                set
                {
                    if (_link != IntPtr.Zero)
                    {
                        int to = CNeuroNetWrapperFunctions.__GetConnectionTo(_link);
                        int from = CNeuroNetWrapperFunctions.__GetConnectionFrom(_link);
                        double w = CNeuroNetWrapperFunctions.__GetWeight(_link);
                        CNeuroNetWrapperFunctions.__RemoveConnection(_neuroNet,from,to);
                        _link = CNeuroNetWrapperFunctions.__AddConnection(_neuroNet, value, to, w);
                        if (_link == IntPtr.Zero)
                        {
                            throw new ArgumentNullException("Node with that ID does not exists in CNeuroNet");
                        }
                    }
                    else
                    {
                        _from = value;
                    }
                }
            }
            public int To
            {
                get
                {
                    if (_link != IntPtr.Zero)
                    {
                        return CNeuroNetWrapperFunctions.__GetConnectionTo(_link);
                    }
                    else
                    {
                        return _to;
                    }
                }
                set
                {
                    if (_link != IntPtr.Zero)
                    {
                        /*
                            CNeuroNetWrapperFunctions.__SetConnectionFromTo(_link, CNeuroNetWrapperFunctions.__GetConnectionFrom(_link), value);
                        */
                        int to = CNeuroNetWrapperFunctions.__GetConnectionTo(_link);
                        int from = CNeuroNetWrapperFunctions.__GetConnectionFrom(_link);
                        double w = CNeuroNetWrapperFunctions.__GetWeight(_link);
                        CNeuroNetWrapperFunctions.__RemoveConnection(_neuroNet, from, to);
                        _link = CNeuroNetWrapperFunctions.__AddConnection(_neuroNet, from, value, w);
                        if(_link == IntPtr.Zero)
                        {
                            throw new ArgumentNullException("Node with that ID does not exists in CNeuroNet");
                        }

                    }
                    else
                    {
                        _to = value;
                    }
                }
            }

            public double Weight
            {
                get
                {
                    if (_link != IntPtr.Zero)
                    {
                        return CNeuroNetWrapperFunctions.__GetWeight(_link);
                    }
                    else
                    {
                        return _weight;
                    }
                }
                set
                {
                    if (_link != IntPtr.Zero)
                    {
                        CNeuroNetWrapperFunctions.__ChangeWeight(_link, value);
                    }
                    else
                    {
                        _weight = value;
                    }
                }
            }

            int _from;
            int _to;
            double _weight;
            IntPtr _link;
            IntPtr _neuroNet;

            public Connection(int from, int to, double weight)
            {
                this._from = from;
                this._to = to;
                this._weight = weight;
                this._link = IntPtr.Zero;
            }

            Connection(IntPtr con, IntPtr net)
            {
                _link = con;
                _neuroNet = net;
            }

            public static Connection GetLinkedConnection(IntPtr connectionPointer, IntPtr neuroNetPointer)
            {
                Connection c = null;

                if (connectionPointer != IntPtr.Zero)
                {
                    c = new Connection(connectionPointer,neuroNetPointer);
                }

                return c;
            }

        }


        public class ConnectionsCollection : ICollection<Connection>, IEnumerable<Connection>
        {

            protected ConnectionsCollection(IntPtr link)
            {
                net = link;
            }

            public static ConnectionsCollection GetLinkedCollection(IntPtr ptr)
            {
                ConnectionsCollection c = null;

                if(ptr != IntPtr.Zero)
                {
                    c = new ConnectionsCollection(ptr);
                }

                return c;
            }
            IntPtr net;
            public int Count
            {
                get
                {
                    return CNeuroNetWrapperFunctions.__GetConnectionsCount(net);
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            public void Add(Connection item)
            {
                CNeuroNetWrapperFunctions.__AddConnection(net, item.From, item.To, item.Weight);
            }

            public void Clear()
            {
                CNeuroNetWrapperFunctions.__ClearConnections(net);
            }

            public bool Contains(Connection item)
            {
                return CNeuroNetWrapperFunctions.__FindConnection(net, item.From, item.To) != IntPtr.Zero;
            }

            public void CopyTo(Connection[] array, int arrayIndex)
            {
                int i = 0;
                foreach (var v in this)
                {
                    array[i] = new Connection(v.From, v.To, v.Weight);
                }
            }

            public class ConnectionsEnumerator : IEnumerator<Connection>
            {
                IntPtr enumPtr;
                IntPtr neuroNetPtr;

                protected ConnectionsEnumerator(IntPtr ptr, IntPtr neuroNetPtr)
                {
                    this.enumPtr = ptr;
                    this.neuroNetPtr = neuroNetPtr;
                }

                public Connection Current
                {
                    get
                    {
                        return Connection.GetLinkedConnection(CNeuroNetWrapperFunctions.__GetEnumeratorCurrent(enumPtr),neuroNetPtr);
                    }
                }

                object IEnumerator.Current
                {
                    get
                    {
                        return Current;
                    }
                }

                public void Dispose()
                {
                    Marshal.FreeHGlobal(enumPtr);
                }

                public bool MoveNext()
                {
                    return CNeuroNetWrapperFunctions.__EnumeratorGoNext(enumPtr) != 0;
                }

                public void Reset()
                {
                    CNeuroNetWrapperFunctions.__ResetEnumerator(enumPtr);
                }
                ~ConnectionsEnumerator()
                {
                    Dispose();
                }
            }

            private class CEP : ConnectionsEnumerator
            {
                public CEP(IntPtr ptr, IntPtr nnPtr) : base(ptr,nnPtr)
                {

                }
            }



            public IEnumerator<Connection> GetEnumerator()
            {
                return new CEP(CNeuroNetWrapperFunctions.__GetEnumerator(CNeuroNetWrapperFunctions.__GetConnections(net)),net);
            }

            public bool Remove(Connection item)
            {
                return CNeuroNetWrapperFunctions.__RemoveConnection(net, item.From, item.To) != 0;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new CEP(CNeuroNetWrapperFunctions.__GetEnumerator(CNeuroNetWrapperFunctions.__GetConnections(net)), net);
            }
        }








        private IntPtr _cnet;

        public int InputsCount
        {
            get
            {
                return _numInputs;
            }
        }
        public int OutputsCount
        {
            get
            {
                return _numOutputs;
            }
        }

        

        int _numInputs;
        int _numOutputs;

        ConnectionsCollection _connections;

        NodesCollection _nodes;
       


        public ConnectionsCollection Connections
        {
            get
            {
                return _connections;
            }
        }

        public NodesCollection Nodes
        {
            get
            {
                return _nodes;
            }
        }

        public CNeuroNetWrapper(int numInputs, int numOutputs)
        {
            _cnet = CNeuroNetWrapperFunctions.__NewNeuroNet(numInputs, numOutputs);
            _numInputs = numInputs;
            _numOutputs = numOutputs;
            _connections = ConnectionsCollection.GetLinkedCollection(_cnet);
            _nodes = NodesCollection.GetLinkedConnection(_cnet);
        }

        public CNeuroNetWrapper(CNeuroNetWrapper copy)
        {

            _numInputs = copy._numInputs;
            _numOutputs = copy._numOutputs;

            _cnet = CNeuroNetWrapperFunctions.__NewNeuroNet(_numInputs, _numOutputs);

            foreach(var n in copy.Nodes)
            {
                if(n.ID > _numInputs + _numOutputs)
                {
                    CNeuroNetWrapperFunctions.__AddNode(_cnet, n.ID);
                }
            }

            foreach (var c in copy.Connections)
            {
                CNeuroNetWrapperFunctions.__AddConnection(_cnet, c.From, c.To, c.Weight);
            }
            
            
            _connections = ConnectionsCollection.GetLinkedCollection(_cnet);
            _nodes = NodesCollection.GetLinkedConnection(_cnet);
        }

        public bool AddNode(int ID)
        {
            if (CNeuroNetWrapperFunctions.__FindNode(_cnet, ID) == IntPtr.Zero)
            {
                CNeuroNetWrapperFunctions.__AddNode(_cnet, ID);
                return true;
            }
            return false;
        }

        public bool AddConnection(int from, int to, double weight)
        {
            if (CNeuroNetWrapperFunctions.__FindConnection(_cnet, from, to) == IntPtr.Zero)
            {
                CNeuroNetWrapperFunctions.__AddConnection(_cnet, from, to, weight);
                return true;
            }
            return false;
        }



        public double[] Compute(double[] inputs)
        {
            double[] res = new double[this.OutputsCount];

            double[] topass = new double[this.InputsCount];

            for(int i = 0;i< topass.Length; ++i)
            {
                if(i< inputs.Length)
                {
                    topass[i] = inputs[i];
                }else
                {
                    topass[i] = 0.0;
                }
            }

            IntPtr arr = Marshal.AllocHGlobal(topass.Length * sizeof(double));
            Marshal.Copy(topass, 0, arr, topass.Length);

            try
            {
                CNeuroNetWrapperFunctions.__InitInputs(_cnet, arr);
            }
            catch (System.Exception ex)
            {
                throw new Exception("Error during initialization",ex);
            }
            finally
            {
                Marshal.FreeHGlobal(arr);
            }

            IntPtr cres = CNeuroNetWrapperFunctions.__Compute(_cnet);

            Marshal.Copy(cres, res, 0, res.Length);
            Marshal.FreeHGlobal(cres);

            return res;
        }

        public void Dispose()
        {
            CNeuroNetWrapperFunctions.__FreeNeuroNet(_cnet);
        }

        ~CNeuroNetWrapper()
        {
            Dispose();
        }





    }
}
