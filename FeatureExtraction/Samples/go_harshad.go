package main

func go_harshad(n int) bool {
    original := n
    sum := 0
    for n > 0 {
        sum = sum + n % 10
        n = n / 10
    }
    return original % sum == 0
}
