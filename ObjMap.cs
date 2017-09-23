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

        internal override FireNode genChild( DBResult ds) {
            FireNode fn = new FireNode(pid, ds.key());
            FireRepo.Handler h = FirebaseManager.getRepo().handler;
            h.onDataInit(pid, ds.key(), fn, parseRemoteData(ds));
            return fn;
        }


        public static RemoteData parseRemoteData(DBResult ds) {
            string json = ds.getRawJsonValue();
            RemoteData rd = JsonConvert.DeserializeObject<RemoteData>(json);
            rd.setSource(json);
            return rd;
        }
    }
}
