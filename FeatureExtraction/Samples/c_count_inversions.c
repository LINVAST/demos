static int c_count_inversions(int arr[], int n) {
    int count = 0;
    int i = 0;
    while (i < n) {
        int j = i + 1;
        while (j < n) {
            if (arr[i] > arr[j]) {
                count = count + 1;
            }
            j = j + 1;
        }
        i = i + 1;
    }
    return count;
}
