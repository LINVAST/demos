local function lua_count_digits(s)
    local n = #s
    local count = 0
    local i = 1
    while i <= n do
        local ch = string.sub(s, i, i)
        if ch >= "0" and ch <= "9" then
            count = count + 1
        end
        i = i + 1
    end
    return count
end
