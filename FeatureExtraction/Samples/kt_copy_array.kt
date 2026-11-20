fun kt_copy_array(src: Array<Int>, dst: Array<Int>) {
    for (i in src.indices) {
        dst[i] = src[i]
    }
}
