// Fast exponentiation algorithm
fun power(base: Int, exp: Int): Int {
    var result: Int = 1
    var b: Int = base
    var e: Int = exp
    while (e > 0) {
        if (e % 2 == 1) {
            result = result * b
        }
        e = e / 2
        b = b * b
    }
    return result
}