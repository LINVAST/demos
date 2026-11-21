def py_find_disappeared(arr):
    count = 0
    for x in range(1, len(arr) + 1):
        found = False
        for v in arr:
            if v == x:
                found = True
        if not found:
            count = count + 1
    return count


