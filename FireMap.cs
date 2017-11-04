﻿using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RFNEet.firebase {
    public abstract class FireMap<T> : Map<string, T> {
        private DBRefenece dbRef;
        public FireMap(DBRefenece dbRef) {
            this.dbRef = dbRef;
        }

        private void onChildAdded(DBResult ea) {
            if (this.ContainsKey(ea.key())) return;
            this.Add(ea.key(), genChild(ea));
        }

        internal void injectData(DBResult snapshot) {
            foreach (DBResult ds in snapshot.children()) {
                T t = genChild(ds);
            }
            dbRef.childAdded += onChildAdded;
            dbRef.childRemoved += onChildRemoved;
        }

        private void onChildRemoved(DBResult ea) {
            if (!this.ContainsKey(ea.key())) return;
            T t = this[ea.key()];
            onChildRemoved(ea.key(), t);
            this.Remove(ea.key());
        }

        internal virtual void removeMe() {
            dbRef.childAdded -= onChildAdded;
            dbRef.childRemoved -= onChildRemoved;
        } 

        internal abstract void onChildRemoved(string v,T t);
        internal abstract T genChild(DBResult snapshot);
    }
}
