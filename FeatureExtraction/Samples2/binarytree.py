class Node:
    def __init__(self, data: int):
        self.data = data
        self.left = None
        self.right = None


def insert(root, data: int):
    if root is None:
        return Node(data)
    if data < root.data:
        root.left = insert(root.left, data)
    else:
        root.right = insert(root.right, data)
    return root


def inorder(root):
    if root is not None:
        inorder(root.left)
        print(root.data)
        inorder(root.right)