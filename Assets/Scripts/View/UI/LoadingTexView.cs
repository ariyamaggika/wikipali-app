using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingTexView : MonoBehaviour
{
    RectTransform rectTrans;
    // Start is called before the first frame update
    void Start()
    {
        rectTrans = this.GetComponent<RectTransform>();
    }
    int rotate = 0;
    // Update is called once per frame
    void Update()
    {
        rotate = (rotate + 1) % 360;
        rectTrans.eulerAngles = new Vector3(0, 0, rotate);
    }
    bool isRotating;
    public void StartRotateSelf()
    {
        isRotating = true;
    }
    public void StopRotateSelf()
    {
        isRotating = false;

    }
}
