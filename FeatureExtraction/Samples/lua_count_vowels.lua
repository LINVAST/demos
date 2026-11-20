local function lua_count_vowels(s)
    local n = #s
    local count = 0
    local i = 1
    while i <= n do
        local ch = string.sub(s, i, i)
        if ch == "a" or ch == "e" or ch == "i" or ch == "o" or ch == "u" then
            count = count + 1
        end
        i = i + 1
    end
    return count
end
