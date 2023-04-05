﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static DictManager;
//todo:这个面板做成prefab加载，可以显示多个
public class CommonGroupView : MonoBehaviour
{
    public Button returnBtn;
    public Button addBtn;
    public Text titleText;
    //DicGroupPopView
    public ItemDicGroupWordView wordItem;
    DicGroupInfo dicGroupInfo;
    public void InitDicGroupWordView(DicGroupInfo _dicGroupInfo)
    {
        dicGroupInfo = _dicGroupInfo;
        addBtn.gameObject.SetActive(false);
        titleText.text = dicGroupInfo.groupName;
        RefreshGroupList();
    }
    void Start()
    {
        returnBtn.onClick.AddListener(OnCloseBtnClick);
        addBtn.onClick.AddListener(OnAddBtnClick);
    }
    public void OnCloseBtnClick()
    {
        DelAllListGO();
        this.gameObject.SetActive(false);
    }
    public void OnAddBtnClick()
    {

    }
    public void DelAllListGO()
    {
        int l = itemList.Count;
        for (int i = 0; i < l; i++)
        {
            Destroy(itemList[i].gameObject);
        }
        itemList.Clear();

    }
    List<ItemDicGroupWordView> itemList = new List<ItemDicGroupWordView>();
    /// <summary> 
    /// 刷新分组信息
    /// </summary>
    public void RefreshGroupList()
    {
        DelAllListGO();
        int l = itemList.Count;
        for (int i = 0; i < l; i++)
        {
            Destroy(itemList[i].gameObject);
        }
        itemList.Clear();

        int gl = dicGroupInfo.wordList.Count;
        for (int i = 0; i < gl; i++)
        {
            GameObject inst = Instantiate(wordItem.gameObject, wordItem.transform.parent, false);
            inst.transform.position = wordItem.transform.position;
            //inst.GetComponent<RectTransform>().position -= Vector3.up * height;
            ItemDicGroupWordView iv = inst.GetComponent<ItemDicGroupWordView>();
            iv.Init(dicGroupInfo.wordList[i], dicGroupInfo.groupID, this);
            inst.SetActive(true);
            itemList.Add(iv);
        }

    }
}