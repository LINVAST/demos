public class power {
    static int power(int base, int exp) {
        int result = 1;
        int b = base;
        int e = exp;
        while (e > 0) {
            if (e % 2 == 1) {
                result = result * b;
            }
            e = e / 2;
            b = b * b;
        }
        return result;
    }
}