package main

func go_digit_sum(n int) int {
    sum := 0
    for n > 0 {
        sum = sum + n % 10
        n = n / 10
    }
    return sum
}
