-- Dijkstra shortest path algorithm

local function dijkstra(graph, start)
    local n = #graph
    local dist = {}
    local visited = {}
    local i = 0
    while i < n do
        dist[i] = 2147483647
        visited[i] = false
        i = i + 1
    end
    dist[start] = 0
    local count = 0
    while count < n do
        local u = -1
        i = 0
        while i < n do
            if not visited[i] then
                if u == -1 or dist[i] < dist[u] then
                    u = i
                end
            end
            i = i + 1
        end
        if u == -1 then
            break
        end
        visited[u] = true
        local v = 0
        while v < n do
            if graph[u][v] ~= 0 and not visited[v] then
                local alt = dist[u] + graph[u][v]
                if alt < dist[v] then
                    dist[v] = alt
                end
            end
            v = v + 1
        end
        count = count + 1
    end
    return dist
end