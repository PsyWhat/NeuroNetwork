using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;


namespace EvolutionNetwork
{
    public class Connection
    {
        public int ID;
        public int From;
        public int To;
        public double Weight;
        public bool Active;

        public Connection(int id, int from, int to, double weight, bool active = true)
        {
            ID = id;
            From = from;
            To = to;
            Weight = weight;
            Active = active;
        }
        public Connection(Connection con)
        {
            ID = con.ID;
            From = con.From;
            To = con.To;
            Weight = con.Weight;
            Active = con.Active;
        }

        public override string ToString()
        {
            return string.Format("Connection ID:{0}\tFrom:{1}\tTo:{2}\tWeight:{3}", ID, From, To, Weight);
        }

    }

    public class Node
    {
        public override string ToString()
        {
            return string.Format("Node:\t[{0}]",ID);
        }

        public double Value;
        public int ID;
        public bool IsInput;
        public bool calculated;
        public bool calculating;
        public double Sum;

        public Node(int id, bool isInput = false)
        {
            ID = id;
            IsInput = isInput;
            Value = 0;
            calculating = false;
            calculated = false;
            Sum = 0;
        }

        public Node(Node copy)
        {
            ID = copy.ID;
            IsInput = copy.IsInput;
            Value = copy.Value;
            calculating = copy.calculating;
            calculated = copy.calculated;
            Sum = copy.Sum;
        }

    }

    public class CSNeuroNet
    {
        public List<Connection> Connections;
        public List<Node> Nodes;
        List<Node> outputs;
        List<Node> inputs;
        public int Inputs;
        public int Outputs;
        public int OperationsAfterSM = 0;
        public int Age;
        public int Copies = 0;
        public double ParentResult = 0;
        public double LastResult = 0;





        public CSNeuroNet(int inputs, int outputs, int Age = 0)
        {
            Nodes = new List<Node>();
            this.outputs = new List<Node>();
            this.inputs = new List<Node>();
            Connections = new List<Connection>();
            int i = 0;
            Inputs = inputs;
            Outputs = outputs;
            for (; i < inputs; ++i)
            {
                Node n = new Node(i, true);
                Nodes.Add(n);
                this.inputs.Add(n);
            }
            for (; i < inputs + outputs; ++i)
            {
                Node n = new Node(i);
                Nodes.Add(n);
                this.outputs.Add(n);
            }
        }

        public CSNeuroNet(CSNeuroNet copy)
        {
            


            Copies = copy.Copies;
            Nodes = new List<Node>();
            outputs = new List<Node>();
            inputs = new List<Node>();
            Inputs = copy.Inputs;
            Outputs = copy.Outputs;
            copy.Nodes.ForEach(x =>
            {
                Node n = new Node(x);

                

                Nodes.Add(n);
                if (x.ID < Inputs)
                {
                    inputs.Add(n);
                }
                else if (x.ID < Inputs + Outputs)
                {
                    outputs.Add(n);
                }

            });

            Connections = new List<Connection>();


            copy.Connections.ForEach(x =>
            {
                Connection c = new Connection(x);
                

                Connections.Add(c);
            });




            


        }

        public void AddConnection(int id, int from, int to, double weight, bool active = true)
        {
            var h = Connections.Find(x => x.From == from && x.To == to);
            if (h != null)
            {

            }
            else
            {

                Connections.Add(new Connection(id, from, to, weight, active));
            }
        }
        public void AddConnection(Connection con)
        {

            var h = Connections.Find(x => x.From == con.From && x.To == con.To);
            if (h != null)
            {

            } else
            {
                Connections.Add(new Connection(con));
            }
        }

        public bool ContainsConnection(Connection con)
        {
            return this.Connections.Exists(x => x.ID == con.ID);
        }
        public bool ContainsConnection(int id)
        {
            return this.Connections.Exists(x => x.ID == id);
        }

        public bool ContainsNode(Node node)
        {
            return this.Nodes.Exists(x => x.ID == node.ID);
        }
        public bool ContainsNode(int id)
        {
            return this.Nodes.Exists(x => x.ID == id);
        }

        public void AddNode(Node node)
        {
            if (!Nodes.Exists(x => x.ID == node.ID))
            {
                Nodes.Add(new Node(node.ID));
            }
        }
        public void AddNode(int id)
        {
            if (!Nodes.Exists(x => x.ID == id))
            {
                Nodes.Add(new Node(id));
            }
        }


        public void Flush()
        {
            Nodes.ForEach(x =>
            {
                x.Value = 0;
                x.Sum = 0;
            });
        }

        public bool JustCreated;

        void Calculate(Node node)
        {
            if (!node.calculated)
            {
                node.calculating = true;
                node.Sum = 0;
                Connections.FindAll(x => { return x.Active && x.To == node.ID; }).ForEach(h =>
                {
                    var s = Nodes.Find(y => y.ID == h.From);
                    if (s.IsInput)
                    {
                        node.Sum += s.Value * h.Weight;
                    }
                    else if (s.calculated)
                    {
                        node.Sum += s.Value * h.Weight;
                    }
                    else if (!s.calculating)
                    {
                        this.Calculate(s);
                        node.Sum += s.Value * h.Weight;
                    }
                    else
                    {
                        node.Sum += s.Value * h.Weight;
                    }
                });
                node.Value = 1.0 / (1.0 + Math.Exp(-node.Sum));
                node.Sum = 0;
                node.calculating = false;
                node.calculated = true;
            }
        }

        public double[] Calculate(double[] inputs)
        {
            Nodes.ForEach(x => { x.calculated = false; x.Sum = 0; });
            for (int i = 0; i < this.inputs.Count; ++i)
            {
                if (inputs != null && i < inputs.Length)
                {
                    this.inputs[i].Value = inputs[i];
                }
                else
                {
                    this.inputs[i].Value = 0;
                }
            }
            outputs.ForEach(x => Calculate(x));
            double[] res = new double[outputs.Count];
            for (int i = 0; i < res.Length; ++i)
            {
                res[i] = outputs[i].Value;
            }
            return res;
        }

        public override string ToString()
        {
            return string.Format("Nodes count:{0} Connections count:{1}", Nodes.Count, Connections.Count);
        }

        public void Optimize()
        {
            List<Node> toRem = new List<Node>();
            List<Connection> toCRem = new List<Connection>();
            foreach (Node n in Nodes)
            {
                if (n.ID > Inputs + Outputs)
                {
                    bool cont = false;
                    foreach (Connection c in Connections)
                    {
                        if (c.From == n.ID)
                        {
                            if (c.To != n.ID)
                            {
                                cont = true;
                                break;
                            }
                        }
                    }
                    if (!cont)
                    {
                        foreach (Connection c in Connections)
                        {
                            if (c.To == n.ID)
                            {
                                if (!toCRem.Contains(c))
                                {
                                    toCRem.Add(c);
                                }
                            }
                        }
                        if (!toRem.Contains(n))
                        {
                            toRem.Add(n);
                        }
                    }
                }
            }
            foreach (var n in toRem)
            {
                Nodes.Remove(n);
            }
            foreach (var x in toCRem)
            {
                Connections.Remove(x);
            }

            toCRem.Clear();
            foreach (Connection c in Connections)
            {
                if (c.Active)
                {
                    foreach (Connection b in Connections)
                    {
                        if (b.Active)
                        {
                            if (c.From == b.From)
                            {
                                if (c.To == b.To)
                                {
                                    if (c != b)
                                    {
                                        c.Weight += b.Weight;
                                        if (!toCRem.Contains(b))
                                        {
                                            toCRem.Add(b);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!toCRem.Contains(b))
                            {
                                toCRem.Add(b);
                            }
                        }
                    }
                }
                else
                {
                    if (!toCRem.Contains(c))
                    {
                        toCRem.Add(c);
                    }
                }
            }

            foreach (var x in toCRem)
            {
                Connections.Remove(x);
            }


        }

        public byte[] ToByteArray()
        {
            CSNeuroNet copy = new CSNeuroNet(this);
            //copy.Optimize();
            List<byte> result = new List<byte>();

            result.AddRange(BitConverter.GetBytes(copy.Inputs));
            result.AddRange(BitConverter.GetBytes(copy.Outputs));

            result.AddRange(BitConverter.GetBytes(copy.Nodes.Count - Inputs - Outputs));
            foreach (Node n in copy.Nodes)
            {
                if (n.ID > Inputs + Outputs - 1)
                {
                    result.AddRange(BitConverter.GetBytes(n.ID));
                    result.AddRange(BitConverter.GetBytes(n.Value));
                }
            }
            result.AddRange(BitConverter.GetBytes(copy.Connections.Count));
            foreach (Connection c in copy.Connections)
            {
                result.AddRange(BitConverter.GetBytes(c.ID));
                result.AddRange(BitConverter.GetBytes(c.From));
                result.AddRange(BitConverter.GetBytes(c.To));
                result.AddRange(BitConverter.GetBytes(c.Weight));
            }

            return result.ToArray();
        }

        public static CSNeuroNet FromByteArray(byte[] array)
        {
            int i = 0;
            int inputsc = BitConverter.ToInt32(array, i); i += 4;
            int outputsc = BitConverter.ToInt32(array, i); i += 4;

            CSNeuroNet result = new CSNeuroNet(inputsc, outputsc);

            int nodesC = BitConverter.ToInt32(array, i); i += 4;
            for (int k = 0; k < nodesC; ++k)
            {
                Node n = new Node(BitConverter.ToInt32(array, i)); i += 4;
                n.Value = BitConverter.ToDouble(array, i); i += 8;
                result.AddNode(n);
            }

            int connectionsCount = BitConverter.ToInt32(array, i); i += 4;
            for (int k = 0; k < connectionsCount; ++k)
            {
                int id = BitConverter.ToInt32(array, i); i += 4;
                int from = BitConverter.ToInt32(array, i); i += 4;
                int to = BitConverter.ToInt32(array, i); i += 4;
                double weight = BitConverter.ToDouble(array, i); i += 8;

                Connection c = new Connection(id, from, to, weight);
                result.AddConnection(c);
            }
            return result;
        }

    }

}