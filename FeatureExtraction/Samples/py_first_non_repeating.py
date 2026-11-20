def py_first_non_repeating(arr):
    for i in range(len(arr)):
        unique = True
        for j in range(len(arr)):
            if i != j and arr[j] == arr[i]:
                unique = False
        if unique:
            return arr[i]
    return -1
