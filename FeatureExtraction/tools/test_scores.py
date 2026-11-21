"""Run with python3 -m unittest discover -s tools -p 'test_*.py'."""
import csv
import io
import json
from pathlib import Path
import subprocess
import sys
import tempfile
import unittest

ROOT = Path(__file__).resolve().parent

class ScoreTests(unittest.TestCase):
    def test_population_constants_and_ties(self):
        columns = ['FunctionName', 'SourceFile', 'CyclomaticComplexity', 'MaxDepth',
                   'TotalNodes', 'FuncCallCount', 'ArrAccessExprCount']
        stream = io.StringIO()
        writer = csv.writer(stream)
        writer.writerow(columns)
        writer.writerows([['b', 'b.c', 1, 4, 10, 0, 0], ['a', 'a.c', 3, 4, 10, 0, 0]])
        with tempfile.TemporaryDirectory() as directory:
            stats = Path(directory) / 'stats.json'
            result = subprocess.run([sys.executable, str(ROOT / 'complexity_scores.py'), '--stats', str(stats)],
                                    input=stream.getvalue(), text=True, capture_output=True, check=True)
            rows = list(csv.DictReader(io.StringIO(result.stdout)))
            self.assertEqual([r['Score'] for r in rows], ['0.400', '-0.400'])
            data = json.loads(stats.read_text())
            self.assertEqual(data['features']['CyclomaticComplexity']['population_stddev'], 1)
            self.assertEqual(data['features']['MaxDepth']['population_stddev'], 0)
            self.assertAlmostEqual(sum(f['weight'] for f in data['features'].values()), 1)
            tied = stream.getvalue().replace('a,a.c,3', 'a,a.c,1')
            result = subprocess.run([sys.executable, str(ROOT / 'complexity_scores.py')],
                                    input=tied, text=True, capture_output=True, check=True)
            self.assertEqual([r['SourceFile'] for r in csv.DictReader(io.StringIO(result.stdout))], ['a.c', 'b.c'])

    def test_nonfinite_input_rejected(self):
        result = subprocess.run([sys.executable, str(ROOT / 'complexity_scores.py'), '--weight', 'TotalNodes=nan'],
                                text=True, capture_output=True)
        self.assertNotEqual(result.returncode, 0)

if __name__ == '__main__':
    unittest.main()
