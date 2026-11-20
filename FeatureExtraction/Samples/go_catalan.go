package main

func go_catalan(n int) int {
    if n <= 1 {
        return 1
    }
    res := 0
    for i := 0; i < n; i++ {
        res = res + go_catalan(i) * go_catalan(n - 1 - i)
    }
    return res
}
