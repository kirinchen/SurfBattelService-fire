using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet.firebase {
    public class RFObjGener : MonoBehaviour, FireRepo.Handler {
        public List<LocalPrefabBundle> prefabs;
        private SyncCenter center;

        void Awake() {
            center = SyncCenter.getInstance();
        }

        public GameObject onDataInit(string pid, string oid, FireNode fn, RemoteData v) {
            string mePid = PidGeter.getPid();
            if (string.IsNullOrEmpty(mePid)) throw new NullReferenceException("me Pid is null");
            if (string.Equals(pid, mePid)) {
                LocalPrefabBundle lob = prefabs.Find(p => { return string.Equals(p.tag, v.tag); });
                LocalObject lo = lob.prefab.gameObject.scene == null ? Instantiate(lob.prefab) : lob.prefab;
                return center.localRepo.create(lo).gameObject;
            } else {
                RemotePlayerRepo rpr = center.getRemoteRepos()[pid];
                rpr.createNewObject(v);
                return rpr.get(oid).gameObject;
            }
        }

        [System.Serializable]
        public class LocalPrefabBundle {
            public LocalObject prefab;
            public string tag;
        }
    }
}
