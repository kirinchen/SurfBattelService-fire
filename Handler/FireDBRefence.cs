using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStomp;

namespace RFNEet.firebase {
    public class FireDBRefence : DBRefenece {


        private DatabaseReference body;
        private Action<DBResult> childAddedAction;
        private Action<DBResult> valueChangedAction;


        public FireDBRefence(DatabaseReference body) {
            this.body = body;
        }

        public void fetchValue(Action<DBResult> a) {
            new ValueChangedListenerSetup(body, true, (e) => {
                a(new FireDBReslut(e.Snapshot));
            });
        }

        public DBRefenece Child(string pid) {
            DatabaseReference dr = body.Child(pid);
            return new FireDBRefence(dr);
        }

        public void SetRawJsonValueAsync(string s) {
            body.SetRawJsonValueAsync(s);
        }

        public void removeMe() {
            body.RemoveValueAsync();
        }

        public Action<DBResult> childAdded
        {
            get
            {
                return childAddedAction;
            }
            set
            {
                bool notset = childAddedAction == null;
                childAddedAction = value;
                if (notset) {
                    body.ChildAdded += (s, e) => {
                        childAddedAction(new FireDBReslut(e.Snapshot));
                    };
                }
            }
        }

        public Action<DBResult> ValueChanged
        {
            get
            {
                return valueChangedAction;
            }

            set
            {
                bool notset = valueChangedAction == null;
                valueChangedAction = value;
                if (notset) {
                    body.ValueChanged += (s, e) => {
                        valueChangedAction(new FireDBReslut(e.Snapshot));
                    };
                }
            }
        }


    }
}
