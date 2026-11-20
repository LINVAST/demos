package main

func power(base int, exp int) int {
	result := 1
	b := base
	e := exp
	for e > 0 {
		if e%2 == 1 {
			result = result * b
		}
		e = e / 2
		b = b * b
	}
	return result
}