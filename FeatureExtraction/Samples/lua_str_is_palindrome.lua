local function lua_str_is_palindrome(s)
    local n = #s
    local i = 1
    while i <= n / 2 do
        if string.sub(s, i, i) ~= string.sub(s, n - i + 1, n - i + 1) then
            return false
        end
        i = i + 1
    end
    return true
end
