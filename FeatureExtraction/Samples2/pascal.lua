-- Pascal triangle algorithm

local function pascal(rows)
    local triangle = {}
    local i = 1
    while i <= rows do
        local row = {}
        local j = 1
        while j <= i do
            if j == 1 or j == i then
                row[j] = 1
            else
                local prev = triangle[i - 1]
                row[j] = prev[j - 1] + prev[j]
            end
            j = j + 1
        end
        triangle[i] = row
        i = i + 1
    end
    return triangle[rows]
end