local function lua_mid_char(s)
    local n = #s
    if n == 0 then
        return ""
    end
    local mid = math.floor((n + 1) / 2)
    return string.sub(s, mid, mid)
end
