package main

func sumofdigits(n int) int {
	sum := 0
	x := n
	for x > 0 {
		sum = sum + x%10
		x = x / 10
	}
	return sum
}