def py_find_missing_number(arr):
    n = len(arr)
    expected = n * (n + 1) // 2
    actual = 0
    for v in arr:
        actual = actual + v
    return expected - actual


