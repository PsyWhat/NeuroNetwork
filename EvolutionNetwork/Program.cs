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


            int seed = 3;
            int passes = 500;
            int individuals = 1000;
            float dispersion = 200.0f;
            float a = 0.5f;
            float b = 0.5f;
            Console.Write("Enter seed:");
            if (!int.TryParse(Console.ReadLine(), out seed))
                seed = 1234321;

            Console.Write("Enter number of passes:");
            if(!int.TryParse(Console.ReadLine(), out passes))
                passes = 1000;

            Console.Write("Enter number of individuals:");
            if (!int.TryParse(Console.ReadLine(), out individuals))
                individuals = 200;

            Console.Write("Enter disspersion:");
            if (!float.TryParse(Console.ReadLine(), out dispersion))
                dispersion = 10.0f;

            Console.Write("Enter a:");
            if (!float.TryParse(Console.ReadLine(), out a))
                a = 0.5f;

            Console.Write("Enter b:");
            if (!float.TryParse(Console.ReadLine(), out b))
                b = 0.5f;


            Teacher t = new Teacher();
            Thread thread = new Thread(() =>
            {
                t = new Teacher();
                t.Start(seed,passes,individuals, dispersion,a,b);
            });
            thread.Start();
            int selected = 0;
            bool isselected = false;
            while(true)
            {
                int ch = _getch();
                if(ch == 'e')
                {
                    t.pause = true;
                    while (true)
                    {
                        Console.Write("Enter a command:");
                        string com = Console.ReadLine().ToLower();
                        if (com == "print")
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
                        else if (com.Contains("select"))
                        {
                            int d = com.IndexOf("select");
                            d += "select ".Length;
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
                                byte[] buff = t.ET.LastResult[selected].b.ToByteArray();
                                f.Write(buff, 0, buff.Length);
                                f.Close();
                                f.Dispose();
                                Console.WriteLine("File successfuly have been saved to: {0}, it took {1} bytes", path, buff.Length);
                                f = File.Open(path, FileMode.Open);
                                buff = new byte[160000];
                                f.Seek(0, SeekOrigin.Begin);
                                f.Read(buff, 0, buff.Length);
                                NeuroNet j = NeuroNet.FromByteArray(buff);
                                Console.WriteLine("Checking saved nn: {0}", j);
                                f.Close();
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
