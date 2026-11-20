public class gcd {
    static int gcd(int a, int b) {
        int x = a;
        int y = b;
        while (x != y) {
            if (x > y) {
                x = x - y;
            } else {
                y = y - x;
            }
        }
        return x;
    }
}