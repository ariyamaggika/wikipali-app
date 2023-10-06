/*
 * uGUI-Hypertext (https://github.com/setchi/uGUI-Hypertext)
 * Copyright (c) 2019 setchi
 * Licensed under MIT (https://github.com/setchi/uGUI-Hypertext/blob/master/LICENSE)
 */

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using static ArticleMarkdownManager;

namespace Hypertext
{
    public class RegexHypertext : HypertextBase
    {
        public const string GRM_ABBR_TAG = "#INCLUDE_GRM_ABBR";
        public const string TERM_TAG = "#INCLUDE_TERM";

        public const string TERM_REGEX = @"\<(.+?)\>";
        readonly List<Entry> entries = new List<Entry>();

        struct Entry
        {
            public readonly string RegexPattern;
            public readonly Color Color;
            public readonly Action<string> Callback;

            public Entry(string regexPattern, Color color, Action<string> callback)
            {
                RegexPattern = regexPattern;
                Color = color;
                Callback = callback;
            }
            //termTag
            //1. 自己数据 蓝色
            //2. 社区数据 绿色
            //3. 没匹配到 红色
            public Entry(string regexPattern, Action<string> callback)
            {
                RegexPattern = regexPattern;
                Callback = callback;
                Color = Color.black;
            }
        }

        /// <summary>
        /// 正規表現にマッチした部分文字列にクリックイベントリスナを登録します
        /// </summary>
        /// <param name="regexPattern">正規表現</param>
        /// <param name="onClick">クリック時のコールバック</param>
        public void OnClick(string regexPattern, Action<string> onClick)
        {
            OnClick(regexPattern, color, onClick);
        }

        /// <summary>
        /// 正規表現にマッチした部分文字列に色とクリックイベントリスナを登録します
        /// </summary>
        /// <param name="regexPattern">正規表現</param>
        /// <param name="color">テキストカラー</param>
        /// <param name="onClick">クリック時のコールバック</param>
        public void OnClick(string regexPattern, Color color, Action<string> onClick)
        {
            if (string.IsNullOrEmpty(regexPattern) || onClick == null)
            {
                return;
            }

            entries.Add(new Entry(regexPattern, color, onClick));
        }

        public override void RemoveListeners()
        {
            base.RemoveListeners();
            entries.Clear();
        }

        /// <summary>
        /// イベントリスナを追加します
        /// テキストの変更などでイベントの再登録が必要なときにも呼び出されます
        /// <see cref="HypertextBase.OnClick"/> を使ってクリックイベントリスナを登録してください
        /// </summary>
        protected override void AddListeners()
        {
            foreach (var entry in entries)
            {
                //匹配grm_abbr字典
                if (GRM_ABBR_TAG == entry.RegexPattern)
                {
                    foreach (KeyValuePair<string, string> kvp in UserGrammar.grm_abbr)
                    {
                        string regexPattern = kvp.Key;
                        if (text.Contains(kvp.Key))
                        {
                            int startIndex = 0;
                            while (true)
                            {
                                int index = text.IndexOf(kvp.Key, startIndex);
                                if (index == -1)
                                    break;
                                OnClick(index, kvp.Key.Length, entry.Color, entry.Callback);
                                startIndex = index + kvp.Key.Length;
                                if (startIndex >= text.Length)
                                    break;
                            }
                        }
                        //foreach (Match match in Regex.Matches(text, regexPattern))
                        //{
                        //    OnClick(match.Index, match.Value.Length, entry.Color, entry.Callback);
                        //}

                    }
                }
                else if (TERM_TAG == entry.RegexPattern)//术语采用多种颜色格式
                {
                    Color color = Color.black;

                    foreach (Match match in Regex.Matches(text, TERM_REGEX))
                    {
                        string term = match.Value.Substring(1);
                        term = term.Substring(0, term.Length - 1);
                        TermInfoJson termJ = ArticleMarkdownManager.Instance().GetTermByWord(term);
                        if (termJ == null)
                        {
                            color = Color.yellow;
                        }
                        else
                        {
                            if (termJ.isCommunity)
                                color = Color.green;
                            else
                                color = Color.blue;

                        }
                        OnClick(match.Index, match.Value.Length, color, entry.Callback);
                    }
                }
                else//正则表达式
                {
                    foreach (Match match in Regex.Matches(text, entry.RegexPattern))
                    {
                        OnClick(match.Index, match.Value.Length, entry.Color, entry.Callback);
                    }
                }

            }
        }
    }
}
