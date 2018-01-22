using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStomp;
using System.Threading.Tasks;
using RFNEet.realtimeDB;
using surfm.tool;

namespace RFNEet.firebase {
    public class FireDBRefence : DBRefenece {


        private DatabaseReference body;
        private Action<DBResult> childAddedAction;
        private Action<DBResult> valueChangedAction;
        private Action<DBResult> childRemovedAction;

        private EventMap<ValueChangedEventArgs> valueEventMap;
        private EventMap<ChildChangedEventArgs> addEventMap;
        private EventMap<ChildChangedEventArgs> removeEventMap;
        private MonoBehaviour context;


        public FireDBRefence(MonoBehaviour c, DatabaseReference body) {
            context = c;
            this.body = body;
            valueEventMap = new EventMap<ValueChangedEventArgs>(body, t => {
                return t.Snapshot;
            }, h => {
                this.body.ValueChanged += h;
            }, h => {
                this.body.ValueChanged -= h;
            });
            addEventMap = new EventMap<ChildChangedEventArgs>(body, t => {
                return t.Snapshot;
            }, h => {
                this.body.ChildAdded += h;
            }, h => {
                this.body.ChildAdded -= h;
            });
            removeEventMap = new EventMap<ChildChangedEventArgs>(body, t => {
                return t.Snapshot;
            }, h => {
                this.body.ChildRemoved += h;
            }, h => {
                this.body.ChildRemoved -= h;
            });
        }

        public void fetchValue(Action<DBResult> a) {
            new ValueChangedListenerSetup(body, true, (e) => {
                a(new FireDBReslut(e.Snapshot));
            });
        }

        public DBRefenece Child(string pid) {
            DatabaseReference dr = body.Child(pid);
            return new FireDBRefence(context, dr);
        }

        public DBRefenece parent() {
            DatabaseReference d = body.Parent;
            return new FireDBRefence(context, d);
        }

        public void SetRawJsonValueAsync(string s, Action<bool, object> cb = null) {
            Task t = body.SetRawJsonValueAsync(s);
            if (cb != null) {
                context.StartCoroutine(setAsync(t, cb));
            }
        }

        public void removeMe() {
            body.RemoveValueAsync();
        }

        public void SetValueAsync(object value, Action<bool, object> cb = null) {
            Task t = body.SetValueAsync(value);
            if (cb != null) {
                context.StartCoroutine(setAsync(t, cb));
            }
        }

        public static IEnumerator setAsync(Task task, Action<bool, object> cb) {
            yield return new WaitUntil(() => task.IsCompleted);
            if (task.IsFaulted) {
                cb(false, task.Exception);
            } else {
                cb(true, "");
            }
        }

        public void addChildAdded(Action<DBResult> a) {
            addEventMap.addEvent(a);
        }

        public void removeChildAdded(Action<DBResult> a) {
            addEventMap.removeEvent(a);
        }

        public void addValueChanged(Action<DBResult> a) {
            valueEventMap.addEvent(a);
        }

        public void removeValueChanged(Action<DBResult> a) {
            valueEventMap.removeEvent(a);
        }

        public void addChildRemoved(Action<DBResult> a) {
            removeEventMap.addEvent(a);
        }

        public void removeChildRemoved(Action<DBResult> a) {
            removeEventMap.removeEvent(a);
        }



        public class EventMap<T> where T : EventArgs {
            private Dictionary<Action<DBResult>, EventHandler<T>> changeEventMap = new Dictionary<Action<DBResult>, EventHandler<T>>();
            private DatabaseReference body;
            private Func<T, DataSnapshot> getSnapshot;
            private Action<EventHandler<T>> add, remove;
            internal EventMap(DatabaseReference b, Func<T, DataSnapshot> gs, Action<EventHandler<T>> add, Action<EventHandler<T>> remove) {
                body = b;
                getSnapshot = gs;
                this.add = add;
                this.remove = remove;
            }

            public void addEvent(Action<DBResult> a) {
                EventHandler<T> h = (s, e) => {
                    a(new FireDBReslut(getSnapshot(e)));
                };
                changeEventMap.Add(a, h);
                //body.ValueChanged += h;
                add(h);
            }

            public void removeEvent(Action<DBResult> a) {
                if (!changeEventMap.ContainsKey(a)) return;
                EventHandler<T> h = changeEventMap[a];
                //body.ValueChanged -= h;
                remove(h);
                changeEventMap.Remove(a);
            }
        }
    }
}
