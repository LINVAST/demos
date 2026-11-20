local function lua_first_char_code(s)
    if #s == 0 then
        return -1
    end
    return string.byte(s, 1)
end
