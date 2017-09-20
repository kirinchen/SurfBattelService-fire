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

        internal FireNode(string p, string o, DatabaseReference rf) {
            pid = p;
            oid = o;
            dataFire = rf.Child(pid).Child(oid);

        }

        internal void post(RemoteData o) {
            string s = JsonConvert.SerializeObject(o);
            dataFire.SetRawJsonValueAsync(s);
        }
    }
}
