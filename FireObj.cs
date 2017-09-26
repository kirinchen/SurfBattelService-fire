using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet.firebase {
    public abstract class FireObj : MonoBehaviour {

        private FireNode node;
        public string pid;
        public string oid;
        private RemoteData _lastData;

        void initAtFire(ObjMap.InitBundle ib) {
            node = ib.node;
            setData(ib.data);
            node.addValueChangedListener(onValueChnaged);
            node.addChangePostActions(onNotifyChangePost);
        }

        private void setData(RemoteData data) {
            _lastData = data;
            pid = data.pid;
            oid = data.oid;
            onInit(data);
        }

        private void onNotifyChangePost() {
            RemoteData nrd = getCurrentData();
            bool b = RemoteData.isValueSame(nrd, _lastData);
            if (!b) {
                node.post(nrd);
            }
        }

        public void init(string p, string o) {
            if (node != null) return;
            pid = p;
            oid = o;
            node = FirebaseManager.getRepo().get(pid, oid);
            postData();
            node.addValueChangedListener(onValueChnaged);
        }

        public void postData() {
            node.post(getCurrentData());
        }

        internal abstract RemoteData getCurrentData();
        internal abstract void onValueChnaged(RemoteData obj);
        internal abstract void onInit(RemoteData d);
    }
}