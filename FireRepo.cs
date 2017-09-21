using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

namespace RFNEet.firebase {
    public class FireRepo : MonoBehaviour {

        public readonly string keyTag = "tag";

        public class MyMap<T> : Map<string, T> { }

        private DatabaseReference dataFire;
        private MyMap<MyMap<FireNode>> map = new MyMap<MyMap<FireNode>>();

        internal void initFire(string roomId, Action initAct) {
            dataFire = FirebaseDatabase.DefaultInstance.GetReference(FireConfig.getInstance().rootNode).Child(roomId);
            dataFire.ValueChanged += (s, e) => {
                Handler h = GetComponentInChildren<Handler>();
                if (h == null) return;
                foreach (DataSnapshot ds in e.Snapshot.Children) {
                    foreach (DataSnapshot dds in ds.Children) {
                        setupObject(ds.Key, dds, h);
                    }
                }
                initAct();
            };
        }

        private void setupObject(string pid, DataSnapshot dds, Handler h) {
            string json = dds.GetRawJsonValue();
            RemoteData rd = JsonConvert.DeserializeObject<RemoteData>(json);
            rd.setSource(json);
            h.onDataInit(pid, dds.Key, rd);

        }

        internal FireNode get(string pid, string sid) {
            MyMap<FireNode> fm = map.findThanSet(pid, new MyMap<FireNode>());
            return fm.findThanSet(sid, new FireNode(pid, sid, dataFire));
        }

        public interface Handler {

            void onDataInit(string pid, string oid, RemoteData v);
        }


    }
}