public class dijkstra {
    static int[] dijkstra(int[][] graph, int start) {
        int n = graph.length;
        int[] dist = new int[n];
        boolean[] visited = new boolean[n];
        for (int i = 0; i < n; i++) {
            dist[i] = 2147483647;
        }
        dist[start] = 0;

        for (int count = 0; count < n; count++) {
            int u = -1;
            for (int i = 0; i < n; i++) {
                if (!visited[i]) {
                    if (u == -1 || dist[i] < dist[u]) {
                        u = i;
                    }
                }
            }
            if (u == -1) {
                break;
            }
            visited[u] = true;
            for (int v = 0; v < n; v++) {
                if (graph[u][v] != 0 && !visited[v]) {
                    int alt = dist[u] + graph[u][v];
                    if (alt < dist[v]) {
                        dist[v] = alt;
                    }
                }
            }
        }
        return dist;
    }
}