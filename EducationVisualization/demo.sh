#!/usr/bin/env bash
set -euo pipefail

# demo.sh -- Generate LINVAST AST JSON from C snippet variants and visualize
#           in the LINVisualizerJS web app.
#
# Usage:
#   ./demo.sh [v1|v2|v3|v4|all|stop]
#
# The script resolves the linvast CLI (PATH -> $LINVAST_PATH -> relative
# ../../CLI/run fallback), wraps each snippet in int main() { }, pipes it
# to `linvast ast - --language c`, and writes compact JSON to the web
# project's served samples directory. The web visualizer re-fetches on
# each click of the "Sample" button (no-store cache).

SCRIPT_DIR="$(cd -- "$(dirname -- "${BASH_SOURCE[0]}")" && pwd -P)"
DEMO_DIR="$SCRIPT_DIR"
CLI_DIR="$(cd "$DEMO_DIR/../../CLI" && pwd -P)"
WEB_DIR="$(cd "$DEMO_DIR/../../web" && pwd -P)"
WEB_DIST="$WEB_DIR/dist"
SAMPLES_OUT="$WEB_DIST/samples"
OUTPUT_JSON="$SAMPLES_OUT/example-ast.json"
PORT="${LINVAST_DEMO_PORT:-5174}"
SERVER_LOG="/tmp/linvast-demo-server.log"
SERVER_PID_FILE="/tmp/linvast-demo-server.pid"

# -- CLI resolution ----------------------------------------------
# Sets CLI_CMD array: either the linvast binary (PATH or $LINVAST_PATH)
# or the relative run wrapper at ../../CLI/run (with --no-build if
# already compiled for faster startup).
resolve_cli() {
    if command -v linvast >/dev/null 2>&1; then
        CLI_CMD=("linvast")
    elif [[ -n "${LINVAST_PATH:-}" ]]; then
        CLI_CMD=("$LINVAST_PATH")
    else
        local cli_dll="$CLI_DIR/CLI/bin/Debug/net10.0/CLI.dll"
        if [[ -f "$cli_dll" ]]; then
            CLI_CMD=("$CLI_DIR/run" "--no-build" "--")
        else
            echo "  -> CLI not built yet, building linvast CLI..."
            CLI_CMD=("$CLI_DIR/run" "--")
        fi
    fi
}

# -- Snippet wrapping --------------------------------------------
# Wraps a bare C snippet in int main() { ... } for valid parsing.
wrap_snippet() {
    local snippet_file="$1"
    printf 'int main()\n{\n%s\n}\n' "$(cat "$snippet_file")"
}

# -- AST JSON generation -----------------------------------------
generate_ast() {
    local variant="$1"
    local snippet_file="$DEMO_DIR/Samples/sample_${variant}.c"

    if [[ ! -f "$snippet_file" ]]; then
        echo "error: sample file not found: $snippet_file" >&2
        exit 1
    fi

    mkdir -p "$SAMPLES_OUT"

    resolve_cli

    local wrapped
    wrapped="$(wrap_snippet "$snippet_file")"

    echo "  -> Generating AST for ${variant} (via ${CLI_CMD[0]})..."

    local cli_stderr="/tmp/linvast-demo-cli-${variant}.log"
    set +e
    printf '%s' "$wrapped" \
        | "${CLI_CMD[@]}" ast - --language c --compact -o "$OUTPUT_JSON" -q \
        >"$cli_stderr" 2>&1
    local rc=$?
    set -e

    if [[ $rc -ne 0 ]]; then
        echo "error: linvast CLI failed (exit code $rc):" >&2
        cat "$cli_stderr" >&2
        exit 1
    fi

    echo "  -> Wrote $OUTPUT_JSON"
}

# -- Web dist check ----------------------------------------------
ensure_dist() {
    if [[ -d "$WEB_DIST" ]]; then
        return 0
    fi

    echo "  -> dist/ not found, building web project..."
    (cd "$WEB_DIR" && node scripts/build.mjs)
}

# -- Server management -------------------------------------------
server_up() {
    if command -v curl >/dev/null 2>&1; then
        curl -sf --max-time 1 "http://127.0.0.1:${PORT}/" >/dev/null 2>&1
    else
        timeout 1 bash -c "exec 3<>/dev/tcp/127.0.0.1/${PORT}" 2>/dev/null
    fi
}

start_server() {
    if server_up; then
        echo "  -> Web server already running at http://127.0.0.1:${PORT}"
        return 0
    fi

    echo "  -> Starting web server on port ${PORT}..."
    nohup node "$WEB_DIR/scripts/server.mjs" \
        --root "$WEB_DIST" --port "$PORT" --open \
        >"$SERVER_LOG" 2>&1 &

    local pid=$!
    echo "$pid" >"$SERVER_PID_FILE"

    # Wait for readiness (up to 15 s)
    local i=0
    while ! server_up; do
        i=$((i + 1))
        if [[ $i -ge 30 ]]; then
            echo "error: web server did not become ready (check $SERVER_LOG)" >&2
            tail -5 "$SERVER_LOG" >&2 || true
            exit 1
        fi
        sleep 0.5
    done

    echo "  -> Web server started (PID $pid)"
}

stop_server() {
    if [[ -f "$SERVER_PID_FILE" ]]; then
        local pid
        pid="$(cat "$SERVER_PID_FILE")"
        if kill "$pid" >/dev/null 2>&1; then
            echo "  -> Stopped web server (PID $pid)"
        else
            echo "  -> Server PID $pid not running"
        fi
        rm -f "$SERVER_PID_FILE"
    else
        pkill -f "server.mjs.*${PORT}" 2>/dev/null && \
            echo "  -> Stopped web server (pattern match)" || \
            echo "  -> No server to stop"
    fi
}

# -- Main --------------------------------------------------------
main() {
    local variant="${1:-v1}"

    echo ""
    echo " LINVAST AST Visualization Demo"
    echo " ================================"
    echo ""

    case "$variant" in
        v1|v2|v3|v4)
            ensure_dist
            generate_ast "$variant"
            start_server
            echo ""
            echo "  Open http://127.0.0.1:${PORT} in your browser"
            echo "  1. Click the 'Sample' button to load the AST"
            echo "  2. Try another variant:  ./demo.sh v2   (or v3, v4)"
            echo "  Control-C or ./demo.sh stop to kill the server."
            ;;
        all)
            ensure_dist
            for v in v1 v2 v3 v4; do
                generate_ast "$v"
                cp "$OUTPUT_JSON" "$SAMPLES_OUT/${v}.json"
            done
            cp "$SAMPLES_OUT/v1.json" "$OUTPUT_JSON"
            start_server
            echo ""
            echo "  All variants pre-generated in samples/."
            echo "  Use the file-upload icon to compare individual variants."
            ;;
        stop)
            stop_server
            ;;
        *)
            echo "Usage: $0 [v1|v2|v3|v4|all|stop]"
            echo ""
            echo "Variants:"
            echo "  v1  return 0;"
            echo "  v2  int x = 5; + return x;"
            echo "  v3  + if (x > 0) { x = x + 1; }"
            echo "  v4  + for (int i = 0; i < 3; i++) { x += i; }"
            echo "  all  Generate all variants (for file-upload comparison)"
            echo "  stop Stop the web server"
            exit 1
            ;;
    esac
}

main "$@"
