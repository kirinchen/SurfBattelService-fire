﻿using System;
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

        internal override FireNode genChild(DBResult ds) {
            RemoteData rd = parseRemoteData(ds);
            FireNode fn = new FireNode(pid, ds.key());
            GameObject go = null;
            if (FirePlayerQueuer.KEY_TAG.Equals(rd.tag)) {
                go = FirebaseManager.getInstance().playerQueuer.gameObject;
            } else {
                FireRepo.Handler h = FirebaseManager.getRepo().handler;
                go = h.onDataInit(pid, ds.key(), fn, parseRemoteData(ds));
            }
            InitBundle ib = new InitBundle(fn, rd);
            go.SendMessage("initAtFire", ib, SendMessageOptions.DontRequireReceiver);
            return fn;
        }

        public void remove(string oid) {
            if (ContainsKey(oid)) {
                FireNode fn = this[oid];
                fn.removeMe();
                Remove(oid);
            }
        }

        public static RemoteData parseRemoteData(DBResult ds) {
            string json = ds.getRawJsonValue();
            RemoteData rd = JsonConvert.DeserializeObject<RemoteData>(json);
            rd.setSource(json);
            return rd;
        }

        public class InitBundle {
            public FireNode node;
            public RemoteData data;
            public InitBundle(FireNode node, RemoteData data) {
                this.node = node;
                this.data = data;
            }
        }

    }
}
