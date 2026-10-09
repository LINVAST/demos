import java.util.HashMap;
import java.util.Map;

public class LruCache<K, V> {
    private final int capacity;
    private final Map<K, Node<K, V>> entries = new HashMap<>();

    // Most recently used at the front; least recently used at the back.
    private final Node<K, V> head = new Node<>(null, null);
    private final Node<K, V> tail = new Node<>(null, null);

    public LruCache(int capacity) {
        if (capacity <= 0) {
            throw new IllegalArgumentException("Capacity must be positive.");
        }
        this.capacity = capacity;
        head.next = tail;
        tail.previous = head;
    }

    public V get(K key) {
        Node<K, V> node = entries.get(key);
        if (node == null) {
            return null;
        }
        moveToFront(node);
        return node.value;
    }

    public void put(K key, V value) {
        Node<K, V> existing = entries.get(key);

        if (existing != null) {
            existing.value = value;
            moveToFront(existing);
            return;
        }

        if (entries.size() == capacity) {
            Node<K, V> leastRecentlyUsed = tail.previous;
            remove(leastRecentlyUsed);
            entries.remove(leastRecentlyUsed.key);
        }

        Node<K, V> node = new Node<>(key, value);
        entries.put(key, node);
        addAfterHead(node);
    }

    private void moveToFront(Node<K, V> node) {
        remove(node);
        addAfterHead(node);
    }

    private void addAfterHead(Node<K, V> node) {
        node.next = head.next;
        node.previous = head;
        head.next.previous = node;
        head.next = node;
    }

    private void remove(Node<K, V> node) {
        node.previous.next = node.next;
        node.next.previous = node.previous;
    }

    // Nested class representing one cache entry.
    private static class Node<K, V> {
        private K key;
        private V value;
        private Node<K, V> previous;
        private Node<K, V> next;

        private Node(K key, V value) {
            this.key = key;
            this.value = value;
        }
    }

    public static void main(String[] args) {
        LruCache<String, Integer> cache = new LruCache<>(2);

        cache.put("apples", 3);
        cache.put("oranges", 5);
        System.out.println(cache.get("apples")); // 3 (apples becomes most recent)

        cache.put("bananas", 2); // Evicts oranges
        System.out.println(cache.get("oranges")); // null
        System.out.println(cache.get("bananas")); // 2
    }
}
