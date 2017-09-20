using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RFNEet.firebase {
    public class FireConfig : MonoBehaviour {
        private static FireConfig instance;
        public string firebaseUrl = "https://apphi-224fb.firebaseio.com/";

        public static FireConfig getInstance() {
            if (instance == null) {
                instance = Resources.Load<FireConfig>("FireConfig");
            }
            return instance;
        }

    }
}
