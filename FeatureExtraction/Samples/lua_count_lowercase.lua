local function lua_count_lowercase(s)
    local n = #s
    local count = 0
    local i = 1
    while i <= n do
        local ch = string.sub(s, i, i)
        if ch >= "a" and ch <= "z" then
            count = count + 1
        end
        i = i + 1
    end
    return count
end
