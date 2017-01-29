#r "BenchmarkDotNet.dll"
#r "Microsoft.CodeAnalysis.dll"

using System.Diagnostics.Contracts;
using System.Reflection;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

var source = @"
using BenchmarkDotNet.Attributes;

namespace biz.dfch.CS.Commons.Benchmarks
{
    public class MyBenchmarks
    {
        [Benchmark]
        public static void MyBenchmark()
        {
            var sut = new object();
        }
    }
}
";

// file name will look like this: 'dxxbefjl.m1g'
var assemblyName = Path.GetRandomFileName();
var fileName = assemblyName;
//var fileName = string.Concat(assemblyName, ".dll");
var assemblyLocation = Path.Combine(Directory.GetCurrentDirectory(), fileName);

var syntaxTree = CSharpSyntaxTree.ParseText(source);

var references = new[]
{
    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
    MetadataReference.CreateFromFile(typeof(BenchmarkRunner).Assembly.Location),
    MetadataReference.CreateFromFile(typeof(BenchmarkAttribute).Assembly.Location),
};
var compilation = CSharpCompilation.Create
(
    assemblyName,
    new[] { syntaxTree },
    references,
    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
);

var emitResult = compilation.Emit(assemblyLocation);

var assembly = Assembly.Load(AssemblyName.GetAssemblyName(assemblyLocation));
var type = assembly.DefinedTypes.First();

var summary = BenchmarkRunner.Run(type);
