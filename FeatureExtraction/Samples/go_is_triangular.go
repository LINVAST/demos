package main

func go_is_triangular(n int) bool {
    sum := 0
    for i := 1; sum < n; i++ {
        sum = sum + i
    }
    return sum == n
}
