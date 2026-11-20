// Dijkstra shortest path algorithm
fun dijkstra(graph: Array<Array<Int>>, start: Int): Array<Int> {
    val n: Int = graph.size
    val dist: Array<Int> = Array(n) { 0 }
    val visited: Array<Boolean> = Array(n) { false }
    var i: Int = 0
    while (i < n) {
        dist[i] = 2147483647
        i = i + 1
    }
    dist[start] = 0
    var count: Int = 0
    while (count < n) {
        var u: Int = -1
        i = 0
        while (i < n) {
            if (!visited[i]) {
                if (u == -1 || dist[i] < dist[u]) {
                    u = i
                }
            }
            i = i + 1
        }
        if (u == -1) {
            break
        }
        visited[u] = true
        var v: Int = 0
        while (v < n) {
            if (graph[u][v] != 0 && !visited[v]) {
                val alt: Int = dist[u] + graph[u][v]
                if (alt < dist[v]) {
                    dist[v] = alt
                }
            }
            v = v + 1
        }
        count = count + 1
    }
    return dist
}