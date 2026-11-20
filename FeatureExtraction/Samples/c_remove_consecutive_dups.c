static int c_remove_consecutive_dups(int arr[], int n) {
    if (n == 0) {
        return 0;
    }
    int write = 1;
    int i = 1;
    while (i < n) {
        if (arr[i] != arr[write - 1]) {
            arr[write] = arr[i];
            write = write + 1;
        }
        i = i + 1;
    }
    return write;
}
