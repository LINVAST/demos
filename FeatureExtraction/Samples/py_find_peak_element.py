def py_find_peak_element(arr):
    n = len(arr)
    for i in range(n):
        left_ok = (i == 0) or (arr[i] > arr[i - 1])
        right_ok = (i == n - 1) or (arr[i] > arr[i + 1])
        if left_ok and right_ok:
            return i
    return -1
