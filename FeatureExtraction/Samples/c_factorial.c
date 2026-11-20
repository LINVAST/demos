static int c_factorial(int n) {
    if (n <= 1) {
        return 1;
    }
    return n * c_factorial(n - 1);
}
