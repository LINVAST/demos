#!/usr/bin/env python3
import csv
import sys

WEIGHTS = {
    "CyclomaticComplexity": 0.40,
    "MaxDepth": 0.25,
    "TotalNodes": 0.20,
    "FuncCallCount": 0.10,
    "ArrAccessExprCount": 0.05,
}


def main():
    reader = csv.DictReader(sys.stdin)
    required = {"FunctionName", "SourceFile", *WEIGHTS}
    missing = required - set(reader.fieldnames or [])
    if missing:
        sys.exit(f"Missing CSV column(s): {', '.join(sorted(missing))}")

    rows = list(reader)
    if not rows:
        sys.exit("No function rows found in the input.")

    # Read feature values as numbers.
    for row in rows:
        for feature in WEIGHTS:
            try:
                row[feature] = float(row[feature])
            except (TypeError, ValueError):
                sys.exit(
                    f"Invalid value for {feature} in "
                    f"{row.get('SourceFile', '?')}:{row.get('FunctionName', '?')}"
                )

    # Calculate the mean and population standard deviation for each feature.
    stats = {}
    for feature in WEIGHTS:
        values = [row[feature] for row in rows]
        mean = sum(values) / len(values)
        variance = sum((value - mean) ** 2 for value in values) / len(values)
        stats[feature] = (mean, variance ** 0.5)

    # Calculate the weighted composite score for each function.
    for row in rows:
        score = 0.0
        for feature, weight in WEIGHTS.items():
            mean, stddev = stats[feature]
            z_score = (row[feature] - mean) / stddev if stddev else 0.0
            score += weight * z_score
        row["Score"] = score

    rows.sort(key=lambda row: row["Score"], reverse=True)

    writer = csv.writer(sys.stdout)
    writer.writerow(["Rank", "FunctionName", "SourceFile",
                     "Score", "CyclomaticComplexity"])
    for rank, row in enumerate(rows, start=1):
        writer.writerow([
            rank,
            row["FunctionName"],
            row["SourceFile"],
            f"{row['Score']:.3f}",
            int(row["CyclomaticComplexity"]),
        ])


if __name__ == "__main__":
    main()

