package main

func go_lcm(a int, b int) int {
    x := a
    y := b
    for y != 0 {
        temp := x % y
        x = y
        y = temp
    }
    gcd := x
    return a * b / gcd
}
