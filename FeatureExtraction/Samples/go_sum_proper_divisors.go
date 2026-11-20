package main

func go_sum_proper_divisors(n int) int {
    sum := 0
    for i := 1; i < n; i++ {
        if n % i == 0 {
            sum = sum + i
        }
    }
    return sum
}
