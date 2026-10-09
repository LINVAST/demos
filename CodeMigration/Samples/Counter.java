public class Counter {
    private int count;

    public Counter(int initial) {
        this.count = initial;
    }

    public void increment() {
        this.count = this.count + 1;
    }

    public boolean isPositive() {
        return this.count > 0;
    }

    public int factorial(int n) {
        int result = 1;
        int i = 1;
        while (i <= n) {
            result = result * i;
            i = i + 1;
        }
        return result;
    }

    public int sumTo(int n) {
        int total = 0;
        for (int i = 1; i <= n; i = i + 1) {
            total = total + i;
        }
        return total;
    }

    public String getStatus() {
        if (this.count > 0) {
            return "positive";
        } else {
            return "non-positive";
        }
    }
}
