﻿using System;
using System.IO;
using System.Runtime.InteropServices;

namespace GraphMolWrap
{
    partial class RDKFuncsPINVOKE
    {
#if NETFRAMEWORK
        private const string ModuleName = "RDKFuncs";

        [System.Security.SuppressUnmanagedCodeSecurity]
        internal static class UnsafeNativeMethods
        {
            [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern bool SetDllDirectory(string lpPathName);
        }
#endif

        internal static void LoadDll()
        {
#if NETFRAMEWORK    
            var os = Environment.OSVersion;
            switch (os.Platform)
            {
                case PlatformID.Win32NT:
                    const string DllFileName = ModuleName + ".dll";
                    var cpu = Environment.Is64BitProcess ? "x64" : "x86";
                    var executingAsm = System.Reflection.Assembly.GetExecutingAssembly();
                    foreach (var subdir in new[] {
                        cpu, 
                        Path.Combine("runtimes", $"win-{cpu}", "native"),
                    })
                    {
                        if (SetDllDirectoryIfFileExist(Path.GetDirectoryName(executingAsm.Location), subdir, DllFileName))
                            break;
                        // for ASP.NET
                        var uri = new Uri(executingAsm.CodeBase);
                        if (uri.Scheme == "file")
                        {
                            if (SetDllDirectoryIfFileExist(Path.GetDirectoryName(uri.AbsolutePath), subdir, DllFileName))
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
#endif
        }

#if NETFRAMEWORK
        /// <summary>
        /// SetDllDirectory if <paramref name="directoryName"/>/<paramref name="subdir"/>/<paramref name="dllName"/> or <paramref name="directoryName"/>/<paramref name="dllName"/>exists.
        /// </summary>
        /// <param name="directoryName">Directory of the DLL.</param>
        /// <param name="subdir">Sub directory name, typically "x86" or "x64".</param>
        /// <param name="dllName">Base name of the DLL.</param>
        /// <returns><see langword="true"/> if file exists and set it.</returns>
        private static bool SetDllDirectoryIfFileExist(string directoryName, string subdir, string dllName)
        {
            if (SetDllDirectoryIfFileExist(Path.Combine(directoryName, subdir), dllName))
                return true;

            if (SetDllDirectoryIfFileExist(directoryName, dllName))
                return true;

            return false;
        }

        /// <summary>
        /// SetDllDirectory if <paramref name="directoryName"/>/<paramref name="dllName"/> exists.
        /// </summary>
        /// <param name="directoryName">Directory of the DLL.</param>
        /// <param name="dllName">Base name of the DLL.</param>
        /// <returns><see langword="true"/> if file exists and set it.</returns>
        private static bool SetDllDirectoryIfFileExist(string directoryName, string dllName)
        {
            var dllPath = Path.Combine(directoryName, dllName);
            if (File.Exists(dllPath))
            {
                UnsafeNativeMethods.SetDllDirectory(null);
                UnsafeNativeMethods.SetDllDirectory(directoryName);
                return true;
            }
            return false;
        }
#endif
    }
}
