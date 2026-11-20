def py_group_consecutive_count(arr):
    if len(arr) == 0:
        return 0
    groups = 1
    for i in range(1, len(arr)):
        if arr[i] != arr[i - 1]:
            groups = groups + 1
    return groups
