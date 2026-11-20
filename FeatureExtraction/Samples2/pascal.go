package main

func pascal(rows int) []int {
	var triangle [][]int
	for i := 1; i <= rows; i++ {
		var row []int
		for j := 1; j <= i; j++ {
			if j == 1 || j == i {
				row = append(row, 1)
			} else {
				prev := triangle[i-1]
				row = append(row, prev[j-2]+prev[j-1])
			}
		}
		triangle = append(triangle, row)
	}
	return triangle[rows-1]
}