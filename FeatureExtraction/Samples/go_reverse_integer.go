package main

func go_reverse_integer(n int) int {
    rev := 0
    for n != 0 {
        rev = rev * 10 + n % 10
        n = n / 10
    }
    return rev
}
