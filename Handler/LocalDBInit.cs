using System;
using System.Collections;
using System.Collections.Generic;
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
            public Action<DBResult> childAdded
            {
                get
                {
                    return childAdded;
                }

                set
                {
                    childAdded = value;
                }
            }

            public Action<DBResult> childRemoved
            {
                get
                {
                    return childRemoved;
                }

                set
                {
                    childRemoved = value;
                }
            }

            private Action<DBResult> _ValueChanged;
            public Action<DBResult> ValueChanged
            {
                get
                {
                    return _ValueChanged;
                }

                set
                {
                    _ValueChanged = value;
                    _ValueChanged(new DBRR());
                }
            }

            public DBRefenece Child(string pid) {
                return new DBR();
            }

            public void fetchValue(Action<DBResult> a) {
                a(new DBRR());
            }

            public void removeMe() {
            }

            public void SetRawJsonValueAsync(string s) {
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

            public string key() {
                return string.Empty;
            }
        }
    }
}
