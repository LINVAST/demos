package main

func go_is_perfect_square(n int) bool {
    if n < 0 {
        return false
    }
    i := 0
    for i * i <= n {
        if i * i == n {
            return true
        }
        i = i + 1
    }
    return false
}
