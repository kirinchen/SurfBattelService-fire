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
            node.addValueChangedListener(_onValueChnaged);
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

        public void init(string p, string o, bool autoPost = true) {
            if (node != null) return;
            pid = p;
            oid = o;
            node = FirebaseManager.getRepo().get(pid, oid);
            node.addValueChangedListener(onValueChnaged);
            node.addChangePostActions(onNotifyChangePost);
            if (autoPost) postData();
        }

        public void postData() {
            _lastData = getCurrentData();
            node.post(_lastData);
        }

        private void _onValueChnaged(RemoteData obj) {
            _lastData = getCurrentData();
            onValueChnaged(obj);
        }

        public void removeMe() {
            if (node == null) return;
            if (!node.removed) {
                node.removeMe();
            }
        }

        private bool _appQuited = false;
        void OnApplicationQuit() {
            _appQuited = true;
        }

        void OnDestroy() {
            if (!_appQuited) {
                removeMe();
            }
        }

        internal abstract RemoteData getCurrentData();
        internal abstract void onValueChnaged(RemoteData obj);
        internal abstract void onInit(RemoteData d);
    }
}