using System;
using System.Reflection;
using System.Reflection.Emit;

namespace IlGenerator;

/// <summary>
/// Demonstrates dynamic code generation using IL generators.
/// Creates types and methods at runtime using System.Reflection.Emit.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== IL Generator Demo ===\n");
        Console.WriteLine("Demonstrates runtime IL code generation.\n");

        // Demo 1: Dynamic method
        Console.WriteLine("--- Dynamic Method (Add Two Numbers) ---\n");
        var addMethod = CreateDynamicAddMethod();
        int result = (int)addMethod.Invoke(null, new object[] { 7, 3 });
        Console.WriteLine($"Result: 7 + 3 = {result}\n");

        // Demo 2: Dynamic type
        Console.WriteLine("--- Dynamic Type Creation ---\n");
        var personType = CreateDynamicPersonType();
        
        var person = Activator.CreateInstance(personType);
        var nameProperty = personType.GetProperty("Name");
        var ageProperty = personType.GetProperty("Age");
        var greetMethod = personType.GetMethod("Greet");

        nameProperty?.SetValue(person, "Alice");
        ageProperty?.SetValue(person, 30);
        
        var greeting = greetMethod?.Invoke(person, null);
        Console.WriteLine(greeting);
        Console.WriteLine($"Person created dynamically: Name={nameProperty?.GetValue(person)}, Age={ageProperty?.GetValue(person)}\n");

        // Demo 3: Dynamic calculator
        Console.WriteLine("--- Dynamic Calculator ---\n");
        var calculator = CreateCalculatorType();
        var calcInstance = Activator.CreateInstance(calculator);
        
        var add = calculator.GetMethod("Add");
        var subtract = calculator.GetMethod("Subtract");
        var multiply = calculator.GetMethod("Multiply");
        var divide = calculator.GetMethod("Divide");

        Console.WriteLine($"10 + 5 = {add?.Invoke(calcInstance, new object[] { 10, 5 })}");
        Console.WriteLine($"10 - 5 = {subtract?.Invoke(calcInstance, new object[] { 10, 5 })}");
        Console.WriteLine($"10 * 5 = {multiply?.Invoke(calcInstance, new object[] { 10, 5 })}");
        Console.WriteLine($"10 / 5 = {divide?.Invoke(calcInstance, new object[] { 10, 5 })}");
    }

    static DynamicMethod CreateDynamicAddMethod()
    {
        // Create a dynamic method: int Add(int a, int b)
        var method = new DynamicMethod(
            "Add",
            typeof(int),
            [typeof(int), typeof(int)],
            typeof(Program).Module
        );

        ILGenerator il = method.GetILGenerator();

        // Load arguments onto stack
        il.Emit(OpCodes.Ldarg_0);  // Load first argument
        il.Emit(OpCodes.Ldarg_1);  // Load second argument
        il.Emit(OpCodes.Add);      // Add them
        il.Emit(OpCodes.Ret);      // Return

        return method;
    }

    static Type CreateDynamicPersonType()
    {
        // Create dynamic assembly and module
        var assemblyName = new AssemblyName("DynamicAssembly");
        var assembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        var module = assembly.DefineDynamicModule("DynamicModule");

        // Define type
        var typeBuilder = module.DefineType(
            "DynamicPerson",
            TypeAttributes.Public | TypeAttributes.Class
        );

        // Add Name property
        var nameField = typeBuilder.DefineField("_name", typeof(string), FieldAttributes.Private);
        var nameProp = typeBuilder.DefineProperty("Name", PropertyAttributes.HasDefault, typeof(string), null);
        
        var nameGetter = typeBuilder.DefineMethod("get_Name", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, typeof(string), Type.EmptyTypes);
        var nameGetterIl = nameGetter.GetILGenerator();
        nameGetterIl.Emit(OpCodes.Ldarg_0);
        nameGetterIl.Emit(OpCodes.Ldfld, nameField);
        nameGetterIl.Emit(OpCodes.Ret);
        
        var nameSetter = typeBuilder.DefineMethod("set_Name", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, [typeof(string)]);
        var nameSetterIl = nameSetter.GetILGenerator();
        nameSetterIl.Emit(OpCodes.Ldarg_0);
        nameSetterIl.Emit(OpCodes.Ldarg_1);
        nameSetterIl.Emit(OpCodes.Stfld, nameField);
        nameSetterIl.Emit(OpCodes.Ret);
        
        nameProp.SetGetMethod(nameGetter);
        nameProp.SetSetMethod(nameSetter);

        // Add Age property
        var ageField = typeBuilder.DefineField("_age", typeof(int), FieldAttributes.Private);
        var ageProp = typeBuilder.DefineProperty("Age", PropertyAttributes.HasDefault, typeof(int), null);
        
        var ageGetter = typeBuilder.DefineMethod("get_Age", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, typeof(int), Type.EmptyTypes);
        var ageGetterIl = ageGetter.GetILGenerator();
        ageGetterIl.Emit(OpCodes.Ldarg_0);
        ageGetterIl.Emit(OpCodes.Ldfld, ageField);
        ageGetterIl.Emit(OpCodes.Ret);
        
        var ageSetter = typeBuilder.DefineMethod("set_Age", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, [typeof(int)]);
        var ageSetterIl = ageSetter.GetILGenerator();
        ageSetterIl.Emit(OpCodes.Ldarg_0);
        ageSetterIl.Emit(OpCodes.Ldarg_1);
        ageSetterIl.Emit(OpCodes.Stfld, ageField);
        ageSetterIl.Emit(OpCodes.Ret);
        
        ageProp.SetGetMethod(ageGetter);
        ageProp.SetSetMethod(ageSetter);

        // Add Greet method
        var greetMethod = typeBuilder.DefineMethod(
            "Greet",
            MethodAttributes.Public,
            typeof(string),
            Type.EmptyTypes
        );
        var greetIl = greetMethod.GetILGenerator();
        greetIl.Emit(OpCodes.Ldstr, "Hello, my name is ");
        greetIl.Emit(OpCodes.Ldarg_0);
        greetIl.Emit(OpCodes.Ldfld, nameField);
        greetIl.Emit(OpCodes.Call, typeof(string).GetMethod("Concat", [typeof(string), typeof(string)])!);
        greetIl.Emit(OpCodes.Ret);

        return typeBuilder.CreateType()!;
    }

    static Type CreateCalculatorType()
    {
        var assemblyName = new AssemblyName("CalculatorAssembly");
        var assembly = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        var module = assembly.DefineDynamicModule("CalculatorModule");

        var typeBuilder = module.DefineType("DynamicCalculator", TypeAttributes.Public | TypeAttributes.Class);

        // Create arithmetic methods
        CreateArithmeticMethod(typeBuilder, "Add", OpCodes.Add);
        CreateArithmeticMethod(typeBuilder, "Subtract", OpCodes.Sub);
        CreateArithmeticMethod(typeBuilder, "Multiply", OpCodes.Mul);
        CreateArithmeticMethod(typeBuilder, "Divide", OpCodes.Div);

        return typeBuilder.CreateType()!;
    }

    static void CreateArithmeticMethod(TypeBuilder typeBuilder, string methodName, OpCode opCode)
    {
        var method = typeBuilder.DefineMethod(
            methodName,
            MethodAttributes.Public,
            typeof(int),
            [typeof(int), typeof(int)]
        );
        var il = method.GetILGenerator();
        il.Emit(OpCodes.Ldarg_1);
        il.Emit(OpCodes.Ldarg_2);
        il.Emit(opCode);
        il.Emit(OpCodes.Ret);
    }
}
