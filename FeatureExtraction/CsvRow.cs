using System.Collections.Generic;
using System.Linq;
using LINVAST.Imperative.Nodes;
using LINVAST.Imperative.Nodes.Common;
using LINVAST.Nodes;

namespace LINVAST.Demos.FeatureExtraction;

public sealed class CsvRow
{
    public string Language { get; set; } = "";
    public string SourceFile { get; set; } = "";
    public string FunctionName { get; set; } = "";
    public string ReturnType { get; set; } = "";
    public string ParamType1 { get; set; } = "";
    public string ParamType2 { get; set; } = "";
    public string ParamType3 { get; set; } = "";
    public string ParamType4 { get; set; } = "";
    public string ParamType5 { get; set; } = "";

    public int TotalNodes { get; set; }
    public int MaxDepth { get; set; }
    public int IdNodeCount { get; set; }
    public int IfCount { get; set; }
    public int ElseCount { get; set; }
    public int ForCount { get; set; }
    public int WhileCount { get; set; }
    public int ForeachCount { get; set; }
    public int LogicExprCount { get; set; }
    public int CondExprCount { get; set; }
    public int SwitchCount { get; set; }
    public int TryCount { get; set; }
    public int AssignExprCount { get; set; }
    public int ArithmExprCount { get; set; }
    public int RelExprCount { get; set; }
    public int FuncCallCount { get; set; }
    public int ReturnCount { get; set; }
    public int BreakCount { get; set; }
    public int ContinueCount { get; set; }
    public int LitExprCount { get; set; }
    public int ArrAccessExprCount { get; set; }
    public int VarDeclCount { get; set; }
    public int ParamCount { get; set; }
    public bool IsRecursive { get; set; }
    public int CyclomaticComplexity { get; set; }
    public int NumUniqueIdentifiers { get; set; }

    public static CsvRow FromFuncNode(string language, string sourceFile, FuncNode fn, FeatureExtractor extractor)
    {
        extractor.Visit(fn.Definition ?? throw new InvalidOperationException("Function has no body"));

        string[] paramTypes = fn.Parameters?.Select(p => p.Specifiers.TypeName).ToArray()
            ?? System.Array.Empty<string>();

        return BuildRow(language, sourceFile, fn.Identifier, fn.ReturnTypeName, paramTypes, extractor);
    }

    public static CsvRow FromFuncDeclNode(string language, string sourceFile, FuncDeclNode fn, DeclStatNode parent, FeatureExtractor extractor)
    {
        extractor.Visit(fn.Definition ?? throw new InvalidOperationException("Method has no body"));

        string[] paramTypes = fn.Parameters?.Select(p => p.Specifiers.TypeName).ToArray()
            ?? System.Array.Empty<string>();

        return BuildRow(language, sourceFile, fn.Identifier, parent.Specifiers.TypeName, paramTypes, extractor);
    }

    private static CsvRow BuildRow(string language, string sourceFile, string functionName, string returnType,
        string[] paramTypes, FeatureExtractor extractor)
    {
        var row = new CsvRow
        {
            Language = language,
            SourceFile = sourceFile,
            FunctionName = functionName,
            ReturnType = returnType,
            ParamType1 = paramTypes.Length > 0 ? paramTypes[0] : "",
            ParamType2 = paramTypes.Length > 1 ? paramTypes[1] : "",
            ParamType3 = paramTypes.Length > 2 ? paramTypes[2] : "",
            ParamType4 = paramTypes.Length > 3 ? paramTypes[3] : "",
            ParamType5 = paramTypes.Length > 4 ? paramTypes[4] : "",

            TotalNodes = extractor.TotalNodes,
            MaxDepth = extractor.MaxDepth,
            IdNodeCount = extractor.IdNodeCount,
            IfCount = extractor.GetCount(nameof(IfStatNode)),
            ElseCount = extractor.ElseCount,
            ForCount = extractor.GetCount(nameof(ForStatNode)),
            WhileCount = extractor.GetCount(nameof(WhileStatNode)),
            ForeachCount = extractor.GetCount(nameof(ForeachStatNode)),
            LogicExprCount = extractor.GetCount(nameof(LogicExprNode)),
            CondExprCount = extractor.GetCount(nameof(CondExprNode)),
            SwitchCount = extractor.GetCount(nameof(SwitchStatNode)),
            TryCount = extractor.GetCount(nameof(TryStatNode)),
            AssignExprCount = extractor.GetCount(nameof(AssignExprNode)),
            ArithmExprCount = extractor.GetCount(nameof(ArithmExprNode)),
            RelExprCount = extractor.GetCount(nameof(RelExprNode)),
            FuncCallCount = extractor.GetCount(nameof(FuncCallExprNode)),
            ReturnCount = extractor.JumpStatCounts.GetValueOrDefault(JumpStatType.Return),
            BreakCount = extractor.JumpStatCounts.GetValueOrDefault(JumpStatType.Break),
            ContinueCount = extractor.JumpStatCounts.GetValueOrDefault(JumpStatType.Continue),
            LitExprCount = extractor.GetCount(nameof(LitExprNode)),
            ArrAccessExprCount = extractor.GetCount(nameof(ArrAccessExprNode)),
            VarDeclCount = extractor.GetCount(nameof(VarDeclNode)) + extractor.GetCount(nameof(FuncParamNode)),
            ParamCount = paramTypes.Length,
            IsRecursive = extractor.IsRecursive,
            NumUniqueIdentifiers = extractor.UniqueIdentifiers.Count,
            CyclomaticComplexity = 1
                + extractor.GetCount(nameof(IfStatNode))
                + extractor.GetCount(nameof(ForStatNode))
                + extractor.GetCount(nameof(WhileStatNode))
                + extractor.GetCount(nameof(ForeachStatNode))
                + extractor.GetCount(nameof(LogicExprNode))
                + extractor.GetCount(nameof(CondExprNode)),
        };

        return row;
    }

    public static string GetHeader() =>
        "Language,SourceFile,FunctionName,ReturnType,ParamType1,ParamType2,ParamType3,ParamType4,ParamType5,"
        + "TotalNodes,MaxDepth,IdNodeCount,IfCount,ElseCount,ForCount,WhileCount,ForeachCount,LogicExprCount,CondExprCount,"
        + "SwitchCount,TryCount,AssignExprCount,ArithmExprCount,RelExprCount,FuncCallCount,ReturnCount,BreakCount,ContinueCount,"
        + "LitExprCount,ArrAccessExprCount,VarDeclCount,ParamCount,IsRecursive,CyclomaticComplexity,NumUniqueIdentifiers";

    public string ToCsv()
    {
        var values = new object[]
        {
            Quote(Language), Quote(SourceFile), Quote(FunctionName), Quote(ReturnType),
            Quote(ParamType1), Quote(ParamType2), Quote(ParamType3), Quote(ParamType4), Quote(ParamType5),
            TotalNodes, MaxDepth, IdNodeCount, IfCount, ElseCount, ForCount, WhileCount, ForeachCount,
            LogicExprCount, CondExprCount, SwitchCount, TryCount, AssignExprCount, ArithmExprCount,
            RelExprCount, FuncCallCount, ReturnCount, BreakCount, ContinueCount, LitExprCount,
            ArrAccessExprCount, VarDeclCount, ParamCount, IsRecursive ? "true" : "false",
            CyclomaticComplexity, NumUniqueIdentifiers,
        };
        return string.Join(",", values);
    }

    private static string Quote(string value) =>
        $"\"{value.Replace("\"", "\"\"")}\"";
}
