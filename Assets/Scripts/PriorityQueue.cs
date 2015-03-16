using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class PriorityQueue<T> {
    /*This priority queue uses Binary Heap. Google it for more info*/
    //Reminder: queue[0] is always default(T) and keep it that way
    public List<Eppy.Tuple<T, float>> queue;
    public PriorityQueue() {
        this.queue = new List<Eppy.Tuple<T, float>>();
        //T nul = null;
        this.queue.Add(default(Eppy.Tuple<T, float>));
    }
    public void insert(T obj, float value) {
        Eppy.Tuple<T, float> n = new Eppy.Tuple<T, float>(obj, value);
        if (queue.Count > 1) {
            queue.Add(default(Eppy.Tuple<T, float>));
            int pos = queue.Count - 1;
            while (pos > 1) {
                if (n.Item2 < queue[pos/2].Item2) {
                    queue[pos] = queue[pos/2];
                } else {
                    break;
                }
            }
            queue[pos] = n;
        } else {
            queue.Add(n);
        }
    }

    public T get() {
        T ret;
        if (queue.Count == 1) {
            return default(T);
        }
        if (queue.Count == 2) {
            ret = queue[1].Item1;
            queue.RemoveAt(1);
            return ret;
        }
        ret = queue[1].Item1;
        Eppy.Tuple<T, float> last_element = queue[queue.Count - 1];
        queue.RemoveAt(queue.Count - 1);
        queue[1] = default(Eppy.Tuple<T, float>);
        int pos = 1;
        if(queue.Count == 2){
            queue[1] = last_element;
        } else if (queue.Count == 3){
            if (last_element.Item2 > queue[2].Item2) {
                queue[1] = queue[2];
                queue[2] = last_element;
            } else {
                queue[1] = last_element;
            }
        } else {
            while (pos * 2 < queue.Count - 1) {
                if (queue[pos * 2].Item2 < queue[pos * 2 + 1].Item2) {
                    queue[pos] = queue[pos * 2];
                    pos = pos * 2;
                } else {
                    queue[pos] = queue[pos * 2 + 1];
                    pos = pos * 2 + 1;
                }    
            }
            queue[pos] = last_element;
        }
        return ret;
    }

    public int Count() {
        return queue.Count - 1;
    }
    public bool isEmpty() {
        return queue.Count < 1;
    }
    public void print() {
        //foreach (Eppy.Tuple<T, float> obj in queue) {
        //    Debug.Log(obj.Item1 + " " + obj.Item2);
        //}
        for (int i = 1; i < queue.Count; i++) {
            Debug.Log(queue[i].Item1 + " "  + queue[i].Item2);
        }
            
    }
}
