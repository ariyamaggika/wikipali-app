using Imdork.SQLite;
using iTextSharp.text;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using static ArticleController;
using static SettingManager;

public class ArticleManager
{
    //懒汉式单例类.在第一次调用的时候实例化自己 
    private ArticleManager() { }
    private static ArticleManager manager = null;
    //静态工厂方法 
    public static ArticleManager Instance()
    {
        if (manager == null)
        {
            manager = new ArticleManager();
        }
        return manager;
    }
    public DBManager dbManager = DBManager.Instance();
    public ArticleView articleView;

    /// <summary>
    /// 读取本地文件夹中的Json文件
    /// <param name="jsonName">json文件名或文件名路径</param>
    /// </summary>
    string ReadJsonFromStreamingAssetsPath(string jsonName)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(jsonName);
        return textAsset.text;
    }
    #region 读取Json目录树
    const string defualtJsonFilePath = "defualt";
    const string cscd4JsonFilePath = "cscd";
    public string ReadDefualtJson()
    {
        return ReadJsonFromStreamingAssetsPath("Json/PalicanonCategory/" + defualtJsonFilePath);
    }
    public string ReadCSCDJson()
    {
        return ReadJsonFromStreamingAssetsPath("Json/PalicanonCategory/" + cscd4JsonFilePath);
    }
    #endregion
    #region 读取Json目录翻译
    Dictionary<Language, string> languageTSPath = new Dictionary<Language, string>()
    {
        //{ "pali","default"},
        { Language.ZH_CN,"zh-cn"},
        { Language.ZH_TW,"zh-tw"},
        { Language.EN,"en"},
        { Language.JP,"en"},
        { Language.MY,"my"},
        { Language.SI,"si"},
        { Language.TH,"th"},
    };

    public string ReadCurrLanguageBookJson()
    {
        return ReadJsonFromStreamingAssetsPath("Json/book_index/a/" + languageTSPath[SettingManager.Instance().language]);
    }
    public string ReadPaliBookJson()
    {
        return ReadJsonFromStreamingAssetsPath("Json/book_index/a/default");
    }
    #endregion
    #region 读取Json圣典目录翻译

    public string ReadArticleCatalogueJson()
    {
        return ReadJsonFromStreamingAssetsPath("Json/ArticleCatalogue/term-vocabulary");
    }

    #endregion
    #region 读取数据库目录树
    public class BookDBData
    {
        public int id;
        public string toc;
        public int level;
        public int paragraph;//段落数
        public int chapter_len;//章节paragraph长度
        public int parent;//是父的paragraph?
        //public string translateName;
    }
    /// <summary>
    /// 输入Tag，返回book数据
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public (List<BookDBData>, List<int>) GetBooksFromTags(List<string> tag)
    {
        int length = tag.Count;
        List<BookDBData> bookList = new List<BookDBData>();
        List<int> bookIDList = new List<int>();
        if (tag == null || length == 0)
            return (bookList, bookIDList);
        List<string> anchorIDList = new List<string>();
        dbManager.Getdb(db =>
        {
            //查找tagID
            var readerTag = db.SelectArticleTag(tag.ToArray());
            Dictionary<string, object>[] tagPairs = SQLiteTools.GetValues(readerTag);
            //因为有orderby 减少循环次数
            bool isCoCountMatch = false;
            string tagCount = length.ToString();
            if (tagPairs != null)
            {
                int tagLength = tagPairs.Length;
                for (int t = 0; t < tagLength; t++)
                {
                    //取所有tag获取到的结果的交集
                    string anchorID = tagPairs[t]["anchor_id"].ToString();
                    string co = tagPairs[t]["co"].ToString();
                    if (co == tagCount)
                    {
                        isCoCountMatch = true;
                        anchorIDList.Add(anchorID);
                    }
                    else
                    {
                        if (isCoCountMatch)
                            break;
                    }
                };
            }

            var readerPali = db.SelectArticle(anchorIDList.ToArray());
            Dictionary<string, object>[] paliPairs = SQLiteTools.GetValues(readerPali);
            if (paliPairs != null)
            {
                int paliLength = paliPairs.Length;
                for (int p = 0; p < paliLength; p++)
                {
                    string toc = "";
                    if (paliPairs[p].ContainsKey("toc"))
                        toc = paliPairs[p]["toc"].ToString();
                    BookDBData book = new BookDBData()
                    {
                        id = int.Parse(paliPairs[p]["book"].ToString()),
                        paragraph = int.Parse(paliPairs[p]["paragraph"].ToString()),
                        level = int.Parse(paliPairs[p]["level"].ToString()),
                        toc = toc,
                        chapter_len = int.Parse(paliPairs[p]["chapter_len"].ToString()),
                        parent = int.Parse(paliPairs[p]["parent"].ToString()),
                    };
                    bookList.Add(book);
                    if (!bookIDList.Contains(book.id))
                        bookIDList.Add(book.id);
                }
            }

        }, DBManager.SentencePaliDBurl);
        return (bookList, bookIDList);
    }
    /// <summary>
    /// 输入BookID，返回指定pargraph范围内的level>2&&<100的book数据
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public List<BookDBData> GetBookChildrenFromID(int bookID, int minPargraph, int maxPargraph)
    {

        List<BookDBData> bookList = new List<BookDBData>();

        dbManager.Getdb(db =>
        {
            var readerPali = db.SelectArticleChildren(bookID.ToString(), minPargraph.ToString(), maxPargraph.ToString());
            Dictionary<string, object>[] paliPairs = SQLiteTools.GetValues(readerPali);
            if (paliPairs != null)
            {
                int paliLength = paliPairs.Length;
                for (int p = 0; p < paliLength; p++)
                {
                    string toc = "";
                    if (paliPairs[p].ContainsKey("toc"))
                        toc = paliPairs[p]["toc"].ToString();

                    BookDBData book = new BookDBData()
                    {
                        id = int.Parse(paliPairs[p]["book"].ToString()),
                        paragraph = int.Parse(paliPairs[p]["paragraph"].ToString()),
                        level = int.Parse(paliPairs[p]["level"].ToString()),
                        toc = toc,
                        chapter_len = int.Parse(paliPairs[p]["chapter_len"].ToString()),
                        parent = int.Parse(paliPairs[p]["parent"].ToString()),
                    };
                    bookList.Add(book);
                }
            }

        }, DBManager.SentencePaliDBurl);
        return bookList;
    }
    /// <summary>
    /// 输入BookID，pargraph返回指定book数据
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public BookDBData GetBookChildrenFromID(int bookID, int pargraph)
    {
        BookDBData res = null;
        dbManager.Getdb(db =>
        {
            var readerPali = db.SelectArticle(bookID, pargraph);
            Dictionary<string, object> paliPair = SQLiteTools.GetValue(readerPali);
            if (paliPair != null)
            {
                string toc = "";
                if (paliPair.ContainsKey("toc"))
                    toc = paliPair["toc"].ToString();
                BookDBData book = new BookDBData()
                {
                    id = int.Parse(paliPair["book"].ToString()),
                    paragraph = int.Parse(paliPair["paragraph"].ToString()),
                    level = int.Parse(paliPair["level"].ToString()),
                    toc = toc,
                    chapter_len = int.Parse(paliPair["chapter_len"].ToString()),
                    parent = int.Parse(paliPair["parent"].ToString()),
                };
                res = book;
            }

        }, DBManager.SentencePaliDBurl);
        return res;
    }

    public class ChannelChapterDBData
    {
        public string channel_id;
        public string name;
        public Language language;
        public string summary;
    }
    public class ChapterDBData
    {
        public string id;
        public int bookID;
        public int paragraph;//段落数
        public string language;
        public string title;
        public string channel_id;
        public ChannelChapterDBData channelData;
        public float progress;
        //public Date
        //public string translateName;
    }
    public ChapterDBData GetChapter(int bookID, int paragraph, string channel_id)
    {

        ChapterDBData res = null;
        dbManager.Getdb(db =>
        {
            var readerPali = db.SelectChapter(bookID, paragraph, channel_id);
            Dictionary<string, object> paliPair = SQLiteTools.GetValue(readerPali);
            if (paliPair != null)
            {
                string title = "";
                if (paliPair.ContainsKey("title"))
                    title = paliPair["title"].ToString();
                string language = "pali";
                if (paliPair.ContainsKey("language"))
                    language = paliPair["language"].ToString();
                ChapterDBData c = new ChapterDBData()
                {
                    id = paliPair["id"].ToString(),
                    bookID = int.Parse(paliPair["book"].ToString()),
                    paragraph = int.Parse(paliPair["paragraph"].ToString()),
                    language = language,
                    title = title,
                    channel_id = paliPair["channel_id"].ToString(),
                    progress = float.Parse(paliPair["progress"].ToString()),
                };
                res = c;
            }

        }, DBManager.SentenceDBIndexurl);
        return res;
    }
    /// <summary>
    /// 输入bookID List，返回chapter数据
    /// </summary>
    public List<ChapterDBData> GetChaptersFromBookIDs(List<int> bookIDList)
    {
        List<ChapterDBData> cList = new List<ChapterDBData>();
        if (bookIDList == null || bookIDList.Count == 0)
            return cList;
        dbManager.Getdb(db =>
        {

            var readerPali = db.SelectChapter(bookIDList.ToArray());
            Dictionary<string, object>[] paliPairs = SQLiteTools.GetValues(readerPali);
            if (paliPairs != null)
            {
                int paliLength = paliPairs.Length;
                for (int p = 0; p < paliLength; p++)
                {

                    string title = "";
                    if (paliPairs[p].ContainsKey("title"))
                        title = paliPairs[p]["title"].ToString();


                    //int.Parse(paliPairs[p]["book"].ToString());
                    //int.Parse(paliPairs[p]["paragraph"].ToString());
                    string language = "pali";
                    if (paliPairs[p].ContainsKey("language"))
                        language = paliPairs[p]["language"].ToString();
                    //paliPairs[p]["language"].ToString();
                    //paliPairs[p]["channel_id"].ToString();
                    //float.Parse(paliPairs[p]["progress"].ToString());

                    ChapterDBData c = new ChapterDBData()
                    {
                        id = paliPairs[p]["id"].ToString(),
                        bookID = int.Parse(paliPairs[p]["book"].ToString()),
                        paragraph = int.Parse(paliPairs[p]["paragraph"].ToString()),
                        language = language,
                        title = title,
                        channel_id = paliPairs[p]["channel_id"].ToString(),
                        progress = float.Parse(paliPairs[p]["progress"].ToString()),
                    };
                    cList.Add(c);
                }
            }

        }, DBManager.SentenceDBIndexurl);
        return cList;
    }
    public List<ChapterDBData> GetChaptersFromBookID(int bookID)
    {
        List<ChapterDBData> cList = new List<ChapterDBData>();
        dbManager.Getdb(db =>
        {

            var readerPali = db.SelectChapter(bookID);
            Dictionary<string, object>[] paliPairs = SQLiteTools.GetValues(readerPali);
            if (paliPairs != null)
            {
                int paliLength = paliPairs.Length;
                for (int p = 0; p < paliLength; p++)
                {

                    string title = "";
                    if (paliPairs[p].ContainsKey("title"))
                        title = paliPairs[p]["title"].ToString();


                    //int.Parse(paliPairs[p]["book"].ToString());
                    //int.Parse(paliPairs[p]["paragraph"].ToString());
                    string language = "pali";
                    if (paliPairs[p].ContainsKey("language"))
                        language = paliPairs[p]["language"].ToString();
                    //paliPairs[p]["language"].ToString();
                    //paliPairs[p]["channel_id"].ToString();
                    //float.Parse(paliPairs[p]["progress"].ToString());

                    ChapterDBData c = new ChapterDBData()
                    {
                        id = paliPairs[p]["id"].ToString(),
                        bookID = int.Parse(paliPairs[p]["book"].ToString()),
                        paragraph = int.Parse(paliPairs[p]["paragraph"].ToString()),
                        language = language,
                        title = title,
                        channel_id = paliPairs[p]["channel_id"].ToString(),
                        progress = float.Parse(paliPairs[p]["progress"].ToString()),
                    };
                    cList.Add(c);
                }
            }

        }, DBManager.SentenceDBIndexurl);
        return cList;
    }
    public List<ChapterDBData> GetChaptersSearchTitle(string input)
    {
        List<ChapterDBData> cList = new List<ChapterDBData>();
        dbManager.Getdb(db =>
        {

            var readerPali = db.SearchChapter(input, SEARCH_LIMIT_COUNT);
            Dictionary<string, object>[] paliPairs = SQLiteTools.GetValues(readerPali);
            if (paliPairs != null)
            {
                int paliLength = paliPairs.Length;
                for (int p = 0; p < paliLength; p++)
                {

                    string title = "";
                    if (paliPairs[p].ContainsKey("title"))
                        title = paliPairs[p]["title"].ToString();


                    //int.Parse(paliPairs[p]["book"].ToString());
                    //int.Parse(paliPairs[p]["paragraph"].ToString());
                    string language = "pali";
                    if (paliPairs[p].ContainsKey("language"))
                        language = paliPairs[p]["language"].ToString();
                    //paliPairs[p]["language"].ToString();
                    //paliPairs[p]["channel_id"].ToString();
                    //float.Parse(paliPairs[p]["progress"].ToString());

                    ChapterDBData c = new ChapterDBData()
                    {
                        id = paliPairs[p]["id"].ToString(),
                        bookID = int.Parse(paliPairs[p]["book"].ToString()),
                        paragraph = int.Parse(paliPairs[p]["paragraph"].ToString()),
                        language = language,
                        title = title,
                        channel_id = paliPairs[p]["channel_id"].ToString(),
                        progress = float.Parse(paliPairs[p]["progress"].ToString()),
                    };
                    c.title = c.title.Replace("\n","");
                    c.title = c.title.Replace(input, CommonTool.COLOR_BLUE_FLAG + input + "</color>");
                    cList.Add(c);
                }
            }

        }, DBManager.SentenceDBIndexurl);
        return cList;
    }
    /// <summary>
    /// 返回channel数据，key : channelID ,value:channelData
    /// </summary>
    public Dictionary<string, ChannelChapterDBData> GetChannelDataByIDs(List<string> channelIDList)
    {
        Dictionary<string, ChannelChapterDBData> data = new Dictionary<string, ChannelChapterDBData>();
        if (channelIDList == null || channelIDList.Count == 0)
            return data;
        dbManager.Getdb(db =>
        {

            var readerPali = db.SelectChannel(channelIDList.ToArray());
            Dictionary<string, object>[] paliPairs = SQLiteTools.GetValues(readerPali);
            if (paliPairs != null)
            {
                int paliLength = paliPairs.Length;
                for (int p = 0; p < paliLength; p++)
                {

                    //?????默认为null的是中文？
                    Language l = Language.ZH_CN;
                    if (paliPairs[p].ContainsKey("language"))
                    {
                        string language = paliPairs[p]["language"].ToString();
                        switch (language)
                        {
                            case "zh":
                            case "zh-cn":
                                l = Language.ZH_CN;
                                break;
                            case "zh-tw":
                                l = Language.ZH_TW;
                                break;
                            case "en":
                            case "jp":
                                l = Language.EN;
                                break;
                            case "my":
                                l = Language.MY;
                                break;
                            case "si":
                                l = Language.MY;
                                break;
                            case "th":
                                l = Language.TH;
                                break;
                        }
                    }

                    string summary = "";
                    if (paliPairs[p].ContainsKey("summary"))
                        summary = paliPairs[p]["summary"].ToString();
                    string name = "";
                    if (paliPairs[p].ContainsKey("name"))
                        name = paliPairs[p]["name"].ToString();

                    ChannelChapterDBData c = new ChannelChapterDBData()
                    {
                        channel_id = paliPairs[p]["id"].ToString(),
                        name = name,
                        language = l,
                        summary = summary,
                    };
                    data.Add(c.channel_id, c);
                }
            }
        }, DBManager.SentenceDBIndexurl);
        return data;
    }
    /// <summary>
    /// 返回channel数据，key : channelID ,value:channelData
    /// </summary>
    public ChannelChapterDBData GetChannelDataByID(string channelID)
    {
        ChannelChapterDBData data = null;

        dbManager.Getdb(db =>
        {

            var readerPali = db.SelectChannel(channelID);
            Dictionary<string, object> paliPair = SQLiteTools.GetValue(readerPali);
            if (paliPair != null)
            {

                //?????默认为null的是中文？
                Language l = Language.ZH_CN;
                if (paliPair.ContainsKey("language"))
                {
                    string language = paliPair["language"].ToString();
                    switch (language)
                    {
                        case "zh":
                        case "zh-cn":
                            l = Language.ZH_CN;
                            break;
                        case "zh-tw":
                            l = Language.ZH_TW;
                            break;
                        case "en":
                        case "jp":
                            l = Language.EN;
                            break;
                        case "my":
                            l = Language.MY;
                            break;
                        case "si":
                            l = Language.MY;
                            break;
                        case "th":
                            l = Language.TH;
                            break;
                    }
                }

                string summary = "";
                if (paliPair.ContainsKey("summary"))
                    summary = paliPair["summary"].ToString();
                string name = "";
                if (paliPair.ContainsKey("name"))
                    name = paliPair["name"].ToString();

                ChannelChapterDBData c = new ChannelChapterDBData()
                {
                    channel_id = paliPair["id"].ToString(),
                    name = name,
                    language = l,
                    summary = summary,
                };
                data = c;
            }

        }, DBManager.SentenceDBIndexurl);
        return data;
    }
    #endregion
    #region 读取数据库句子与释义
    public class SentenceDBData
    {
        //public string id;
        public int bookID;
        public int paragraph;
        public int word_start;
        public int word_end;
        public string content;
        //
        public string channel_id;
        public SentenceDBData()
        {
        }
        public SentenceDBData(int _bookID, int _paragraph, int _word_start, int _word_end, string _content)
        {
            bookID = _bookID;
            paragraph = _paragraph;
            word_start = _word_start;
            word_end = _word_end;
            content = _content;
        }
        public SentenceDBData(int _bookID, int _paragraph, int _word_start, int _word_end, string _content, string _channel_id)
        {
            bookID = _bookID;
            paragraph = _paragraph;
            word_start = _word_start;
            word_end = _word_end;
            content = _content;
            channel_id = _channel_id;
        }
    }
    public List<SentenceDBData> GetPaliSentenceByBookParagraph(int bookID, int min, int max)
    {
        List<SentenceDBData> res = new List<SentenceDBData>();
        dbManager.Getdb(db =>
        {

            var readerPali = db.SelectSentence(bookID, min.ToString(), max.ToString());
            Dictionary<string, object>[] paliPairs = SQLiteTools.GetValues(readerPali);
            if (paliPairs != null)
            {
                int paliLength = paliPairs.Length;
                for (int p = 0; p < paliLength; p++)
                {
                    string content = "";
                    if (paliPairs[p].ContainsKey("content"))
                        content = paliPairs[p]["content"].ToString();
                    SentenceDBData s = new SentenceDBData()
                    {
                        //id = paliPairs[p]["id"].ToString(),
                        bookID = int.Parse(paliPairs[p]["book"].ToString()),
                        paragraph = int.Parse(paliPairs[p]["paragraph"].ToString()),
                        word_start = int.Parse(paliPairs[p]["word_start"].ToString()),
                        word_end = int.Parse(paliPairs[p]["word_end"].ToString()),
                        content = content,

                    };
                    res.Add(s);
                }
            }
        }, DBManager.SentencePaliDBurl);
        return res;
    }
    public List<SentenceDBData> GetPaliSentenceTranslationByBookParagraph(int bookID, int min, int max, string channel)
    {
        List<SentenceDBData> res = new List<SentenceDBData>();
        dbManager.Getdb(db =>
        {

            var readerPali = db.SelectSentenceTranslation(bookID, min.ToString(), max.ToString(), channel);
            Dictionary<string, object>[] paliPairs = SQLiteTools.GetValues(readerPali);
            if (paliPairs != null)
            {
                int paliLength = paliPairs.Length;
                for (int p = 0; p < paliLength; p++)
                {
                    string content = "";
                    if (paliPairs[p].ContainsKey("content"))
                        content = paliPairs[p]["content"].ToString();
                    SentenceDBData s = new SentenceDBData()
                    {
                        //id = paliPairs[p]["id"].ToString(),
                        bookID = int.Parse(paliPairs[p]["book"].ToString()),
                        paragraph = int.Parse(paliPairs[p]["paragraph"].ToString()),
                        word_start = int.Parse(paliPairs[p]["word_start"].ToString()),
                        word_end = int.Parse(paliPairs[p]["word_end"].ToString()),
                        content = content,
                    };
                    res.Add(s);
                }
            }
        }, DBManager.SentenceDBurl);
        return res;
    }
    #endregion
    #region 搜索单词匹配句子和标题返回文章信息
    const int SEARCH_LIMIT_COUNT = 20;
    public List<SentenceDBData> GetSentencesChineseByWord(string word)
    {
        NetPackLogicEnum netPackEnum = ArticleManager.Instance().CheckIsUseOfflinePack();
        List<SentenceDBData> res = new List<SentenceDBData>();
        //检测离线包
        if (netPackEnum == NetPackLogicEnum.OfflineWithPack)
        {
            dbManager.Getdb(db =>
            {

                var readerPali = db.SelectSentencesTranslationByWord(word, SEARCH_LIMIT_COUNT * 3);
                Dictionary<string, object>[] paliPairs = SQLiteTools.GetValues(readerPali);
                if (paliPairs != null)
                {
                    int paliLength = paliPairs.Length;
                    for (int p = 0; p < paliLength; p++)
                    {
                        string content = "";
                        if (paliPairs[p].ContainsKey("content"))
                            content = paliPairs[p]["content"].ToString();
                        SentenceDBData s = new SentenceDBData()
                        {
                            //id = paliPairs[p]["id"].ToString(),
                            bookID = int.Parse(paliPairs[p]["book"].ToString()),
                            paragraph = int.Parse(paliPairs[p]["paragraph"].ToString()),
                            word_start = int.Parse(paliPairs[p]["word_start"].ToString()),
                            word_end = int.Parse(paliPairs[p]["word_end"].ToString()),
                            channel_id = paliPairs[p]["channel_id"].ToString(),
                            content = content,
                        };
                        s.content = s.content.Replace(word, CommonTool.COLOR_BLUE_FLAG + word + "</color>");
                        res.Add(s);
                    }
                }
            }, DBManager.SentenceDBurl);//todo 有离线包才行
        }
        return res;
    }
    public List<SentenceDBData> GetSentencesAllByWord(string word)
    {
        List<SentenceDBData> res = new List<SentenceDBData>();
        List<SentenceDBData> resTrans = new List<SentenceDBData>();
        dbManager.Getdb(db =>
        {

            var readerPali = db.SelectSentencesByWord(word, SEARCH_LIMIT_COUNT);
            Dictionary<string, object>[] paliPairs = SQLiteTools.GetValues(readerPali);
            if (paliPairs != null)
            {
                int paliLength = paliPairs.Length;
                for (int p = 0; p < paliLength; p++)
                {
                    string content = "";
                    if (paliPairs[p].ContainsKey("content"))
                        content = paliPairs[p]["content"].ToString();
                    SentenceDBData s = new SentenceDBData()
                    {
                        ////id = paliPairs[p]["id"].ToString(),
                        bookID = int.Parse(paliPairs[p]["book"].ToString()),
                        paragraph = int.Parse(paliPairs[p]["paragraph"].ToString()),
                        word_start = int.Parse(paliPairs[p]["word_start"].ToString()),
                        word_end = int.Parse(paliPairs[p]["word_end"].ToString()),
                        content = content,
                    };
                    s.content = s.content.Replace(word, CommonTool.COLOR_BLUE_FLAG + word + "</color>");

                    res.Add(s);
                }
            }
            //if (res.Count < 30)
            //{
            //    var readerPaliTrans = db.SelectSentencesTranslationByWord(word, SEARCH_LIMIT_COUNT);
            //    Dictionary<string, object>[] paliPairsTrans = SQLiteTools.GetValues(readerPaliTrans);
            //    if (paliPairsTrans != null)
            //    {
            //        int paliLength = paliPairsTrans.Length;
            //        for (int p = 0; p < paliLength; p++)
            //        {
            //            string content = "";
            //            if (paliPairsTrans[p].ContainsKey("content"))
            //                content = paliPairsTrans[p]["content"].ToString();
            //            SentenceDBData s = new SentenceDBData()
            //            {
            //                //id = paliPairs[p]["id"].ToString(),
            //                bookID = int.Parse(paliPairsTrans[p]["book"].ToString()),
            //                paragraph = int.Parse(paliPairsTrans[p]["paragraph"].ToString()),
            //                word_start = int.Parse(paliPairsTrans[p]["word_start"].ToString()),
            //                word_end = int.Parse(paliPairsTrans[p]["word_end"].ToString()),
            //                channel_id = paliPairs[p]["channel_id"].ToString(),
            //                content = content,
            //            };
            //            res.Add(s);
            //            if (res.Count >= 30)
            //                break;
            //            //resTrans.Add(s);
            //        }
            //    }
            //}
            //todo 排序


        }, DBManager.SentencePaliDBurl);
        return res;
    }
    #endregion

    #region 文章收藏
    public class ArticleGroupInfo
    {
        public int groupID;
        public string groupName;
        public List<string> bookTitleList;
        public List<int> bookIDList;
        public List<int> bookParagraphList;
        public List<int> bookChapterLenList;
        public List<string> channelIDList;//channel ID为空是pali原文
        public List<string> channelNameList;//channel ID为空是pali原文
    }

    //所有单词本
    public List<ArticleGroupInfo> allArticleGroup = new List<ArticleGroupInfo>();
    public int articleGroupCount;
    /// <summary>
    /// 加载所有收藏
    /// </summary>
    public void LoadAllArticleGroup()
    {
        int groupCount = PlayerPrefs.GetInt("articleGroupCount");
        string[] dicGroupNameArr = PlayerPrefsX.GetStringArray("articleGroupName");
        allArticleGroup.Clear();
        for (int i = 0; i < groupCount; i++)
        {
            ArticleGroupInfo dg = new ArticleGroupInfo();
            dg.groupID = i;
            dg.groupName = dicGroupNameArr[i];
            dg.bookTitleList = new List<string>();
            dg.bookIDList = new List<int>();
            dg.bookParagraphList = new List<int>();
            dg.bookChapterLenList = new List<int>();
            dg.channelIDList = new List<string>();
            dg.channelNameList = new List<string>();
            string[] articleTitleArr = PlayerPrefsX.GetStringArray("articleTitle" + i);
            int[] bookIDArr = PlayerPrefsX.GetIntArray("bookID" + i);
            int[] bookParagraphArr = PlayerPrefsX.GetIntArray("bookParagraph" + i);
            int[] bookChapterLenArr = PlayerPrefsX.GetIntArray("bookChapterLen" + i);
            string[] channelIDArr = PlayerPrefsX.GetStringArray("channelID" + i);
            string[] channelNameArr = PlayerPrefsX.GetStringArray("channelName" + i);
            int wl = articleTitleArr.Length;
            for (int j = 0; j < wl; j++)
            {
                dg.bookTitleList.Add(articleTitleArr[j]);
                dg.bookIDList.Add(bookIDArr[j]);
                dg.bookParagraphList.Add(bookParagraphArr[j]);
                dg.bookChapterLenList.Add(bookChapterLenArr[j]);
                dg.channelIDList.Add(channelIDArr[j]);
                dg.channelNameList.Add(channelNameArr[j]);
            }
            allArticleGroup.Add(dg);
        }
        articleGroupCount = groupCount;
    }
    void ClearArticleGroupData()
    {
        PlayerPrefs.DeleteKey("articleGroupName");
        for (int i = 0; i < articleGroupCount; i++)
        {
            PlayerPrefs.DeleteKey("articleTitle" + i);
            PlayerPrefs.DeleteKey("bookID" + i);
            PlayerPrefs.DeleteKey("bookParagraph" + i);
            PlayerPrefs.DeleteKey("bookChapterLen" + i);
            PlayerPrefs.DeleteKey("channelID" + i);
            PlayerPrefs.DeleteKey("channelName" + i);
        }
        //dicGroupCount = 0;
    }
    public void ModifyArticleGroup()
    {
        PlayerPrefs.SetInt("articleGroupCount", allArticleGroup.Count);
        articleGroupCount = allArticleGroup.Count;
        ClearArticleGroupData();
        List<string> dicNameList = new List<string>();
        for (int i = 0; i < articleGroupCount; i++)
        {
            dicNameList.Add(allArticleGroup[i].groupName);
            PlayerPrefsX.SetStringArray("articleTitle" + i, allArticleGroup[i].bookTitleList.ToArray());
            PlayerPrefsX.SetIntArray("bookID" + i, allArticleGroup[i].bookIDList.ToArray());
            PlayerPrefsX.SetIntArray("bookParagraph" + i, allArticleGroup[i].bookParagraphList.ToArray());
            PlayerPrefsX.SetIntArray("bookChapterLen" + i, allArticleGroup[i].bookChapterLenList.ToArray());
            PlayerPrefsX.SetStringArray("channelID" + i, allArticleGroup[i].channelIDList.ToArray());
            PlayerPrefsX.SetStringArray("channelName" + i, allArticleGroup[i].channelNameList.ToArray());

        }
        PlayerPrefsX.SetStringArray("articleGroupName", dicNameList.ToArray());
    }
    public void DelGroup(int id)
    {
        allArticleGroup.RemoveAt(id);
        int groupCount = allArticleGroup.Count;
        articleGroupCount = groupCount;
        for (int i = 0; i < groupCount; i++)
        {
            allArticleGroup[i].groupID = i;
        }
        ModifyArticleGroup();
    }
    public void AddGroup(string gName)
    {
        ArticleGroupInfo group = new ArticleGroupInfo();
        group.groupName = gName;
        group.groupID = articleGroupCount;
        group.bookTitleList = new List<string>();
        group.bookIDList = new List<int>();
        group.bookParagraphList = new List<int>();
        group.bookChapterLenList = new List<int>();
        group.channelIDList = new List<string>();
        group.channelNameList = new List<string>();
        allArticleGroup.Add(group);
        int groupCount = allArticleGroup.Count;
        articleGroupCount = groupCount;
        ModifyArticleGroup();
    }
    public void DelArticle(int groupID, string articleTitle)//,int bookID,string channelID)
    {
        //todo 以文章标题查找是否唯一？？？？？？可能会出现误删bug
        int index = allArticleGroup[groupID].bookTitleList.IndexOf(articleTitle);
        allArticleGroup[groupID].bookTitleList.RemoveAt(index);
        allArticleGroup[groupID].bookIDList.RemoveAt(index);
        allArticleGroup[groupID].bookParagraphList.RemoveAt(index);
        allArticleGroup[groupID].bookChapterLenList.RemoveAt(index);
        allArticleGroup[groupID].channelIDList.RemoveAt(index);
        allArticleGroup[groupID].channelNameList.RemoveAt(index);
        PlayerPrefsX.SetStringArray("articleTitle" + groupID, allArticleGroup[groupID].bookTitleList.ToArray());
        PlayerPrefsX.SetIntArray("bookID" + groupID, allArticleGroup[groupID].bookIDList.ToArray());
        PlayerPrefsX.SetIntArray("bookParagraph" + groupID, allArticleGroup[groupID].bookParagraphList.ToArray());
        PlayerPrefsX.SetIntArray("bookChapterLen" + groupID, allArticleGroup[groupID].bookChapterLenList.ToArray());
        PlayerPrefsX.SetStringArray("channelID" + groupID, allArticleGroup[groupID].channelIDList.ToArray());
        PlayerPrefsX.SetStringArray("channelName" + groupID, allArticleGroup[groupID].channelNameList.ToArray());
    }
    public void AddArticle(int groupID, string articleTitle, int bookID, int bookParagraph, int bookChapterLen, string channelID, string channelName)
    {
        allArticleGroup[groupID].bookTitleList.Add(articleTitle);
        allArticleGroup[groupID].bookIDList.Add(bookID);
        allArticleGroup[groupID].bookParagraphList.Add(bookParagraph);
        allArticleGroup[groupID].bookChapterLenList.Add(bookChapterLen);
        allArticleGroup[groupID].channelIDList.Add(channelID);
        allArticleGroup[groupID].channelNameList.Add(channelName);
        PlayerPrefsX.SetStringArray("articleTitle" + groupID, allArticleGroup[groupID].bookTitleList.ToArray());
        PlayerPrefsX.SetIntArray("bookID" + groupID, allArticleGroup[groupID].bookIDList.ToArray());
        PlayerPrefsX.SetIntArray("bookParagraph" + groupID, allArticleGroup[groupID].bookParagraphList.ToArray());
        PlayerPrefsX.SetIntArray("bookChapterLen" + groupID, allArticleGroup[groupID].bookChapterLenList.ToArray());
        PlayerPrefsX.SetStringArray("channelID" + groupID, allArticleGroup[groupID].channelIDList.ToArray());
        PlayerPrefsX.SetStringArray("channelName" + groupID, allArticleGroup[groupID].channelNameList.ToArray());
    }
    //改组名
    public void ChangeGroupName(int groupID, string name)
    {
        string[] nameArr = PlayerPrefsX.GetStringArray("articleGroupName");
        nameArr[groupID] = name;
        PlayerPrefsX.SetStringArray("articleGroupName", nameArr);
    }
    public StarGroupArticleView articleStarGroup;
    /// <summary>
    /// 当前单词是否被收藏
    /// </summary>
    /// <param name="word"></param>
    public void SetArticleStar(string articleTitle, int bookID, int bookParagraph, int bookChapterLen, string channelID)
    {
        bool isStar = false;
        int l = allArticleGroup.Count;
        for (int i = 0; i < l; i++)
        {
            if (allArticleGroup[i].bookTitleList.Contains(articleTitle))
            {
                for (int j = 0; j < allArticleGroup[i].bookTitleList.Count; j++)
                {
                    if (allArticleGroup[i].bookTitleList[j] == articleTitle &&
                       allArticleGroup[i].bookIDList[j] == bookID &&
                       allArticleGroup[i].bookParagraphList[j] == bookParagraph &&
                       allArticleGroup[i].bookChapterLenList[j] == bookChapterLen &&
                       allArticleGroup[i].channelIDList[j] == channelID)
                    {
                        isStar = true;
                        break;
                    }
                }
            }
        }
        articleStarGroup.SetToggleValue(isStar);
    }

    public bool IsContainsArticle(int groupId, string articleTitle, int bookID, int bookParagraph, int bookChapterLen, string channelID)
    {
        for (int j = 0; j < allArticleGroup[groupId].bookTitleList.Count; j++)
        {
            if (allArticleGroup[groupId].bookTitleList[j] == articleTitle &&
                allArticleGroup[groupId].bookIDList[j] == bookID &&
                allArticleGroup[groupId].bookParagraphList[j] == bookParagraph &&
                allArticleGroup[groupId].bookChapterLenList[j] == bookChapterLen &&
                allArticleGroup[groupId].channelIDList[j] == channelID)
            {
                return true;
            }
        }
        return false;
    }
    #endregion
    #region 在线与联网逻辑
    //点进翻译的文章，检测是否联网
    //1.如果联网，直接在线阅读
    //2.如果未联网，判断是否有离线包，如果有离线包使用离线包，如果没有离线包显示offline界面
    public enum NetPackLogicEnum
    {
        Online = 1,
        OfflineWithPack = 2,
        OfflineNoPack = 3
    }
    //检测是否启用离线包逻辑
    //1.有网络默认在线功能/设置默认使用离线包/判断最新
    //2.没网络判断是否有离线包数据库，有就使用离线包，没有就log提示无网络下载离线包
    public NetPackLogicEnum CheckIsUseOfflinePack()
    {

        if (!NetworkMangaer.Instance().CheckIsHaveNetwork())
        {
            string path;
            path = Application.persistentDataPath + "/DB/Sentence.db";

            //如果查找该文件路径
            if (File.Exists(path))
            {
                return NetPackLogicEnum.OfflineWithPack;
            }
            return NetPackLogicEnum.OfflineNoPack;
        }
        else
        {
            return NetPackLogicEnum.Online;
        }
    }

    #endregion
}
