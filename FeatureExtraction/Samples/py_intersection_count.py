def py_intersection_count(a, b):
    count = 0
    for x in a:
        for y in b:
            if x == y:
                count = count + 1
    return count


