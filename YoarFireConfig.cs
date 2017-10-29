using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RFNEet.firebase {
    public class YoarFireConfig : FireConfig {
#if Yoar

        public override string getFirebaseUrl() {
            return com.surfm.yoar.Constant.FIREBASE_URL;
        }

        public override string getRootNode() {
            return Application.productName;
        }

#endif
    }
}
