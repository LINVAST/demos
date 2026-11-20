local function lua_count_consonants(s)
    local n = #s
    local count = 0
    local i = 1
    while i <= n do
        local ch = string.sub(s, i, i)
        if ((ch >= "a" and ch <= "z") and ch ~= "a" and ch ~= "e" and ch ~= "i" and ch ~= "o" and ch ~= "u") then
            count = count + 1
        end
        i = i + 1
    end
    return count
end
