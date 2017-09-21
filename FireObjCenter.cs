using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet.firebase {
    public class FireObjCenter : MonoBehaviour, FireRepo.Handler {
        public List<PrefabBundle> prefabs;
        private PlayerMap map = new PlayerMap();


        public void onDataInit(string pid, string oid, RemoteData v) {
            PrefabBundle pb = prefabs.Find(p => { return string.Equals(v.tag, p.tag); });
            GameObject go = pb.prefab.scene == null ? Instantiate(pb.prefab) : pb.prefab;
            map.add(pid, oid, go);
            FireNode fn = FirebaseManager.getRepo().get(pid, oid);
            InitBundle ib = new InitBundle(fn, v);
            go.SendMessage("initAtFire", ib, SendMessageOptions.DontRequireReceiver);
        }

        [System.Serializable]
        public class PrefabBundle {
            public GameObject prefab;
            public string tag;
        }

        public class PlayerMap : Map<string, ObjMap> {
            internal void add(string pid, string oid, GameObject go) {
                ObjMap om = findThanSet(pid, new ObjMap());
                om.Add(oid, go);
            }
        }
        public class ObjMap : Map<string, GameObject> { }

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
