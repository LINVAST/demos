package main

type Node struct {
	data  int
	left  *Node
	right *Node
}

func insert(root *Node, data int) *Node {
	if root == nil {
		return &Node{data: data}
	}
	if data < root.data {
		root.left = insert(root.left, data)
	} else {
		root.right = insert(root.right, data)
	}
	return root
}

func inorder(root *Node) {
	if root != nil {
		inorder(root.left)
		println(root.data)
		inorder(root.right)
	}
}