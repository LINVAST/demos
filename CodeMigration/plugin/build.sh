#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd -P)"
PLUGIN_DIR="$SCRIPT_DIR"
DEMO_DIR="$(cd "$PLUGIN_DIR/.." && pwd -P)"

CONFIG="${CONFIG:-Release}"
DOTNET="${DOTNET:-dotnet}"

if [[ -z "${OUT_DIR:-}" ]]; then
    OUT_DIR="$PLUGIN_DIR/build"
fi
mkdir -p "$OUT_DIR"

detect_rid() {
    local os_name os_arch
    os_name="$(uname -s)"
    os_arch="$(uname -m)"
    case "$os_name" in
        Linux*)  echo "linux-${os_arch}" ;;
        Darwin*) echo "osx-${os_arch}" ;;
        MINGW*|MSYS*|CYGWIN*) echo "win-${os_arch}" ;;
        *) echo "linux-${os_arch}" ;;
    esac
}

is_windows() {
    case "$(uname -s)" in
        MINGW*|MSYS*|CYGWIN*) return 0 ;;
        *) return 1 ;;
    esac
}

echo "=== LINVAST CodeMigration VS Code plugin build ==="
echo "plugin:  $PLUGIN_DIR"
echo "demo:    $DEMO_DIR"
echo "config:  $CONFIG"
echo "out:     $OUT_DIR"
echo

# --- 1. Build the .NET CodeMigration (framework-dependent DLL) ---------------
echo ">> [1/3] Building CodeMigration ($CONFIG) (framework-dependent)"
"$DOTNET" build "$DEMO_DIR/CodeMigration.csproj" -c "$CONFIG" -o "$OUT_DIR/bin"

# Copy the runtime DLLs + deps/*.runtimeconfig.json into the extension's engine/
# folder so they ship in the VSIX and the extension can locate them via
# context.extensionPath.
ENGINE_DIR="$PLUGIN_DIR/engine"
rm -rf "$ENGINE_DIR"
mkdir -p "$ENGINE_DIR"
cp "$OUT_DIR/bin"/*.dll "$OUT_DIR/bin"/CodeMigration.deps.json "$OUT_DIR/bin"/CodeMigration.runtimeconfig.json "$ENGINE_DIR/"
echo "   -> copied .NET runtime to $ENGINE_DIR"

# --- 2. Compile the TypeScript extension -------------------------------------
echo ">> [2/3] Compiling TypeScript"
if [[ ! -d "$PLUGIN_DIR/node_modules" ]]; then
    npm install --prefix "$PLUGIN_DIR" --no-audit --no-fund
fi
npm run --prefix "$PLUGIN_DIR" compile

# --- 3. Package the VSIX ----------------------------------------------------
echo ">> [3/3] Packaging VSIX"

# Ensure vsce is available; install into the plugin's node_modules if missing.
if ! npx --no --prefix "$PLUGIN_DIR" vsce --version >/dev/null 2>&1; then
    echo "   -> installing vsce..."
    npm install --prefix "$PLUGIN_DIR" --no-audit --no-fund @vscode/vsce@latest
fi

VSIX_NAME="CodeMigration.vsix"
(
    cd "$PLUGIN_DIR"
    npx --prefix "$PLUGIN_DIR" --no-install vsce package --no-dependencies --out "$OUT_DIR/$VSIX_NAME"
)

echo
echo "=== Done ==="
echo "VSIX: $OUT_DIR/$VSIX_NAME"
echo
echo "Install in VS Code:"
echo "  code --install-extension '$OUT_DIR/$VSIX_NAME'"
echo
echo "The packaged VSIX includes the CodeMigration .NET runtime in engine/ and will"
echo "auto-run via 'dotnet' + engine/CodeMigration.dll. If dotnet is not on PATH, set"
echo "linvastCodeMigration.binaryPath to a self-contained binary (see note below)."
