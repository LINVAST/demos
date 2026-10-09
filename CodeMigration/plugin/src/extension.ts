import * as path from 'path';
import * as fs from 'fs';
import * as vscode from 'vscode';
import { execFile, ExecFileException } from 'child_process';

export function activate(context: vscode.ExtensionContext) {
    const migrateFile = vscode.commands.registerCommand(
        'linvast-codemigration.migrateFile',
        async (uri: vscode.Uri | undefined) => runMigration(context, uri)
    );

    const migrateSelection = vscode.commands.registerCommand(
        'linvast-codemigration.migrateSelection',
        async () => runMigration(context, undefined)
    );

    context.subscriptions.push(migrateFile, migrateSelection);
}

export function deactivate() {}

interface ResolvedRunner {
    command: string;
    args: string[];
    label: string;
}

function resolveRunner(context: vscode.ExtensionContext): ResolvedRunner {
    const cfg = vscode.workspace.getConfiguration('linvastCodeMigration');
    const binaryPath = cfg.get<string>('binaryPath')?.trim() ?? '';
    const dotnetPath = cfg.get<string>('dotnetPath')?.trim() || 'dotnet';
    const projectPath = cfg.get<string>('projectPath')?.trim() ?? '';

    // 1) Configured self-contained binary (zero-dependency runtime).
    if (binaryPath) {
        if (!fs.existsSync(binaryPath)) {
            throw new Error(`Configured binaryPath does not exist: ${binaryPath}`);
        }
        return { command: binaryPath, args: ['--migrate-stdin'], label: 'CodeMigration binary' };
    }

    // 2) DLLs bundled in engine/ by build.sh (framework-dependent — needs dotnet runtime).
    const bundledDll = path.join(context.extensionPath, 'engine', 'CodeMigration.dll');
    if (fs.existsSync(bundledDll)) {
        return {
            command: dotnetPath,
            args: [bundledDll, '--migrate-stdin'],
            label: 'bundled CodeMigration.dll'
        };
    }

    // 3) Configured .csproj (uses `dotnet run`; rebuilds automatically).
    const resolvedProject = projectPath
        ? projectPath
        : path.resolve(context.extensionPath, '..', 'CodeMigration.csproj');

    if (fs.existsSync(resolvedProject)) {
        return {
            command: dotnetPath,
            args: ['run', '--project', resolvedProject, '--no-build', '--', '--migrate-stdin'],
            label: 'dotnet run CodeMigration'
        };
    }

    throw new Error(
        'Could not locate the CodeMigration binary or project. ' +
        'Build the plugin with `bash build.sh` (which bundles the .NET runtime in engine/) ' +
        'or set "linvastCodeMigration.binaryPath" to a published CodeMigration binary.'
    );
}

async function getJavaInput(
    uri: vscode.Uri | undefined
): Promise<{ code: string; label: string }> {
    if (uri) {
        const doc = await vscode.workspace.openTextDocument(uri);
        return { code: doc.getText(), label: doc.fileName };
    }

    const editor = vscode.window.activeTextEditor;
    if (editor) {
        const sel = editor.selection;
        if (!sel.isEmpty) {
            return { code: editor.document.getText(sel), label: editor.document.fileName };
        }
        return { code: editor.document.getText(), label: editor.document.fileName };
    }

    throw new Error('No Java file or selection found to migrate.');
}

async function runMigration(context: vscode.ExtensionContext, uri: vscode.Uri | undefined) {
    try {
        const { code, label } = await getJavaInput(uri);
        if (!code.trim()) {
            vscode.window.showWarningMessage('LINVAST Code Migration: no Java source to migrate.');
            return;
        }

        const runner = resolveRunner(context);
        const output = await invokeRunner(runner, code);

        const cfg = vscode.workspace.getConfiguration('linvastCodeMigration');
        const mode = cfg.get<'newTab' | 'replaceSelection' | 'diff'>('outputMode') ?? 'newTab';

        const doc = await vscode.workspace.openTextDocument({ content: output, language: 'go' });
        const target = vscode.window.tabGroups.activeTabGroup;

        if (mode === 'diff') {
            const originalDoc = await vscode.workspace.openTextDocument({ content: code, language: 'java' });
            try {
                await vscode.commands.executeCommand(
                    'vscode.executeCompareEditors',
                    originalDoc.uri,
                    doc.uri,
                    'left'
                );
            } catch (e) {
                vscode.window.showWarningMessage(
                    'LINVAST Code Migration: inline diff unavailable; opening Go output in a new tab.'
                );
                await vscode.window.showTextDocument(doc, target);
            }
        } else if (mode === 'replaceSelection') {
            const editor = vscode.window.activeTextEditor;
            if (editor && !editor.selection.isEmpty) {
                await editor.edit((builder) => {
                    builder.replace(editor.selection, output);
                });
            } else {
                await vscode.window.showTextDocument(doc, target);
            }
        } else {
            await vscode.window.showTextDocument(doc, target);
        }

        vscode.window.showInformationMessage(
            `LINVAST Code Migration: migrated ${label} -> Go`
        );
    } catch (err) {
        const message = err instanceof Error ? err.message : String(err);
        vscode.window.showErrorMessage(`LINVAST Code Migration failed: ${message}`);
    }
}

function invokeRunner(runner: ResolvedRunner, code: string): Promise<string> {
    return new Promise<string>((resolve, reject) => {
        const child = execFile(runner.command, runner.args, { maxBuffer: 10 * 1024 * 1024 }, (err: ExecFileException | null, stdout: string, stderr: string) => {
            if (err) {
                const detail = stderr ? `: ${stderr.trim()}` : '';
                reject(new Error(`Failed to run ${runner.label}${detail}`));
                return;
            }
            resolve(stdout);
        });

        child.stdin?.write(code, 'utf8', (e: Error | null | undefined) => {
            if (e) { reject(e); }
            child.stdin?.end();
        });

        child.on('error', (e: Error) => {
            reject(new Error(`Could not launch ${runner.label}: ${e.message}`));
        });
    });
}
