// See https://aka.ms/new-console-template for more information

using System.Numerics;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using GenericVector;

// Console.WriteLine("Hello, World!");

// var writer = new StringWriter();
// for (var i = 0; i < 100000; i++)
// {
//     writer.WriteLine(Scalar<float>.Two);
//     writer.WriteLine(Scalar.BillboardMinAngle<float>());
//     writer.WriteLine(Scalar.NormalizeEpsilon<float>());
// }
//
// Console.WriteLine(writer.ToString()[^1000..]);

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);

[DryJob(RuntimeMoniker.Net80, Jit.RyuJit, Platform.X64)]
// [ShortRunJob(RuntimeMoniker.Net80, Jit.RyuJit, Platform.X64)]
[DisassemblyDiagnoser(printSource: true)]
[GenericTypeArguments(typeof(float))]
[GenericTypeArguments(typeof(double))]
[GenericTypeArguments(typeof(decimal))]
[GenericTypeArguments(typeof(int))]
public class Mark<T> where T : INumberBase<T>
{
    [Benchmark] public T Two() => T.CreateChecked(2);
    [Benchmark] public T Half() => T.CreateChecked(0.5f);
    [Benchmark] public T Four() => T.CreateChecked(4);
    [Benchmark] public T OneAndAHalf() => T.CreateChecked(1.5f);
    [Benchmark] public T Quarter() => T.CreateChecked(0.25f);

    [Benchmark] public T BillboardEpsilon() => T.CreateChecked(0.0001m);
    [Benchmark] public T DecomposeEpsilon() => T.CreateChecked(0.0001m);

    [Benchmark] public T SlerpEpsilon() => T.CreateChecked(0.000001m);
    [Benchmark] public T ThreeQuarters() => T.CreateChecked(0.75f);
}