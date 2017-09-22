using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet.firebase {
    public class FirePlayerCenter : FireObj {


        private PlayerCenter cenetr;

        void Awake() {
            cenetr = GetComponent<PlayerCenter>();
        }

        void Start() {
            FirebaseManager.getInstance().addInitedAction(b => {
                init(pid, oid);
                cenetr.addPlayerIntoListener(s => {
                    postData();
                });
                cenetr.addTokenPlayerChangeListener(s => {
                    postData();
                });

            });
        }



        internal override RemoteData getCurrentData() {
            return cenetr.getCurrentData();
        }

        internal override void onInit(RemoteData d) {
            cenetr.setByData(d.to<PlayerCenter.Data>());
        }

        internal override void onValueChnaged(RemoteData obj) {
            cenetr.setByData(obj.to<PlayerCenter.Data>());
        }
    }
}
