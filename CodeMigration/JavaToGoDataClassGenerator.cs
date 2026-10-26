using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LINVAST.Imperative.Nodes;
using LINVAST.Imperative.Nodes.Common;
using LINVAST.Imperative.Visitors;
using LINVAST.Nodes;

namespace LINVAST.Demos.CodeMigration;

public sealed class JavaToGoDataClassGenerator : BaseASTVisitor<string>
{
    private readonly StringBuilder _output = new();
    private readonly HashSet<string> _privateFields = new(StringComparer.Ordinal);
    private string _className = string.Empty;
    private int _indentLevel;
    private int _blockDepth;

    protected override string DefaultResult => string.Empty;
    protected override string AggregateResult(string aggregate, string nextResult) => aggregate + nextResult;

    public string Generate(SourceNode source)
    {
        _output.Clear();
        _className = string.Empty;
        _indentLevel = 0;
        _blockDepth = 0;
        _privateFields.Clear();

        _output.AppendLine("package main");
        _output.AppendLine();

        this.Visit(source);
        return _output.ToString();
    }

    private string Indent() => new string(' ', _indentLevel * 4);

    public override string Visit(SourceNode node)
    {
        foreach (var child in node.Children)
            this.Visit(child);
        return string.Empty;
    }

    public override string Visit(DeclStatNode node)
    {
        if (node is ClassNode classNode)
            return HandleClassNode(classNode);

        if (_blockDepth == 0)
            return string.Empty;

        var firstDecl = node.DeclaratorList.Declarators.FirstOrDefault();
        if (firstDecl is VarDeclNode varDecl)
        {
            string goType = MapType(node.Specifiers.TypeName);
            if (varDecl.Initializer is not null)
            {
                string init = this.Visit(varDecl.Initializer);
                return $"{varDecl.Identifier} := {init}";
            }
            return $"var {varDecl.Identifier} {goType}";
        }

        return string.Empty;
    }

    private string HandleClassNode(ClassNode node)
    {
        var declList = node.Children.ElementAt(1).As<DeclListNode>();
        var typeDecl = declList.Declarators.First().As<TypeDeclNode>();

        _className = typeDecl.Identifier;

        var fields = new List<(string GoType, string GoName, bool IsPrivate)>();
        foreach (var decl in typeDecl.Declarations)
        {
            var firstDecl = decl.DeclaratorList.Declarators.FirstOrDefault();
            bool isPrivate = decl.Specifiers.Modifiers.AccessModifiers == AccessModifiers.Private;
            if (firstDecl is VarDeclNode varDecl)
            {
                string goType = MapType(decl.Specifiers.TypeName);
                string goName = ToGoFieldName(varDecl.Identifier, isPrivate);
                fields.Add((goType, goName, isPrivate));
                if (isPrivate)
                    _privateFields.Add(varDecl.Identifier);
            }
            else if (firstDecl is ArrDeclNode arrDecl)
            {
                string goType = MapType(decl.Specifiers.TypeName);
                string goName = ToGoFieldName(arrDecl.Identifier, isPrivate);
                fields.Add((goType, goName, isPrivate));
                if (isPrivate)
                    _privateFields.Add(arrDecl.Identifier);
            }
        }

        _output.AppendLine($"type {_className} struct {{");
        foreach (var (goType, goName, _) in fields)
            _output.AppendLine($"    {goName} {goType}");
        _output.AppendLine("}");
        _output.AppendLine();

        foreach (var decl in typeDecl.Declarations)
        {
            var firstDecl = decl.DeclaratorList.Declarators.FirstOrDefault();
            if (firstDecl is FuncDeclNode funcDecl)
                GenerateMethod(funcDecl, decl.Specifiers);
        }

        return string.Empty;
    }

    private void GenerateMethod(FuncDeclNode funcDecl, DeclSpecsNode specs)
    {
        string methodName = funcDecl.Identifier;
        bool isConstructor = string.Equals(methodName, _className, StringComparison.Ordinal);
        string goName = isConstructor ? $"New{_className}" : ToGoMethodName(methodName);

        string returnType = string.Empty;
        if (!isConstructor && specs.TypeName != "void")
            returnType = MapType(specs.TypeName);

        var paramTexts = new List<string>();
        if (funcDecl.Parameters is not null)
        {
            foreach (var param in funcDecl.Parameters)
            {
                string pType = MapType(param.Specifiers.TypeName);
                string pName = param.Declarator.Identifier;
                paramTexts.Add($"{pName} {pType}");
            }
        }
        string paramsStr = string.Join(", ", paramTexts);

        if (isConstructor)
        {
            _output.AppendLine($"func {goName}({paramsStr}) *{_className} {{");
            _output.AppendLine($"    p := &{_className}{{}}");
        }
        else
        {
            string sig = string.IsNullOrEmpty(returnType)
                ? $"func (p *{_className}) {goName}({paramsStr}) {{"
                : $"func (p *{_className}) {goName}({paramsStr}) {returnType} {{";
            _output.AppendLine(sig);
        }

        if (funcDecl.Definition is not null)
            this.Visit(funcDecl.Definition);

        if (isConstructor)
            _output.AppendLine("    return p");

        _output.AppendLine("}");
        _output.AppendLine();
    }

    public override string Visit(BlockStatNode node)
    {
        _blockDepth++;
        _indentLevel++;
        foreach (var child in node.Children)
        {
            string line = this.Visit(child);
            if (!string.IsNullOrEmpty(line))
                _output.AppendLine($"{Indent()}{line}");
        }
        _indentLevel--;
        _blockDepth--;
        return string.Empty;
    }

    public override string Visit(IfStatNode node)
    {
        string cond = this.Visit(node.Condition);
        _output.AppendLine($"{Indent()}if {cond} {{");
        _indentLevel++;
        VisitStatement(node.ThenStat);
        _indentLevel--;

        StatNode? elseStat = node.ElseStat;
        if (elseStat is not null)
        {
            if (elseStat is IfStatNode elseIf)
            {
                string elseCond = this.Visit(elseIf.Condition);
                _output.AppendLine($"{Indent()}}} else if {elseCond} {{");
                _indentLevel++;
                VisitStatement(elseIf.ThenStat);
                _indentLevel--;

                if (elseIf.ElseStat is not null)
                {
                    AppendElse(elseIf.ElseStat);
                }
                else
                {
                    _output.AppendLine($"{Indent()}}}");
                }
            }
            else
            {
                _output.AppendLine($"{Indent()}}} else {{");
                _indentLevel++;
                VisitStatement(elseStat);
                _indentLevel--;
                _output.AppendLine($"{Indent()}}}");
            }
        }
        else
        {
            _output.AppendLine($"{Indent()}}}");
        }

        return string.Empty;
    }

    private void AppendElse(StatNode elseStat)
    {
        if (elseStat is IfStatNode elseIf)
        {
            string elseCond = this.Visit(elseIf.Condition);
            _output.AppendLine($"{Indent()}}} else if {elseCond} {{");
            _indentLevel++;
            VisitStatement(elseIf.ThenStat);
            _indentLevel--;

            if (elseIf.ElseStat is not null)
                AppendElse(elseIf.ElseStat);
            else
                _output.AppendLine($"{Indent()}}}");
        }
        else
        {
            _output.AppendLine($"{Indent()}}} else {{");
            _indentLevel++;
            VisitStatement(elseStat);
            _indentLevel--;
            _output.AppendLine($"{Indent()}}}");
        }
    }

    private void VisitStatement(StatNode stat)
    {
        if (stat is BlockStatNode block)
        {
            foreach (var child in block.Children)
            {
                string line = this.Visit(child);
                if (!string.IsNullOrEmpty(line))
                    _output.AppendLine($"{Indent()}{line}");
            }
        }
        else
        {
            string line = this.Visit(stat);
            if (!string.IsNullOrEmpty(line))
                _output.AppendLine($"{Indent()}{line}");
        }
    }

    public override string Visit(WhileStatNode node)
    {
        string cond = this.Visit(node.Condition);
        _output.AppendLine($"{Indent()}for {cond} {{");
        _indentLevel++;
        VisitStatement(node.Statement);
        _indentLevel--;
        _output.AppendLine($"{Indent()}}}");
        return string.Empty;
    }

    public override string Visit(ForStatNode node)
    {
        string init = ConvertForInit(node);
        string cond = node.Condition is not null ? this.Visit(node.Condition) : "true";
        string update = node.IncrExpr is not null ? this.Visit(node.IncrExpr) : string.Empty;

        _output.AppendLine($"{Indent()}for {init}; {cond}; {update} {{");
        _indentLevel++;
        VisitStatement(node.Statement);
        _indentLevel--;
        _output.AppendLine($"{Indent()}}}");
        return string.Empty;
    }

    private string ConvertForInit(ForStatNode node)
    {
        if (node.ForDeclaration is not null)
            return ConvertJavaLocalDeclToGo(node.ForDeclaration.GetText());

        if (node.InitExpr is not null)
            return ConvertInitExprToGo(node.InitExpr);

        return string.Empty;
    }

    private string ConvertInitExprToGo(ExprNode initExpr)
    {
        if (initExpr is FuncCallExprNode marker && marker.Identifier.StartsWith("__linvast_"))
        {
            var arg = marker.Arguments?.Expressions?.FirstOrDefault();
            if (arg is IdNode idNode)
                return ConvertJavaLocalDeclToGo(idNode.Identifier);
        }
        return this.Visit(initExpr);
    }

    private static string ConvertJavaLocalDeclToGo(string javaDecl)
    {
        javaDecl = javaDecl.Trim().TrimEnd(';').Trim();
        var parts = javaDecl.Split('=', 2, StringSplitOptions.RemoveEmptyEntries);
        string decl = parts[0].Trim();
        string init = parts.Length > 1 ? parts[1].Trim() : string.Empty;

        var tokens = decl.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Length >= 2)
        {
            string name = tokens[1];
            if (!string.IsNullOrEmpty(init))
                return $"{name} := {init}";
            string goType = MapType(tokens[0]);
            return $"var {name} {goType}";
        }
        return javaDecl;
    }

    public override string Visit(JumpStatNode node)
    {
        if (node.Type == JumpStatType.Return)
        {
            if (node.ReturnExpr is not null)
            {
                string expr = this.Visit(node.ReturnExpr);
                return $"return {expr}";
            }
            return "return";
        }
        if (node.Type == JumpStatType.Break)
            return "break";
        if (node.Type == JumpStatType.Continue)
            return "continue";
        return string.Empty;
    }

    public override string Visit(ExprStatNode node) => this.Visit(node.Expression);

    public override string Visit(AssignExprNode node)
    {
        string left = this.Visit(node.LeftOperand);
        string right = this.Visit(node.RightOperand);
        string op = node.Operator.Symbol;
        return $"{left} {op} {right}";
    }

    public override string Visit(ArithmExprNode node)
    {
        string left = this.Visit(node.LeftOperand);
        string right = this.Visit(node.RightOperand);
        string op = node.Operator.Symbol;
        return $"{left} {op} {right}";
    }

    public override string Visit(RelExprNode node)
    {
        string left = this.Visit(node.LeftOperand);
        string right = this.Visit(node.RightOperand);
        string op = node.Operator.Symbol;
        return $"{left} {op} {right}";
    }

    public override string Visit(LogicExprNode node)
    {
        string left = this.Visit(node.LeftOperand);
        string right = this.Visit(node.RightOperand);
        string op = node.Operator.Symbol;
        return $"{left} {op} {right}";
    }

    public override string Visit(UnaryExprNode node)
    {
        string operand = this.Visit(node.Operand);
        string op = node.Operator.Symbol;
        return $"{op}{operand}";
    }

    public override string Visit(FuncCallExprNode node)
    {
        string id = node.Identifier;
        string args = string.Empty;
        if (node.Arguments is not null)
            args = string.Join(", ", node.Arguments.Expressions.Select(a => this.Visit(a)));

        if (id.StartsWith("this."))
        {
            string methodName = id[5..];
            return $"p.{ToGoMethodName(methodName)}({args})";
        }

        return $"{id}({args})";
    }

    public string Visit(ConsExprNode node)
    {
        string id = node.Identifier;
        string args = string.Empty;
        if (node.Arguments is not null)
            args = string.Join(", ", node.Arguments.Expressions.Select(a => this.Visit(a)));

        if (id.StartsWith("this."))
        {
            string methodName = id[5..];
            return $"p.{ToGoMethodName(methodName)}({args})";
        }

        return $"{ToGoMethodName(id)}({args})";
    }

    public override string Visit(NullLitExprNode node) => "nil";

    public override string Visit(LitExprNode node)
    {
        if (node.Value is string str)
        {
            string content = str;
            if (content.Length >= 2 && content[0] == '"' && content[^1] == '"')
            {
                content = content[1..^1];
            }
            return $"\"{content.Replace("\\", "\\\\").Replace("\"", "\\\"")}\"";
        }
        if (node.Value is bool b)
            return b ? "true" : "false";
        return node.Value?.ToString() ?? "nil";
    }

    public override string Visit(IdNode node)
    {
        string name = node.Identifier;

        if (name.StartsWith("this."))
        {
            string fieldName = name[5..];
            return $"p.{ToGoFieldName(fieldName, _privateFields.Contains(fieldName))}";
        }

        if (name == "this")
            return "p";

        return name;
    }

    public override string Visit(VarDeclNode node) => node.Identifier;

    public override string Visit(FuncParamNode node)
    {
        string goType = MapType(node.Specifiers.TypeName);
        string name = node.Declarator.Identifier;
        return $"{name} {goType}";
    }

    public override string Visit(DeclSpecsNode node) => MapType(node.TypeName);

    public string Visit(TypeNameNode node) => MapType(node.TypeName);

    private static string MapType(string javaType)
    {
        if (string.IsNullOrEmpty(javaType))
            return string.Empty;

        if (javaType.EndsWith("[]"))
        {
            string inner = javaType[..^2];
            return "[]" + MapType(inner);
        }

        return javaType switch
        {
            "String" => "string",
            "boolean" => "bool",
            "double" => "float64",
            "float" => "float32",
            "long" => "int64",
            "int" => "int",
            "short" => "int16",
            "byte" => "int8",
            "char" => "rune",
            "void" => string.Empty,
            _ => javaType,
        };
    }

    private static string ToGoFieldName(string javaName, bool isPrivate)
    {
        if (string.IsNullOrEmpty(javaName))
            return javaName;
        if (isPrivate)
            return char.ToLower(javaName[0]) + javaName[1..];
        return char.ToUpper(javaName[0]) + javaName[1..];
    }

    private static string ToGoMethodName(string javaName)
    {
        if (string.IsNullOrEmpty(javaName))
            return javaName;
        return char.ToUpper(javaName[0]) + javaName[1..];
    }
}
