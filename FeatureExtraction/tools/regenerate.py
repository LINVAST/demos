#!/usr/bin/env python3
"""Regenerate and verify the demo corpus using the explicitly selected local LINVAST."""
import collections
import csv
import io
import json
from pathlib import Path
import subprocess
import sys
import tempfile

TOOLS = Path(__file__).resolve().parent
ROOT = TOOLS.parent

def run(*args, **kwargs):
    return subprocess.run(args, cwd=ROOT, check=True, **kwargs)

def main():
    run('dotnet', 'build', '-p:UseLocalLinvast=true', '--artifacts-path', str(ROOT / '.artifacts'),
        '-m:1', '-p:UseSharedCompilation=false', '--nologo')
    dll = str(ROOT / '.artifacts/bin/FeatureExtraction/debug/FeatureExtraction.dll')
    run('dotnet', dll, '--self-test')
    run(sys.executable, '-m', 'unittest', 'discover', '-s', str(TOOLS), '-p', 'test_*.py')
    samples = str(ROOT / 'Samples')
    first = run('dotnet', dll, samples, capture_output=True).stdout
    second = run('dotnet', dll, samples, capture_output=True).stdout
    if first != second:
        raise RuntimeError('Extraction is not deterministic')
    rows = list(csv.DictReader(io.StringIO(first.decode())))
    counts = collections.Counter(row['Language'] for row in rows)
    if counts != {language: 20 for language in ['C', 'Java', 'Go', 'Kotlin', 'Lua', 'Python']}:
        raise RuntimeError(f'Unexpected corpus counts: {counts}')
    files = {p.name for p in (ROOT / 'Samples').iterdir() if p.is_file()}
    if len(rows) != 120 or {r['SourceFile'] for r in rows} != files:
        raise RuntimeError('Expected exactly one row per corpus source')
    with tempfile.TemporaryDirectory() as directory:
        stats = Path(directory) / 'statistics.json'
        scores = run(sys.executable, str(TOOLS / 'complexity_scores.py'), '--stats', str(stats), input=first, capture_output=True).stdout
        repeat = run(sys.executable, str(TOOLS / 'complexity_scores.py'), input=second, capture_output=True).stdout
        if scores != repeat:
            raise RuntimeError('Scores are not deterministic')
        revision = run('git', '-C', '../../LINVAST', 'rev-parse', 'HEAD', capture_output=True, text=True).stdout.strip()
        dirty = run('git', '-C', '../../LINVAST', 'status', '--porcelain', capture_output=True, text=True).stdout.strip()
        metadata = json.loads(stats.read_text())
        metadata.update(linvast_revision=revision, linvast_worktree_dirty=bool(dirty),
                        dependency_source='local development checkout; UseLocalLinvast=true')
        (ROOT / 'features.csv').write_bytes(first)
        (ROOT / 'complexity-scores.csv').write_bytes(scores)
        (ROOT / 'score-statistics.json').write_text(json.dumps(metadata, indent=2) + '\n')
    print('Verified and regenerated 120 rows (20 per language), scores, and statistics.')

if __name__ == '__main__':
    main()
