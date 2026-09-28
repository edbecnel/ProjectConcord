using System.Runtime.InteropServices;

namespace Edf.Desktop.Platform;

internal static class MacOSDockIcon
{
    private const string LibObjC = "/usr/lib/libobjc.A.dylib";
    private const string Foundation = "/System/Library/Frameworks/Foundation.framework/Foundation";

    [DllImport(LibObjC, EntryPoint = "objc_getClass")]
    private static extern IntPtr GetClass(string name);

    [DllImport(LibObjC, EntryPoint = "sel_registerName")]
    private static extern IntPtr GetSelector(string name);

    [DllImport(LibObjC, EntryPoint = "objc_msgSend")]
    private static extern IntPtr SendMessage(IntPtr receiver, IntPtr selector);

    [DllImport(LibObjC, EntryPoint = "objc_msgSend")]
    private static extern IntPtr SendMessageIntPtr(IntPtr receiver, IntPtr selector, IntPtr argument);

    [DllImport(LibObjC, EntryPoint = "objc_msgSend")]
    private static extern IntPtr SendMessageIntPtrIntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1, IntPtr arg2);

    [DllImport(LibObjC, EntryPoint = "objc_msgSend")]
    private static extern void SendMessageVoidIntPtr(IntPtr receiver, IntPtr selector, IntPtr argument);

    [DllImport(Foundation)]
    private static extern IntPtr CFDataCreate(IntPtr allocator, IntPtr bytes, nint length);

    public static void TrySetFromPngBytes(byte[] pngBytes)
    {
        if (!OperatingSystem.IsMacOS() || pngBytes.Length == 0)
        {
            return;
        }

        var pinned = GCHandle.Alloc(pngBytes, GCHandleType.Pinned);
        try
        {
            var data = CFDataCreate(IntPtr.Zero, pinned.AddrOfPinnedObject(), pngBytes.Length);
            if (data == IntPtr.Zero)
            {
                return;
            }

            var nsDataClass = GetClass("NSData");
            var nsData = SendMessageIntPtrIntPtr(
                SendMessage(nsDataClass, GetSelector("alloc")),
                GetSelector("initWithBytes:length:"),
                data,
                (IntPtr)pngBytes.Length);
            if (nsData == IntPtr.Zero)
            {
                return;
            }

            var nsImageClass = GetClass("NSImage");
            var image = SendMessageIntPtr(
                SendMessage(nsImageClass, GetSelector("alloc")),
                GetSelector("initWithData:"),
                nsData);
            if (image == IntPtr.Zero)
            {
                return;
            }

            var nsApplicationClass = GetClass("NSApplication");
            var sharedApplication = SendMessage(nsApplicationClass, GetSelector("sharedApplication"));
            SendMessageVoidIntPtr(
                sharedApplication,
                GetSelector("setApplicationIconImage:"),
                image);

            SendMessage(
                SendMessage(sharedApplication, GetSelector("dockTile")),
                GetSelector("display"));
        }
        catch (DllNotFoundException)
        {
        }
        catch (EntryPointNotFoundException)
        {
        }
        finally
        {
            pinned.Free();
        }
    }
}
