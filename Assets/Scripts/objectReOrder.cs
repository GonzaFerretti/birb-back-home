using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
[ExecuteInEditMode]
public class objectReOrder : MonoBehaviour
{
    public GameObject[] winds, grounds, bugs, trees, crows;
#if UNITY_EDITOR
    public void Update()
    {
        if (!UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode)
        {
        sortAndNameObjects();
        }
    }
#endif

    public void Start()
    {
        sortAndNameObjects();
    }
    public GameObject[] getBugs()
    {
        return bugs;
    }

    void sortAndNameObjects()
    {
        AutoArrange(ref trees, "tree", "tree");
        AutoArrange(ref winds, "wind", "Wind");
        AutoArrange(ref bugs, "bug", "BugV");
        AutoArrange(ref grounds, "ground", "Ground");
        AutoArrange(ref grounds, "crow", "crow");
        arrangeLevelItems();
    }

    void arrangeLevelItems()
    {
        Transform[] trans = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            trans[i] = transform.GetChild(i);
        }
        trans = trans.OrderBy(groupObject => groupObject.position.x).ToArray();
        for (int i = 0; i < trans.Length; i++)
        {
            trans[i].SetSiblingIndex(i);
        }
    }
    public void AutoArrange(ref GameObject[] group, string name, string tag)
    {
        group = GameObject.FindGameObjectsWithTag(tag);
        group = group.OrderBy(groupObject => groupObject.transform.position.x).ToArray();
        for (int i = 0; i < group.Length; i++)
        {
            string numberString = (i / 10).ToString() + (i % 10).ToString();
            group[i].name = name + numberString;
            group[i].transform.SetParent(transform);
        }
    }
}
