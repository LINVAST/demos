static void dijkstra(int graph[10][10], int n, int start, int dist[]) {
    int visited[10];
    int i = 0;
    while (i < n) {
        dist[i] = 2147483647;
        visited[i] = 0;
        i = i + 1;
    }
    dist[start] = 0;
    int count = 0;
    while (count < n) {
        int u = -1;
        i = 0;
        while (i < n) {
            if (!visited[i]) {
                if (u == -1 || dist[i] < dist[u]) {
                    u = i;
                }
            }
            i = i + 1;
        }
        if (u == -1) {
            break;
        }
        visited[u] = 1;
        int v = 0;
        while (v < n) {
            if (graph[u][v] != 0 && !visited[v]) {
                int alt = dist[u] + graph[u][v];
                if (alt < dist[v]) {
                    dist[v] = alt;
                }
            }
            v = v + 1;
        }
        count = count + 1;
    }
}