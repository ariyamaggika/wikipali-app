using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingTexView : MonoBehaviour
{
    RectTransform rectTrans;
    Image img;
    public float speed = 1;
    public float maxFadeTime = 0.1f;
    // Start is called before the first frame update
    void Awake()
    {
        rectTrans = this.GetComponent<RectTransform>();
        img = this.GetComponent<Image>();
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
        //if (Input.GetKeyDown(KeyCode.Z))
        //    StartLoadingTex();
        //if (Input.GetKeyDown(KeyCode.X))
        //    StopLoadingTex();
        //if (fadeIn)
        //{
        //    curFadeTime += Time.smoothDeltaTime;
        //    if (curFadeTime >= maxFadeTime)
        //    {
        //        fadeIn = false;
        //    }
        //}
        //if (fadeOut)
        //{ 

        //}
    }
    bool isRotating;
    float curFadeTime;
    public void StartLoadingTex()
    {
        isRotating = true;
        this.gameObject.SetActive(true);
        FadeIn();
    }
    public void StopLoadingTex()
    {
        isRotating = false;
        this.gameObject.SetActive(false);
        FadeOut();
    }

    bool fadeIn = false;
    bool fadeOut = false;
    public void FadeIn()
    {
        curFadeTime = 0;
        fadeOut = false;
        fadeIn = true;
        //img.color = new Color(img.color.r, img.color.g, img.color.b, 0);
        img.CrossFadeAlpha(0, 0, true);
        img.CrossFadeAlpha(1, maxFadeTime, false);

    }
    public void FadeOut()
    {
        curFadeTime = 0;
        fadeIn = false;
        fadeOut = true;
        //img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
        img.CrossFadeAlpha(1, 0, true);
        img.CrossFadeAlpha(0, maxFadeTime, false);
    }
}
