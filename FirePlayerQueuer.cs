using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet.firebase {
    public class FirePlayerQueuer : FireObj {


        public enum ChangePostType {
            JustME, ALLChange,
        }

        public static readonly string KEY_TAG = "@FPQ";
        private static readonly string KEY_PID = "@C";
        private static readonly string KEY_OID = "@FirePlayerQueuer";

        private PlayerQueuer ceneter;
        public string meId { get; private set; }
        public ChangePostType changePostType;

        void Awake() {
            meId = PidGeter.getPid();
            ceneter = gameObject.AddComponent<PlayerQueuer>();
        }

        void Start() {
            FirebaseManager.getInstance().addInitedAction(b => {
                init(KEY_PID, KEY_OID);
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
            });
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
            Debug.Log("onValueChnaged" + obj);
        }
    }
}
