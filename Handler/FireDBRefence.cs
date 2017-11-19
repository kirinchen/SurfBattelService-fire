﻿using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStomp;
using System.Threading.Tasks;

namespace RFNEet.firebase {
    public class FireDBRefence : DBRefenece {


        private DatabaseReference body;
        private Action<DBResult> childAddedAction;
        private Action<DBResult> valueChangedAction;
        private Action<DBResult> childRemovedAction;


        public FireDBRefence(DatabaseReference body) {
            this.body = body;
         
            // body.ChildRemoved
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

        public DBRefenece parent() {
            DatabaseReference d = body.Parent;
            return new FireDBRefence(d);
        }

        public Task SetRawJsonValueAsync(string s) {
            return body.SetRawJsonValueAsync(s);
        }

        public void removeMe() {
            body.RemoveValueAsync();
        }

        public Task SetValueAsync(object value) {
            return body.SetValueAsync(value);
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

        public Action<DBResult> childRemoved
        {
            get
            {
                return childRemovedAction;
            }

            set
            {
                bool notset = childRemovedAction == null;
                childRemovedAction = value;
                if (notset) {
                    body.ChildRemoved += (s, e) => {
                        childRemovedAction(new FireDBReslut(e.Snapshot));
                    };
                }
            }
        }
    }
}
