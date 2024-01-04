using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static ArticleController;
using static C2SArticleGetNewDBInfo;

public class NewArticleView : MonoBehaviour
{
    public LoadingTexView loadingTexView;    //���±�����
    public GameObject listViewGO;
    public NewArticleNodeItemView nodeItem;
    public NewArticleScrollRectRef thisSrr;

    public NewArticleTitleReturnBtn returnBtn;

    //todo ͨ���Ķ����֣�ͨ��prefab�ʹ���
    //�������� paliԭ�ĺͷ���
    public ArticleContentScrollView contentView;


    //����ˢ��
    // Start is called before the first frame update
    void Start()
    {
        //thisSrr.callback1 = (state) => { s.SetActive(state); };
        //thisSrr.callback1 = (a) => { Debug.LogError(1); Debug.LogError(a); return null; };
        thisSrr.endDragUpdateCallback = () => { SendServerRequest(); Debug.LogError("ˢ���б�"); return null; };
        SendServerRequest();

    }
    //������ת����
    void StartLoadingAnim()
    {
        loadingTexView.transform.parent.gameObject.SetActive(true);
        loadingTexView.StartLoadingTex();
    }
    void StopLoadingAnim()
    {
        loadingTexView.transform.parent.gameObject.SetActive(false);
        loadingTexView.StopLoadingTex();
    }
    public void SendServerRequest()
    {
        StartLoadingAnim();
        C2SArticleGetNewDBInfo.GetNewArticleList(CallBack);
    }
    List<GameObject> nodeList = new List<GameObject>();
    //���������б�GO
    private void DestroyNodeList()
    {
        int length = nodeList.Count;
        if (length == 0)
            return;
        for (int i = 0; i < length; i++)
        {
            Destroy(nodeList[i]);
        }
        nodeList.Clear();
    }
    object CallBack(NewArticleListJson article)
    {
        StopLoadingAnim();

        //thisArticle = article;
        RefreshList(article);
        return null;
    }
    void RefreshList(NewArticleListJson article)
    {
        DestroyNodeList();
        if (article == null || article.data == null || article.data.rows == null || article.data.rows.Count == 0)
            return;
        List<NewArticleData> info = article.data.rows;
        int length = info.Count;
        float height = nodeItem.GetComponent<RectTransform>().sizeDelta.y;
        for (int i = 0; i < length; i++)
        {
            GameObject inst = Instantiate(nodeItem.gameObject, nodeItem.transform.parent);
            inst.transform.position = nodeItem.transform.position;
            inst.GetComponent<RectTransform>().position -= Vector3.up * height * i;

            inst.GetComponent<NewArticleNodeItemView>().Init(info[i],this);
            inst.SetActive(true);
            nodeList.Add(inst);
        }

    }
    public void ReturnBtnClick()
    {
        returnBtn.SetBackBtnClick(false);
        listViewGO.SetActive(true);
        contentView.gameObject.SetActive(false);
        returnBtn.SetText("����");
    }
    public void ArticleNodeBtnClick()
    {
        //contentView.gameObject.SetActive(false);
        returnBtn.SetBackBtnClick(true);
        contentView.gameObject.SetActive(true);
        returnBtn.SetText("����");
    }
    // Update is called once per frame
    void Update()
    {

    }
}
