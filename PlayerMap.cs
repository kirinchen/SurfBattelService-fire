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


        internal override ObjMap genChild(string cn, DataSnapshot ds) {
            ObjMap obj = new ObjMap(ds.Key);
            obj.injectData(ds);
            return obj;
        }

        internal FireNode getThanSet(string pid, string oid) {
            ObjMap om = findThanSet(pid, () => { return new ObjMap(pid); });
            return om.findThanSet(oid, () => { return new FireNode(pid, oid); });
        }


    }
}
