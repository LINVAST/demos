static void c_rotate_left(int arr[], int n) {
    if (n <= 1) {
        return;
    }
    int first = arr[0];
    int i = 0;
    while (i < n - 1) {
        arr[i] = arr[i + 1];
        i = i + 1;
    }
    arr[n - 1] = first;
}
