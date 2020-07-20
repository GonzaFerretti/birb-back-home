using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class tree : MonoBehaviour
{
    private GameObject[] branches;
    public MeshRenderer mr;
    private float maxHeight, minHeight;
    public void Start()
    {
        setupBranches();
    }

#if UNITY_EDITOR
    public void Update()
    {
        setupBranches();
        enabled = !UnityEditor.EditorApplication.isPlayingOrWillChangePlaymode;
    }
#endif

    private void updateBranchPosition(int index)
    {
        minHeight = mr.bounds.min.y;
        maxHeight = mr.bounds.max.y;
        branches[index].transform.position = new Vector3(transform.position.x, Mathf.Clamp(branches[index].transform.position.y , minHeight, maxHeight));
    }
        
    private void setupBranches()
    {
        int branchCount = 0;
        int currentBranch = 0;
        for (int i = 0; i < transform.childCount;i++)
        {
            if (transform.GetChild(i).tag == "branchP")
            {
                branchCount++;
            }
        }
        branches = new GameObject[branchCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).tag == "branchP")
            {
                branches[currentBranch] = transform.GetChild(i).gameObject;
                currentBranch++;
            }
        }
        for (int i = 0; i < branches.Length; i++)
        {
            branches[i].transform.SetParent(transform);
            updateBranchPosition(i);
            branches[i].name = "branch" + i;
        }
    }
}
