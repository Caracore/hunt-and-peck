using System.Runtime.InteropServices;

namespace HuntAndPeck.NativeMethods;

public static class Kernel32
{
    [DllImport("kernel32.dll")]
    public static extern uint GetCurrentThreadId();
}
