using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ����Word/PDF/TXT
/// </summary>
public class DocGenManager
{
    private DocGenManager() { }
    private static DocGenManager manager = null;
    //��̬�������� 
    public static DocGenManager Instance()
    {
        if (manager == null)
        {
            manager = new DocGenManager();
        }
        return manager;
    }
}
