/*
 * uGUI-Hypertext (https://github.com/setchi/uGUI-Hypertext)
 * Copyright (c) 2019 setchi
 * Licensed under MIT (https://github.com/setchi/uGUI-Hypertext/blob/master/LICENSE)
 */

using UnityEngine;

namespace Hypertext
{
    public class RegexExample : MonoBehaviour
    {
        [SerializeField] RegexHypertext text = default;

        const string RegexUrl = @"https?://(?:[!-~]+\.)+[!-~]+";
        const string RegexHashTag = @"《(\w+)》";

        //const string RegexHashtag = @"[\s^][#＃]\w+";

        void Start()
        {
            text.OnClick(RegexUrl, new Color(0, 0.2235f, 0.898f, 1), url => Application.OpenURL(url));
            text.OnClick(RegexHashTag, new Color(0, 0.2235f, 0.898f, 1), hashtag => HttpLink(hashtag));

            //text.OnClick(RegexHashtag, Color.green, hashtag => Debug.Log(hashtag));
        }
        void HttpLink(string content)
        {
            if (content == "《用户隐 私政策》" || content == "《用户隐私政策》")
            {
                string url = "https://staging.wikipali.org/privacy";
                if (UpdateManager.Instance() != null)
                    url = UpdateManager.Instance().currentOInfo.privacyUrl;
                Application.OpenURL(url);
            }
        }
    }
}
