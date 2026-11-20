static int c_gcd(int a, int b) {
    if (b == 0) {
        return a;
    }
    return c_gcd(b, a % b);
}
