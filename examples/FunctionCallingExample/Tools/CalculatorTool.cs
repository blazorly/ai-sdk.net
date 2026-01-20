using AiSdk.Tools;

namespace FunctionCallingExample.Tools;

/// <summary>
/// Input parameters for the calculator tool.
/// </summary>
public record CalculatorInput
{
    /// <summary>
    /// The mathematical operation to perform.
    /// </summary>
    public required string Operation { get; init; }

    /// <summary>
    /// The first number.
    /// </summary>
    public required double A { get; init; }

    /// <summary>
    /// The second number.
    /// </summary>
    public required double B { get; init; }
}

/// <summary>
/// Output from the calculator tool.
/// </summary>
public record CalculatorOutput
{
    public required string Operation { get; init; }
    public required double A { get; init; }
    public required double B { get; init; }
    public required double Result { get; init; }
    public required string Expression { get; init; }
}

/// <summary>
/// A tool that performs mathematical calculations.
/// </summary>
public static class CalculatorTool
{
    /// <summary>
    /// Creates the calculator tool definition.
    /// </summary>
    public static ToolWithExecution<CalculatorInput, CalculatorOutput> Create()
    {
        return Tool.Create<CalculatorInput, CalculatorOutput>(
            name: "calculate",
            description: "Perform mathematical calculations. Supported operations: add, subtract, multiply, divide, power, modulo.",
            execute: Calculate
        );
    }

    /// <summary>
    /// Performs the requested mathematical operation.
    /// </summary>
    private static CalculatorOutput Calculate(CalculatorInput input)
    {
        double result;
        string expression;

        switch (input.Operation.ToLowerInvariant())
        {
            case "add":
            case "+":
                result = input.A + input.B;
                expression = $"{input.A} + {input.B} = {result}";
                break;

            case "subtract":
            case "-":
                result = input.A - input.B;
                expression = $"{input.A} - {input.B} = {result}";
                break;

            case "multiply":
            case "*":
                result = input.A * input.B;
                expression = $"{input.A} × {input.B} = {result}";
                break;

            case "divide":
            case "/":
                if (input.B == 0)
                {
                    throw new InvalidOperationException("Cannot divide by zero");
                }
                result = input.A / input.B;
                expression = $"{input.A} ÷ {input.B} = {result}";
                break;

            case "power":
            case "^":
                result = Math.Pow(input.A, input.B);
                expression = $"{input.A}^{input.B} = {result}";
                break;

            case "modulo":
            case "%":
                result = input.A % input.B;
                expression = $"{input.A} mod {input.B} = {result}";
                break;

            default:
                throw new InvalidOperationException(
                    $"Unknown operation: {input.Operation}. " +
                    $"Supported: add, subtract, multiply, divide, power, modulo");
        }

        return new CalculatorOutput
        {
            Operation = input.Operation,
            A = input.A,
            B = input.B,
            Result = result,
            Expression = expression
        };
    }
}
