using System;
using System.Runtime.InteropServices;

namespace InteropWrapper;

/// <summary>
/// Demonstrates P/Invoke and interop wrappers for calling native code.
/// Shows how to safely wrap native APIs in managed C# code.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Interop Wrapper Demo ===\n");
        Console.WriteLine("Demonstrates P/Invoke and native code interop.\n");

        // Demo 1: Platform information using native APIs
        Console.WriteLine("--- Platform Information ---\n");
        DisplayPlatformInfo();

        // Demo 2: Memory operations
        Console.WriteLine("\n--- Memory Operations ---\n");
        DemonstrateMemoryOps();

        // Demo 3: Safe native wrapper
        Console.WriteLine("\n--- Safe Native Wrapper ---\n");
        using var nativeWrapper = new SafeNativeBuffer(256);
        nativeWrapper.WriteString("Hello from C#!");
        Console.WriteLine($"Buffer content: {nativeWrapper.ReadString()}");
        Console.WriteLine($"Buffer size: {nativeWrapper.Size} bytes");
    }

    static void DisplayPlatformInfo()
    {
        Console.WriteLine($"OS: {Environment.OSVersion.Platform}");
        Console.WriteLine($"OS Version: {Environment.OSVersion.Version}");
        Console.WriteLine($"Process Architecture: {RuntimeInformation.ProcessArchitecture}");
        Console.WriteLine($"OS Architecture: {RuntimeInformation.OSArchitecture}");
        Console.WriteLine($"Framework Description: {RuntimeInformation.FrameworkDescription}");
        Console.WriteLine($"Is 64-bit: {Environment.Is64BitProcess}");

        // Get native library path
        string libcPath = RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
            ? "libc.so.6"
            : RuntimeInformation.IsOSPlatform(OSPlatform.OSX)
                ? "/usr/lib/libSystem.dylib"
                : "kernel32.dll";

        Console.WriteLine($"Native library: {libcPath}");
    }

    static void DemonstrateMemoryOps()
    {
        // Allocate unmanaged memory
        IntPtr ptr = Marshal.AllocHGlobal(100);
        
        try
        {
            // Write data
            Marshal.WriteInt32(ptr, 0, 42);
            Marshal.WriteInt64(ptr, 4, 123456789);
            
            // Read data
            int value1 = Marshal.ReadInt32(ptr, 0);
            long value2 = Marshal.ReadInt64(ptr, 4);
            
            Console.WriteLine($"Unmanaged memory test:");
            Console.WriteLine($"  Int32 at offset 0: {value1}");
            Console.WriteLine($"  Int64 at offset 4: {value2}");
            
            // String marshaling
            string managedString = "Hello, Native World!";
            IntPtr nativeString = Marshal.StringToHGlobalAnsi(managedString);
            
            try
            {
                string roundTrip = Marshal.PtrToStringAnsi(nativeString);
                Console.WriteLine($"\nString marshaling:");
                Console.WriteLine($"  Original: {managedString}");
                Console.WriteLine($"  Round-trip: {roundTrip}");
            }
            finally
            {
                Marshal.FreeHGlobal(nativeString);
            }
        }
        finally
        {
            Marshal.FreeHGlobal(ptr);
        }
    }
}

/// <summary>
/// Safe wrapper for unmanaged memory buffer.
/// Implements IDisposable for proper cleanup.
/// </summary>
public sealed class SafeNativeBuffer : IDisposable
{
    private IntPtr _buffer;
    private bool _disposed;

    public int Size { get; }

    public SafeNativeBuffer(int size)
    {
        if (size <= 0) throw new ArgumentException("Size must be positive", nameof(size));
        
        _buffer = Marshal.AllocHGlobal(size);
        Size = size;
        
        // Zero-initialize
        for (int i = 0; i < size; i++)
        {
            Marshal.WriteByte(_buffer, i, 0);
        }
    }

    public void WriteByte(int offset, byte value)
    {
        ThrowIfDisposed();
        if (offset < 0 || offset >= Size) throw new ArgumentOutOfRangeException(nameof(offset));
        Marshal.WriteByte(_buffer, offset, value);
    }

    public byte ReadByte(int offset)
    {
        ThrowIfDisposed();
        if (offset < 0 || offset >= Size) throw new ArgumentOutOfRangeException(nameof(offset));
        return Marshal.ReadByte(_buffer, offset);
    }

    public void WriteString(string text)
    {
        ThrowIfDisposed();
        var bytes = System.Text.Encoding.UTF8.GetBytes(text);
        if (bytes.Length >= Size) throw new ArgumentException("Text too large for buffer");
        
        for (int i = 0; i < bytes.Length; i++)
        {
            Marshal.WriteByte(_buffer, i, bytes[i]);
        }
        Marshal.WriteByte(_buffer, bytes.Length, 0); // Null terminator
    }

    public string ReadString()
    {
        ThrowIfDisposed();
        return Marshal.PtrToStringAnsi(_buffer) ?? string.Empty;
    }

    private void ThrowIfDisposed()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(SafeNativeBuffer));
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            if (_buffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_buffer);
                _buffer = IntPtr.Zero;
            }
            _disposed = true;
        }
    }
}
