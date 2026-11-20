package main

func go_is_strong_number(n int) bool {
    original := n
    sum := 0
    for n > 0 {
        d := n % 10
        fact := 1
        for k := 2; k <= d; k++ {
            fact = fact * k
        }
        sum = sum + fact
        n = n / 10
    }
    return sum == original
}
