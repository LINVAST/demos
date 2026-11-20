package main

func go_count_even_digits(n int) int {
    count := 0
    for n > 0 {
        if n % 10 % 2 == 0 {
            count = count + 1
        }
        n = n / 10
    }
    return count
}
