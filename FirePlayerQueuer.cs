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

        public enum ChangePostType {
            JustME, ALLChange,
        }

        public static readonly string KEY_TAG = "@FPQ";
        private static readonly string KEY_PID = "@C";
        private static readonly string KEY_OID = "@FirePlayerQueuer";

        private PlayerQueuer ceneter;
        public string meId { get; private set; }
        public ChangePostType changePostType;

        public Data data = new Data();
        public List<string> debugIds = new List<string>();

        public readonly CallbackList fetchDataDone = new CallbackList();

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
                node.initCallback.add(onFirstFetch);
                if (createData) {
                    Task t = postData();
                    UnityUtils.setAsync(this, t, a);
                } else {
                    fetchDataDone.add(a);
                }
            });
        }


        private void onFirstFetch() {
            ceneter.addTokenPlayerChangeListener(s => {
                switch (changePostType) {
                    case ChangePostType.ALLChange:
                        FirebaseManager.getRepo().notifyChangePost();
                        break;
                    case ChangePostType.JustME:
                        postData();
                        break;
                }
            });
            fetchDataDone.done();
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
            foreach (string s in d.intoMap.Keys) {
                if (!data.intoMap.ContainsKey(s)) {
                    data.intoMap.Add(s, d.intoMap[s]);
                }
            }

            ceneter.triggerTokenChange(d.token);
        }

        internal override void onValueChnaged(RemoteData obj) {
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
                Debug.Log("setTokenPlayer=" + v);
            } else if (post == PlayerQueuer.TokePost.FIELD) {
                node.dataFire.Child("token").SetValueAsync(v);
                node.dataFire.Child("sid").SetValueAsync(FirebaseManager.getMePid());
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

        [System.Serializable]
        public class Data : RemoteData {
            public Dictionary<string, DateTime> intoMap = new Dictionary<string, DateTime>();
            public string token;
        }
    }
}
