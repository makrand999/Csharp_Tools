using System;

namespace BitwiseOperations;

/// <summary>
/// Demonstrates bitwise operations and bit manipulation
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Bitwise Operations & Bit Manipulation ===\n");

        // Demo 1: Basic bitwise operators
        Console.WriteLine("--- Basic Bitwise Operators ---");
        int a = 12; // 1100 in binary
        int b = 5;  // 0101 in binary

        Console.WriteLine($"a = {a} (binary: {Convert.ToString(a, 2).PadLeft(8, '0')})");
        Console.WriteLine($"b = {b} (binary: {Convert.ToString(b, 2).PadLeft(8, '0')})");
        Console.WriteLine();
        Console.WriteLine($"a & b (AND)      = {a & b,3} (binary: {Convert.ToString(a & b, 2).PadLeft(8, '0')})");
        Console.WriteLine($"a | b (OR)       = {a | b,3} (binary: {Convert.ToString(a | b, 2).PadLeft(8, '0')})");
        Console.WriteLine($"a ^ b (XOR)      = {a ^ b,3} (binary: {Convert.ToString(a ^ b, 2).PadLeft(8, '0')})");
        Console.WriteLine($"~a (NOT)         = {~a,3} (binary: {Convert.ToString(~a, 2).PadLeft(8, '0')})");

        // Demo 2: Bit shifting
        Console.WriteLine("\n--- Bit Shifting ---");
        int value = 8; // 1000 in binary
        Console.WriteLine($"Original: {value} (binary: {Convert.ToString(value, 2).PadLeft(8, '0')})");
        Console.WriteLine($"Left shift << 1:  {value << 1,3} (binary: {Convert.ToString(value << 1, 2).PadLeft(8, '0')})");
        Console.WriteLine($"Left shift << 2:  {value << 2,3} (binary: {Convert.ToString(value << 2, 2).PadLeft(8, '0')})");
        Console.WriteLine($"Right shift >> 1: {value >> 1,3} (binary: {Convert.ToString(value >> 1, 2).PadLeft(8, '0')})");
        Console.WriteLine($"Right shift >> 2: {value >> 2,3} (binary: {Convert.ToString(value >> 2, 2).PadLeft(8, '0')})");

        // Demo 3: Check if bit is set
        Console.WriteLine("\n--- Check If Bit Is Set ---");
        int flags = 0b10110100;
        for (int i = 0; i < 8; i++)
        {
            bool isSet = (flags & (1 << i)) != 0;
            Console.WriteLine($"Bit {i}: {(isSet ? "1" : "0")}");
        }

        // Demo 4: Set, Clear, Toggle bits
        Console.WriteLine("\n--- Set, Clear, Toggle Bits ---");
        int num = 0b00000000;
        Console.WriteLine($"Initial: {Convert.ToString(num, 2).PadLeft(8, '0')}");

        num |= (1 << 3); // Set bit 3
        Console.WriteLine($"Set bit 3: {Convert.ToString(num, 2).PadLeft(8, '0')}");

        num |= (1 << 5); // Set bit 5
        Console.WriteLine($"Set bit 5: {Convert.ToString(num, 2).PadLeft(8, '0')}");

        num &= ~(1 << 3); // Clear bit 3
        Console.WriteLine($"Clear bit 3: {Convert.ToString(num, 2).PadLeft(8, '0')}");

        num ^= (1 << 5); // Toggle bit 5
        Console.WriteLine($"Toggle bit 5: {Convert.ToString(num, 2).PadLeft(8, '0')}");

        num ^= (1 << 5); // Toggle bit 5 again
        Console.WriteLine($"Toggle bit 5 again: {Convert.ToString(num, 2).PadLeft(8, '0')}");

        // Demo 5: Flags enum
        Console.WriteLine("\n--- Flags Enum ---");
        Permissions userPermissions = Permissions.Read | Permissions.Write;
        Console.WriteLine($"User permissions: {userPermissions}");
        Console.WriteLine($"Has Read: {userPermissions.HasFlag(Permissions.Read)}");
        Console.WriteLine($"Has Write: {userPermissions.HasFlag(Permissions.Write)}");
        Console.WriteLine($"Has Execute: {userPermissions.HasFlag(Permissions.Execute)}");

        // Check with bitwise AND
        bool hasRead = (userPermissions & Permissions.Read) != 0;
        Console.WriteLine($"Has Read (bitwise): {hasRead}");

        // Demo 6: Masking
        Console.WriteLine("\n--- Bit Masking ---");
        int data = 0b11010110;
        int mask = 0b00001111;
        int masked = data & mask;
        Console.WriteLine($"Data:  {Convert.ToString(data, 2).PadLeft(8, '0')}");
        Console.WriteLine($"Mask:  {Convert.ToString(mask, 2).PadLeft(8, '0')}");
        Console.WriteLine($"Result:{Convert.ToString(masked, 2).PadLeft(8, '0')}");

        // Demo 7: Extract specific bits
        Console.WriteLine("\n--- Extract Specific Bits ---");
        int register = 0b11011010;
        int extracted = (register >> 4) & 0b1111; // Extract bits 4-7
        Console.WriteLine($"Register: {Convert.ToString(register, 2).PadLeft(8, '0')}");
        Console.WriteLine($"Bits 4-7: {extracted} (binary: {Convert.ToString(extracted, 2).PadLeft(4, '0')})");

        // Demo 8: Swap without temp variable
        Console.WriteLine("\n--- XOR Swap (No Temp Variable) ---");
        int x = 5, y = 10;
        Console.WriteLine($"Before: x={x}, y={y}");
        x ^= y;
        y ^= x;
        x ^= y;
        Console.WriteLine($"After:  x={x}, y={y}");

        // Demo 9: Check if power of 2
        Console.WriteLine("\n--- Check If Power of 2 ---");
        int[] testNumbers = { 1, 2, 4, 5, 8, 10, 16, 32, 33, 64 };
        foreach (int n in testNumbers)
        {
            bool isPowerOf2 = (n & (n - 1)) == 0 && n > 0;
            Console.WriteLine($"{n,3}: {(isPowerOf2 ? "Yes" : "No")}");
        }

        // Demo 10: Count set bits
        Console.WriteLine("\n--- Count Set Bits (Hamming Weight) ---");
        int[] bitCounts = { 0b0000, 0b0101, 0b1111, 0b10101010, 0b11110000 };
        foreach (int n in bitCounts)
        {
            int count = CountSetBits(n);
            Console.WriteLine($"{Convert.ToString(n, 2).PadLeft(8, '0')}: {count} bits set");
        }

        // Demo 11: Even/Odd check
        Console.WriteLine("\n--- Even/Odd Check ---");
        for (int i = 0; i <= 10; i++)
        {
            string parity = (i & 1) == 0 ? "Even" : "Odd";
            Console.WriteLine($"{i,2}: {parity}");
        }

        // Demo 12: Color manipulation (RGB)
        Console.WriteLine("\n--- RGB Color Manipulation ---");
        int red = 255, green = 128, blue = 64;
        int rgbColor = (red << 16) | (green << 8) | blue;
        Console.WriteLine($"RGB Color: 0x{rgbColor:X6}");
        
        int extractedRed = (rgbColor >> 16) & 0xFF;
        int extractedGreen = (rgbColor >> 8) & 0xFF;
        int extractedBlue = rgbColor & 0xFF;
        Console.WriteLine($"Red:   {extractedRed}");
        Console.WriteLine($"Green: {extractedGreen}");
        Console.WriteLine($"Blue:  {extractedBlue}");

        // Demo 13: Multiply/Divide by powers of 2
        Console.WriteLine("\n--- Multiply/Divide by Powers of 2 ---");
        int baseNum = 7;
        Console.WriteLine($"{baseNum} << 1 = {baseNum << 1} (×2)");
        Console.WriteLine($"{baseNum} << 2 = {baseNum << 2} (×4)");
        Console.WriteLine($"{baseNum} << 3 = {baseNum << 3} (×8)");
        Console.WriteLine($"{baseNum} >> 1 = {baseNum >> 1} (÷2)");
        Console.WriteLine($"{baseNum} >> 2 = {baseNum >> 2} (÷4)");
    }

    /// <summary>
    /// Counts the number of set bits (1s) in an integer
    /// </summary>
    static int CountSetBits(int n)
    {
        int count = 0;
        while (n > 0)
        {
            count += n & 1;
            n >>= 1;
        }
        return count;
    }
}

/// <summary>
/// Example flags enum for permissions
/// </summary>
[Flags]
enum Permissions
{
    None = 0,
    Read = 1,      // 0001
    Write = 2,     // 0010
    Execute = 4,   // 0100
    Delete = 8     // 1000
}
