def pascal(rows: int) -> list:
    triangle = []
    i = 1
    while i <= rows:
        row = []
        j = 1
        while j <= i:
            if j == 1 or j == i:
                row.append(1)
            else:
                prev = triangle[i - 2]
                row.append(prev[j - 2] + prev[j - 1])
            j = j + 1
        triangle.append(row)
        i = i + 1
    return triangle[rows - 1]