def py_count_frequency_mode(arr):
    best_val = arr[0]
    best_count = 0
    for i in range(len(arr)):
        count = 0
        for j in range(len(arr)):
            if arr[j] == arr[i]:
                count = count + 1
        if count > best_count:
            best_count = count
            best_val = arr[i]
    return best_val


