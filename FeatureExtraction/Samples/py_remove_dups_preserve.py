def py_remove_dups_preserve(arr):
    write = 0
    for i in range(len(arr)):
        seen = False
        for j in range(write):
            if arr[j] == arr[i]:
                seen = True
        if not seen:
            arr[write] = arr[i]
            write = write + 1
    return write


