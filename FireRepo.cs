using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

namespace RFNEet.firebase {
    public class FireRepo : MonoBehaviour {

        public readonly string keyTag = "tag";


        public DatabaseReference dbRef { get; private set; }
        private PlayerMap map;
        public Handler handler { get; private set; }

        internal void initFire(string roomId, Action initAct) {
            dbRef = FirebaseDatabase.DefaultInstance.GetReference(FireConfig.getInstance().rootNode).Child(roomId);
            handler = getHandler();
            map = new PlayerMap();
            new ValueChangedListenerSetup(dbRef, true, (e) => {
                map.injectData(e.Snapshot);
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