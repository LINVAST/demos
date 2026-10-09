# LINVAST CodeMigration — VS Code Plugin

A VS Code extension that migrates **Java** source to **Go** using the
[`CodeMigration` demo](../README.md) and the [LINVAST](https://github.com/LINVAST/LINVAST)
language-invariant AST library.

## What it does

- Parses Java with the ANTLR-based `JavaASTBuilder` (via `ImperativeASTFactory`).
- Walks the AST with `JavaToGoDataClassGenerator` to emit Go data-class code.
- Runs the .NET tool as a **subprocess** (`dotnet <dll> --migrate-stdin`) so no
  logic is duplicated in the extension.

## Commands

| Where | Command | Effect |
|-------|---------|--------|
| Explorer → Java file (right-click) | **LINVAST Code Migration: Migrate Java File to Go** | Open the file, emit Go in a new tab. |
| Editor → selected Java text (right-click) | **LINVAST Code Migration: Migrate Selected Java to Go** | Migrate the selection; otherwise migrate the whole document. |
| Command Palette | `linvast-codemigration.migrateFile` / `...migrateSelection` | Same commands via palette. |

## Settings

```jsonc
{
    "linvastCodeMigration.binaryPath": "",   // path to a self-contained CodeMigration binary (overrides engine/)
    "linvastCodeMigration.dotnetPath": "dotnet",
    "linvastCodeMigration.outputMode": "newTab"  // newTab | replaceSelection | diff
}
```

## Build & package

```bash
bash build.sh
```

This:
1. builds `CodeMigration.csproj` (Release) into `engine/` (DLLs + runtimeconfig),
2. compiles the TypeScript (`npm run compile`),
3. packages a `.vsix` at `../build/CodeMigration.vsix`.

Install the result:

```bash
code --install-extension ../build/CodeMigration.vsix
```

The packaged VSIX includes the .NET runtime, so only a .NET **runtime** (not the SDK)
is required on the host. For a zero-dependency install, publish a self-contained
binary and set `linvastCodeMigration.binaryPath`.

## Scope

This generator is a **demo subset**: plain data classes, simple methods,
`while`/`for`/`if`, and basic expressions. Generics, inheritance, interfaces,
exceptions, lambdas, and most of the Java standard library are **not** handled.

## License

GPL-3.0-only — see [LICENSE](LICENSE). Based on [LINVAST](https://github.com/LINVAST/LINVAST).
