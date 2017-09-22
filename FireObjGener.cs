using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet.firebase {
    public class FireObjGener : MonoBehaviour, FireRepo.Handler {
        public List<PrefabBundle> prefabs;
      


        public void onDataInit(string pid, string oid, FireNode fn, RemoteData v) {
            PrefabBundle pb = prefabs.Find(p => { return string.Equals(v.tag, p.tag); });
            GameObject go = pb.prefab.scene == null ? Instantiate(pb.prefab) : pb.prefab;
            InitBundle ib = new InitBundle(fn, v);
            go.SendMessage("initAtFire", ib, SendMessageOptions.DontRequireReceiver);
        }

        [System.Serializable]
        public class PrefabBundle {
            public GameObject prefab;
            public string tag;
        }


        public class InitBundle {
            public FireNode node;
            public RemoteData data;
            public InitBundle(FireNode node, RemoteData data) {
                this.node = node;
                this.data = data;
            }
        }
    }
}
