using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunCtrl : MonoBehaviour {
    
    void Update () {
        gameObject.transform.Rotate(new Vector3(0, Time.deltaTime * 0.1f, 0));
    }
}
