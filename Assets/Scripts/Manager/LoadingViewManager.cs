using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//管理各种加载转菊花页面
public class LoadingViewManager
{    
    //懒汉式单例类.在第一次调用的时候实例化自己 
    private LoadingViewManager() { }
    private static LoadingViewManager manager = null;
    //静态工厂方法 
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
