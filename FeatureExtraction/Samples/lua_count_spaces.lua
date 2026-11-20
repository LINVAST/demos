local function lua_count_spaces(s)
    local n = #s
    local count = 0
    local i = 1
    while i <= n do
        if string.sub(s, i, i) == " " then
            count = count + 1
        end
        i = i + 1
    end
    return count
end
