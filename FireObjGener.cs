using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet.firebase {
    public class FireObjGener : MonoBehaviour, FireRepo.Handler {
        public List<PrefabBundle> prefabs;

        public GameObject onDataInit(string pid, string oid, FireNode fn, RemoteData v) {
            PrefabBundle pb = getPrefabBundle(v.tag);
            GameObject go = pb.prefab.scene == null ? Instantiate(pb.prefab) : pb.prefab;
            return go;
        }

        private PrefabBundle getPrefabBundle(string tag) {
            return prefabs.Find(p => { return string.Equals(tag, p.tag); });
        }

        [System.Serializable]
        public class PrefabBundle {
            public GameObject prefab;
            public string tag;
        }


    }
}
