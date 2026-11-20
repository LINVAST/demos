package main

func swap(arr []int, i int, j int) {
	temp := arr[i]
	arr[i] = arr[j]
	arr[j] = temp
}

func bubblesort(arr []int) {
	n := len(arr)
	i := 1
	swapped := true
	for i < n && swapped {
		swapped = false
		for j := 1; j <= n-i; j++ {
			if arr[j] > arr[j+1] {
				swap(arr, j, j+1)
				swapped = true
			}
		}
		i = i + 1
	}
}