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
        public DBRefenece dataFire;
        private List<Action<RemoteData>> valueChangedListeners = new List<Action<RemoteData>>();


        internal FireNode(string p, string o) {
            pid = p;
            oid = o;
            dataFire = FirebaseManager.getDBRef().Child(pid).Child(oid);
            dataFire.ValueChanged += onValueChanged;
        }

        internal void post(RemoteData o) {
            o.pid = pid;
            o.oid = oid;
            o.sid = FirebaseManager.getMePid();
            string s = JsonConvert.SerializeObject(o);
            dataFire.SetRawJsonValueAsync(s);
        }

        public void addValueChangedListener(Action<RemoteData> a) {
            valueChangedListeners.Add(a);
        }


        private void onValueChanged(DBResult ea) {
            string s = ea.getRawJsonValue();
            if (string.IsNullOrEmpty(s)) return;
            RemoteData rd = JsonConvert.DeserializeObject<RemoteData>(s);
            rd.setSource(s);
            if (!string.Equals(rd.sid, FirebaseManager.getMePid())) {
                valueChangedListeners.ForEach(l => {
                    l(rd);
                });
            }
        }
    }
}
