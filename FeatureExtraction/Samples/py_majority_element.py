def py_majority_element(arr):
    cand = arr[0]
    count = 1
    for i in range(1, len(arr)):
        if count == 0:
            cand = arr[i]
            count = 1
        elif arr[i] == cand:
            count = count + 1
        else:
            count = count - 1
    return cand


