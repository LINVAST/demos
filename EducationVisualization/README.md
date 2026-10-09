# EducationVisualization Demo

A demonstration of how **source-code changes affect the AST structure** produced by [LINVAST](https://github.com/LINVAST/LINVAST), visualized in [LINVisualizerJS](https://github.com/LINVAST/web).

Four C snippet variants progressively add language constructs so you can see exactly how each edit changes the tree:

| Variant | Code added | New AST node types |
|---------|-----------|--------------------|
| `v1` | `return 0;` | `JumpStatNode`, `LitExprNode` |
| `v2` | `+ int x = 5;` | `DeclStatNode`, `VarDeclNode` |
| `v3` | `+ if (x > 0) { x = x + 1; }` | `IfStatNode`, `RelExprNode`, `RelOpNode`, `AssignExprNode`, `ArithmExprNode` |
| `v4` | `+ for (int i = 0; i < 3; i++) { x += i; }` | `ForStatNode`, `ComplexAssignOpNode`, `IncExprNode` |

## Prerequisites

- **.NET 10 SDK** — to run the `linvast` CLI (built automatically via `../../CLI/run`)
- **Node.js >= 20** — to serve the web visualizer
- **curl** (or any HTTP client) — used to check if the server is already running

## Usage

```bash
# Generate AST for variant v1 and open the visualizer
./demo.sh v1

# Step through variants (server stays running)
./demo.sh v2
./demo.sh v3
./demo.sh v4

# Pre-generate all variants as separate JSON files
./demo.sh all

# Stop the web server
./demo.sh stop
```

### How it works

1. **`demo.sh`** reads a C snippet from `Samples/sample_v<n>.c`, wraps it in `int main() { ... }`, and pipes it to the **linvast CLI** via stdin (`linvast ast - --language c`).
2. The CLI generates compact **AST JSON** which is written to `../../web/dist/samples/example-ast.json`.
3. The web server (`node scripts/server.mjs`) serves the static site at `http://127.0.0.1:5174` with `no-store` cache headers.
4. In the browser, click the **Sample** button to load the latest AST. To see a different variant, run `./demo.sh v<n>` and click **Sample** again.

### CLI resolution

The script resolves the `linvast` CLI in this order:

1. `linvast` on `$PATH`
2. `$LINVAST_PATH` environment variable
3. Relative fallback: `../../CLI/run` (auto-builds if needed via `dotnet run`)

### File-upload comparison

After running `./demo.sh all`, each variant is saved as a separate JSON file (`v1.json`–`v4.json`) in `../../web/dist/samples/`. Use the **file-upload** icon (📁) in the visualizer to load and compare any variant directly.

## Project layout

```
EducationVisualization/
├── demo.sh              # Entry point — generates AST JSON and manages the web server
├── Program.cs           # C# stub (for the .NET solution); demo is run via demo.sh
├── Samples/
│   ├── sample_v1.c      # return 0;
│   ├── sample_v2.c      # + int x = 5; return x;
│   ├── sample_v3.c      # + if (x > 0) { x = x + 1; }
│   └── sample_v4.c      # + for (int i = 0; i < 3; i++) { x += i; }
└── EducationVisualization.csproj
```
