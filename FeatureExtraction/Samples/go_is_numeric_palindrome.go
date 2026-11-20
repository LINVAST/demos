package main

func go_is_numeric_palindrome(n int) bool {
    if n < 0 {
        return false
    }
    original := n
    rev := 0
    for n > 0 {
        rev = rev * 10 + n % 10
        n = n / 10
    }
    return rev == original
}
