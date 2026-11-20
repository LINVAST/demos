// Sum of digits algorithm
fun sumofdigits(n: Int): Int {
    var sum: Int = 0
    var x: Int = n
    while (x > 0) {
        sum = sum + x % 10
        x = x / 10
    }
    return sum
}