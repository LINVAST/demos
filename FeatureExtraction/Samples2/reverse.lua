-- Reverse array algorithm

local function reverse(arr)
    local left = 1
    local right = #arr
    while left < right do
        local temp = arr[left]
        arr[left] = arr[right]
        arr[right] = temp
        left = left + 1
        right = right - 1
    end
end