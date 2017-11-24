﻿using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using surfm.tool;
using System.Threading.Tasks;

namespace RFNEet.firebase {
    public class FireNode {

        public string pid;
        public string oid;
        public DBRefenece dataFire;
        private List<Action<RemoteData>> valueChangedListeners = new List<Action<RemoteData>>();
        internal Func<RemoteData> changePostFunc = () => { return null; };
        internal Action onRemoveAction = () => { };
        public bool removed { get; private set; }
        public readonly CallbackList initCallback = new CallbackList();

        internal FireNode(string p, string o) {
            pid = p;
            oid = o;
            dataFire = FirebaseManager.getDBRef().Child(pid).Child(oid);
            dataFire.ValueChanged += onValueChanged;

        }

        internal Task post(RemoteData o) {
            o.pid = pid;
            o.oid = oid;
            o.sid = FirebaseManager.getMePid();
            string s = JsonConvert.SerializeObject(o);
            return dataFire.SetRawJsonValueAsync(s);
        }

        internal Task notifyChangePost() {
            RemoteData rd = changePostFunc();
            if (rd != null) {
                Debug.Log("notifyChangePost=" + rd.oid);
                return post(rd);
            }
            return Task.Factory.StartNew<object>(() => { return null; });
        }

        public void removeMe() {
            if (!removed) {
                removed = true;
                dataFire.ValueChanged -= onValueChanged;
                dataFire.removeMe();
                FirebaseManager.getRepo().remove(pid, oid);
                onRemoveAction();
            }
        }

        public void addValueChangedListener(Action<RemoteData> a) {
            valueChangedListeners.Add(a);
        }


        private void onValueChanged(DBResult ea) {
            string s = ea.getRawJsonValue();
            if (!string.IsNullOrEmpty(s)) {
                RemoteData rd = JsonConvert.DeserializeObject<RemoteData>(s);
                rd.setSource(s);
                if (!string.Equals(rd.sid, FirebaseManager.getMePid())) {
                    valueChangedListeners.ForEach(l => {
                        l(rd);
                    });
                }
            }
            initCallback.done();
        }
    }
}
