static int c_kadane_max_subarray(int arr[], int n) {
    int best = arr[0];
    int cur = arr[0];
    int i = 1;
    while (i < n) {
        int v = arr[i];
        int cand = cur + v;
        cur = (v > cand) ? v : cand;
        if (cur > best) {
            best = cur;
        }
        i = i + 1;
    }
    return best;
}
