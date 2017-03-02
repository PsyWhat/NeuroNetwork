using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace EvolutionNetwork.NeuroNet
{
    class CNeuroNetWrapperFunctions
    {
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        static extern IntPtr LoadLibrary(string dllToLoad);

        [DllImport("kernel32", CharSet = CharSet.Ansi, SetLastError = true)]
        static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        static extern ushort GetLastError();

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        static extern bool FreeLibrary(IntPtr hModule);

        const string DllPath = "DLLs";
        const string DllCont = "CNeuroNet{0}.dll";

        /// <summary>
        /// Pointer to a loaded DLL
        /// </summary>
        public static IntPtr LibPTR;


        public delegate IntPtr AddNodeDelegate(IntPtr pointerToANet, int NodeID);
        public delegate IntPtr AddConnectionDelegate(IntPtr pointerToANet, int from, int to, double weight);

        public delegate IntPtr ComputeDelegate(IntPtr pointerToANet);
        public delegate IntPtr NewNeuroNetDelegate(int numInputs, int numOtputs);


        public delegate void FreeNeuroNetDelegate(IntPtr pointerToANet);


        public delegate double GetConnectionWeightDelegate(IntPtr con);
        public delegate void ChangeConnectionWeightDelegate(IntPtr con, double newWeight);

        public delegate IntPtr FindNodeByIDDelegate(IntPtr pointerToANet, int ID);
        public delegate IntPtr FindConnectionDelegate(IntPtr pointerToANet, int from, int to);

        public delegate int RemoveNodeDelegate(IntPtr pointerToANet, int ID);
        public delegate int RemoveConnectionDelegate(IntPtr pointerToANet, int from, int to);

        public delegate IntPtr GetConnectionsDelegate(IntPtr p);
        public delegate IntPtr GetNodesDelegate(IntPtr p);

        public delegate int GetListLenghtDelegate(IntPtr p);
        public delegate IntPtr GetEnumeratorDelegate(IntPtr p);
        public delegate int EnumeratorGoNextDelegate(IntPtr p);
        public delegate IntPtr EnumeratorCurrentDelegate(IntPtr p);
        public delegate void ResetEnumeratorDelegate(IntPtr p);
        public delegate int GetNodeIDDelegate(IntPtr p);
        public delegate int GetConnectionToDelegate(IntPtr p);
        public delegate int GetGetConnectionFromDelegate(IntPtr p);
        public delegate void ClearConnectionsDelegate(IntPtr p);
        public delegate void ClearNodesDelegate(IntPtr p);
        public delegate IntPtr SetConnectionFromToDelegate(IntPtr p, IntPtr from, IntPtr to);

        public delegate UInt64 GetMemoryAllocatedDelegate();

        public delegate int GetCountDelegate(IntPtr p);

        public delegate void InitInputsDelegate(IntPtr net, IntPtr arr);



        public static AddNodeDelegate __AddNode;
        public static AddConnectionDelegate __AddConnection;
        public static ComputeDelegate __Compute;
        public static NewNeuroNetDelegate __NewNeuroNet;
        public static FreeNeuroNetDelegate __FreeNeuroNet;
        public static GetConnectionWeightDelegate __GetWeight;
        public static ChangeConnectionWeightDelegate __ChangeWeight;
        public static FindNodeByIDDelegate __FindNode;
        public static FindConnectionDelegate __FindConnection;
        public static RemoveNodeDelegate __RemoveNode;
        public static RemoveConnectionDelegate __RemoveConnection;

        public static GetConnectionsDelegate __GetConnections;
        public static GetNodesDelegate __GetNodes;

        public static GetListLenghtDelegate __GetListLenght;
        public static GetEnumeratorDelegate __GetEnumerator;
        public static EnumeratorGoNextDelegate __EnumeratorGoNext;
        public static EnumeratorCurrentDelegate __GetEnumeratorCurrent;
        public static ResetEnumeratorDelegate __ResetEnumerator;

        public static GetNodeIDDelegate __GetNodeID;
        public static GetConnectionToDelegate __GetConnectionTo;
        public static GetGetConnectionFromDelegate __GetConnectionFrom;

        public static ClearConnectionsDelegate __ClearConnections;
        public static ClearNodesDelegate __ClearNodes;

        public static SetConnectionFromToDelegate __SetConnectionFromTo;

        public static GetCountDelegate __GetNodesCount;
        public static GetCountDelegate __GetConnectionsCount;

        public static InitInputsDelegate __InitInputs;





        static CNeuroNetWrapperFunctions()
        {
            string pv = "x86";
            if (Environment.Is64BitProcess)
            {
                pv = "x64";
            }
            string filePath = string.Format("{0}\\{1}\\{2}", Environment.CurrentDirectory, DllPath, string.Format(DllCont, pv));

            IntPtr libPtr = LoadLibrary(filePath);

            if (libPtr == IntPtr.Zero)
            {
                throw new PlatformNotSupportedException("Can't load CNeuroNet DDL");
            }
            LibPTR = libPtr;

            var err = GetLastError();




            IntPtr fun = GetProcAddress(LibPTR, "AddConnection");

            err = GetLastError();

            __AddConnection =
                Marshal.GetDelegateForFunctionPointer<AddConnectionDelegate>(fun);

            __AddNode = Marshal.GetDelegateForFunctionPointer<AddNodeDelegate>
                (GetProcAddress(LibPTR, "AddNode"));

            __Compute = Marshal.GetDelegateForFunctionPointer<ComputeDelegate>
                (GetProcAddress(LibPTR, "Compute"));

            __NewNeuroNet = Marshal.GetDelegateForFunctionPointer<NewNeuroNetDelegate>
                (GetProcAddress(LibPTR, "NewNeuroNet"));


            __FreeNeuroNet = Marshal.GetDelegateForFunctionPointer<FreeNeuroNetDelegate>
                (GetProcAddress(LibPTR, "FreeNeuroNet"));


            __GetWeight = Marshal.GetDelegateForFunctionPointer<GetConnectionWeightDelegate>
                (GetProcAddress(LibPTR, "GetConnectionWeight"));

            __ChangeWeight = Marshal.GetDelegateForFunctionPointer<ChangeConnectionWeightDelegate>
                (GetProcAddress(LibPTR, "ChangeConnectionWeight"));

            __FindNode = Marshal.GetDelegateForFunctionPointer<FindNodeByIDDelegate>
                (GetProcAddress(LibPTR, "FindNodeByID"));

            __FindConnection = Marshal.GetDelegateForFunctionPointer<FindConnectionDelegate>
                (GetProcAddress(LibPTR, "FindConnection"));

            __RemoveNode = Marshal.GetDelegateForFunctionPointer<RemoveNodeDelegate>
                (GetProcAddress(LibPTR, "RemoveNode"));

            __RemoveConnection = Marshal.GetDelegateForFunctionPointer<RemoveConnectionDelegate>
                (GetProcAddress(LibPTR, "RemoveConnection"));

            __GetConnections = Marshal.GetDelegateForFunctionPointer<GetConnectionsDelegate>
                (GetProcAddress(LibPTR, "GetConnections"));

            __GetNodes = Marshal.GetDelegateForFunctionPointer<GetNodesDelegate>
                (GetProcAddress(LibPTR, "GetNodes"));



            __GetListLenght = Marshal.GetDelegateForFunctionPointer<GetListLenghtDelegate>
                (GetProcAddress(LibPTR, "GetListLenght"));

            __EnumeratorGoNext = Marshal.GetDelegateForFunctionPointer<EnumeratorGoNextDelegate>
                (GetProcAddress(LibPTR, "EnumeratorGoNext"));

            __GetEnumeratorCurrent = Marshal.GetDelegateForFunctionPointer<EnumeratorCurrentDelegate>
                (GetProcAddress(LibPTR, "GetEnumeratorCurrent"));

            __ResetEnumerator = Marshal.GetDelegateForFunctionPointer<ResetEnumeratorDelegate>
                (GetProcAddress(LibPTR, "ResetEnumerator"));

            __GetEnumerator = Marshal.GetDelegateForFunctionPointer<GetEnumeratorDelegate>
                (GetProcAddress(LibPTR, "GetEnumerator"));




            __GetNodeID = Marshal.GetDelegateForFunctionPointer<GetNodeIDDelegate>
                (GetProcAddress(LibPTR, "GetNodeID"));

            __GetConnectionTo = Marshal.GetDelegateForFunctionPointer<GetConnectionToDelegate>
                (GetProcAddress(LibPTR, "GetConnectionTo"));

            __GetConnectionFrom = Marshal.GetDelegateForFunctionPointer<GetGetConnectionFromDelegate>
                (GetProcAddress(LibPTR, "GetConnectionFrom"));


            __ClearConnections = Marshal.GetDelegateForFunctionPointer<ClearConnectionsDelegate>
                (GetProcAddress(LibPTR, "ClearConnections"));

            __ClearNodes = Marshal.GetDelegateForFunctionPointer<ClearNodesDelegate>
                (GetProcAddress(LibPTR, "ClearNodes"));

            //static Func<IntPtr, int, int, IntPtr> __SetConnectionFromTo;

            __SetConnectionFromTo = Marshal.GetDelegateForFunctionPointer<SetConnectionFromToDelegate>
                (GetProcAddress(LibPTR, "SetConnectionFromTo"));



            /*     public static GetCountDelegate __GetNodesCount;
                 public static GetCountDelegate __GetConnectionsCount;*/


            __GetNodesCount = Marshal.GetDelegateForFunctionPointer<GetCountDelegate>
                (GetProcAddress(LibPTR, "GetNodesCount"));

            __GetConnectionsCount = Marshal.GetDelegateForFunctionPointer<GetCountDelegate>
                (GetProcAddress(LibPTR, "GetConnectionsCount"));


            __InitInputs = Marshal.GetDelegateForFunctionPointer<InitInputsDelegate>
                (GetProcAddress(LibPTR, "InitInputs"));


            /*
        public static InitInputsDelegate __InitInputs;
            __GetAllocatedMem = Marshal.GetDelegateForFunctionPointer<GetMemoryAllocatedDelegate>
            (GetProcAddress(LibPTR,"GetMemoryAllocated"));

            __GetAllocatedNN = Marshal.GetDelegateForFunctionPointer<GetMemoryAllocatedDelegate>
            (GetProcAddress(LibPTR, "GetNeuroNetsAllocated"));*/
        }

    }
}
