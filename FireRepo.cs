using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

namespace RFNEet.firebase {
    public class FireRepo : MonoBehaviour {

        public readonly string keyTag = "tag";

        public class Map<T> : Dictionary<string, T> {

            public T findThanSet(string k, T t) {
                if (ContainsKey(k)) {
                    return this[k];
                } else {
                    Add(k, t);
                    return this[k];
                }
            }

        }

        private DatabaseReference dataFire;
        private Map<Map<FireNode>> map = new Map<Map<FireNode>>();

        internal void initFire(string roomId) {
            dataFire = FirebaseDatabase.DefaultInstance.GetReference("/data").Child(roomId);
            Handler h = GetComponentInChildren<Handler>();
            if (h == null) return;
            dataFire.ValueChanged += (s, e) => {
                foreach (DataSnapshot ds in e.Snapshot.Children) {
                    foreach (DataSnapshot dds in ds.Children) {
                        setupObject(ds.Key, dds, h);
                    }
                }
            };
        }

        private void setupObject(string pid,DataSnapshot dds,Handler h) {
            string json = dds.GetRawJsonValue();
            RemoteData rd = JsonConvert.DeserializeObject<RemoteData>(json);
            rd.setSource(json);
            h.onDataInit( pid,dds.Key, rd);
            
        }

        internal FireNode get(string pid, string sid) {
            Map<FireNode> fm = map.findThanSet(pid, new Map<FireNode>());
            return fm.findThanSet(sid, new FireNode(pid, sid, dataFire));
        }

        public interface Handler {
          
            void onDataInit(string pid, string oid, RemoteData v);
        }


    }
}