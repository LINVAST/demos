def py_partition_by_pivot(arr, pivot):
    write = 0
    for i in range(len(arr)):
        if arr[i] < pivot:
            arr[write] = arr[i]
            write = write + 1
    return write
