# Feature extraction demo

This demo measures LINVAST function-body structure in 120 source files, 20 each for C, Java, Go, Kotlin, Lua, and Python. It produces an **illustrative composite structural score**. The score has no demonstrated predictive validity for difficulty, maintenance effort, defects, or language-neutral complexity. The weights are demonstration choices, not fitted or validated coefficients.

## Reproduce the results

Use .NET 10 and Python 3, with the development LINVAST checkout at `../../LINVAST`. The deposited results use revision `1cefa1e4b4aad40c172054a0851e88a29716121b` with a clean worktree. The project also supports packages, but these results and checks specifically require the development implementation, including its updated loop children.

From this folder:

```sh
python3 regenerate.py
```

The script explicitly builds with `-p:UseLocalLinvast=true`, places all build artifacts (including dependency artifacts) under this folder, runs regression checks, verifies exactly 120 rows and 20 per language, and compares two extraction and scoring runs for identical output. It writes `out.txt` (CSV), `illustrative-scores.csv`, and `score-statistics.json`. `Samples2` is not part of this corpus. No paper files are generated. The previously deleted `out-scores.txt` is not restored.

The individual commands are:

```sh
dotnet build -p:UseLocalLinvast=true --artifacts-path "$PWD/.artifacts" -m:1 -p:UseSharedCompilation=false
dotnet .artifacts/bin/FeatureExtraction/debug/FeatureExtraction.dll --self-test
python3 -m unittest test_scores.py
dotnet .artifacts/bin/FeatureExtraction/debug/FeatureExtraction.dll Samples > out.txt
python3 complexity_scores.py --stats score-statistics.json < out.txt > illustrative-scores.csv
```

Use `regenerate.py` to include dependency provenance in the statistics file. The executable accepts an optional samples-directory argument. Unsupported extensions are ignored; exceptions during extraction produce a nonzero exit code and no CSV. The parser may recover from some syntax errors without throwing; successful extraction does not establish source validity. Corpus completeness is enforced by the regeneration script.

## Feature definitions

CSV field names are retained for compatibility. `Language` now contains the language name; `SourceFile` and `FunctionName` identify the row. Rows cover top-level `FuncNode` definitions and Java `FuncDeclNode` methods discovered through descendant declarations. Nested functions are not emitted as separate rows; their represented descendants contribute to the containing body's measurements.

A fresh visitor traverses the function body, including the body root, once through `Children`. Function signatures and their parameter declarations are outside this traversal. Operator and other representation nodes count as nodes. The current development LINVAST includes for-loop initializers and increments in `Children`; no supplementary traversal is needed.

| Fields | Operational definition |
| --- | --- |
| `TotalNodes` | Number of visited AST node occurrences, including identifiers exactly once. |
| `MaxDepth` | Maximum root-to-node depth, with the body root at 1. |
| `IdNodeCount`, `NumUniqueIdentifiers` | Number of `IdNode` occurrences and number of distinct identifier strings, respectively; no binding analysis. |
| `IfCount`, `ForCount`, `WhileCount`, `ForeachCount`, `SwitchCount`, `TryCount` | Counts of corresponding `IfStatNode`, `ForStatNode`, `WhileStatNode`, `ForeachStatNode`, `SwitchStatNode`, and `TryStatNode` occurrences. |
| `ElseCount` | Number of `IfStatNode` occurrences with a non-null `ElseStat`. |
| `LogicExprCount`, `CondExprCount` | Counts of `LogicExprNode` and `CondExprNode`. |
| `AssignExprCount`, `ArithmExprCount`, `RelExprCount` | Counts with the exact corresponding node type name. Increment/decrement nodes are not assignment or arithmetic expression counts. |
| `FuncCallCount`, `LitExprCount`, `ArrAccessExprCount` | Counts with exact names `FuncCallExprNode`, `LitExprNode`, `ArrAccessExprNode`; these depend on parser representation. |
| `ReturnCount`, `BreakCount`, `ContinueCount` | `JumpStatNode` counts grouped by the corresponding jump type. |
| `VarDeclCount` | Body traversal count of `VarDeclNode` plus `FuncParamNode`; signature parameters are normally outside the traversal. |
| `ParamCount` | Total number of parameters in the function signature. |
| `ReturnType`, `ParamType1`–`ParamType5` | Parser-provided type labels subject to the qualification below; only the first five parameter labels are exported, with empty cells for absent parameter positions. |
| `IsRecursive` | True if a visited `FuncCallExprNode.Identifier` equals the current function's identifier. |
| `CyclomaticComplexity` | Structural approximation defined below. |

### Structural approximation to cyclomatic complexity

The retained `CyclomaticComplexity` column is exactly:

```text
1 + IfCount + ForCount + WhileCount + ForeachCount + LogicExprCount + CondExprCount
```

Each represented binary logical AND (`&&`, `and`) or OR (`||`, `or`) contributes one through `LogicExprNode`. Chained expressions contribute once per binary node. Unary negation, relational comparisons, and bitwise arithmetic operations do not contribute separately. Conditional expressions contribute one. Switch alternatives, exception handlers, and return/break/continue statements do not contribute. The formula does not build a control-flow graph or analyze reachability, constant conditions, or exception flow.

Hand-computed fixtures check an ordinary loop (2), an if plus two binary logical operations plus a conditional expression plus a while (6), and if with unary negation (2). A switch with multiple alternatives still scores 1, as does a try/catch without another counted decision; these explicitly demonstrate limitations rather than agreement with control-flow complexity. Parser omissions also matter: the tested Java try/catch fixture does not produce a `TryStatNode` in this dependency revision. Python exception counting is tested separately.

### Type and self-call limitations

Type labels are neither semantic type resolution nor reliable inference. This development revision preserves tested C pointer labels such as `int*` and normalizes tested Kotlin `Int` and `Array<Int>` to `int` and `int[]`. The demo applies no additional Kotlin mapping. Other qualifiers or type detail depend on the parser; outputs from older LINVAST releases are not interchangeable.

Lua types are reported as `unknown`. Python `void` and `object` labels are conservatively reported as `unknown`, because the AST does not retain enough provenance to distinguish absent annotations from all explicit annotations that normalize to those defaults (including `Any`). Other Python annotation labels are retained as emitted, including parser normalization. An explicit C `void` remains `void`. No inference from return statements is attempted. A tested C edge case, `void f(int *p) { *p=1; }`, is misrepresented by this dependency revision (function identity and signature); the demo cannot repair that upstream parser limitation and does not claim general signature accuracy.

`IsRecursive` means a **direct syntactic self-call heuristic**. Arguments are deliberately not compared: `f(n - 1)` matches inside `f`. Parameter shadowing and calls to another overload with the same name can produce false positives. Indirect calls and mutual recursion are not detected. Qualified calls depend on how the parser encodes the called identifier. The checks include changed arguments, shadowing, overloads, and mutual recursion.

## Composite score

For each of the five features, compute the mean and population standard deviation over all input function rows (`ddof = 0`). Then compute `z = (value - mean) / stddev`; if the standard deviation is zero, define `z = 0`. The score is the weighted sum of these z-scores, rounded to three decimals only when displayed. Ranking uses unrounded scores and resolves ties by source filename, then function name.

| Feature | Weight | Illustrative emphasis |
| --- | ---: | --- |
| `CyclomaticComplexity` | 0.40 | Counted decision structure |
| `MaxDepth` | 0.25 | Represented nesting |
| `TotalNodes` | 0.20 | Represented size |
| `FuncCallCount` | 0.10 | Call occurrences |
| `ArrAccessExprCount` | 0.05 | Array-access occurrences |

The default weights sum to one. `--weight FEATURE=WEIGHT` replaces or adds an individual weight; custom weights are not automatically normalized. Weights must be finite and nonnegative. Features must be finite numeric values. Constant features make zero contribution without redistribution of their weights.

Scores are relative to the input corpus. AST size and depth depend on representation, and correlated features can count the same structural property repeatedly. No learned aggregation method validates these particular manual weights. See `score-statistics.json` for regenerated means, population standard deviations, weights, and dependency provenance.

## Tested API example and change audit

`FeatureExtractor.cs` is the compiling visitor example: it derives from `BaseASTVisitor<int>` in `LINVAST.Imperative.Visitors`, implements the protected aggregation contract, and routes both generic and typed entry points through its counting traversal. `RegressionChecks.cs` exercises it with a single identifier, a hand-counted mixed tree, parsed fixtures, and independent tree enumeration for every corpus body. `Program.cs` supplies the full function-discovery example, including Java-method descent. Published snippets should be taken from these tested files rather than independently transcribed pseudocode.

`measurement-changes.csv` records every changed field across three runs matched by source filename and function name: the deposited CSV, the original demo code rebuilt against the development LINVAST revision, and the corrected demo. It is a historical migration audit, not an output overwritten by routine regeneration.

Differences between the first two runs arise from dependency changes (loop components and type labels). Differences between the last two arise from correcting `Language`, qualifying unknown types, removing repeated loop-component visits and duplicate identifiers, and counting the body root. In particular, all 120 depth values increase by 1 because the old typed entry point bypassed root tracking. That uniform depth shift does not change standardized depth values. Node totals now include the root once. Counts are validated against independent traversal rather than repaired by subtracting values from old tables.
