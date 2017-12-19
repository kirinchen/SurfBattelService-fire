using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RFNEet.realtimeDB;

namespace RFNEet.firebase {
    public abstract class FireMap<T> : Map<string, T> {

        private DBRefenece dbRef;
        public FireMap(DBRefenece dbRef) {
            this.dbRef = dbRef;
        }

        private void onChildAdded(DBResult ea) {
            if (ea.key().StartsWith(FireRepo.SKIP_KEY_PREFIX)) return;
            if (this.ContainsKey(ea.key())) return;
            this.Add(ea.key(), genChild(ea));
        }

        internal void injectData(DBResult snapshot) {
            foreach (DBResult ds in snapshot.children()) {
                T t = genChild(ds);
            }
            dbRef.addChildAdded( onChildAdded);
            dbRef.addChildRemoved( onChildRemoved);
        }

        private void onChildRemoved(DBResult ea) {
            if (!this.ContainsKey(ea.key())) return;
            T t = this[ea.key()];
            onChildRemoved(ea.key(), t);
            this.Remove(ea.key());
        }

        internal virtual void removeMe() {
            dbRef.removeChildAdded(onChildAdded);
            dbRef.removeChildRemoved(onChildRemoved);
            dbRef.removeMe();
        }

        internal abstract void onChildRemoved(string v, T t);
        internal abstract T genChild(DBResult snapshot);
    }
}
