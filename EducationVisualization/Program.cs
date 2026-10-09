using System;

namespace LINVAST.Demos.EducationVisualization;

internal static class Program
{
    private static int Main()
    {
        Console.WriteLine("EducationVisualization demo");
        Console.WriteLine("");
        Console.WriteLine("This demo is driven by demo.sh, which uses the linvast CLI");
        Console.WriteLine("to generate AST JSON from C code snippets and serves them");
        Console.WriteLine("in the LINVisualizerJS web app.");
        Console.WriteLine("");
        Console.WriteLine("  ./demo.sh v1    # Generate & view AST for sample_v1.c");
        Console.WriteLine("  ./demo.sh v2    # ...progressively richer variants");
        Console.WriteLine("  ./demo.sh v3");
        Console.WriteLine("  ./demo.sh v4");
        Console.WriteLine("  ./demo.sh all   # Pre-generate all variants");
        Console.WriteLine("  ./demo.sh stop  # Stop the web server");
        Console.WriteLine("");
        return 0;
    }
}
