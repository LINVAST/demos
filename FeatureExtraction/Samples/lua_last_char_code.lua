local function lua_last_char_code(s)
    local n = #s
    if n == 0 then
        return -1
    end
    return string.byte(s, n)
end
