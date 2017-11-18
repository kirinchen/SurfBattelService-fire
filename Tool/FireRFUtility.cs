using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RFNEet.firebase {
    public class FireRFUtility {
        internal static bool hasToke() {
            return string.Equals(FirebaseManager.getMePid(), PlayerQueuer.instance.getTokenPlayer());
        }
    }
}
