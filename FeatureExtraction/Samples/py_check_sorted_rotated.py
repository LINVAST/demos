def py_check_sorted_rotated(arr):
    drops = 0
    for i in range(len(arr)):
        if arr[i] > arr[(i + 1) % len(arr)]:
            drops = drops + 1
    return drops <= 1


