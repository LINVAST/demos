package main

import "math"

func dijkstra(graph [][]int, start int) []int {
    n := len(graph)
    dist := make([]int, n)
    visited := make([]bool, n)
    for i := 0; i < n; i++ {
        dist[i] = math.MaxInt
    }
    dist[start] = 0

    for count := 0; count < n; count++ {
        u := -1
        for i := 0; i < n; i++ {
            if !visited[i] {
                if u == -1 || dist[i] < dist[u] {
                    u = i
                }
            }
        }
        if u == -1 {
            break
        }
        visited[u] = true
        for v := 0; v < n; v++ {
            if graph[u][v] != 0 && !visited[v] {
                alt := dist[u] + graph[u][v]
                if alt < dist[v] {
                    dist[v] = alt
                }
            }
        }
    }
    return dist
}
