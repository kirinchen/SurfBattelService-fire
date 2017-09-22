using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

namespace RFNEet.firebase {
    public class FireNode {

        public string pid;
        public string oid;
        public DatabaseReference dataFire;
        private List<Action<RemoteData>> valueChangedListeners = new List<Action<RemoteData>>();


        internal FireNode(string p, string o, DatabaseReference rf) {
            pid = p;
            oid = o;
            dataFire = rf.Child(pid).Child(oid);
            dataFire.ValueChanged += onValueChanged;
        }

        internal void post(RemoteData o) {
            string s = JsonConvert.SerializeObject(o);
            dataFire.SetRawJsonValueAsync(s);
        }

        public void addValueChangedListener(Action<RemoteData> a) {
            valueChangedListeners.Add(a);
        }


        private void onValueChanged(object sender, ValueChangedEventArgs ea) {
            string s = ea.Snapshot.GetRawJsonValue();
            RemoteData rd = JsonConvert.DeserializeObject<RemoteData>(s);
            rd.setSource(s);
            valueChangedListeners.ForEach(l => {
                l(rd);
            });
        }
    }
}
