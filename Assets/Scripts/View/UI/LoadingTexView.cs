using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingTexView : MonoBehaviour
{
    RectTransform rectTrans;
    public float speed = 1;
    // Start is called before the first frame update
    void Start()
    {
        rectTrans = this.GetComponent<RectTransform>();
    }
    float rotate = 0;
    // Update is called once per frame
    void Update()
    {
        if (isRotating)
        {
            rotate = (rotate + speed * Time.smoothDeltaTime);// % 360;
            if (rotate > 360)
                rotate = 0;
            rectTrans.eulerAngles = new Vector3(0, 0, rotate);
        }
    }
    bool isRotating;
    public void StartLoadingTex()
    {
        isRotating = true;
        this.gameObject.SetActive(true);
    }
    public void StopLoadingTex()
    {
        isRotating = false;
        this.gameObject.SetActive(false);

    }
}
