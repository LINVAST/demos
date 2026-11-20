static int c_sum_of_digits(int n) {
    int total = 0;
    while (n > 0) {
        total = total + n % 10;
        n = n / 10;
    }
    return total;
}
