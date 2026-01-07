# Math Functions

Demonstrates C# Math class methods, Random number generation, rounding, trigonometry, statistics, and geometry calculations.

## Usage

```bash
dotnet run --project MathFunctions.csproj
```

## Example

```
=== Math Class Basics ===

--- Absolute Value ---
Math.Abs(-42) = 42
Math.Abs(42) = 42
Math.Abs(-3.14) = 3.14

--- Min and Max ---
Math.Min(10, 20) = 10
Math.Max(10, 20) = 20
Math.Min(-5, 5) = -5

--- Power and Square Root ---
Math.Pow(2, 10) = 1024
Math.Pow(3, 4) = 81
Math.Sqrt(144) = 12
Math.Sqrt(2) = 1.414214

--- Rounding ---
Math.Round(3.14159, 2) = 3.14
Math.Round(3.5) = 4
Math.Round(4.5) = 4
Math.Floor(3.7) = 3
Math.Floor(-3.7) = -4
Math.Ceiling(3.2) = 4
Math.Ceiling(-3.2) = -3

--- Trigonometry ---
Angle: 0.7854 radians (45 degrees)
Math.Sin(π/4) = 0.7071
Math.Cos(π/4) = 0.7071
Math.Tan(π/4) = 1.0000

60 degrees = 1.0472 radians
Math.Sin(60°) = 0.8660

--- Logarithms ---
Math.Log(Math.E) = 1
Math.Log10(100) = 2
Math.Log10(1000) = 3
Math.Log2(256) = 8

--- Math Constants ---
Math.PI = 3.1415926536
Math.E = 2.7182818285
Math.Tau = 6.2831853072

=== Random Number Generation ===

--- Random Integers ---
Random int (0 to MaxValue): 1234567890
Random int (0 to 100): 42
Random int (50 to 100): 73

--- Random Doubles ---
Random double [0.0 to 1.0): 0.5432
Random double [0.0 to 1.0): 0.1234
Random double [0.0 to 1.0): 0.8765

--- Simulating Dice Roll ---
Rolling 2 dice 5 times:
  Roll 1: [3] + [5] = 8
  Roll 2: [6] + [2] = 8
  Roll 3: [1] + [4] = 5
  Roll 4: [5] + [5] = 10
  Roll 5: [2] + [6] = 8

--- Coin Flip Simulation ---
Flipping coin 10 times:
H T H H T T H T H T
Result: 5 Heads, 5 Tails

=== Statistics Calculations ===

Data: [12.5, 15.3, 18.7, 22.1, 25.9, 30.2, 35.8, 40.1]

Count: 8
Min: 12.5
Max: 40.1
Sum: 200.60
Average: 25.08
Standard Deviation: 9.32
Median: 24.00

=== Geometry Calculations ===

Circle with radius 5:
  Circumference: 31.42
  Area: 78.54

Sphere with radius 5:
  Surface Area: 314.16
  Volume: 523.60

--- Distance Between Points ---
Distance from (0, 0) to (3, 4): 5
Hypotenuse of right triangle (3, 4): 5

--- Clamp Value to Range ---
Clamp 150 to [0, 100]: 100
Clamp 50 to [0, 100]: 50

=== Program Complete ===
```

## Concepts Demonstrated

- Math.Abs - absolute value
- Math.Min / Math.Max - minimum and maximum
- Math.Pow - exponentiation
- Math.Sqrt - square root
- Math.Round - rounding (banker's rounding)
- Math.Floor - round down
- Math.Ceiling - round up
- Math.Sin, Math.Cos, Math.Tan - trigonometric functions
- Radians to degrees conversion
- Math.Log, Math.Log10, Math.Log2 - logarithms
- Math.PI, Math.E, Math.Tau - mathematical constants
- Random class for random number generation
- Random.Next() - random integers
- Random.NextDouble() - random doubles
- Simulations: dice roll, coin flip
- Statistics: Min, Max, Sum, Average, Standard Deviation, Median
- Geometry: circle circumference/area, sphere surface/volume
- Distance formula (Pythagorean theorem)
- Clamping values to a range
