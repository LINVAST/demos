static int sumofdigits(int n) {
    int sum = 0;
    int x = n;
    while (x > 0) {
        sum = sum + x % 10;
        x = x / 10;
    }
    return sum;
}