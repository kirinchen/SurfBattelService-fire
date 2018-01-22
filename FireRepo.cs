using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using RFNEet.realtimeDB;
using surfm.tool;

namespace RFNEet.firebase {
    public class FireRepo : MonoBehaviour {

        public readonly string keyTag = "tag";

        public static readonly string SKIP_KEY_PREFIX = "!SKP_";


        public DBRefenece dbRef { get; private set; }
        private PlayerMap map;
        public Handler handler { get; private set; }

        internal void initFire(DBRefenece df, Action initAct) {
            dbRef = df;
            handler = getHandler();
            map = new PlayerMap();
            dbRef.fetchValue((e) => {
                map.injectData(e);
                initAct();
            });

        }

        public void notifyChangePost(Action cb = null) {
            CallbacksDone allDone = new CallbacksDone(0, b => {
                if (b) {
                    cb();
                } else {
                    throw new Exception("Not fetch doen");
                }
            });
            map.foreachNode(fn => {
                Action<bool> a = allDone.cutdown();
                fn.notifyChangePost((b, o) => { a(b); });
            });
        }



        internal FireNode get(string pid, string sid) {
            return map.getThanSet(pid, sid);
        }

        public interface Handler {

            GameObject onDataInit(string pid, string oid, FireNode fn, RemoteData v);
        }

        private Handler getHandler() {
            Handler h = GetComponentInChildren<Handler>();
            return h /*== null ? emth : h*/;
        }

        internal void remove(string pid, string oid) {
            map.remove(pid, oid);
        }

        internal void removeAll() {
            map.removeMe();
        }


        //private Empty emth = new Empty();
        //public class Empty : Handler {
        //    public void onDataInit(string pid, string oid, FireNode fn, RemoteData v) {
        //    }
        //}


    }
}