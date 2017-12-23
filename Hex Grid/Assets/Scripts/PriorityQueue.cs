using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityQueue<T> where T : IComparable<T> {

    // Array Implementation of priority queue
    private T[] queue;
    private const int DEFAULT_SIZE = 10;

    private int index;

    /**
     * Constructor
     */
    public PriorityQueue() {
        queue = new T[DEFAULT_SIZE];
        index = 0;
    }

    public bool IsEmpty() {
        return index == 0;
    }

    public T Peek() {
        if (IsEmpty()) {
            return default(T);
        }
        return queue[0];
    }

    public int Size() {
        return index;
    }

    public void Push(T t) {
        if (index >= queue.Length) {
            T[] bigger = new T[2 * queue.Length];
            for (int i = 0; i < queue.Length; i++) {
                bigger[i] = queue[i];
            }
            queue = bigger;
        }

        queue[index] = t;

        int curr = index;
        int parent = (curr - 1) / 2;
        
        while (curr != parent && parent >= 0) {
            if (queue[parent].CompareTo(queue[curr]) < 0) {
                T temp = queue[curr];
                queue[curr] = queue[parent];
                queue[parent] = temp;
            } else {
                break;
            }

            curr = parent;
            parent = (curr - 1) / 2;
        }

        index++;
    }

    public T Pop() {
        T top = queue[0];
        queue[0] = queue[index - 1];
        index--;

        int curr = 0;
        int child = 2 * curr + 1;

        while (child < index) {
            if (child + 1 < index) {
                if (queue[child].CompareTo(queue[child + 1]) < 0) {
                    child++;
                }
            }

            if (queue[child].CompareTo(queue[curr]) > 0) {
                T temp = queue[curr];
                queue[curr] = queue[child];
                queue[child] = temp;
            } else {
                break;
            }

            curr = child;
            child = 2 * curr + 1;
        }

        return top;
    }

    public void Clear() {
        index = 0;
    }

    public void Print() {
        for (int i = 0; i < index; i++) {
            Debug.Log(queue[i]);
        }
    }

    public static void HeapSort(T[] arr) {
        PriorityQueue<T> heap = new PriorityQueue<T>();

        for (int i = 0; i < arr.Length; i++) {
            heap.Push(arr[i]);
        }

        T[] sorted = new T[arr.Length];

        for (int i = arr.Length - 1; i >= 0; i--) {
            sorted[i] = heap.Pop();
        }

        arr = sorted;
    }
}
