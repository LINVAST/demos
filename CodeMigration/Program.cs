using System;
using System.IO;
using System.Linq;
using LINVAST.Imperative;
using LINVAST.Imperative.Builders.Java;
using LINVAST.Imperative.Nodes;
using LINVAST.Nodes;

namespace LINVAST.Demos.CodeMigration;

internal static class Program
{
    private static int Main(string[] args)
    {
        var factory = new ImperativeASTFactory();
        var generator = new JavaToGoDataClassGenerator();

        if (args.Length == 0)
        {
            string samplesDir = Path.Combine(AppContext.BaseDirectory, "Samples");
            foreach (string javaPath in Directory.GetFiles(samplesDir, "*.java")
                .OrderBy(p => p))
            {
                string fileName = Path.GetFileName(javaPath);
                Console.WriteLine($"// === Generated from {fileName} ===");

                ASTNode ast = factory.BuildFromFile(javaPath);
                SourceNode source = ast.As<SourceNode>();

                string goCode = generator.Generate(source);
                Console.WriteLine(goCode);
                Console.WriteLine();
            }
            return 0;
        }

        if (args.Length == 1 && args[0] == "--migrate-stdin")
        {
            ASTNode ast = new JavaASTBuilder().BuildFromSource(Console.In.ReadToEnd());
            SourceNode source = ast.As<SourceNode>();
            string goCode = generator.Generate(source);
            Console.Write(goCode);
            return 0;
        }

        if (args.Length == 1 && File.Exists(args[0]))
        {
            ASTNode ast = factory.BuildFromFile(args[0]);
            SourceNode source = ast.As<SourceNode>();
            string goCode = generator.Generate(source);
            Console.Write(goCode);
            return 0;
        }

        Console.Error.WriteLine($"Usage: {Path.GetFileName(AppContext.BaseDirectory)} [file.java|--migrate-stdin]");
        Console.Error.WriteLine("  No arguments      -> process Samples/*.java in the working directory.");
        Console.Error.WriteLine("  --migrate-stdin   -> read Java source from stdin, emit Go to stdout.");
        Console.Error.WriteLine("  file.java         -> parse the given Java file, emit Go to stdout.");
        return 1;
    }
}
