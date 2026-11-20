def py_max_consecutive_ones(arr):
    best = 0
    cur = 0
    for v in arr:
        if v == 1:
            cur = cur + 1
        else:
            cur = 0
        if cur > best:
            best = cur
    return best
