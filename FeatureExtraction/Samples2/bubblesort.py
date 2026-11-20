def swap(arr, i, j):
    temp = arr[i]
    arr[i] = arr[j]
    arr[j] = temp


def bubblesort(arr):
    n = len(arr)
    i = 1
    swapped = True
    while i < n and swapped:
        swapped = False
        j = 1
        while j <= n - i:
            if arr[j - 1] > arr[j]:
                swap(arr, j - 1, j)
                swapped = True
            j = j + 1
        i = i + 1