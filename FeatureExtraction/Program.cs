using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LINVAST.Imperative;
using LINVAST.Imperative.Nodes;
using LINVAST.Nodes;

namespace LINVAST.Demos.FeatureExtraction;

internal static class Program
{
    private static int Main()
    {
        string samplesDir = Path.Combine(AppContext.BaseDirectory, "Samples");
        var factory = new ImperativeASTFactory();

        var fileMap = new (string Language, string Ext)[]
        {
            ("C", ".c"),
            ("Java", ".java"),
            ("Go", ".go"),
            ("Lua", ".lua"),
            ("Kotlin", ".kt"),
            ("Python", ".py"),
        };

        var rows = new List<CsvRow>();

        foreach (string filePath in Directory.GetFiles(samplesDir))
        {
            string ext = Path.GetExtension(filePath);
            string language = Path.GetFileNameWithoutExtension(filePath);

            try
            {
                ASTNode ast = factory.BuildFromFile(filePath);

                foreach (CsvRow row in ExtractFunctions(ast, language, Path.GetFileName(filePath), ext))
                    rows.Add(row);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error processing {filePath}: {ex.Message}");
            }
        }

        Console.WriteLine(CsvRow.GetHeader());
        foreach (CsvRow row in rows)
            Console.WriteLine(row.ToCsv());

        return 0;
    }

    private static IEnumerable<CsvRow> ExtractFunctions(ASTNode ast, string language, string fileName, string ext)
    {
        foreach (ASTNode child in ast.Children)
        {
            if (child is FuncNode fn)
            {
                var extractor = new FeatureExtractor(fn.Identifier);
                yield return CsvRow.FromFuncNode(language, fileName, fn, extractor);
            }
        }

        if (ext == ".java")
        {
            foreach (DeclStatNode decl in Descendants(ast).OfType<DeclStatNode>()
                .Where(d => d.DeclaratorList.Declarators.OfType<FuncDeclNode>().Any()))
            {
                foreach (FuncDeclNode fn in decl.DeclaratorList.Declarators.OfType<FuncDeclNode>())
                {
                    var extractor = new FeatureExtractor(fn.Identifier);
                    yield return CsvRow.FromFuncDeclNode(language, fileName, fn, decl, extractor);
                }
            }
        }
    }

    private static IEnumerable<ASTNode> Descendants(ASTNode node)
    {
        foreach (ASTNode child in node.Children)
        {
            yield return child;
            foreach (ASTNode descendant in Descendants(child))
                yield return descendant;
        }
    }
}
