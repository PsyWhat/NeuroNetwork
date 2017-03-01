using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;
using System.IO;




namespace EvolutionNetwork
{




    static class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [DllImport("msvcrt")]
        static extern int _getch();
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            AllocConsole();


          /*  FileStream fs;
            byte[] buff = null;
            try
            {
                fs = File.Open("e:\\e.e", FileMode.Open);
                fs.Seek(0, SeekOrigin.Begin);
                buff = new byte[1024 * 1024 * 50];
                fs.Read(buff, 0, buff.Length);
                fs.Close();
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Error: \"{0}\"", ex);
            }
            try
            {

                if (buff != null)
                {
                    NeuroNet net = NeuroNet.FromByteArray(buff);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Error: \"{0}\"", ex);
            }*/



            Teacher tsd = new Teacher(1241, 10, 10.0f, 0.3f, 0.9f);
            var tsdn = tsd.ET.GenerateNeuroNet(3, 13);
            for(int i = 0;i<100;++i)
            {
                Console.WriteLine(tsd.Test(tsdn));
            }


            int seed = 3;
            int passes = 500;
            int individuals = 1000;
            double dispersion = 200.0;
            double a = 0.5;
            double b = 0.5;
            Console.Write("Enter seed:");
            if (!int.TryParse(Console.ReadLine(), out seed))
                seed = 1234321;

            Console.Write("Enter number of passes:");
            if (!int.TryParse(Console.ReadLine(), out passes))
                passes = 100000;

            Console.Write("Enter number of individuals:");
            if (!int.TryParse(Console.ReadLine(), out individuals))
                individuals = 1000;

            Console.Write("Enter dispersion:");
            if (!double.TryParse(Console.ReadLine(), out dispersion))
            {
                dispersion = 0.2;
                Console.WriteLine("Error!");
            }

            Console.Write("Enter a:");
            if (!double.TryParse(Console.ReadLine(), out a))
                a = 0.1;

            Console.Write("Enter b:");
            if (!double.TryParse(Console.ReadLine(), out b))
                b = 1.4;



            Teacher t = null;
            Thread thread = new Thread(() =>
            {
                t = new Teacher(seed, individuals, dispersion, a, b);
                t.Start(passes);
            });
            thread.Start();
            int selected = 0;
            bool isselected = false;
            while (true)
            {
                int ch = _getch();
                if (ch == 'e')
                {
                    t.pause = true;
                    while (true)
                    {
                        Console.Write("Enter a command:");
                        string com = Console.ReadLine().ToLower();
                        if (com == "stats")
                        {
                            Console.WriteLine("seed:{0}; passes:{1}; individuals:{2}; {3}; a:{4}; b:{5};", seed, passes, t.ET.Individuals,t.ET.WeightDispersion, t.ET.a, t.ET.b);
                        }
                        else if (com == "print")
                        {
                            foreach (var v in t.ET.LastResult)
                            {
                                Console.WriteLine(v);
                            }
                        }
                        else if (com == "continue")
                        {
                            break;
                        }
                        else if (com.Contains("disp"))
                        {
                            double nd = 0.0;
                            if (double.TryParse(com.Substring("disp".Length), out nd))
                            {
                                t.ET.WeightDispersion = Math.Abs(nd);
                                Console.WriteLine("Changed dispersion to {0}", nd);
                            }
                            else
                            {
                                Console.WriteLine("Cant parse");
                            }
                        }
                        else if (com.Contains("score"))
                        {
                            double nd = 0.0;
                            if (double.TryParse(com.Substring("score".Length), out nd))
                            {
                                t.ET.scoreLim = nd;
                                Console.WriteLine("Changed score to {0}", nd);
                            }
                            else
                            {
                                Console.WriteLine("Cant parse");
                            }
                        }
                        else if (com.Contains("set a"))
                        {
                            double nd = 0.0;
                            if (double.TryParse(com.Substring("set a".Length), out nd))
                            {
                                t.ET.a = nd;
                                Console.WriteLine("a {0}", nd);
                            }
                            else
                            {
                                Console.WriteLine("Cant parse");
                            }
                        }
                        else if (com.Contains("set b"))
                        {
                            double nd = 0.0;
                            if (double.TryParse(com.Substring("set b".Length), out nd))
                            {
                                t.ET.b = nd;
                                Console.WriteLine("Changed b to {0}", nd);
                            }
                            else
                            {
                                Console.WriteLine("Cant parse");
                            }
                        }
                        else if (com.Contains("kill"))
                        {
                            int am = 0;
                            if (int.TryParse(com.Substring("kill".Length), out am))
                            {
                                am = Math.Min(t.ET._currentNets.Count, Math.Max(0, am));
                                Console.WriteLine("Killed {0}", am);
                                for (int i = 0; i < am; ++i)
                                {
                                    t.ET._currentNets.Remove(t.ET.LastResult[0].b);
                                    t.ET.LastResult.RemoveAt(0);
                                }
                                t.ET.Individuals -= am;
                            }
                            else
                            {
                                Console.WriteLine("Cant parse");
                            }
                        }
                        else if (com.Contains("undone"))
                        {
                            if (com.Contains("true"))
                            {
                                t.ET.UndoneEnabled = true;
                                Console.WriteLine("Undone enabled");
                            }
                            else
                            {
                                t.ET.UndoneEnabled = false;
                                Console.WriteLine("Undone disabled");
                            }
                        }
                        else if (com.Contains("spawn"))
                        {
                            int am = 0;
                            if (int.TryParse(com.Substring("spawn".Length), out am))
                            {
                                am = Math.Max(0, am);
                                for (int i = 0; i < am; ++i)
                                {
                                    t.ET._currentNets.Add(t.ET.GenerateNeuroNet());
                                }
                                t.ET.Individuals += am;
                                Console.WriteLine("Spawned {0}", am);
                            }
                            else
                            {
                                Console.WriteLine("Cant parse");
                            }
                        }
                        else if (com.Contains("select"))
                        {
                            int d = com.IndexOf("select");
                            d += "select".Length;
                            int sel = 0;
                            if (int.TryParse(com.Substring(d), out sel))
                            {
                                selected = t.ET.LastResult.Count - Math.Min(t.ET.LastResult.Count, Math.Max(1, sel));
                                Console.WriteLine("Selected {0}", t.ET.LastResult[selected]);
                                isselected = true;
                            }
                            else
                            {
                                Console.WriteLine("Cant parse");
                            }
                        }
                        else if (com == "load")
                        {
                            Console.Write("Enter path:");
                            string path = Console.ReadLine();
                            System.IO.FileInfo fi = null;
                            do
                            {
                                try
                                {
                                    fi = new System.IO.FileInfo(path);
                                }
                                catch (ArgumentException) { }
                                catch (System.IO.PathTooLongException) { }
                                catch (NotSupportedException) { }
                                if (fi == null)
                                {
                                    Console.Write("Wrong file path, enter new file path:");
                                    path = Console.ReadLine();
                                }
                            } while (fi == null);

                            FileStream fs1;
                            byte[] buff1 = null;
                            try
                            {
                                fs1 = File.Open(path, FileMode.Open);
                                fs1.Seek(0, SeekOrigin.Begin);
                                buff1 = new byte[1024 * 1024 * 50];
                                fs1.Read(buff1, 0, buff1.Length);
                                fs1.Close();
                            }
                            catch (System.Exception ex)
                            {
                                Console.WriteLine("Error: \"{0}\"", ex);
                            }
                            try
                            {

                                if (buff1 != null)
                                {
                                    NeuroNet net = NeuroNet.FromByteArray(buff1);


                                    Console.WriteLine("Readed: {0}, Save it?", net);
                                    string conf = Console.ReadLine().ToLower();
                                    if (conf.Contains("y"))
                                    {
                                        t.ET.LoadNet(net);
                                    }

                                }
                            }
                            catch (System.Exception ex)
                            {
                                Console.WriteLine("Error: \"{0}\"", ex);
                            }

                        }
                        else if (com == "save")
                        {
                            if (isselected)
                            {
                                Console.Write("Enter path:");
                                string path = Console.ReadLine();
                                System.IO.FileInfo fi = null;
                                do
                                {
                                    try
                                    {
                                        fi = new System.IO.FileInfo(path);
                                    }
                                    catch (ArgumentException) { }
                                    catch (System.IO.PathTooLongException) { }
                                    catch (NotSupportedException) { }
                                    if (fi == null)
                                    {
                                        Console.Write("Wrong file path, enter new file path:");
                                        path = Console.ReadLine();
                                    }
                                } while (fi == null);
                                FileStream f = File.Open(path, FileMode.Create);
                                byte[] buff1 = t.ET.LastResult[selected].b.ToByteArray();
                                f.Write(buff1, 0, buff1.Length);
                                f.Close();
                                f.Dispose();
                                Console.WriteLine("File successfuly have been saved to: {0}, it took {1} bytes", path, buff1.Length);
                                f = File.Open(path, FileMode.Open);
                                buff1 = new byte[160000];
                                f.Seek(0, SeekOrigin.Begin);
                                f.Read(buff1, 0, buff1.Length);
                                NeuroNet j = NeuroNet.FromByteArray(buff1);
                                Console.WriteLine("Checking saved nn: {0}", j);
                                f.Close();
                            }
                            else
                            {
                                Console.WriteLine("Nothing is selected!");
                            }
                        }
                        else if (com == "test")
                        {
                            if (isselected)
                            {
                                NeuroNet n = t.ET.LastResult[selected].b;
                                double r = t.Test(t.ET.LastResult[selected].b);
                                Console.WriteLine("Test passed with F:{0}, Last result was: {1}", r, n.LastResult);
                            }
                            else
                            {
                                Console.WriteLine("Nothing is selected!");
                            }
                        }
                        else if (com == "optimized")
                        {
                            if (isselected)
                            {
                                NeuroNet net = new NeuroNet(t.ET.LastResult[selected].b);
                                net.Optimize();
                                Console.WriteLine("Optimized: {0}", net);
                            }
                            else
                            {
                                Console.WriteLine("Nothing is selected!");
                            }
                        }
                        }
                        isselected = false;
                        t.pause = false;
                    }
                }
            }
        }
    }
