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

    public int TotalNodes => _nodeCounts.Values.Sum();
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

    // Both generic and typed visitor entry points reach VisitChildren exactly once.
    public override int Visit(ASTNode node) => VisitChildren(node);

    public override int VisitChildren(ASTNode node)
    {
        _currentDepth++;
        MaxDepth = Math.Max(MaxDepth, _currentDepth);
        _nodeCounts[node.NodeType] = GetCount(node.NodeType) + 1;
        switch (node)
        {
            case IdNode id:
                _idNodeCount++;
                _uniqueIdentifiers.Add(id.Identifier);
                break;
            case IfStatNode conditional when conditional.ElseStat is not null:
                _elseCount++;
                break;
            case JumpStatNode jump:
                _jumpStatCounts[jump.Type] = _jumpStatCounts.GetValueOrDefault(jump.Type) + 1;
                break;
            case FuncCallExprNode call when call.Identifier == _functionName:
                _foundRecursion = true;
                break;
        }
        try
        {
            // Development LINVAST includes all for-loop components in Children.
            return base.VisitChildren(node);
        }
        finally
        {
            _currentDepth--;
        }
    }

    protected override int DefaultResult => 0;

    protected override int AggregateResult(int aggregate, int nextResult) => aggregate + nextResult;
}
