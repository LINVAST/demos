package main

func gcd(a int, b int) int {
	x := a
	y := b
	for x != y {
		if x > y {
			x = x - y
		} else {
			y = y - x
		}
	}
	return x
}