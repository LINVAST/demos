fun kt_rank_of_element(arr: Array<Int>, value: Int): Int {
    var rank = 1
    for (v in arr) {
        if (v < value) {
            rank = rank + 1
        }
    }
    return rank
}
