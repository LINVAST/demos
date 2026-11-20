local function lua_double_chars(s)
    local n = #s
    local out = ""
    local i = 1
    while i <= n do
        local ch = string.sub(s, i, i)
        out = out .. ch .. ch
        i = i + 1
    end
    return out
end
