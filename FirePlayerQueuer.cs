using Newtonsoft.Json;
using RFNEet.firebase;
using surfm.tool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet.firebase {
    public class FirePlayerQueuer : FireObj {
        public static FirePlayerQueuer instance { get; private set; }

        public enum ChangePostType {
            JustME, ALLChange,
        }

        public static readonly string KEY_EX_PIDS = "@EXFPIDS";
        public static readonly string KEY_TAG = "@FPQ";
        private static readonly string KEY_PID = "@C";
        private static readonly string KEY_OID = "@FirePlayerQueuer";

        private PlayerQueuer ceneter;
        public string meId { get; private set; }
        public ChangePostType changePostType;

        //  public List<string> realPids;

        void Awake() {
            instance = this;
            meId = PidGeter.getPid();
            ceneter = gameObject.AddComponent<PlayerQueuer>();
        }

        void Start() {
            FirebaseManager.getInstance().addInitedAction(b => {
                init(KEY_PID, KEY_OID, false);
                node.initCallback.add(onFirstFetch);
            });
        }

        private void onFirstFetch() {
            ceneter.addPlayerIntoListener(s => {
                postData();
            });
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

            getExRef().ValueChanged += onPidsChange;
        }


        new public void OnDestroy() {
            Debug.Log("~~~~OnDestroy " + node.removed);
            getExRef().ValueChanged -= onPidsChange;
            base.OnDestroy();
        }

        private DBRefenece getExRef() {
            return node.dataFire.parent().Child(FireRepo.SKIP_KEY_PREFIX + KEY_EX_PIDS);
        }

        internal override void onRemoveAction() {
            getExRef().Child(meId).removeMe();
            base.onRemoveAction();
        }

        private void onPidsChange(DBResult obj) {
            if (node.removed) return;
            string s = obj.getRawJsonValue();
            Debug.Log("~~~~onPidsChange " + s + " rm=" + node.removed);
            if (string.IsNullOrEmpty(s)) {
                addMeId();
                return;
            }
            Dictionary<string, DateTime> d = JsonConvert.DeserializeObject<Dictionary<string, DateTime>>(s);
            if (!d.ContainsKey(meId)) {
                addMeId();
                return;
            }
            ceneter.getCurrentData().playerIds = new List<string>(d.Keys);
            ceneter.getCurrentData().playerIds.Sort((a, b) => {
                return d[a].CompareTo(d[b]);
            });
            postData();

        }

        private void addMeId() {
            ceneter.addPlayer(meId);
            getExRef().Child(meId).SetValueAsync(NistService.getTime().ToString("o"));
            Debug.Log("c m =" + ceneter.meId + "t=" + ceneter.isToken() + " FireM " + FirebaseManager.getInstance().isOK() + " rm=" + node.removed);
        }

        internal override RemoteData getCurrentData() {
            RemoteData ans = ceneter.getCurrentData();
            ans.tag = KEY_TAG;
            return ans;
        }

        internal override void onInit(RemoteData d) {
            ceneter.setByData(d.to<PlayerQueuer.Data>());
        }

        internal override void onValueChnaged(RemoteData obj) {

            ceneter.setByData(obj.to<PlayerQueuer.Data>());
        }
    }
}
