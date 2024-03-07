using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//������ּ���ת�ջ�ҳ��
public class LoadingViewManager
{    
    //����ʽ������.�ڵ�һ�ε��õ�ʱ��ʵ�����Լ� 
    private LoadingViewManager() { }
    private static LoadingViewManager manager = null;
    //��̬�������� 
    public static LoadingViewManager Instance()
    {
        if (manager == null)
        {
            manager = new LoadingViewManager();
        }
        return manager;
    }

    public LoadingTexView titleLoadingView;
    

    public void StartTitleLoadingAnim()
    {
        titleLoadingView.StartLoadingTex();
    }
    public void StopTitleLoadingAnim()
    {
        titleLoadingView.StopLoadingTex();
    }


    public void StartArticleLoadingAnim()
    {

    }
    public void StopArticleLoadingAnim()
    {

    }




}
