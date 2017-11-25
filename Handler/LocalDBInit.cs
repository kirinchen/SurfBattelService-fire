﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace RFNEet.firebase {
    public class LocalDBInit : DBInit {
        public void createConnect() {

        }

        public DBRefenece createRootRef(string roomId) {
            return new DBR();
        }

        public void init(Action<string> onFailInitializeFirebase, Action initializeFirebase) {
            initializeFirebase();
        }

        public bool isOK() {
            return true;
        }


        public class DBR : DBRefenece {
           

            public DBRefenece Child(string pid) {
                return new DBR();
            }

            public void fetchValue(Action<DBResult> a) {
                a(new DBRR());
            }

            public void removeMe() {
            }

            public Task SetRawJsonValueAsync(string s) {
                return Task.Factory.StartNew<object>(()=> { return null; });
            }

            public DBRefenece parent() {
                return new DBR();
            }

            public Task SetValueAsync(object value) {
                return Task.Factory.StartNew<object>(() => { return null; });
            }

            public void addChildAdded(Action<DBResult> a) {
            }

            public void removeChildAdded(Action<DBResult> a) {
            }

            public void addChildRemoved(Action<DBResult> a) {
            }

            public void removeChildRemoved(Action<DBResult> a) {
            }

            public void addValueChanged(Action<DBResult> a) {
                a(new DBRR());
            }

            public void removeValueChanged(Action<DBResult> a) {
            }
        }

        public class DBRR : DBResult {
            public IEnumerable<DBResult> children() {
                List<DBResult> list = new List<DBResult>();
                return list;
            }

            public string getRawJsonValue() {
                return string.Empty;
            }

            public object GetValue() {
                return null;
            }

            public string key() {
                return string.Empty;
            }
        }
    }
}
