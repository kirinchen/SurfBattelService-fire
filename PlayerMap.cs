using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;
namespace RFNEet.firebase {

    public class PlayerMap : FireMap<ObjMap> {
        private DatabaseReference dataFire;

        internal PlayerMap() : base(FirebaseManager.getDBRef()) {

        }


        internal override ObjMap genChild(DBResult ds) {
            ObjMap obj = new ObjMap(ds.key());
            obj.injectData(ds);
            return obj;
        }

        internal FireNode getThanSet(string pid, string oid) {
            ObjMap om = findThanSet(pid, () => { return new ObjMap(pid); });
            return om.findThanSet(oid, () => { return new FireNode(pid, oid); });
        }

        public void foreachNode(Action<FireNode> fnA) {
            foreach (ObjMap om in Values) {
                foreach (FireNode fn in om.Values) {
                    fnA(fn);
                }
            }
        }

        internal void remove(string pid, string oid) {
            if (ContainsKey(pid)) {
                this[pid].remove(oid);
            }
        }

        internal override void onChildRemoved(string v,ObjMap om) {
            om.removeMe();
        }
    }
}
