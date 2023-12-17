using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NewArticleScrollRectRef : ScrollRect
{
    //下拉刷新高度距离
    float heightThreshold = 120;
    //高度 往下拉是负数   往上拉是正数
    //float f = -30f;
    //是否刷新
    bool isRef = false;
    //是否处于拖动
    bool isDrag = false;
    //显示、隐藏刷新字段
    public Func<bool, object> callback1;
    //如果满足刷新条件 执行的方法
    public Func<object> endDragUpdateCallback;

    protected override void Awake()
    {
        base.Awake();
        onValueChanged.AddListener(ScrollValueChanged);
    }

    /// <summary>
    /// 当ScrollRect被拖动时
    /// </summary>
    /// <param name="vector">被拖动的距离与Content的大小比例</param>
    void ScrollValueChanged(Vector2 vector)
    {
        //如果不拖动 当然不执行之下的代码
        if (!isDrag)
            return;
        //这个就是Content
        RectTransform rect = GetComponentInChildren<ContentSizeFitter>().GetComponent<RectTransform>();
        //Debug.LogError(rect.transform.localPosition.y);
        //Debug.LogError(rect.localPosition.y);
        //Debug.LogError(rect.rect.height);
        //Debug.LogError(vector.y);
        //Debug.LogError(rect.rect.height * vector.y);
        //Debug.LogError("----------------------");
        //vector.y == 1 上拉
        //vector.y == 0 下拉
        //rect.localPosition.y<0上拉
        //rect.localPosition.y>0下拉
        //如果拖动的距离大于给定的值
        if (rect.localPosition.y > heightThreshold)//上拉加载
        {
            isRef = false;
            //callback1?.Invoke(false);
        }
        else if (rect.localPosition.y < -heightThreshold)//下拉刷新
        {
            isRef = true;
            // callback1?.Invoke(true);
        }
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        isDrag = true;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        //callback1?.Invoke(false);
        if (isRef)
            endDragUpdateCallback?.Invoke();
        isRef = false;
        isDrag = false;
    }

}