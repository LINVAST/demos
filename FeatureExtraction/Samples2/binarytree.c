typedef struct Node {
    int data;
    struct Node* left;
    struct Node* right;
} Node;

static Node* create(int data) {
    Node* node = (Node*)malloc(sizeof(Node));
    node->data = data;
    node->left = NULL;
    node->right = NULL;
    return node;
}

static Node* insert(Node* root, int data) {
    if (root == NULL) {
        return create(data);
    }
    if (data < root->data) {
        root->left = insert(root->left, data);
    } else {
        root->right = insert(root->right, data);
    }
    return root;
}

static int getdata(Node* node) {
    return node->data;
}

static Node* getleft(Node* node) {
    return node->left;
}

static Node* getright(Node* node) {
    return node->right;
}

static void inorder(Node* root) {
    if (root != NULL) {
        inorder(getleft(root));
        printf("%d\n", getdata(root));
        inorder(getright(root));
    }
}