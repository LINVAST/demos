static int c_majority_element(int arr[], int n) {
    int cand = arr[0];
    int count = 1;
    int i = 1;
    while (i < n) {
        if (count == 0) {
            cand = arr[i];
            count = 1;
        } else if (arr[i] == cand) {
            count = count + 1;
        } else {
            count = count - 1;
        }
        i = i + 1;
    }
    return cand;
}
