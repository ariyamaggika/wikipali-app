using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArticleLoadingView : MonoBehaviour
{
    public LoadingTexView loadingTexView;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //todo ��ӳ�ʱˢ�¹���
    public void StartLoading()
    {
        this.gameObject.SetActive(true);
        loadingTexView.StartLoadingTex();
    }
    public void StopLoading()
    {
        this.gameObject.SetActive(false);
        loadingTexView.StopLoadingTex();
    }
}
