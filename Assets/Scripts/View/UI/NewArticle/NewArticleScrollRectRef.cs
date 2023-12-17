using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NewArticleScrollRectRef : ScrollRect
{
    //����ˢ�¸߶Ⱦ���
    float heightThreshold = 120;
    //�߶� �������Ǹ���   ������������
    //float f = -30f;
    //�Ƿ�ˢ��
    bool isRef = false;
    //�Ƿ����϶�
    bool isDrag = false;
    //��ʾ������ˢ���ֶ�
    public Func<bool, object> callback1;
    //�������ˢ������ ִ�еķ���
    public Func<object> endDragUpdateCallback;

    protected override void Awake()
    {
        base.Awake();
        onValueChanged.AddListener(ScrollValueChanged);
    }

    /// <summary>
    /// ��ScrollRect���϶�ʱ
    /// </summary>
    /// <param name="vector">���϶��ľ�����Content�Ĵ�С����</param>
    void ScrollValueChanged(Vector2 vector)
    {
        //������϶� ��Ȼ��ִ��֮�µĴ���
        if (!isDrag)
            return;
        //�������Content
        RectTransform rect = GetComponentInChildren<ContentSizeFitter>().GetComponent<RectTransform>();
        //Debug.LogError(rect.transform.localPosition.y);
        //Debug.LogError(rect.localPosition.y);
        //Debug.LogError(rect.rect.height);
        //Debug.LogError(vector.y);
        //Debug.LogError(rect.rect.height * vector.y);
        //Debug.LogError("----------------------");
        //vector.y == 1 ����
        //vector.y == 0 ����
        //rect.localPosition.y<0����
        //rect.localPosition.y>0����
        //����϶��ľ�����ڸ�����ֵ
        if (rect.localPosition.y > heightThreshold)//��������
        {
            isRef = false;
            //callback1?.Invoke(false);
        }
        else if (rect.localPosition.y < -heightThreshold)//����ˢ��
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