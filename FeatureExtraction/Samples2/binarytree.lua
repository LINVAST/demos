-- Binary tree algorithm

local function insert(root, data)
    if root == nil then
        return { data = data, left = nil, right = nil }
    end
    if data < root.data then
        root.left = insert(root.left, data)
    else
        root.right = insert(root.right, data)
    end
    return root
end

local function inorder(root)
    if root ~= nil then
        inorder(root.left)
        print(root.data)
        inorder(root.right)
    end
end