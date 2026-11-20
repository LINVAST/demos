-- Bubble sort algorithm

local function swap(arr, i, j)
    local temp = arr[i]
    arr[i] = arr[j]
    arr[j] = temp
end

local function bubblesort(arr)
    local n = #arr
    local i = 1
    local swapped = true
    while i < n and swapped do
        swapped = false
        local j = 1
        while j <= n - i do
            if arr[j] > arr[j + 1] then
                swap(arr, j, j + 1)
                swapped = true
            end
            j = j + 1
        end
        i = i + 1
    end
end