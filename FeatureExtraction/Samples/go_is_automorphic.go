package main

func go_is_automorphic(n int) bool {
    square := n * n
    for n > 0 {
        if n % 10 != square % 10 {
            return false
        }
        n = n / 10
        square = square / 10
    }
    return true
}
