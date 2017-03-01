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

    public delegate double TestFunctionDelegate(NeuroNet nnt);


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
        public double WeightDispersion;
        public double MergeMutationChance = 0.5;
        public double a = 0.5;
        public double b = 0.5;
        public List<Pair<double, NeuroNet>> LastResult;
        TestFunctionDelegate TestFunction;
        System.Random rand;

        public List<NeuroNet> _currentNets;
        bool _initiated;
        int _currentGeneration;


        public NeuroNet this[int index]
        {
            get
            {
                return _currentNets[index];
            }
        }

        public double GetRandom(double min = -1.0, double max = 1.0)
        {
            if (max < min) { double b = max; max = min; min = b; }
            int ns = 1000000000;
            int hj = 123;
            lock (rand)
            {
                hj = rand.Next(ns + 1);
            }
            double res = (double)(((double)hj / (double)ns) * ((double)max - min) + min);
            return res;
        }

        public int GetRandomInt(int max)
        {
            int res;
            lock (rand)
            {
                res = rand.Next(max);
            }
            return res;
        }



        public EvolutionTeaching(int inputs, int outputs, int seed, int individuals, TestFunctionDelegate testFunction, double weightDispersion = 10.0, double a = 0.5, double b = 0.5)
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
            if (weightDispersion > 0.0)
            {
                WeightDispersion = weightDispersion;
            }
            else
            {
                WeightDispersion = 1.0;
            }
        }

        public int GetNewID()
        {
            return LastID++;
        }

        public NeuroNet GenerateNeuroNet(int additionalNodes = 0, int additionalConnections = 1)
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
                res.AddConnection(GetNewID(), avF[GetRandomInt(avF.Count)].ID, nn[i].ID, GetRandom(-WeightDispersion, WeightDispersion));
                res.AddConnection(GetNewID(), nn[i].ID, avT[GetRandomInt(avT.Count)].ID, GetRandom(-WeightDispersion, WeightDispersion));
            }

            for (int i = 0; i < additionalConnections; ++i)
            {

                res.AddConnection(GetNewID(), avF[GetRandomInt(avF.Count)].ID, avT[GetRandomInt(avT.Count)].ID, GetRandom(-WeightDispersion, WeightDispersion));
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
                        if (GetRandom(0.0, 1.0) < MergeMutationChance)
                        {
                            x.Active = !x.Active;
                        }
                    }
                }
            });

            return nn;
        }


        List<Connection> _generationConnections;

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
                    ind = GetRandomInt(nn.Connections.Count);
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
                f = avF[GetRandomInt(avF.Count)].ID;
                t = avT[GetRandomInt(avT.Count)].ID;
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



        public void LoadNet(NeuroNet nn)
        {
            if (nn.Inputs == this.Inputs && nn.Outputs == this.Outputs)
            {
                lock (_currentNets)
                {
                    _currentNets.Add(nn);
                }
                Individuals += 1;
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

        public List<Pair<double, NeuroNet>> PassTests()
        {
            List<Pair<double, NeuroNet>> result = new List<Pair<double, NeuroNet>>();

            List<Thread> pool = new List<Thread>();

            _currentNets.ForEach(x =>
            {
                Thread t = new Thread(() =>
                {
                    var v = new Pair<double, NeuroNet>(TestFunction(x), x);
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


        NeuroNet GetNeuroFromList(List<Pair<double, NeuroNet>> list, double number)
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

        public double record = 0.0;
        public bool UndoneEnabled = true;
        public double scoreLim = -20.0;

        public void WeightMutation(NeuroNet nn)
        {
            Mutation m = new Mutation { WeightChanges = new List<Tuple<Connection, double>>(), Type = MutationType.WeightChange };
            m.PrevParentFun = nn.LastResult;

            foreach (Connection c in nn.Connections)
            {
                double change = GetRandom(-this.WeightDispersion, this.WeightDispersion) * (double)(1 / Math.Pow(nn.OperationsAfterSM, 0.05));
                m.WeightChanges.Add(new Tuple<Connection, double>(c, change));
                c.Weight += change;
            }

            nn.OperationsAfterSM++;
            nn.MutationsDone.Push(m);
            nn.Age += 1;
        }

        public void StructureMutation(NeuroNet nn, double fun)
        {
            Mutation m = new Mutation();
            m.PrevParentFun = nn.ParentResult;

            long maxLen = nn.Inputs * (nn.Nodes.Count - nn.Inputs) + (nn.Nodes.Count - nn.Inputs) * (nn.Nodes.Count - nn.Inputs);
            double fully = (double)Math.Pow(((double)nn.Connections.Count / (double)maxLen), 4);
            double rands = GetRandom(0.0, 1.0);
            if (rands > fully)
            {
                m.Type = MutationType.AddConnection;
                var avT = nn.Nodes.Where(x => !x.IsInput).ToList();
                int f; int t;
                do
                {
                    f = nn.Nodes[GetRandomInt(nn.Nodes.Count)].ID;
                    t = avT[GetRandomInt(avT.Count)].ID;
                } while (nn.Connections.Exists(x => x.From == f && x.To == t));
                Connection con = new Connection(GetNewID(), f, t, GetRandom(-WeightDispersion, WeightDispersion));
                m.NewConnection = con;

                nn.Connections.Add(con);

            }
            else
            {
                m.Type = MutationType.AddNode;
                Connection sel = nn.Connections[GetRandomInt(nn.Connections.Count)];
                nn.Connections.Remove(sel);
                Node n = new Node(nn.Nodes.Count);
                Connection con1 = new Connection(GetNewID(), sel.From, n.ID, GetRandom(-WeightDispersion, WeightDispersion));

                var avT = nn.Nodes.Where(x => !x.IsInput).ToList();

                Connection con2 = new Connection(GetNewID(), n.ID, sel.To, GetRandom(-WeightDispersion, WeightDispersion));

                nn.Connections.Add(con1);
                nn.Connections.Add(con2);
                nn.Nodes.Add(n);

                m.RemovedConnection = sel;
                m.AdditionalNode = n;
                m.NewConnection = con1;
                m.NewConnection2 = con2;


            }

            nn.Copies = 0;
            nn.OperationsAfterSM = 1;
            m.PreviousAge = nn.Age;
            nn.ParentResult = fun;
            nn.Age = 0;
            nn.MutationsDone.Push(m);


        }

        public void PassGeneration()
        {
            var results = PassTests();
            results.Sort((x, y) =>
            {
                if (x.a > y.a)
                {
                    return 1;
                }
                else if (x.a == y.a)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            });
            LastResult = new List<Pair<double, NeuroNet>>();
            Parallel.ForEach(results, x =>
             {
                 bool undone = false;
                 bool rec = false;
                 double mdF = x.a - x.b.LastResult;
                 if (mdF < scoreLim && UndoneEnabled)
                 {
                     if (x.b.MutationsDone.Peek().Type == MutationType.WeightChange)
                     {
                         x.b.UndoChanges();
                         undone = true;
                         x.a = x.b.LastResult;
                     }
                 }

                 if (x.a > record)
                 {
                     record = x.a;
                     lock (_currentNets)
                     {
                         x.b.Copies += 1;
                         NeuroNet nn = new NeuroNet(x.b);
                         nn.Age = 0;
                         nn.OperationsAfterSM = 1;
                         this._currentNets.Add(nn);
                         this._currentNets.Remove(results[0].b);

                     }
                     rec = true;
                 }

                 /*if (x.b == results[results.Count - 1].b)
                 {
                     rec = true;
                 }*/


                 if (true || !undone || x.b.OperationsAfterSM > (x.b.Connections.Count + x.b.Nodes.Count))
                 {

                     if (rec)
                     {
                         if (x.b.Copies / (double)Individuals < 0.1)
                         {
                             lock (_currentNets)
                             {
                                 x.b.Copies += 1;
                                 NeuroNet nn = new NeuroNet(x.b);
                                 nn.Age = 0;
                                 nn.OperationsAfterSM = 1;
                                 this._currentNets.Add(nn);
                                 this._currentNets.Remove(results[0].b);

                             }
                         }
                     }

                     double dF = x.a - x.b.ParentResult;
                     double Psm = ((x.b.Connections.Count + x.b.Nodes.Count) + Math.Abs(this.a * dF)) / (this.b * (x.b.Age + 1));

                     if (GetRandom(0.0, 1.0) > Psm)
                     {

                         StructureMutation(x.b, x.a);
                     }
                     else
                     {
                         WeightMutation(x.b);
                     }

                     x.b.LastResult = x.a;
                 }
                 else
                 {
                     WeightMutation(x.b);
                 }
             });

            results.ForEach(x => LastResult.Add(x));

            LastResult.Sort((x, y) =>
            {
                if (x.a > y.a)
                {
                    return 1;
                }
                else if (x.a == y.a)
                {
                    return 0;
                }
                else
                {
                    return -1;
                }
            });



            _currentGeneration++;
        }


    }

}