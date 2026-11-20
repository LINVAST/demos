static int c_linear_search(int arr[], int n, int value) {
    int i = 0;
    while (i < n) {
        if (arr[i] == value) {
            return i;
        }
        i = i + 1;
    }
    return -1;
}
