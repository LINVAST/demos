-- Greatest common divisor algorithm

local function gcd(a, b)
    local x = a
    local y = b
    while x ~= y do
        if x > y then
            x = x - y
        else
            y = y - x
        end
    end
    return x
end