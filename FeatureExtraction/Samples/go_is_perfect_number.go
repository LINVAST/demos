package main

func go_is_perfect_number(n int) bool {
    if n <= 1 {
        return false
    }
    sum := 0
    for i := 1; i < n; i++ {
        if n % i == 0 {
            sum = sum + i
        }
    }
    return sum == n
}
