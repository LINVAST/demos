package main

func go_is_armstrong(n int) bool {
    original := n
    sum := 0
    for n > 0 {
        d := n % 10
        sum = sum + d * d * d
        n = n / 10
    }
    return sum == original
}
