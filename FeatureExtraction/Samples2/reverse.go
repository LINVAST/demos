package main

func reverse(arr []int) {
	left := 0
	right := len(arr) - 1
	for left < right {
		temp := arr[left]
		arr[left] = arr[right]
		arr[right] = temp
		left = left + 1
		right = right - 1
	}
}