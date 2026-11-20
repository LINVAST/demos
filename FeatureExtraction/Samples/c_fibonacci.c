static int c_fibonacci(int n) {
    if (n <= 1) {
        return n;
    }
    return c_fibonacci(n - 1) + c_fibonacci(n - 2);
}
