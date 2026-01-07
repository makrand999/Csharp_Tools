using System;

namespace AsIsCasting;

/// <summary>
/// Demonstrates type casting using 'as' and 'is' operators
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Type Casting: 'as' and 'is' Operators ===\n");

        // Demo 1: Traditional casting vs 'as' operator
        Console.WriteLine("--- Traditional Cast vs 'as' Operator ---");
        object obj1 = "Hello, World!";
        object obj2 = 42;

        // Traditional cast - throws exception on failure
        try
        {
            string str1 = (string)obj1;
            Console.WriteLine($"Traditional cast success: {str1}");
        }
        catch (InvalidCastException ex)
        {
            Console.WriteLine($"Traditional cast failed: {ex.Message}");
        }

        try
        {
            string str2 = (string)obj2; // This will throw
            Console.WriteLine($"Traditional cast success: {str2}");
        }
        catch (InvalidCastException ex)
        {
            Console.WriteLine($"Traditional cast failed: {ex.Message}");
        }

        // 'as' operator - returns null on failure (no exception)
        string? str3 = obj1 as string;
        Console.WriteLine($"\n'as' operator with string: {(str3 != null ? $"Success: {str3}" : "Failed (null)")}");

        string? str4 = obj2 as string;
        Console.WriteLine($"'as' operator with int: {(str4 != null ? $"Success: {str4}" : "Failed (null)")}");

        // Demo 2: 'is' operator for type checking
        Console.WriteLine("\n--- 'is' Operator for Type Checking ---");
        object[] objects = { "text", 123, 45.67, true, DateTime.Now };

        foreach (var obj in objects)
        {
            Console.WriteLine($"{obj} is:");
            Console.WriteLine($"  - string: {obj is string}");
            Console.WriteLine($"  - int: {obj is int}");
            Console.WriteLine($"  - double: {obj is double}");
            Console.WriteLine($"  - bool: {obj is bool}");
            Console.WriteLine($"  - DateTime: {obj is DateTime}");
        }

        // Demo 3: Pattern matching with 'is'
        Console.WriteLine("\n--- Pattern Matching with 'is' ---");
        object?[] mixed = { "hello", 100, 3.14, null, -50 };

        foreach (var item in mixed)
        {
            if (item is string text)
            {
                Console.WriteLine($"String found: \"{text}\" (length: {text.Length})");
            }
            else if (item is int number && number > 0)
            {
                Console.WriteLine($"Positive integer: {number}");
            }
            else if (item is double pi && pi > 3.0)
            {
                Console.WriteLine($"Double greater than 3: {pi}");
            }
            else if (item is null)
            {
                Console.WriteLine("Null value detected");
            }
            else
            {
                Console.WriteLine($"Other: {item}");
            }
        }

        // Demo 4: 'as' with nullable reference types
        Console.WriteLine("\n--- Safe Casting with 'as' ---");
        object? inputValue = GetInput();
        
        if (inputValue is string userInput)
        {
            Console.WriteLine($"You entered a string: \"{userInput}\"");
        }
        else if (inputValue is int number)
        {
            Console.WriteLine($"You entered an integer: {number}");
        }
        else
        {
            Console.WriteLine("Unknown input type");
        }

        // Demo 5: Casting with inheritance
        Console.WriteLine("\n--- Casting with Inheritance ---");
        Animal[] animals = { new Dog(), new Cat(), new Bird() };

        foreach (var animal in animals)
        {
            Console.Write($"{animal.GetType().Name}: ");
            
            if (animal is Dog dog)
            {
                dog.Bark();
            }
            else if (animal is Cat cat)
            {
                cat.Meow();
            }
            else if (animal is Bird bird)
            {
                bird.Chirp();
            }
        }

        // Demo 6: 'is not' pattern (C# 9+)
        Console.WriteLine("\n--- 'is not' Pattern ---");
        object? testValue = null;
        
        if (testValue is not string)
        {
            Console.WriteLine("Value is NOT a string");
        }
        
        if (testValue is not null)
        {
            Console.WriteLine("Value is NOT null");
        }
        else
        {
            Console.WriteLine("Value IS null");
        }
    }

    static object? GetInput()
    {
        // Simulating input - in real scenario, this could be user input
        return "sample input";
    }
}

// Base class
class Animal
{
    public virtual void MakeSound() => Console.WriteLine("Some animal sound");
}

// Derived classes
class Dog : Animal
{
    public void Bark() => Console.WriteLine("Woof! Woof!");
    public override void MakeSound() => Bark();
}

class Cat : Animal
{
    public void Meow() => Console.WriteLine("Meow! Meow!");
    public override void MakeSound() => Meow();
}

class Bird : Animal
{
    public void Chirp() => Console.WriteLine("Chirp! Chirp!");
    public override void MakeSound() => Chirp();
}
