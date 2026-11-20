def gcd(a: int, b: int) -> int:
    x = a
    y = b
    while x != y:
        if x > y:
            x = x - y
        else:
            y = y - x
    return x