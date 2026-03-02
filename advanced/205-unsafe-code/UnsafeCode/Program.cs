using System;
using System.Runtime.InteropServices;

namespace UnsafeCode;

/// <summary>
/// Demonstrates unsafe code operations in C#.
/// Shows pointer manipulation, stackalloc, and direct memory access.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Unsafe Code Demo ===\n");
        Console.WriteLine("Demonstrates pointer operations and unsafe code.\n");

        // Demo 1: Pointer arithmetic
        Console.WriteLine("--- Pointer Arithmetic ---\n");
        PointerArithmeticDemo();

        // Demo 2: Stack allocation
        Console.WriteLine("\n--- Stack Allocation (stackalloc) ---\n");
        StackAllocationDemo();

        // Demo 3: Fixed statement
        Console.WriteLine("\n--- Fixed Statement (Pinning) ---\n");
        FixedStatementDemo();

        // Demo 4: Unsafe buffer operations
        Console.WriteLine("\n--- Unsafe Buffer Operations ---\n");
        UnsafeBufferDemo();
    }

    static unsafe void PointerArithmeticDemo()
    {
        int value = 42;
        int* ptr = &value;

        Console.WriteLine($"Value: {value}");
        Console.WriteLine($"Address: {(ulong)ptr:X}");
        Console.WriteLine($"Dereferenced: {*ptr}");

        // Pointer arithmetic
        int* ptr2 = ptr + 1;
        Console.WriteLine($"ptr + 1 address: {(ulong)ptr2:X}");
        Console.WriteLine($"Address difference: {(ulong)ptr2 - (ulong)ptr} bytes");

        // Array via pointer
        int[] array = [10, 20, 30, 40, 50];
        fixed (int* arrPtr = array)
        {
            Console.WriteLine($"\nArray via pointer:");
            for (int i = 0; i < array.Length; i++)
            {
                Console.WriteLine($"  array[{i}] = {*(arrPtr + i)}");
            }
        }
    }

    static unsafe void StackAllocationDemo()
    {
        // stackalloc allocates memory on the stack (faster than heap)
        int* buffer = stackalloc int[10];

        Console.WriteLine("Stack-allocated buffer:");
        for (int i = 0; i < 10; i++)
        {
            buffer[i] = i * i;
            Console.WriteLine($"  buffer[{i}] = {buffer[i]}");
        }

        // Span<T> with stackalloc (safe wrapper)
        Span<int> span = stackalloc int[5];
        for (int i = 0; i < 5; i++)
        {
            span[i] = (i + 1) * 10;
        }
        Console.WriteLine($"\nSpan from stackalloc: [{string.Join(", ", span.ToArray())}]");
    }

    static unsafe void FixedStatementDemo()
    {
        // Pin managed object to prevent GC movement
        string text = "Hello, Unsafe World!";
        
        fixed (char* ptr = text)
        {
            Console.WriteLine($"Pinned string: {text}");
            Console.WriteLine($"First char address: {(ulong)ptr:X}");
            
            // Iterate through characters
            char* current = ptr;
            Console.Write("Characters: ");
            while (*current != '\0')
            {
                Console.Write(*current);
                current++;
            }
            Console.WriteLine();
        }

        // Pin array
        byte[] data = [0x48, 0x65, 0x6C, 0x6C, 0x6F]; // "Hello" in ASCII
        fixed (byte* bytePtr = data)
        {
            Console.Write("Byte array as string: ");
            for (int i = 0; i < data.Length; i++)
            {
                Console.Write((char)bytePtr[i]);
            }
            Console.WriteLine();
        }
    }

    static unsafe void UnsafeBufferDemo()
    {
        // Create a buffer and manipulate it unsafely
        const int Size = 256;
        byte* buffer = stackalloc byte[Size];

        // Initialize with zeros
        for (int i = 0; i < Size; i++)
        {
            buffer[i] = 0;
        }

        // Write a pattern
        for (int i = 0; i < 16; i++)
        {
            buffer[i] = (byte)(i * 16);
        }

        Console.WriteLine("Buffer pattern (first 16 bytes):");
        for (int i = 0; i < 16; i++)
        {
            Console.Write($"{buffer[i],3:X2} ");
        }
        Console.WriteLine();

        // Unsafe copy
        byte[] source = [1, 2, 3, 4, 5, 6, 7, 8];
        fixed (byte* srcPtr = source)
        {
            // Copy source to buffer offset 32
            for (int i = 0; i < source.Length; i++)
            {
                buffer[32 + i] = srcPtr[i];
            }
        }

        Console.WriteLine($"\nAfter copy at offset 32:");
        for (int i = 32; i < 40; i++)
        {
            Console.Write($"{buffer[i],3:X2} ");
        }
        Console.WriteLine();
    }
}

// Unsafe utility methods
public static unsafe class UnsafeUtils
{
    /// <summary>
    /// Fast memory copy using pointers
    /// </summary>
    public static unsafe void FastCopy(byte* src, byte* dst, int length)
    {
        for (int i = 0; i < length; i++)
        {
            dst[i] = src[i];
        }
    }

    /// <summary>
    /// Fast memory set using pointers
    /// </summary>
    public static unsafe void FastSet(byte* buffer, byte value, int length)
    {
        for (int i = 0; i < length; i++)
        {
            buffer[i] = value;
        }
    }

    /// <summary>
    /// Compare two memory regions
    /// </summary>
    public static unsafe bool FastCompare(byte* a, byte* b, int length)
    {
        for (int i = 0; i < length; i++)
        {
            if (a[i] != b[i]) return false;
        }
        return true;
    }
}
