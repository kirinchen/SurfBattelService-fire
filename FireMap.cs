using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace RFNEet.firebase {
    public abstract class FireMap<T> : Map<string,T> {
        private DatabaseReference dbRef;
        public FireMap(DatabaseReference dbRef) {
            this.dbRef = dbRef;
        }

        private void onChildAdded(object o , ChildChangedEventArgs ea) {
            if (string.IsNullOrEmpty(ea.PreviousChildName)) return;
            this.Add(ea.PreviousChildName, genChild(ea.PreviousChildName,ea.Snapshot));
        }

        internal void injectData(DataSnapshot snapshot) {
            foreach (DataSnapshot ds in snapshot.Children) {
                T t = genChild(ds.Key, ds);
            }
            dbRef.ChildAdded += onChildAdded;
        }


        internal abstract T genChild(string previousChildName, DataSnapshot snapshot);
    }
}
