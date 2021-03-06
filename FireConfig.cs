﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet.firebase {
    public class FireConfig : MonoBehaviour {
        private static FireConfig instance;
        public string firebaseUrl = "https://apphi-224fb.firebaseio.com/";
        public string rootNode = "/";


        public virtual string getFirebaseUrl() {
            return firebaseUrl;
        }

        public virtual string getRootNode() {
            return rootNode;
        }


        public static void setInstance(FireConfig fc) {
            instance = fc;
        }

        public static FireConfig getInstance() {
            if (instance == null) {
                instance = Resources.Load<FireConfig>("@RFNEetFireConfig");
            }
            return instance;
        }

    }
}
