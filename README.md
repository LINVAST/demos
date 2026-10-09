# LINVAST Demos

Demonstration projects built on [LINVAST](https://github.com/LINVAST/LINVAST) — a
language-invariant AST library that parses source code into a shared node tree
regardless of the input language.

| Demo | What it shows |
|------|----------------|
| [CodeMigration](CodeMigration/) | Parse **Java** and emit **Go** via an AST visitor. Includes a VS Code plugin. |
| [FeatureExtraction](FeatureExtraction/) | Extract code metrics (cyclomatic complexity, max depth, recursion, etc.) from a multi-language corpus into CSV. |
| [EducationVisualization](EducationVisualization/) | Visualize how incremental source edits change the AST, served in the [LINVisualizerJS](https://github.com/LINVAST/web) web app. |

## Prerequisites

- **.NET 10 SDK** — builds and runs the `CodeMigration` and `FeatureExtraction` console apps.
- For the VS Code plugin (CodeMigration): **Node.js >= 20** and the `vsce` packaging tool.
- For the EducationVisualization demo: **Node.js** (for the web server) and the
  `linvast` CLI (resolved automatically via `demo.sh` — see below).

## Solution structure

```
demos/
├── Demos.sln                 # Visual Studio solution pulling in LINVAST libs + demos
├── CodeMigration/            # Java → Go code migration
│   ├── Program.cs
│   ├── JavaToGoDataClassGenerator.cs
│   ├── Samples/              # *.java inputs
│   └── plugin/               # VS Code extension (TypeScript + build.sh)
├── FeatureExtraction/        # Multi-language metrics extractor → CSV
│   ├── Program.cs
│   ├── FeatureExtractor.cs
│   ├── CsvRow.cs
│   └── Samples/              # *.c, *.java, *.go, *.py, *.kt, *.lua
└── EducationVisualization/   # AST visualization demo (bash-driven)
    ├── demo.sh
    └── Samples/              # sample_v1..v4.c
```

## Running the demos

### CodeMigration

```bash
# Process all Samples/*.java in the directory
dotnet run --project CodeMigration/CodeMigration.csproj

# Migrate a single file
dotnet run --project CodeMigration/CodeMigration.csproj -- CodeMigration/Samples/Counter.java

# Read Java from stdin, emit Go to stdout
echo 'int x = 5;' | dotnet run --project CodeMigration/CodeMigration.csproj -- --migrate-stdin
```

Sample inputs in `CodeMigration/Samples/`:

| File | Description |
|------|-------------|
| `JavaDataClass.java` | A simple data class with fields, a constructor, and getters/setters. |
| `Counter.java` | A class with loops, conditionals, and arithmetic expressions. |
| `LruCache.java` | A linked-list LRU cache (uses generics — beyond the generator's scope). |

> See `CodeMigration/plugin/README.md` for the VS Code extension build & usage guide.

### FeatureExtraction

```bash
dotnet run --project FeatureExtraction/FeatureExtraction.csproj
```

Reads every file in `FeatureExtraction/Samples/` and writes a CSV table (one
row per function/method) to stdout, covering 6 languages:

| File | Language |
|------|----------|
| `quicksort.c` | C |
| `binarytree.java` | Java |
| `dijkstra.go` | Go |
| `fibonacci.py` | Python |
| `gcd.kt` | Kotlin |
| `knapsack.lua` | Lua |

Metrics include total AST nodes, max nesting depth, identifier count, branch/loop
counts, cyclomatic complexity, and recursion detection.

### EducationVisualization

```bash
cd EducationVisualization
./demo.sh v1     # Generate AST for sample_v1.c and open the web visualizer
./demo.sh v2     # Step through v1 → v4 to see how the tree changes
./demo.sh v3
./demo.sh v4
./demo.sh all    # Pre-generate all four variants as JSON files
./demo.sh stop   # Stop the web server
```

Each variant adds a language construct so you can see how each AST node type
appears:

| Variant | Adds | Key AST nodes introduced |
|---------|------|--------------------------|
| `v1` | `return 0;` | `JumpStatNode`, `LitExprNode` |
| `v2` | `int x = 5;` | `DeclStatNode`, `VarDeclNode` |
| `v3` | `if (x > 0) { ... }` | `IfStatNode`, `RelExprNode`, `AssignExprNode`, `ArithmExprNode` |
| `v4` | `for (...) { x += i; }` | `ForStatNode`, `ComplexAssignOpNode`, `IncExprNode` |

The script generates the AST via the `linvast` CLI (resolved from PATH, the
`LINVAST_PATH` env var, or the `../CLI/run` fallback) and serves it in
[LINVisualizerJS](https://github.com/LINVAST/web) at
`http://127.0.0.1:5174`.

## Building everything

Open `Demos.sln` in Visual Studio, or build a single project:

```bash
dotnet build Demos.sln
```

