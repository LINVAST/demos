public class BinaryTree {
    static class Node {
        int data;
        Node left;
        Node right;
        Node(int data) { this.data = data; }
    }

    static void inorder(Node root) {
        if (root != null) {
            inorder(root.left);
            System.out.println(root.data);
            inorder(root.right);
        }
    }

    static Node insert(Node root, int data) {
        if (root == null) {
            return new Node(data);
        }
        if (data < root.data) {
            root.left = insert(root.left, data);
        } else {
            root.right = insert(root.right, data);
        }
        return root;
    }

    public static void main(String[] args) {
        Node root = null;
        root = insert(root, 5);
        insert(root, 3);
        insert(root, 7);
        inorder(root);
    }
}
