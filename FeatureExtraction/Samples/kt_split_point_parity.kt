fun kt_split_point_parity(arr: Array<Int>): Int {
    var evenSum = 0
    var oddSum = 0
    for (i in arr.indices) {
        if (i % 2 == 0) {
            evenSum = evenSum + arr[i]
        } else {
            oddSum = oddSum + arr[i]
        }
    }
    if (evenSum > oddSum) {
        return 1
    }
    if (oddSum > evenSum) {
        return -1
    }
    return 0
}
