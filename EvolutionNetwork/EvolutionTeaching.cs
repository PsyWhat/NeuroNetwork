using System.Threading;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace EvolutionNetwork
{

    public delegate float TestFunctionDelegate(NeuroNet nnt);


    public class Pair<A, B>
    {
        public A a;
        public B b;

        public Pair(A a, B b)
        {
            this.a = a;
            this.b = b;
        }

        public Pair(Pair<A, B> copy)
        {
            this.a = copy.a;
            this.b = copy.b;
        }

        public override string ToString()
        {
            return string.Format("\"{0}\", \"{1}\"", a, b);
        }

    }


    public class EvolutionTeaching
    {
        public int Inputs;
        public int Outputs;
        public int Seed;
        public int LastID;
        public int Individuals;
        public float WeightDispersion;
        public float MergeMutationChance = 0.5f;
        public float a = 0.5f;
        public float b = 0.5f;
        public List<Pair<float, NeuroNet>> LastResult;
        TestFunctionDelegate TestFunction;
        System.Random rand;

        List<NeuroNet> _currentNets;
        bool _initiated;
        int _currentGeneration;


        public NeuroNet this[int index]
        {
            get
            {
                return _currentNets[index];
            }
        }

        public float GetRandom(float min = -1.0f, float max = 1.0f)
        {
            if (max < min) { float b = max; max = min; min = b; }
            return (float)(rand.NextDouble() * ((double)max - min) + min);
        }



        public EvolutionTeaching(int inputs, int outputs, int seed, int individuals, TestFunctionDelegate testFunction, float weightDispersion = 10.0f,float a = 0.5f,float b = 0.5f)
        {
            this.a = a;
            this.b = b;
            Inputs = inputs;
            Outputs = outputs;
            LastID = Inputs + Outputs + 1;
            Individuals = individuals;
            TestFunction = testFunction;
            this.Seed = seed;
            rand = new System.Random(seed);
            _initiated = false;
            if (weightDispersion > 0.0f)
            {
                WeightDispersion = weightDispersion;
            }
            else
            {
                WeightDispersion = 1.0f;
            }
        }

        public int GetNewID()
        {
            return LastID++;
        }

        public NeuroNet GenerateNeuroNet(int additionalNodes = 0, int additionalConnections = 2)
        {
            NeuroNet res = new NeuroNet(Inputs, Outputs);
            List<Node> nn = new List<Node>();
            for (int i = 0; i < additionalNodes; ++i)
            {
                Node n = new Node(res.Nodes.Count);
                nn.Add(n);
                res.AddNode(n);
            }

            List<Node> avF = res.Nodes;
            List<Node> avT = res.Nodes.FindAll(x => x.ID >= Inputs);

            for (int i = 0; i < additionalNodes; ++i)
            {
                res.AddConnection(GetNewID(), avF[rand.Next(avF.Count)].ID, nn[i].ID, GetRandom(-WeightDispersion, WeightDispersion));
                res.AddConnection(GetNewID(), nn[i].ID, avT[rand.Next(avT.Count)].ID, GetRandom(-WeightDispersion, WeightDispersion));
            }

            for (int i = 0; i < additionalConnections; ++i)
            {

                res.AddConnection(GetNewID(), avF[rand.Next(avF.Count)].ID, avT[rand.Next(avT.Count)].ID, GetRandom(-WeightDispersion, WeightDispersion));
            }

            return res;
        }

        public NeuroNet MergeNeuroNets(NeuroNet a, NeuroNet b)
        {
            NeuroNet nn = new NeuroNet(Inputs, Outputs);
            a.Nodes.ForEach(x =>
            {
                if (x.ID > Inputs + Outputs)
                {
                    nn.AddNode(new Node(x.ID));
                }
            });
            a.Connections.ForEach(x => nn.AddConnection(x.ID, x.From, x.To, x.Weight, x.Active));

            b.Nodes.ForEach(x =>
            {
                if (!nn.Nodes.Exists(y => y.ID == x.ID))
                {
                    nn.AddNode(x);
                }
            });

            b.Connections.ForEach(x =>
            {
                Connection con = nn.Connections.Find(y => y.From == x.From && y.To == x.To);
                if (con == null)
                {
                    nn.AddConnection(x);
                }
                else
                {
                    con.Weight = (con.Weight / 2 + x.Weight / 2);
                    con.ID = GetNewID();
                    if (con.Active && !x.Active)
                    {
                        con.Active = false;
                    }
                    else if (!con.Active == !x.Active)
                    {
                        if (GetRandom(0.0f, 1.0f) < MergeMutationChance)
                        {
                            x.Active = !x.Active;
                        }
                    }
                }
            });

            return nn;
        }


        List<Connection> _generationConnections;

        public void StructureMutation(NeuroNet nn,float fun)
        {
            Mutation m = new Mutation();
            m.PrevParentFun = nn.ParentResult;

            long maxLen = nn.Inputs * (nn.Nodes.Count - nn.Inputs) + (nn.Nodes.Count - nn.Inputs) * (nn.Nodes.Count - nn.Inputs);
            float fully = (float)((double)nn.Connections.Count / (double)maxLen);
            if(GetRandom(0.0f,1.0f) > fully)
            {
                m.Type = MutationType.AddConnection;
                var avT = nn.Nodes.Where(x => !x.IsInput).ToList();
                int f;int t;
                do 
                {
                    f = nn.Nodes[rand.Next(nn.Nodes.Count)].ID;
                    t = avT[rand.Next(avT.Count)].ID;
                } while (nn.Connections.All(x=>x.From != f && x.To != t));
                Connection con = new Connection(GetNewID(), f, t, GetRandom(-WeightDispersion, WeightDispersion));
                m.NewConnection = con;
                m.PreviousAge = nn.Age;

                nn.Connections.Add(con);

            }else
            {
                m.Type = MutationType.AddNode;
                Connection sel = nn.Connections[rand.Next(nn.Connections.Count)];
                nn.Connections.Remove(sel);
                Node n = new Node(nn.Nodes.Count);
                nn.Nodes.Add(n);
                Connection con1 = new Connection(GetNewID(), nn.Nodes[rand.Next(nn.Nodes.Count)].ID, n.ID, GetRandom(-WeightDispersion, WeightDispersion));

                var avT = nn.Nodes.Where(x => !x.IsInput).ToList();

                Connection con2 = new Connection(GetNewID(), n.ID, avT[rand.Next(avT.Count)].ID, GetRandom(-WeightDispersion, WeightDispersion));

                nn.Connections.Add(con1);
                nn.Connections.Add(con2);

                m.RemovedConnection = sel;
                m.AdditionalNode = n;
                m.NewConnection = con1;
                m.NewConnection2 = con2;


            }

            nn.ParentResult = fun;
            nn.Age = 0;
            nn.MutationsDone.Enqueue(m);


        }

        public void WeightMutation(NeuroNet nn)
        {
            Mutation m = new Mutation { WeightChanges = new List<Tuple<Connection, float>>(), Type = MutationType.WeightChange };

            foreach(Connection c in nn.Connections)
            {
                float change = GetRandom(-this.WeightDispersion, this.WeightDispersion);
                m.WeightChanges.Add(new Tuple<Connection, float>(c,change));
                c.Weight += change;
            }

            nn.MutationsDone.Enqueue(m);
            nn.Age += 1;
        }

        public void MutateAddNode(NeuroNet nn)
        {
            bool active = false;
            foreach (var v in nn.Connections)
            {
                if (v.Active)
                {
                    active = true;
                    break;
                }
            }
            if (active)
            {
                int ind = 0;
                do
                {
                    ind = rand.Next(nn.Connections.Count);
                } while (!nn.Connections[ind].Active);
                Connection con = nn.Connections[ind];
                con.Active = false;
                //TODO: вместо неактивной связи ставим новый node и делаем связи от прошлой
                Node n = new Node(nn.Nodes.Count);
                Connection con1 = new Connection(GetNewID(), con.From, n.ID, GetRandom(-WeightDispersion, WeightDispersion));
                Connection con2 = new Connection(GetNewID(), n.ID, con.To, GetRandom(-WeightDispersion, WeightDispersion));
                nn.Nodes.Add(n);
                nn.Connections.Add(con1);
                nn.Connections.Add(con2);
            }
        }

        public void MutateAddConnection(NeuroNet nn)
        {
            //TODO: Добавляем новую случайную связь.
            List<Node> avF = nn.Nodes;
            List<Node> avT = nn.Nodes.FindAll(x => x.ID >= Inputs);
            /*

                    int maxT = 2*(avT.Count * (avT.Count - 1) / 2) + avT.Count + (Inputs* avT.Count);
                    int numAs = 0;
                    foreach (Connection c in nn.Connections)
                    {
                        if(c.Active)
                        {
                            numAs++;
                        }
                    }

                    if(maxT < numAs)
                    {*/
            int f = 0;
            int t = 0;
            do
            {
                f = avF[rand.Next(avF.Count)].ID;
                t = avT[rand.Next(avT.Count)].ID;
            } while (nn.Connections.Exists(x => x.From == f && x.To == t));

            Connection ex = _generationConnections.Find(x => x.From == f && x.To == t);

            if (ex == null)
            {
                Connection con = new Connection(GetNewID(), f, t, GetRandom(-WeightDispersion, WeightDispersion));
                nn.AddConnection(con);
                _generationConnections.Add(con);
            }
            else
            {
                nn.AddConnection(new Connection(ex));
            }


        }

        public void Init()
        {
            _currentNets = new List<NeuroNet>();

            for (int i = 0; i < Individuals; ++i)
            {
                _currentNets.Add(GenerateNeuroNet());
            }

            _currentGeneration = 0;
            _initiated = true;
        }

        public List<Pair<float, NeuroNet>> PassTests()
        {
            List<Pair<float, NeuroNet>> result = new List<Pair<float, NeuroNet>>();

            List<Thread> pool = new List<Thread>();

            _currentNets.ForEach(x =>
            {
                Thread t = new Thread(() =>
                {
                    var v = new Pair<float, NeuroNet>(TestFunction(x), x);
                    lock (result)
                    {
                        result.Add(v);
                    }
                });
                t.Start();
                pool.Add(t);
            });

            foreach (Thread t in pool)
            {
                t.Join();
            }

            return result;
        }


        NeuroNet GetNeuroFromList(List<Pair<float, NeuroNet>> list, float number)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                if (list[i].a > number)
                {
                    return list[i].b;
                }
            }
            return list[list.Count - 1].b;
        }

        public void PassGeneration()
        {
            var results = PassTests();
            LastResult = results;

            Parallel.ForEach(results, x =>
             {
                 float mdF = x.a - x.b.LastResult;
                 if(mdF < 0.0f)
                 {
                     x.b.UndoChanges();
                 }
                 float dF = x.a - x.b.ParentResult;
                 float Psm = (this.a * dF) / (this.b * (x.b.Age+1));



                 x.b.LastResult = x.a;
                 if (GetRandom(0.0f,1.0f) < Psm)
                 {
                     StructureMutation(x.b,x.a);
                 }else
                 {
                     WeightMutation(x.b);
                 }

                 x.b.LastResult = x.a;
             });

            _currentGeneration++;
            LastResult = results;
        }


    }

}