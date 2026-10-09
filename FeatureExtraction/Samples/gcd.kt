// Euclidean GCD algorithm (arithmetic-only, compatible with Kotlin AST builder limitations)
fun gcd(a: Int, b: Int): Int {
    var x: Int = a
    var y: Int = b
    while (x != y) {
        if (x > y) {
            x = x - y
        } else {
            y = y - x
        }
    }
    return x
}
