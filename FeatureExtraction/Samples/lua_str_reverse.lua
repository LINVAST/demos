local function lua_str_reverse(s)
    local n = #s
    local out = ""
    local i = n
    while i >= 1 do
        out = out .. string.sub(s, i, i)
        i = i - 1
    end
    return out
end
