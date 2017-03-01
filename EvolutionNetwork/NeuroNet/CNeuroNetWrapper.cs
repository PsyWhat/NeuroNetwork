using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace EvolutionNetwork.NeuroNet
{
    public class CNeuroNetWrapper
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32.dll")]
        static extern bool FreeLibrary(IntPtr hModule);

        const string DllPath = "DLLs";
        const string DllCont = "CNeuroNet{0}.dll";

        /// <summary>
        /// Pointer to a loaded DLL
        /// </summary>
        static IntPtr LibPTR;


        /*delegate IntPtr AddNodeDelegate(IntPtr pointerToANet, int NodeID);
        delegate IntPtr AddConnectionDelegate(IntPtr pointerToANet, int from, int to, double weight);

        delegate IntPtr ComputeDelegate(IntPtr pointerToANet);
        delegate IntPtr NewNeuroNetDelegate(int numInputs, int numOtputs);*/


        delegate void FreeNeuroNetDelegate(IntPtr pointerToANet);


       /* delegate double GetConnectionWeightDelegate(IntPtr con);*/
        delegate void ChangeConnectionWeightDelegate(IntPtr con, double newWeight);

        /*delegate IntPtr FindNodeByIDDelegate(IntPtr pointerToANet, int ID);
        delegate IntPtr FindConnectionDelegate(IntPtr pointerToANet, int from, int to);

        delegate int RemoveNodeDelegate(IntPtr pointerToANet, int ID);
        delegate int RemoveConnectionDelegate(IntPtr pointerToANet, int from, int to);*/


        static Func<IntPtr,int,IntPtr> __AddNode;
        static Func<IntPtr,int,int,double,IntPtr> __AddConnection;
        static Func<IntPtr, IntPtr> __Compute;
        static Func<int, int, IntPtr> __NewNeuroNet;
        static FreeNeuroNetDelegate __FreeNeuroNet;
        static Func<IntPtr,double> __GetWeight;
        static ChangeConnectionWeightDelegate __ChangeWeight;
        static Func<IntPtr,int,IntPtr> __FindNode;
        static Func<IntPtr,int,int,IntPtr> __FindConnection;
        static Func<IntPtr,int,int> __RemoveNode;
        static Func<IntPtr,int,int,int> __RemoveConnection;



        

        static CNeuroNetWrapper()
        {
            string pv = "x86";
            if(Environment.Is64BitProcess)
            {
                pv = "x64";
            }
            string filePath = string.Format("{1}\\{2}",Environment.CurrentDirectory ,DllPath, string.Format(DllCont, pv));

            IntPtr libPtr = LoadLibrary(filePath);
            if(libPtr == IntPtr.Zero)
            {
                throw new PlatformNotSupportedException("Can't load CNeuroNet DDL");
            }
            LibPTR = libPtr;

            __AddConnection = 
                Marshal.GetDelegateForFunctionPointer<Func<IntPtr, int, int, double, IntPtr>>
                (GetProcAddress(LibPTR, "AddConnection"));

            __AddNode = Marshal.GetDelegateForFunctionPointer<Func<IntPtr, int, IntPtr>>
                (GetProcAddress(LibPTR, "AddNode"));

            __Compute = Marshal.GetDelegateForFunctionPointer<Func<IntPtr, IntPtr>>
                (GetProcAddress(LibPTR, "Compute"));

            __NewNeuroNet = Marshal.GetDelegateForFunctionPointer<Func<int, int, IntPtr>>
                (GetProcAddress(LibPTR, "NewNeuroNet"));

            
            __FreeNeuroNet = Marshal.GetDelegateForFunctionPointer<FreeNeuroNetDelegate>
                (GetProcAddress(LibPTR, "FreeNeuroNet"));


            __GetWeight = Marshal.GetDelegateForFunctionPointer<Func<IntPtr, double>>
                (GetProcAddress(LibPTR, "GetConnectionWeight"));

            __ChangeWeight = Marshal.GetDelegateForFunctionPointer<ChangeConnectionWeightDelegate>
                (GetProcAddress(LibPTR, "ChangeConnectionWeight"));

            __FindNode = Marshal.GetDelegateForFunctionPointer<Func<IntPtr, int, IntPtr>>
                (GetProcAddress(LibPTR, "FindNodeByID"));

            __FindConnection = Marshal.GetDelegateForFunctionPointer<Func<IntPtr, int, int, IntPtr>>
                (GetProcAddress(LibPTR, "FindConnection"));

            __RemoveNode = Marshal.GetDelegateForFunctionPointer<Func<IntPtr, int, int>>
                (GetProcAddress(LibPTR, "RemoveNode"));

            __RemoveConnection = Marshal.GetDelegateForFunctionPointer<Func<IntPtr, int, int, int>>
                (GetProcAddress(LibPTR, "RemoveConnection"));
        }

        private IntPtr _cnet;
        

        public CNeuroNetWrapper(int numInputs, int numOutputs)
        {
            _cnet = __NewNeuroNet(numInputs, numOutputs);
        }
        

        public bool AddNode(int ID)
        {
            if(__FindNode(_cnet,ID) == IntPtr.Zero)
            {
                __AddNode(_cnet, ID);
                return true;
            }
            return false;
        }

        public bool AddConnection(int from, int to, double weight)
        {
            if(__FindConnection(_cnet,from,to) == IntPtr.Zero)
            {
                __AddConnection(_cnet, from, to, weight);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Best
        /// </summary>
        /// <returns></returns>

        public double[] Compute()
        {
            IntPtr res = __Compute(_cnet);
        }
        


        ~CNeuroNetWrapper()
        {
            CNeuroNetWrapper.__FreeNeuroNet(_cnet);
        }



        

    }
}
