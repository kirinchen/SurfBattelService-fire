using Newtonsoft.Json;
using RFNEet.firebase;
using surfm.tool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet.firebase {
    public class FirePlayerQueuer : FireObj {


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

        public PlayerQueuer.Data debugData;

      //  public List<string> realPids;

        void Awake() {
            meId = PidGeter.getPid();
            ceneter = gameObject.AddComponent<PlayerQueuer>();
        }

        void Start() {
            FirebaseManager.getInstance().addInitedAction(b => {
                init(KEY_PID, KEY_OID, false);
                node.initCallback.add(onFirstFetch);
            });
        }

        void Update() {
            debugData = ceneter.getCurrentData();
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
            ceneter.addPlayer(meId);
            getExRef().ValueChanged += onPidsChange;
        }


        private DBRefenece getExRef() {
            return node.dataFire.parent().Child(FireRepo.SKIP_KEY_PREFIX + KEY_EX_PIDS);
        }

        internal override void onRemoveAction() {
            getExRef().Child(meId).removeMe();
            base.onRemoveAction();
        }

        private void onPidsChange(DBResult obj) {
            string s = obj.getRawJsonValue();
            if (string.IsNullOrEmpty(s)) {
                getExRef().Child(meId).SetValueAsync(NistService.getTime().ToString("o"));
                return;
            }
            Dictionary<string, DateTime> d = JsonConvert.DeserializeObject<Dictionary<string, DateTime>>(s);
            if (!d.ContainsKey(meId)) {
                getExRef().Child(meId).SetValueAsync(NistService.getTime().ToString("o"));
                return;
            }
            ceneter.getCurrentData().playerIds = new List<string>(d.Keys);
            ceneter.getCurrentData().playerIds.Sort((a, b) => {
                return d[a].CompareTo(d[b]);
            });
            postData();
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
