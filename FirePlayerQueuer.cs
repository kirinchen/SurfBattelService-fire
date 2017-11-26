using Newtonsoft.Json;
using RFNEet.firebase;
using surfm.tool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace RFNEet.firebase {
    public class FirePlayerQueuer : FireObj, PlayerQueuer.DataProvider {
        public static FirePlayerQueuer instance { get; private set; }

        public static readonly string KEY_TAG = "@FPQ";
        private static readonly string KEY_PID = "@C";
        private static readonly string KEY_OID = "@FirePlayerQueuer";

        private PlayerQueuer ceneter;
        public string meId { get; private set; }


        public Data data = new Data();
        public List<string> debugIds = new List<string>();


        void Awake() {
            instance = this;
            meId = PidGeter.getPid();
            ceneter = gameObject.AddComponent<PlayerQueuer>();
            ceneter.setDataProvider(this);
        }

        public void init(bool createData, Action a) {
            FirebaseManager.getInstance().addInitedAction(b => {
                init(KEY_PID, KEY_OID);
                ceneter.addPlayer(meId);
                if (createData) {
                    node.initCallback.reset();
                    node.initCallback.add(a);
                    postData();
                } else {
                    node.initCallback.add(a);
                }
                node.dataFire.Child("intoMap").addChildRemoved(onPlayerLeave);
            });
        }

        private void onPlayerLeave(DBResult obj) {
            data.intoMap.Remove(obj.key());
        }

        internal override RemoteData getCurrentData() {
            RemoteData ans = data;
            ans.tag = KEY_TAG;
            return ans;
        }

        internal override void onInit(RemoteData d) {

            map(d.to<Data>());
        }

        private void map(Data d) {
            if (d.intoMap != null && d.intoMap.Count > 0) {
                foreach (string s in d.intoMap.Keys) {
                    if (!data.intoMap.ContainsKey(s)) {
                        data.intoMap.Add(s, d.intoMap[s]);
                    }
                }
            }
            ceneter.triggerTokenChange(d.token);
        }

        internal override void onValueChanged(RemoteData obj) {
            map(obj.to<Data>());
        }

        public List<string> playerIds() {
            List<string> realPids = new List<string>(data.intoMap.Keys);
            realPids.Sort((a, b) => {
                return data.intoMap[a].CompareTo(data.intoMap[b]);
            });
            debugIds = realPids;
            return realPids;
        }

        public void setTokenPlayer(string v, PlayerQueuer.TokePost post) {

            data.token = v;
            if (post == PlayerQueuer.TokePost.ALL) {
                postData();
            } else if (post == PlayerQueuer.TokePost.FIELD) {
                node.putField("token", v);
            }
        }

        public string tokenPlayer() {
            return data.token;
        }

        public void addPlayer(string id) {
            DateTime d = NistService.getTime();
            data.intoMap.Add(id, d);
            node.dataFire.Child("intoMap").Child(id).SetValueAsync(d.ToString("o"));
            node.dataFire.Child("sid").SetValueAsync(FirebaseManager.getMePid());
        }

        internal override Type getDataType() {
            return typeof(Data);
        }

        public override void OnDestroy() {
            base.OnDestroy();
            node.dataFire.Child("intoMap").Child(meId).removeMe();
        }

        [System.Serializable]
        public class Data : RemoteData {
            public Dictionary<string, DateTime> intoMap = new Dictionary<string, DateTime>();
            public string token;
        }
    }
}
