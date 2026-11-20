-- Quicksort algorithm

local function swap(arr, i, j)
    local temp = arr[i]
    arr[i] = arr[j]
    arr[j] = temp
end

local function partition(arr, low, high)
    local pivot = arr[high]
    local i = low - 1
    local j = low
    while j <= high - 1 do
        if arr[j] < pivot then
            i = i + 1
            swap(arr, i, j)
        end
        j = j + 1
    end
    swap(arr, i + 1, high)
    return i + 1
end

local function quicksort(arr, low, high)
    if low < high then
        local pi = partition(arr, low, high)
        quicksort(arr, low, pi - 1)
        quicksort(arr, pi + 1, high)
    end
end