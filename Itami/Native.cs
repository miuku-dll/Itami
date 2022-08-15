using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

using static Daedalus.Misc.Native.Advapi;
using static Daedalus.Misc.Native.User32;

namespace Daedalus.Misc
{
    public static class Native
    {
        #region User32
        public class User32
        {
            [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);

            [StructLayout(LayoutKind.Sequential)]
            public struct MSLLHOOKSTRUCT
            {
                public POINT pt;
                public int mouseData; // be careful, this must be ints, not uints (was wrong before I changed it...). regards, cmew.
                public int flags;
                public int time;
                public UIntPtr dwExtraInfo;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct POINT
            {
                public int X;
                public int Y;
            }

            public enum MOUSEEVENTF_FLAGS : uint
            {
                MOUSEEVENTF_ABSOLUTE = 0x8000,
                MOUSEEVENTF_LEFTDOWN = 0x0002,
                MOUSEEVENTF_LEFTUP = 0x0004,
                MOUSEEVENTF_MIDDLEDOWN = 0x0020,
                MOUSEEVENTF_MIDDLEUP = 0x0040,
                MOUSEEVENTF_MOVE = 0x0001,
                MOUSEEVENTF_RIGHTDOWN = 0x0008,
                MOUSEEVENTF_RIGHTUP = 0x0010,
                MOUSEEVENTF_WHEEL = 0x0800,
                MOUSEEVENTF_XDOWN = 0x0080,
                MOUSEEVENTF_XUP = 0x0100,
                MOUSEEVENTF_HWHEEL = 0x01000,

                UNKNOWN = 0x0
            }

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool GetCursorPos(out Point lpMousePoint);

            [DllImport("user32.dll")]
            public static extern void mouse_event(MOUSEEVENTF_FLAGS dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

            public const int KEYEVENTF_KEYDOWN = 0x0000; // New definition
            public const int KEYEVENTF_EXTENDEDKEY = 0x0001; //Key down flag
            public const int KEYEVENTF_KEYUP = 0x0002; //Key up flag

            [DllImport("user32.dll")]
            public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

            public delegate IntPtr HookProcedure(int nCode, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr SetWindowsHookEx(int idHook, HookProcedure lpfn, IntPtr hMod, uint dwThreadId);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool UnhookWindowsHookEx(IntPtr hhk);

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool SetCursorPos(int x, int y);

            [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
            public static extern void CopyMemory(IntPtr dest, IntPtr src, uint count);

            [DllImport("user32.dll")]
            public static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

            #region INPUT
            [Flags]
            public enum KEYEVENTF : uint
            {
                KEYDOWN = 0x0000,
                EXTENDEDKEY = 0x0001,
                KEYUP = 0x0002,
                SCANCODE = 0x0008,
                UNICODE = 0x0004
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct HARDWAREINPUT
            {
                public int uMsg;
                public short wParamL;
                public short wParamH;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct KEYBDINPUT
            {
                public VK wVk;
                public ScanCodeShort wScan;
                public KEYEVENTF dwFlags;
                public int time;
                public UIntPtr dwExtraInfo;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct MOUSEINPUT
            {
                public int dx;
                public int dy;
                public uint mouseData;
                public MOUSEEVENTF_FLAGS dwFlags;
                public uint time;
                public UIntPtr dwExtraInfo;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct INPUT
            {
                public uint type;
                public InputUnion U;
                public static int Size => Marshal.SizeOf(typeof(INPUT));
            }

            [StructLayout(LayoutKind.Explicit)]
            public struct InputUnion
            {
                [FieldOffset(0)]
                public MOUSEINPUT mi;
                [FieldOffset(0)]
                public KEYBDINPUT ki;
                [FieldOffset(0)]
                public HARDWAREINPUT hi;
            }

            #endregion
        }
        #endregion



        #region Kernel32
        public class Kernel32
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct SYSTEM_INFO
            {
                public PROCESSOR_INFO_UNION p;
                public uint dwPageSize;
                public IntPtr lpMinimumApplicationAddress;
                public IntPtr lpMaximumApplicationAddress;
                public UIntPtr dwActiveProcessorMask;
                public uint dwNumberOfProcessors;
                public uint dwProcessorType;
                public uint dwAllocationGranularity;
                public ushort wProcessorLevel;
                public ushort wProcessorRevision;
            };

            [StructLayout(LayoutKind.Explicit)]
            public struct PROCESSOR_INFO_UNION
            {
                [FieldOffset(0)] public uint dwOemId;
                [FieldOffset(0)] public ushort wProcessorArchitecture;
                [FieldOffset(2)] public ushort wReserved;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct MEMORY_BASIC_INFORMATION
            {
                public UIntPtr BaseAddress;
                public UIntPtr AllocationBase;
                public uint AllocationProtect;
                public IntPtr RegionSize;
                public uint State;
                public uint Protect;
                public uint Type;
            }

            [Flags]
            public enum AllocationType : uint
            {
                Commit = 0x1000,
                Reserve = 0x2000,
                Decommit = 0x4000,
                Release = 0x8000,
                Reset = 0x80000,
                Physical = 0x400000,
                TopDown = 0x100000,
                WriteWatch = 0x200000,
                LargePages = 0x20000000
            }

            [Flags]
            public enum MemoryProtection : uint
            {
                Execute = 0x10,
                ExecuteRead = 0x20,
                ExecuteReadWrite = 0x40,
                ExecuteWriteCopy = 0x80,
                NoAccess = 0x01,
                ReadOnly = 0x02,
                ReadWrite = 0x04,
                WriteCopy = 0x08,
                GuardModifierflag = 0x100,
                NoCacheModifierflag = 0x200,
                WriteCombineModifierflag = 0x400
            }

            [Flags]
            public enum FreeType
            {
                Decommit = 0x4000,
                Release = 0x8000,
            }

            [DllImport("kernel32.dll")]
            public static extern int VirtualQuery(ref IntPtr lpAddress, ref MEMORY_BASIC_INFORMATION lpBuffer, int dwLength);

            [DllImport("kernel32.dll")]
            public static extern IntPtr VirtualAlloc(IntPtr lpAddress, uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

            [DllImport("kernel32.dll", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
            public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern IntPtr GetModuleHandleW([MarshalAs(UnmanagedType.LPWStr)] string lpModuleName);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            public static extern IntPtr GetModuleHandle(string lpModuleName);

            [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
            public static extern bool VirtualFree(IntPtr lpAddress, int dwSize, FreeType dwFreeType);

            [DllImport("kernel32.dll")]
            public static extern bool VirtualProtect(IntPtr lpAddress, int dwSize, MemoryProtection flNewProtect, out MemoryProtection lpflOldProtect);

            [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
            public static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool FreeLibrary(IntPtr hModule);

            [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
            public static unsafe extern bool DeviceIoControl(IntPtr hDevice, uint dwIoControlCode, void* lpInBuffer, uint nInBufferSize, void* lpOutBuffer, uint nOutBufferSize, ulong* lpBytesReturned, uint lpOverlapped);

            [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, CharSet = CharSet.Auto)]
            public static unsafe extern bool DeviceIoControl(IntPtr hDevice, uint dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize, IntPtr lpOutBuffer, uint nOutBufferSize, ulong* lpBytesReturned, uint lpOverlapped);
        }
        #endregion



        #region Portable Executable
        public class PE
        {
            #region File Header Structures

            public struct IMAGE_DOS_HEADER
            {      // DOS .EXE header
                public UInt16 e_magic;              // Magic number
                public UInt16 e_cblp;               // Bytes on last page of file
                public UInt16 e_cp;                 // Pages in file
                public UInt16 e_crlc;               // Relocations
                public UInt16 e_cparhdr;            // Size of header in paragraphs
                public UInt16 e_minalloc;           // Minimum extra paragraphs needed
                public UInt16 e_maxalloc;           // Maximum extra paragraphs needed
                public UInt16 e_ss;                 // Initial (relative) SS value
                public UInt16 e_sp;                 // Initial SP value
                public UInt16 e_csum;               // Checksum
                public UInt16 e_ip;                 // Initial IP value
                public UInt16 e_cs;                 // Initial (relative) CS value
                public UInt16 e_lfarlc;             // File address of relocation table
                public UInt16 e_ovno;               // Overlay number
                public UInt16 e_res_0;              // Reserved words
                public UInt16 e_res_1;              // Reserved words
                public UInt16 e_res_2;              // Reserved words
                public UInt16 e_res_3;              // Reserved words
                public UInt16 e_oemid;              // OEM identifier (for e_oeminfo)
                public UInt16 e_oeminfo;            // OEM information; e_oemid specific
                public UInt16 e_res2_0;             // Reserved words
                public UInt16 e_res2_1;             // Reserved words
                public UInt16 e_res2_2;             // Reserved words
                public UInt16 e_res2_3;             // Reserved words
                public UInt16 e_res2_4;             // Reserved words
                public UInt16 e_res2_5;             // Reserved words
                public UInt16 e_res2_6;             // Reserved words
                public UInt16 e_res2_7;             // Reserved words
                public UInt16 e_res2_8;             // Reserved words
                public UInt16 e_res2_9;             // Reserved words
                public UInt32 e_lfanew;             // File address of new exe header
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct IMAGE_DATA_DIRECTORY
            {
                public UInt32 VirtualAddress;
                public UInt32 Size;
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct IMAGE_OPTIONAL_HEADER32
            {
                public UInt16 Magic;
                public Byte MajorLinkerVersion;
                public Byte MinorLinkerVersion;
                public UInt32 SizeOfCode;
                public UInt32 SizeOfInitializedData;
                public UInt32 SizeOfUninitializedData;
                public UInt32 AddressOfEntryPoint;
                public UInt32 BaseOfCode;
                public UInt32 BaseOfData;
                public UInt32 ImageBase;
                public UInt32 SectionAlignment;
                public UInt32 FileAlignment;
                public UInt16 MajorOperatingSystemVersion;
                public UInt16 MinorOperatingSystemVersion;
                public UInt16 MajorImageVersion;
                public UInt16 MinorImageVersion;
                public UInt16 MajorSubsystemVersion;
                public UInt16 MinorSubsystemVersion;
                public UInt32 Win32VersionValue;
                public UInt32 SizeOfImage;
                public UInt32 SizeOfHeaders;
                public UInt32 CheckSum;
                public UInt16 Subsystem;
                public UInt16 DllCharacteristics;
                public UInt32 SizeOfStackReserve;
                public UInt32 SizeOfStackCommit;
                public UInt32 SizeOfHeapReserve;
                public UInt32 SizeOfHeapCommit;
                public UInt32 LoaderFlags;
                public UInt32 NumberOfRvaAndSizes;

                public IMAGE_DATA_DIRECTORY ExportTable;
                public IMAGE_DATA_DIRECTORY ImportTable;
                public IMAGE_DATA_DIRECTORY ResourceTable;
                public IMAGE_DATA_DIRECTORY ExceptionTable;
                public IMAGE_DATA_DIRECTORY CertificateTable;
                public IMAGE_DATA_DIRECTORY BaseRelocationTable;
                public IMAGE_DATA_DIRECTORY Debug;
                public IMAGE_DATA_DIRECTORY Architecture;
                public IMAGE_DATA_DIRECTORY GlobalPtr;
                public IMAGE_DATA_DIRECTORY TLSTable;
                public IMAGE_DATA_DIRECTORY LoadConfigTable;
                public IMAGE_DATA_DIRECTORY BoundImport;
                public IMAGE_DATA_DIRECTORY IAT;
                public IMAGE_DATA_DIRECTORY DelayImportDescriptor;
                public IMAGE_DATA_DIRECTORY CLRRuntimeHeader;
                public IMAGE_DATA_DIRECTORY Reserved;
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct IMAGE_OPTIONAL_HEADER64
            {
                public UInt16 Magic;
                public Byte MajorLinkerVersion;
                public Byte MinorLinkerVersion;
                public UInt32 SizeOfCode;
                public UInt32 SizeOfInitializedData;
                public UInt32 SizeOfUninitializedData;
                public UInt32 AddressOfEntryPoint;
                public UInt32 BaseOfCode;
                public UInt64 ImageBase;
                public UInt32 SectionAlignment;
                public UInt32 FileAlignment;
                public UInt16 MajorOperatingSystemVersion;
                public UInt16 MinorOperatingSystemVersion;
                public UInt16 MajorImageVersion;
                public UInt16 MinorImageVersion;
                public UInt16 MajorSubsystemVersion;
                public UInt16 MinorSubsystemVersion;
                public UInt32 Win32VersionValue;
                public UInt32 SizeOfImage;
                public UInt32 SizeOfHeaders;
                public UInt32 CheckSum;
                public UInt16 Subsystem;
                public UInt16 DllCharacteristics;
                public UInt64 SizeOfStackReserve;
                public UInt64 SizeOfStackCommit;
                public UInt64 SizeOfHeapReserve;
                public UInt64 SizeOfHeapCommit;
                public UInt32 LoaderFlags;
                public UInt32 NumberOfRvaAndSizes;

                public IMAGE_DATA_DIRECTORY ExportTable;
                public IMAGE_DATA_DIRECTORY ImportTable;
                public IMAGE_DATA_DIRECTORY ResourceTable;
                public IMAGE_DATA_DIRECTORY ExceptionTable;
                public IMAGE_DATA_DIRECTORY CertificateTable;
                public IMAGE_DATA_DIRECTORY BaseRelocationTable;
                public IMAGE_DATA_DIRECTORY Debug;
                public IMAGE_DATA_DIRECTORY Architecture;
                public IMAGE_DATA_DIRECTORY GlobalPtr;
                public IMAGE_DATA_DIRECTORY TLSTable;
                public IMAGE_DATA_DIRECTORY LoadConfigTable;
                public IMAGE_DATA_DIRECTORY BoundImport;
                public IMAGE_DATA_DIRECTORY IAT;
                public IMAGE_DATA_DIRECTORY DelayImportDescriptor;
                public IMAGE_DATA_DIRECTORY CLRRuntimeHeader;
                public IMAGE_DATA_DIRECTORY Reserved;
            }

            [StructLayout(LayoutKind.Sequential, Pack = 1)]
            public struct IMAGE_FILE_HEADER
            {
                public UInt16 Machine;
                public UInt16 NumberOfSections;
                public UInt32 TimeDateStamp;
                public UInt32 PointerToSymbolTable;
                public UInt32 NumberOfSymbols;
                public UInt16 SizeOfOptionalHeader;
                public UInt16 Characteristics;
            }

            // Grabbed the following 2 definitions from http://www.pinvoke.net/default.aspx/Structures/IMAGE_SECTION_HEADER.html

            [StructLayout(LayoutKind.Explicit)]
            public struct IMAGE_SECTION_HEADER
            {
                [FieldOffset(0)]
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
                public char[] Name;
                [FieldOffset(8)]
                public UInt32 VirtualSize;
                [FieldOffset(12)]
                public UInt32 VirtualAddress;
                [FieldOffset(16)]
                public UInt32 SizeOfRawData;
                [FieldOffset(20)]
                public UInt32 PointerToRawData;
                [FieldOffset(24)]
                public UInt32 PointerToRelocations;
                [FieldOffset(28)]
                public UInt32 PointerToLinenumbers;
                [FieldOffset(32)]
                public UInt16 NumberOfRelocations;
                [FieldOffset(34)]
                public UInt16 NumberOfLinenumbers;
                [FieldOffset(36)]
                public DataSectionFlags Characteristics;

                public string Section
                {
                    get { return new string(Name); }
                }
            }

            [Flags]
            public enum DataSectionFlags : uint
            {
                /// <summary>
                /// Reserved for future use.
                /// </summary>
                TypeReg = 0x00000000,
                /// <summary>
                /// Reserved for future use.
                /// </summary>
                TypeDsect = 0x00000001,
                /// <summary>
                /// Reserved for future use.
                /// </summary>
                TypeNoLoad = 0x00000002,
                /// <summary>
                /// Reserved for future use.
                /// </summary>
                TypeGroup = 0x00000004,
                /// <summary>
                /// The section should not be padded to the next boundary. This flag is obsolete and is replaced by IMAGE_SCN_ALIGN_1BYTES. This is valid only for object files.
                /// </summary>
                TypeNoPadded = 0x00000008,
                /// <summary>
                /// Reserved for future use.
                /// </summary>
                TypeCopy = 0x00000010,
                /// <summary>
                /// The section contains executable code.
                /// </summary>
                ContentCode = 0x00000020,
                /// <summary>
                /// The section contains initialized data.
                /// </summary>
                ContentInitializedData = 0x00000040,
                /// <summary>
                /// The section contains uninitialized data.
                /// </summary>
                ContentUninitializedData = 0x00000080,
                /// <summary>
                /// Reserved for future use.
                /// </summary>
                LinkOther = 0x00000100,
                /// <summary>
                /// The section contains comments or other information. The .drectve section has this type. This is valid for object files only.
                /// </summary>
                LinkInfo = 0x00000200,
                /// <summary>
                /// Reserved for future use.
                /// </summary>
                TypeOver = 0x00000400,
                /// <summary>
                /// The section will not become part of the image. This is valid only for object files.
                /// </summary>
                LinkRemove = 0x00000800,
                /// <summary>
                /// The section contains COMDAT data. For more information, see section 5.5.6, COMDAT Sections (Object Only). This is valid only for object files.
                /// </summary>
                LinkComDat = 0x00001000,
                /// <summary>
                /// Reset speculative exceptions handling bits in the TLB entries for this section.
                /// </summary>
                NoDeferSpecExceptions = 0x00004000,
                /// <summary>
                /// The section contains data referenced through the global pointer (GP).
                /// </summary>
                RelativeGP = 0x00008000,
                /// <summary>
                /// Reserved for future use.
                /// </summary>
                MemPurgeable = 0x00020000,
                /// <summary>
                /// Reserved for future use.
                /// </summary>
                Memory16Bit = 0x00020000,
                /// <summary>
                /// Reserved for future use.
                /// </summary>
                MemoryLocked = 0x00040000,
                /// <summary>
                /// Reserved for future use.
                /// </summary>
                MemoryPreload = 0x00080000,
                /// <summary>
                /// Align data on a 1-byte boundary. Valid only for object files.
                /// </summary>
                Align1Bytes = 0x00100000,
                /// <summary>
                /// Align data on a 2-byte boundary. Valid only for object files.
                /// </summary>
                Align2Bytes = 0x00200000,
                /// <summary>
                /// Align data on a 4-byte boundary. Valid only for object files.
                /// </summary>
                Align4Bytes = 0x00300000,
                /// <summary>
                /// Align data on an 8-byte boundary. Valid only for object files.
                /// </summary>
                Align8Bytes = 0x00400000,
                /// <summary>
                /// Align data on a 16-byte boundary. Valid only for object files.
                /// </summary>
                Align16Bytes = 0x00500000,
                /// <summary>
                /// Align data on a 32-byte boundary. Valid only for object files.
                /// </summary>
                Align32Bytes = 0x00600000,
                /// <summary>
                /// Align data on a 64-byte boundary. Valid only for object files.
                /// </summary>
                Align64Bytes = 0x00700000,
                /// <summary>
                /// Align data on a 128-byte boundary. Valid only for object files.
                /// </summary>
                Align128Bytes = 0x00800000,
                /// <summary>
                /// Align data on a 256-byte boundary. Valid only for object files.
                /// </summary>
                Align256Bytes = 0x00900000,
                /// <summary>
                /// Align data on a 512-byte boundary. Valid only for object files.
                /// </summary>
                Align512Bytes = 0x00A00000,
                /// <summary>
                /// Align data on a 1024-byte boundary. Valid only for object files.
                /// </summary>
                Align1024Bytes = 0x00B00000,
                /// <summary>
                /// Align data on a 2048-byte boundary. Valid only for object files.
                /// </summary>
                Align2048Bytes = 0x00C00000,
                /// <summary>
                /// Align data on a 4096-byte boundary. Valid only for object files.
                /// </summary>
                Align4096Bytes = 0x00D00000,
                /// <summary>
                /// Align data on an 8192-byte boundary. Valid only for object files.
                /// </summary>
                Align8192Bytes = 0x00E00000,
                /// <summary>
                /// The section contains extended relocations.
                /// </summary>
                LinkExtendedRelocationOverflow = 0x01000000,
                /// <summary>
                /// The section can be discarded as needed.
                /// </summary>
                MemoryDiscardable = 0x02000000,
                /// <summary>
                /// The section cannot be cached.
                /// </summary>
                MemoryNotCached = 0x04000000,
                /// <summary>
                /// The section is not pageable.
                /// </summary>
                MemoryNotPaged = 0x08000000,
                /// <summary>
                /// The section can be shared in memory.
                /// </summary>
                MemoryShared = 0x10000000,
                /// <summary>
                /// The section can be executed as code.
                /// </summary>
                MemoryExecute = 0x20000000,
                /// <summary>
                /// The section can be read.
                /// </summary>
                MemoryRead = 0x40000000,
                /// <summary>
                /// The section can be written to.
                /// </summary>
                MemoryWrite = 0x80000000
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct IMAGE_EXPORT_DIRECTORY
            {
                public UInt32 Characteristics;
                public UInt32 TimeDateStamp;
                public UInt16 MajorVersion;
                public UInt16 MinorVersion;
                public UInt32 Name;
                public UInt32 Base;
                public UInt32 NumberOfFunctions;
                public UInt32 NumberOfNames;
                public UInt32 AddressOfFunctions;     // RVA from base of image
                public UInt32 AddressOfNames;     // RVA from base of image
                public UInt32 AddressOfNameOrdinals;  // RVA from base of image
            }

            #endregion File Header Structures

            #region Private Fields

            /// <summary>
            /// The DOS header
            /// </summary>
            public IMAGE_DOS_HEADER dosHeader;
            /// <summary>
            /// The file header
            /// </summary>
            public IMAGE_FILE_HEADER fileHeader;
            /// <summary>
            /// Optional 32 bit file header 
            /// </summary>
            public IMAGE_OPTIONAL_HEADER32 optionalHeader32;
            /// <summary>
            /// Optional 64 bit file header 
            /// </summary>
            public IMAGE_OPTIONAL_HEADER64 optionalHeader64;
            /// <summary>
            /// Image Section headers. Number of sections is in the file header.
            /// </summary>
            public IMAGE_SECTION_HEADER[] imageSectionHeaders;

            #endregion Private Fields



            #region Properties

            /*
             https://stackoverflow.com/questions/8782771/loading-pe-headers
             
PIMAGE_DOS_HEADER pidh = (PIMAGE_DOS_HEADER)buffer;
PIMAGE_NT_HEADERS pinh = (PIMAGE_NT_HEADERS)((BYTE*)pidh + pidh->e_lfanew);
PIMAGE_FILE_HEADER pifh = (PIMAGE_FILE_HEADER)&pinh->FileHeader;
PIMAGE_OPTIONAL_HEADER pioh = (PIMAGE_OPTIONAL_HEADER)&pinh->OptionalHeader;
PIMAGE_SECTION_HEADER pish = (PIMAGE_SECTION_HEADER)((BYTE*)pinh + sizeof(IMAGE_NT_HEADERS) + (pifh->NumberOfSections - 1) * sizeof(IMAGE_SECTION_HEADER));
            */

            /// <summary>
            /// Returns the size of the PE Headers. (Excluding Sections, Resources, etc.)
            /// </summary>
            public int Size
            {
                get
                {
                    var counter = 0;

                    counter += Marshal.SizeOf(dosHeader); // dosheader
                    counter += (int)dosHeader.e_lfanew - counter; // offset
                    counter += 4; // signature 
                    counter += Marshal.SizeOf(fileHeader); // fileheader

                    if (Is32BitHeader)
                    {
                        counter += Marshal.SizeOf(optionalHeader32);
                    }
                    else
                    {
                        counter += Marshal.SizeOf(optionalHeader64);
                    }

                    foreach (var sectionHeader in ImageSectionHeaders)
                    {
                        counter += Marshal.SizeOf(sectionHeader);
                    }

                    return counter;
                }
            }

            /// <summary>
            /// Checks if the PE is valid
            /// </summary>
            public bool IsValidPE
            {
                get
                {

                    // TO DO 

                    return true;
                }
            }

            /// <summary>
            /// Gets if the file header is 32 bit or not
            /// </summary>
            public bool Is32BitHeader
            {
                get
                {
                    UInt16 IMAGE_FILE_32BIT_MACHINE = 0x0100;
                    return (IMAGE_FILE_32BIT_MACHINE & FileHeader.Characteristics) == IMAGE_FILE_32BIT_MACHINE;
                }
            }

            /// <summary>
            /// Gets the file header
            /// </summary>
            public IMAGE_FILE_HEADER FileHeader
            {
                get
                {
                    return fileHeader;
                }
            }

            /// <summary>
            /// Gets the optional header
            /// </summary>
            public IMAGE_OPTIONAL_HEADER32 OptionalHeader32
            {
                get
                {
                    return optionalHeader32;
                }
            }

            /// <summary>
            /// Gets the optional header
            /// </summary>
            public IMAGE_OPTIONAL_HEADER64 OptionalHeader64
            {
                get
                {
                    return optionalHeader64;
                }
            }

            /// <summary>
            /// Gets the image section headers
            /// </summary>
            public IMAGE_SECTION_HEADER[] ImageSectionHeaders
            {
                get
                {
                    return imageSectionHeaders;
                }
            }

            /// <summary>
            /// Gets the timestamp from the file header
            /// </summary>
            public DateTime TimeStamp
            {
                get
                {
                    // Timestamp is a date offset from 1970
                    DateTime returnValue = new DateTime(1970, 1, 1, 0, 0, 0);

                    // Add in the number of seconds since 1970/1/1
                    returnValue = returnValue.AddSeconds(fileHeader.TimeDateStamp);
                    // Adjust to local timezone
                    returnValue += TimeZone.CurrentTimeZone.GetUtcOffset(returnValue);

                    return returnValue;
                }
            }
            #endregion Properties



            #region Methods

            /// <summary>
            /// Get a PE Header from a File.
            /// </summary>
            /// <param name="filePath">The path to your File.</param>
            /// <returns></returns>
            public static PE FromFile(string filePath)
            {
                if (File.Exists(filePath) == false)
                    return null;

                try
                {
                    using (FileStream stream = new FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        return FromStream(stream);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                return null;
            }

            /// <summary>
            /// Get a PE Header from a Stream.
            /// </summary>
            /// <param name="stream">The Stream.</param>
            /// <returns></returns>
            public static PE FromStream(Stream stream)
            {
                if (stream == null)
                    return null;

                try
                {
                    PE result = new PE();

                    using (BinaryReader reader = new BinaryReader(stream))
                    {
                        
                    }

                    return result;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                return null;
            }

            /// <summary>
            /// Get a PE Header from an Address.
            /// </summary>
            /// <param name="addy">The address you want to get the PE Header from.</param>
            /// <returns></returns>
            public static PE FromAddress(IntPtr addy)
            {
                if (addy == IntPtr.Zero)
                    return null;

                try
                {
                    unsafe
                    {
                        using (UnmanagedMemoryStream ms = new UnmanagedMemoryStream((byte*)addy.ToPointer(), 4096))
                        {
                            return FromStream(ms);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                return null;
            }

            /// <summary>
            /// Gets a PE header Object from an array of bytes/shellcode.
            /// </summary>
            /// <param name="bytes">The bytes.</param>
            /// <returns></returns>
            public static PE FromBytes(byte[] bytes)
            {
                if (bytes == null || bytes.Length == 0)
                    return null;

                try
                {
                    using (MemoryStream ms = new MemoryStream(bytes))
                    {
                        return FromStream(ms);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                return null;
            }

            #endregion
        }
        #endregion



        #region advapi
        public class Advapi
        {
            [Flags]
            public enum SERVICE_CONTROL : uint
            {
                STOP = 0x00000001,
                PAUSE = 0x00000002,
                CONTINUE = 0x00000003,
                INTERROGATE = 0x00000004,
                SHUTDOWN = 0x00000005,
                PARAMCHANGE = 0x00000006,
                NETBINDADD = 0x00000007,
                NETBINDREMOVE = 0x00000008,
                NETBINDENABLE = 0x00000009,
                NETBINDDISABLE = 0x0000000A,
                DEVICEEVENT = 0x0000000B,
                HARDWAREPROFILECHANGE = 0x0000000C,
                POWEREVENT = 0x0000000D,
                SESSIONCHANGE = 0x0000000E
            }

            [Flags]
            public enum SERVICE_TYPE : int
            {
                SERVICE_KERNEL_DRIVER = 0x00000001,
                SERVICE_FILE_SYSTEM_DRIVER = 0x00000002,
                SERVICE_WIN32_OWN_PROCESS = 0x00000010,
                SERVICE_WIN32_SHARE_PROCESS = 0x00000020,
                SERVICE_INTERACTIVE_PROCESS = 0x00000100
            }

            public enum SERVICE_STATE : uint
            {
                SERVICE_STOPPED = 0x00000001,
                SERVICE_START_PENDING = 0x00000002,
                SERVICE_STOP_PENDING = 0x00000003,
                SERVICE_RUNNING = 0x00000004,
                SERVICE_CONTINUE_PENDING = 0x00000005,
                SERVICE_PAUSE_PENDING = 0x00000006,
                SERVICE_PAUSED = 0x00000007
            }

            [StructLayout(LayoutKind.Sequential, Pack = 0)]
            public struct SERVICE_STATUS
            {
                public SERVICE_TYPE dwServiceType;
                public SERVICE_STATE dwCurrentState;
                public uint dwControlsAccepted;
                public uint dwWin32ExitCode;
                public uint dwServiceSpecificExitCode;
                public uint dwCheckPoint;
                public uint dwWaitHint;
            }

            [DllImport("advapi32.dll", EntryPoint = "OpenSCManagerW", ExactSpelling = true, CharSet = CharSet.Unicode, SetLastError = true)]
            public static extern IntPtr OpenSCManager(uint machineName, uint databaseName, uint dwAccess);

            [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern IntPtr OpenService(IntPtr hSCManager, string lpServiceName, uint dwDesiredAccess);

            [DllImport("advapi32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool CloseServiceHandle(IntPtr hSCObject);

            [DllImport("advapi32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool ControlService(IntPtr hService, SERVICE_CONTROL dwControl, ref SERVICE_STATUS lpServiceStatus);

            [DllImport("advapi32", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool StartService(
                IntPtr hService,
                int dwNumServiceArgs,
                string[] lpServiceArgVectors
            );

            [DllImport("advapi32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool DeleteService(IntPtr hService);

            [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern IntPtr CreateServiceW(
                IntPtr hSCManager,
                string lpServiceName,
                string lpDisplayName,
                uint dwDesiredAccess,
                uint dwServiceType,
                uint dwStartType,
                uint dwErrorControl,
                string lpBinaryPathName,
                uint lpLoadOrderGroup,
                uint lpdwTagId,
                uint lpdwTagId1,
                uint lpDependencies,
                uint lpServiceStartName,
                uint lpPassword
            );

        }
        #endregion

        #region NtDll

        public class NTDll
        {
            [StructLayout(LayoutKind.Sequential, Pack = 0)]
            public struct IO_STATUS_BLOCK
            {
                public uint status;
                public IntPtr information;
            }

            public struct OBJECT_ATTRIBUTES
            {
                public Int32 Length;
                public IntPtr RootDirectory;
                public IntPtr ObjectName;
                public uint Attributes;
                public IntPtr SecurityDescriptor;
                public IntPtr SecurityQualityOfService;
            }

            [DllImport("ntdll.dll", CharSet = CharSet.Auto)]
            public static unsafe extern uint NtOpenFile(IntPtr* FileHandle, uint DesiredAccess, OBJECT_ATTRIBUTES* ObjectAttributes, IO_STATUS_BLOCK* IoStatusBlock, uint ShareAccess, uint OpenOptions);
        }

        #endregion
    }
}
