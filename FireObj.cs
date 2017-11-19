using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace RFNEet.firebase {
    public abstract class FireObj : MonoBehaviour {

        protected FireNode node { get; private set; }
        public string pid;
        public string oid;
        private RemoteData _lastData;

        void initAtFire(ObjMap.InitBundle ib) {
            node = ib.node;
            setData(ib.data);
            node.addValueChangedListener(_onValueChnaged);
            node.changePostFunc = (onNotifyChangePost);
            node.onRemoveAction = onRemoveAction;
        }

        private void setData(RemoteData data) {
            _lastData = data;
            pid = data.pid;
            oid = data.oid;
            onInit(data);
        }

        private RemoteData onNotifyChangePost() {
            RemoteData nrd = getCurrentData();
            bool b = RemoteData.isValueSame(nrd, _lastData);
            if (!b) {
                node.post(nrd);
                return nrd;
            }
            return null;
        }

        public void init(string p, string o) {
            if (node != null) return;
            pid = p;
            oid = o;
            node = FirebaseManager.getRepo().get(pid, oid);
            node.addValueChangedListener(onValueChnaged);
            node.changePostFunc = (onNotifyChangePost);
          
        }

        public Task postData() {
            _lastData = getCurrentData();
            return node.post(_lastData);
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

        public void OnDestroy() {
            if (!_appQuited && FirebaseManager.getInstance() != null) {
                removeMe();
            }
        }

        internal virtual void onRemoveAction() {
            Destroy(gameObject);
        }

        internal abstract RemoteData getCurrentData();
        internal abstract void onValueChnaged(RemoteData obj);
        internal abstract void onInit(RemoteData d);
    }
}