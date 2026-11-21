using LINVAST.Imperative;
using LINVAST.Imperative.Nodes;
using LINVAST.Nodes;

namespace LINVAST.Demos.FeatureExtraction;

// Executable API example and regression checks, using the same extraction path as the demo.
internal static class RegressionChecks
{
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    internal static int Run()
    {
        var single = new FeatureExtractor("f");
        single.Visit(new IdNode(1, "x")); // Typed entry point must count too.
        Check(single.TotalNodes == 1 && single.IdNodeCount == 1 && single.MaxDepth == 1, "single identifier");
        var logic = new LogicExprNode(1, new IdNode(1, "x"),
            BinaryLogicOpNode.FromSymbol(1, "&&"), new IdNode(1, "y"));
        var mixed = new FeatureExtractor("f");
        mixed.Visit((ASTNode)logic);
        Check(mixed.TotalNodes == 4 && mixed.IdNodeCount == 2 && mixed.MaxDepth == 2, "mixed tree");

        string directory = Path.Combine(Path.GetTempPath(), "feature-checks-" + Guid.NewGuid());
        Directory.CreateDirectory(directory);
        try
        {
            CsvRow[] Extract(string name, string source, string language)
            {
                string path = Path.Combine(directory, name);
                File.WriteAllText(path, source);
                var ast = new ImperativeASTFactory().BuildFromFile(path);
                // Independent enumeration checks every body against the production visitor.
                void Walk(ASTNode node)
                {
                    ASTNode? body = node switch { FuncNode f => f.Definition, FuncDeclNode f => f.Definition, _ => null };
                    if (body is not null)
                    {
                        var extractor = new FeatureExtractor("unused");
                        extractor.Visit(body);
                        (int Count, int Depth, int Ids) Measure(ASTNode n)
                        {
                            var children = n.Children.Select(Measure).ToArray();
                            return (1 + children.Sum(c => c.Count), 1 + children.Select(c => c.Depth).DefaultIfEmpty(0).Max(),
                                (n is IdNode ? 1 : 0) + children.Sum(c => c.Ids));
                        }
                        var expected = Measure(body);
                        Check(extractor.TotalNodes == expected.Count && extractor.MaxDepth == expected.Depth
                            && extractor.IdNodeCount == expected.Ids, name + " traversal");
                    }
                    foreach (var child in node.Children) Walk(child);
                }
                Walk(ast);
                return Program.ExtractFunctions(ast, language, name, Path.GetExtension(name)).ToArray();
            }
            var loop = Extract("loop.c", "int f(int n) { for (int i=0; i<n; i++) { n=n-1; } return n; }", "C").Single();
            Check(loop.ForCount == 1 && loop.CyclomaticComplexity == 2 && loop.AssignExprCount == 1, "declaration loop");
            var expressionLoop = Extract("expr.c", "int f(int n) { int i; for (i=0; i<n; i++) {} return n; }", "C").Single();
            Check(expressionLoop.AssignExprCount == 1, "expression initializer counted once");
            var branches = Extract("branch.c", "int f(int x) { if (x && x>1 || x<0) return x ? 1 : 2; while(x) { x--; } return 0; }", "C").Single();
            Check(branches.CyclomaticComplexity == 6 && branches.LogicExprCount == 2, "structural decision formula");
            var unary = Extract("unary.c", "int f(int x) { if (!x) return 1; return 0; }", "C").Single();
            Check(unary.LogicExprCount == 0 && unary.CyclomaticComplexity == 2, "negation is not a decision");
            var sw = Extract("switch.c", "int f(int x) { switch(x) { case 1: return 1; case 2: return 2; default: return 0; } }", "C").Single();
            Check(sw.SwitchCount == 1 && sw.CyclomaticComplexity == 1, "switch exclusion");
            var java = Extract("Example.java", "class Example { int f(int x) { try { return x; } catch (Exception e) { return 0; } } int f() { return f(1); } }", "Java");
            Check(java.Length == 2 && java[0].CyclomaticComplexity == 1 && java[1].IsRecursive, "Java descent, exception exclusion, overload false positive");
            var exception = Extract("exception.py", "def f(x):\n    try:\n        return x\n    except Exception:\n        return 0\n", "Python").Single();
            Check(exception.TryCount == 1 && exception.CyclomaticComplexity == 1, "exception exclusion");
            var recursion = Extract("recursive.py", "def f(n):\n    return f(n - 1)\n", "Python").Single();
            Check(recursion.IsRecursive && recursion.ReturnType == "unknown" && recursion.ParamType1 == "unknown", "self-call and absent types");
            var shadow = Extract("shadow.py", "def f(f):\n    return f(1)\n", "Python").Single();
            Check(shadow.IsRecursive, "documented shadowing false positive");
            var mutual = Extract("mutual.py", "def f(n):\n    return g(n)\ndef g(n):\n    return f(n)\n", "Python");
            Check(mutual.Length == 2 && mutual.All(r => !r.IsRecursive), "mutual recursion not detected");
            var annotated = Extract("annotated.py", "def f(n: int) -> int:\n    return n\n", "Python").Single();
            Check(annotated.ReturnType == "int" && annotated.ParamType1 == "int", "explicit annotations");
            var pointer = Extract("pointer.c", "int f(int *p, int n) { return n; }", "C").Single();
            Check(pointer.ReturnType == "int" && pointer.ParamType1 == "int*", "development C pointer label");
            var explicitVoid = Extract("void.c", "void g() { return; }", "C").Single();
            Check(explicitVoid.ReturnType == "void", "explicit void preserved");
            var kotlin = Extract("types.kt", "fun f(a: Array<Int>): Int { return 1 }", "Kotlin").Single();
            Check(kotlin.ReturnType == "int" && kotlin.ParamType1 == "int[]", "development Kotlin normalized labels retained");
            foreach (string file in Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "Samples")))
            {
                string language = Path.GetExtension(file) switch { ".c" => "C", ".java" => "Java", ".go" => "Go", ".kt" => "Kotlin", ".lua" => "Lua", ".py" => "Python", _ => throw new Exception(file) };
                Check(Extract(Path.GetFileName(file), File.ReadAllText(file), language).Length == 1, file + " function count");
            }
            Console.WriteLine("All feature extraction checks passed (including all corpus body traversals).");
            return 0;
        }
        finally { Directory.Delete(directory, true); }
    }
}
