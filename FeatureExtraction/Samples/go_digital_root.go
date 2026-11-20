package main

func go_digital_root(n int) int {
    for n >= 10 {
        sum := 0
        m := n
        for m > 0 {
            sum = sum + m % 10
            m = m / 10
        }
        n = sum
    }
    return n
}
