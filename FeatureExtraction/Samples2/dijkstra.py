def dijkstra(graph: list, start: int) -> list:
    n = len(graph)
    dist = []
    visited = []
    for i in range(n):
        dist.append(2147483647)
        visited.append(False)
    dist[start] = 0

    count = 0
    while count < n:
        u = -1
        for i in range(n):
            if not visited[i]:
                if u == -1 or dist[i] < dist[u]:
                    u = i
        if u == -1:
            break
        visited[u] = True
        for v in range(n):
            if graph[u][v] != 0 and not visited[v]:
                alt = dist[u] + graph[u][v]
                if alt < dist[v]:
                    dist[v] = alt
        count = count + 1
    return dist