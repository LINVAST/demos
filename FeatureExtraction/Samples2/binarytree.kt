// Binary tree algorithm
class Node(var data: Int) {
    var left: Node? = null
    var right: Node? = null
}

fun insert(root: Node?, data: Int): Node {
    if (root == null) {
        return Node(data)
    }
    if (data < root.data) {
        root.left = insert(root.left, data)
    } else {
        root.right = insert(root.right, data)
    }
    return root
}

fun inorder(root: Node?) {
    if (root != null) {
        inorder(root.left)
        println(root.data)
        inorder(root.right)
    }
}