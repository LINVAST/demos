local function lua_is_all_lower(s)
    local n = #s
    local i = 1
    while i <= n do
        local ch = string.sub(s, i, i)
        if ch < "a" or ch > "z" then
            return false
        end
        i = i + 1
    end
    return true
end
