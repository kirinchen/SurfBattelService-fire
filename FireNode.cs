using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using surfm.tool;
using System.Threading.Tasks;
using RFNEet.realtimeDB;

namespace RFNEet.firebase {
    public class FireNode {

        public string pid;
        public string oid;
        public DBRefenece dataFire;
        private List<Action<RemoteData>> valueChangedListeners = new List<Action<RemoteData>>();
        public struct ChangePost {
            public RemoteData data;
            public ChangeType change;
            public string field;
            public object fieldData;
        }
        internal Func<ChangePost> changePostFunc = () => { return new ChangePost(); };
        internal Action onRemoveAction = () => { };
        public bool removed { get; private set; }
        public readonly CallbackList initCallback = new CallbackList();

        internal FireNode(string p, string o) {
            pid = p;
            oid = o;
            dataFire = FirebaseManager.getDBRef().Child(pid).Child(oid);
            dataFire.addValueChanged(onValueChanged);

        }

        internal void post(RemoteData o, Action<bool, object> cb=null) {
            o.pid = pid;
            o.oid = oid;
            o.sid = FirebaseManager.getMePid();
            string s = JsonConvert.SerializeObject(o);
            dataFire.SetRawJsonValueAsync(s, cb);
        }

        internal void putField(string field, object o, Action<bool, object> cb = null) {
            if (o is string || o is int || o is float) {
                dataFire.Child(field).SetValueAsync(o);
            } else {
                string s = JsonConvert.SerializeObject(o);
                dataFire.Child(field).SetRawJsonValueAsync(s);
            }
             dataFire.Child("sid").SetValueAsync(pid, cb);

        }

        internal void notifyChangePost(Action<bool, object> cb = null) {
            ChangePost rd = changePostFunc();
            if (rd.change == ChangeType.ALL) {
                 post(rd.data, cb);
            } else if (rd.change == ChangeType.FieldChange) {
                 putField(rd.field, rd.fieldData, cb);
            }
        }

        public void removeMe() {
            if (!removed) {
                removed = true;
                dataFire.removeValueChanged(onValueChanged);
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

        public enum ChangeType {
            NoChange, ALL, FieldChange
        }
    }
}
