-- Prime test algorithm

local function isprime(n)
    if n < 2 then
        return false
    end
    local i = 2
    while i * i <= n do
        if n % i == 0 then
            return false
        end
        i = i + 1
    end
    return true
end