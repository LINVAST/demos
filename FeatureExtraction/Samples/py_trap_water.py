def py_trap_water(arr):
    total = 0
    for i in range(len(arr)):
        max_left = 0
        for j in range(i):
            if arr[j] > max_left:
                max_left = arr[j]
        max_right = 0
        for j in range(i + 1, len(arr)):
            if arr[j] > max_right:
                max_right = arr[j]
        level = max_left
        if max_right < level:
            level = max_right
        if level > arr[i]:
            total = total + level - arr[i]
    return total


