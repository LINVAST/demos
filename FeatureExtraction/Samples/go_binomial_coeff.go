package main

func go_binomial_coeff(n int, k int) int {
    if k == 0 || k == n {
        return 1
    }
    return go_binomial_coeff(n - 1, k - 1) + go_binomial_coeff(n - 1, k)
}
