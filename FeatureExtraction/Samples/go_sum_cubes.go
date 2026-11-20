package main

func go_sum_cubes(n int) int {
    sum := 0
    for i := 1; i <= n; i++ {
        sum = sum + i * i * i
    }
    return sum
}
