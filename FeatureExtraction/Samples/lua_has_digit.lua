local function lua_has_digit(s)
    local n = #s
    local i = 1
    while i <= n do
        local ch = string.sub(s, i, i)
        if ch >= "0" and ch <= "9" then
            return true
        end
        i = i + 1
    end
    return false
end
