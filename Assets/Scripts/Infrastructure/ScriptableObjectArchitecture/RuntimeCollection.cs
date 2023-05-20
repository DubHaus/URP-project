using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Infrastructure {
    public abstract class RuntimeCollection<T> : ScriptableObject {
        public List<T> Items = new List<T>();
        
        public Action<T> ItemAdded;
        public Action<T> ItemRemoved;

        public void AddItem(T item) {
            if (!Items.Contains(item)) {
                Items.Add(item);
                ItemAdded?.Invoke(item);
            }
        }

        public void RemoveItem(T item) {
            if (Items.Contains(item)) {
                Items.Remove(item);
                ItemRemoved?.Invoke(item);
            }
        }

    }
}