using System;
using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using UnityEngine;
using Newtonsoft.Json;

namespace RFNEet.firebase {
    public class ObjMap : FireMap<FireNode> {

        public string pid { get; private set; }
        public ObjMap(string pid) : base(FirebaseManager.getDBRef().Child(pid)) {
            this.pid = pid;
        }

        internal override FireNode genChild(string cn, DataSnapshot ds) {
            FireNode fn = new FireNode(pid, ds.Key);
            FireRepo.Handler h = FirebaseManager.getRepo().handler;
            h.onDataInit(pid, ds.Key, fn, parseRemoteData(ds));
            return fn;
        }


        public static RemoteData parseRemoteData(DataSnapshot ds) {
            string json = ds.GetRawJsonValue();
            RemoteData rd = JsonConvert.DeserializeObject<RemoteData>(json);
            rd.setSource(json);
            return rd;
        }
    }
}
