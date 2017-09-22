using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet.firebase {
    public class Map<K, T> : Dictionary<K, T> {

        public T findThanSet(K k, Func<T> t) {
            if (ContainsKey(k)) {
                return this[k];
            } else {
                Add(k, t());
                return this[k];
            }
        }

    }
}
