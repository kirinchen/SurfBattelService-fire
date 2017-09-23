using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

namespace RFNEet.firebase {
    public class FireRepo : MonoBehaviour {

        public readonly string keyTag = "tag";


        public DBRefenece dbRef { get; private set; }
        private PlayerMap map;
        public Handler handler { get; private set; }

        internal void initFire(DBRefenece df, Action initAct) {
            dbRef = df;
            handler = getHandler();
            map = new PlayerMap();
            dbRef.fetchValue((e)=> {
                map.injectData(e);
                initAct();
            });

        }

        internal FireNode get(string pid, string sid) {
            return map.getThanSet(pid, sid);
        }

        public interface Handler {

            void onDataInit(string pid, string oid, FireNode fn, RemoteData v);
        }

        private Handler getHandler() {
            Handler h = GetComponentInChildren<Handler>();
            return h == null ? emth : h;
        }

        private Empty emth = new Empty();
        public class Empty : Handler {
            public void onDataInit(string pid, string oid, FireNode fn, RemoteData v) {
            }
        }


    }
}