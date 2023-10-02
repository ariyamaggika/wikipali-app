using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 生成Word/PDF/TXT
/// </summary>
public class DocGenManager
{
    private DocGenManager() { }
    private static DocGenManager manager = null;
    //静态工厂方法 
    public static DocGenManager Instance()
    {
        if (manager == null)
        {
            manager = new DocGenManager();
        }
        return manager;
    }
}
