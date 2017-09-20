using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet.firebase {
    public class FireRepo  {

        public class Map<T> : Dictionary<string, T> {

            public T findThanSet(string k, T t) {
                if (ContainsKey(k)) {
                    return this[k];
                } else {
                    Add(k,t);
                    return this[k];
                }
            }

        }

        private DatabaseReference dataFire;
        private Map<Map<FireNode>> map = new Map<Map<FireNode>>();

        internal void initFire(string roomId) {
            dataFire = FirebaseDatabase.DefaultInstance.GetReference("/data").Child(roomId);
        }

        internal FireNode get(string pid, string sid) {
            Map<FireNode> fm = map.findThanSet(pid,new Map<FireNode>());
            return fm.findThanSet(sid,new FireNode(pid,sid, dataFire));

        }




    }
}