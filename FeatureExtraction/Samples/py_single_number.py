def py_single_number(arr):
    result = 0
    for v in arr:
        result = result ^ v
    return result


