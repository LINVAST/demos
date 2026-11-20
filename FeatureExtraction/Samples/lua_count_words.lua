local function lua_count_words(s)
    local n = #s
    local count = 0
    local i = 1
    while i <= n do
        if string.sub(s, i, i) == " " then
            count = count + 1
        end
        i = i + 1
    end
    if n == 0 then
        return 0
    end
    return count + 1
end
