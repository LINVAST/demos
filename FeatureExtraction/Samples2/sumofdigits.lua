-- Sum of digits algorithm

local function sumofdigits(n)
    local sum = 0
    local x = n
    while x > 0 do
        sum = sum + x % 10
        x = math.floor(x / 10)
    end
    return sum
end