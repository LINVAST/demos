using System.Collections.Generic;
using System.Linq;
using LINVAST.Imperative.Nodes;
using LINVAST.Imperative.Nodes.Common;
using LINVAST.Imperative.Visitors;
using LINVAST.Nodes;

namespace LINVAST.Demos.FeatureExtraction;

public sealed class FeatureExtractor : BaseASTVisitor<int>
{
    private readonly string _functionName;
    private int _currentDepth = 0;
    private readonly Dictionary<string, int> _nodeCounts = new();
    private int _elseCount = 0;
    private readonly Dictionary<JumpStatType, int> _jumpStatCounts = new();
    private int _idNodeCount = 0;
    private readonly HashSet<string> _uniqueIdentifiers = new();
    private bool _foundRecursion = false;

    public int TotalNodes => _nodeCounts.Values.Sum() + _idNodeCount;
    public int MaxDepth { get; private set; }
    public int IdNodeCount => _idNodeCount;
    public int ElseCount => _elseCount;
    public IReadOnlyDictionary<JumpStatType, int> JumpStatCounts => _jumpStatCounts;
    public IReadOnlySet<string> UniqueIdentifiers => _uniqueIdentifiers;
    public bool IsRecursive => _foundRecursion;

    public FeatureExtractor(string functionName)
    {
        _functionName = functionName;
    }

    public int GetCount(string nodeType) => _nodeCounts.GetValueOrDefault(nodeType, 0);

    public override int Visit(ASTNode node)
    {
        TrackNode(node);
        int result = DispatchVisit(node);
        UntrackNode();
        return result;
    }

    private void TrackNode(ASTNode node)
    {
        _currentDepth++;
        if (_currentDepth > MaxDepth)
            MaxDepth = _currentDepth;

        string nt = node.NodeType;
        _nodeCounts[nt] = _nodeCounts.GetValueOrDefault(nt, 0) + 1;
    }

    private void UntrackNode()
    {
        _currentDepth--;
    }

    private int DispatchVisit(ASTNode node)
    {
        switch (node)
        {
            case IdNode n: return Visit(n);
            case IfStatNode n: return Visit(n);
            case JumpStatNode n: return Visit(n);
            case FuncCallExprNode n: return Visit(n);
            case ForStatNode n: return Visit(n);
            default: return this.VisitChildren(node);
        }
    }

    public override int Visit(IdNode node)
    {
        _idNodeCount++;
        _uniqueIdentifiers.Add(node.Identifier);
        return base.Visit(node);
    }

    public override int Visit(IfStatNode node)
    {
        if (node.ElseStat is not null)
            _elseCount++;
        return base.Visit(node);
    }

    public override int Visit(JumpStatNode node)
    {
        _jumpStatCounts[node.Type] = _jumpStatCounts.GetValueOrDefault(node.Type, 0) + 1;
        return base.Visit(node);
    }

    public override int Visit(FuncCallExprNode node)
    {
        if (node.Identifier == _functionName)
            _foundRecursion = true;
        return base.Visit(node);
    }

    public override int Visit(ForStatNode node)
    {
        if (node.InitExpr is not null)
            this.Visit(node.InitExpr);
        if (node.IncrExpr is not null)
            this.Visit(node.IncrExpr);
        if (node.ForDeclaration is not null)
            this.Visit(node.ForDeclaration);
        return base.Visit(node);
    }

    protected override int DefaultResult => 0;

    protected override int AggregateResult(int aggregate, int nextResult) => aggregate + nextResult;
}
