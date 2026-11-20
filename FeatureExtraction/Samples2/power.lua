-- Fast exponentiation algorithm

local function power(base, exp)
    local result = 1
    local b = base
    local e = exp
    while e > 0 do
        if e % 2 == 1 then
            result = result * b
        end
        e = math.floor(e / 2)
        b = b * b
    end
    return result
end