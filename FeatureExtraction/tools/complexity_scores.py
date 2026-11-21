#!/usr/bin/env python3
import argparse
import csv
import sys
import json
import math

DEFAULT_WEIGHTS = {
    "CyclomaticComplexity": 0.40,
    "MaxDepth": 0.25,
    "TotalNodes": 0.20,
    "FuncCallCount": 0.10,
    "ArrAccessExprCount": 0.05,
}


def parse_weight(value):
    if "=" not in value:
        sys.exit(f"Invalid --weight value '{value}': expected FEATURE=WEIGHT")
    feature, _, weight_text = value.partition("=")
    feature = feature.strip()
    weight_text = weight_text.strip()
    try:
        weight = float(weight_text)
    except ValueError:
        sys.exit(f"Invalid weight for '{feature}': '{weight_text}' is not a number")
    if not math.isfinite(weight) or weight < 0:
        sys.exit("Weights must be finite and nonnegative")
    return feature, weight


def main():
    parser = argparse.ArgumentParser(
        description="Compute illustrative composite structural scores from FeatureExtraction CSV."
    )
    parser.add_argument(
        "--weight",
        action="append",
        metavar="FEATURE=WEIGHT",
        help="Override the weight for a feature (repeatable). Defaults: "
        + ", ".join(f"{k}={v}" for k, v in DEFAULT_WEIGHTS.items()),
    )
    parser.add_argument("--stats", help="Write normalization statistics as JSON to this path.")
    args = parser.parse_args()

    weights = dict(DEFAULT_WEIGHTS)
    if args.weight:
        for spec in args.weight:
            feature, weight = parse_weight(spec)
            weights[feature] = weight

    reader = csv.DictReader(sys.stdin)
    fieldnames = reader.fieldnames or []
    required = {"FunctionName", "SourceFile", *weights}
    missing = required - set(fieldnames)
    if missing:
        sys.exit(f"Missing CSV column(s): {', '.join(sorted(missing))}")

    rows = list(reader)
    if not rows:
        sys.exit("No function rows found in the input.")

    # Read feature values as numbers.
    for row in rows:
        for feature in weights:
            try:
                row[feature] = float(row[feature])
                if not math.isfinite(row[feature]):
                    raise ValueError("nonfinite value")
            except (TypeError, ValueError):
                sys.exit(
                    f"Invalid value for {feature} in "
                    f"{row.get('SourceFile', '?')}:{row.get('FunctionName', '?')}"
                )

    # Calculate the mean and population standard deviation for each feature.
    stats = {}
    for feature in weights:
        values = [row[feature] for row in rows]
        mean = sum(values) / len(values)
        variance = sum((value - mean) ** 2 for value in values) / len(values)
        stats[feature] = (mean, variance ** 0.5)

    # Calculate the weighted composite score for each function.
    for row in rows:
        score = 0.0
        for feature, weight in weights.items():
            mean, stddev = stats[feature]
            z_score = (row[feature] - mean) / stddev if stddev else 0.0
            score += weight * z_score
        row["Score"] = score

    rows.sort(key=lambda row: (-row["Score"], row["SourceFile"], row["FunctionName"]))
    if args.stats:
        with open(args.stats, "w", encoding="utf-8") as output:
            json.dump({"description": "Illustrative composite structural score",
                       "row_count": len(rows), "ddof": 0,
                       "zero_variance": "zero z-score contribution",
                       "features": {f: {"weight": w, "mean": stats[f][0],
                                        "population_stddev": stats[f][1]}
                                    for f, w in weights.items()}}, output, indent=2)
            output.write("\n")

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
