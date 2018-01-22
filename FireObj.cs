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

        private FireNode.ChangePost onNotifyChangePost() {
            RemoteData nrd = getCurrentData();
            return getValueChange(nrd, _lastData);
        }

        internal virtual FireNode.ChangePost getValueChange(RemoteData nrd, RemoteData _lastData) {
            return getValueChangeStatic(nrd, _lastData);
        }

        public static FireNode.ChangePost getValueChangeStatic(RemoteData nrd, RemoteData _lastData) {
            bool c = RemoteData.isValueSame(nrd, _lastData);
            if (c) {
                return new FireNode.ChangePost() {
                    change = FireNode.ChangeType.ALL,
                    data = nrd
                };
            } else {
                return new FireNode.ChangePost();
            }
        }

        public void init(string p, string o) {
            if (node != null) return;
            pid = p;
            oid = o;
            node = FirebaseManager.getRepo().get(pid, oid);
            node.addValueChangedListener(onValueChanged);
            node.changePostFunc = (onNotifyChangePost);

        }

        public void postData(Action<bool, object> cb = null) {
            _lastData = getCurrentData();
             node.post(_lastData, cb);
        }

        private void _onValueChnaged(RemoteData obj) {
            _lastData = obj.to(getDataType());
            onValueChanged(obj);
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

        public virtual void OnDestroy() {
            if (!_appQuited && FirebaseManager.getInstance() != null && !FirebaseManager.getInstance().keepOnDestoryObj) {
                removeMe();
            }
        }

        internal virtual void onRemoveAction() {
            Destroy(gameObject);
        }

        internal abstract Type getDataType();
        internal abstract RemoteData getCurrentData();
        internal abstract void onValueChanged(RemoteData obj);
        internal abstract void onInit(RemoteData d);
    }
}