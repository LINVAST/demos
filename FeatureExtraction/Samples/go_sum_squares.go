package main

func go_sum_squares(n int) int {
    sum := 0
    for i := 1; i <= n; i++ {
        sum = sum + i * i
    }
    return sum
}
