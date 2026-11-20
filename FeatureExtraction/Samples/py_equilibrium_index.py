def py_equilibrium_index(arr):
    total = 0
    for v in arr:
        total = total + v
    left = 0
    for i in range(len(arr)):
        right = total - left - arr[i]
        if left == right:
            return i
        left = left + arr[i]
    return -1
