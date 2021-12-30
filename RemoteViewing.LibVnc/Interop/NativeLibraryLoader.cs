using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace RemoteViewing.LibVnc.Interop
{
    internal static class NativeLibraryLoader
    {
        static NativeLibraryLoader()
        {
#if !NETSTANDARD2_0 && !NET462
            NativeLibrary.SetDllImportResolver(typeof(NativeMethods).Assembly, ResolveDll);
#endif
        }

#if !NETSTANDARD2_0 && !NET462
        public static IntPtr ResolveDll(string libraryName, Assembly assembly, DllImportSearchPath? searchPath)
        {
            IntPtr lib = IntPtr.Zero;

            // Respsect the search directories (e.g. NuGet packages) when looking for native libraries.
            var delimiter = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ";" : ":";
            var nativeSearchDirectoriesString = AppContext.GetData("NATIVE_DLL_SEARCH_DIRECTORIES") as string;
            var nativeSearchDirectories = nativeSearchDirectoriesString?.Split(delimiter) ?? Array.Empty<string>();

            if (libraryName == NativeMethods.LibraryName)
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    // Various Unix package managers have chosen different names for the "libvncserver" shared library.
                    // Don't attempt to load libvncserver from NuGet packages; we don't distribute libvncserver on Linux.
                    if (NativeLibrary.TryLoad("libvncserver.so.1", assembly, default, out lib))
                    {
                        return lib;
                    }

                    if (NativeLibrary.TryLoad("libvncserver.so.0", assembly, default, out lib))
                    {
                        return lib;
                    }

                    if (NativeLibrary.TryLoad("libvncserver.so", assembly, default, out lib))
                    {
                        return lib;
                    }
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    // We distribute libvncserver.dll, so look in the search directories.
                    foreach (var directory in nativeSearchDirectories)
                    {
                        if (NativeLibrary.TryLoad(Path.Combine(directory, "libvncserver.dylib"), assembly, default, out lib))
                        {
                            return lib;
                        }
                    }
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // We distribute libvncserver.dll, so look in the search directories.
                    foreach (var directory in nativeSearchDirectories)
                    {
                        if (NativeLibrary.TryLoad(Path.Combine(directory, "vncserver.dll"), assembly, default, out lib))
                        {
                            return lib;
                        }
                    }
                }
            }
            else if (libraryName == NativeLogging.LoggerLibraryName)
            {
                // We distribute vnclogger for all platforms, so look in the search directories.
                foreach (var directory in nativeSearchDirectories)
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        if (NativeLibrary.TryLoad(Path.Combine(directory, "libvnclogger.so"), assembly, default, out lib))
                        {
                            return lib;
                        }
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        if (NativeLibrary.TryLoad(Path.Combine(directory, "libvnclogger.dylib"), assembly, default, out lib))
                        {
                            return lib;
                        }
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        if (NativeLibrary.TryLoad(Path.Combine(directory, "vnclogger.dll"), assembly, default, out lib))
                        {
                            return lib;
                        }
                    }
                }
            }

            // This function may return a null handle. If it does, individual functions loaded from it will throw a DllNotFoundException,
            // but not until an attempt is made to actually use the function (rather than load it). This matches how PInvokes behave.
            return lib;
        }
#endif
    }
}
