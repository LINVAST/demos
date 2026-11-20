def sumofdigits(n: int) -> int:
    total = 0
    x = n
    while x > 0:
        total = total + x % 10
        x = x // 10
    return total