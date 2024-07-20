using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QueryController : MonoBehaviour
{
    public TMP_InputField starInputField;
    public GameObject resultGO;
    public StarField starField;
    public List<int> starIndexList;
    public Transform tableTransform;
    public void FindStar(string name)
    {
        starIndexList = new List<int>();
        for(int i=0; i<starField.stars.Count; i++)
        {
            if (starField.stars[i].proper.Contains(name))
            {
                starIndexList.Add(i);
            }
        }
    }
    void ClearTable()
    {
        for(int i=tableTransform.childCount-1; i>=0; i--)
        {
            Destroy(tableTransform.GetChild(i).gameObject);
        }
    }
    public void UpdateList()
    {
        ClearTable();
        for(int i=0; i<starIndexList.Count; i++)
        {
            GameObject starInfo = Instantiate(resultGO);
            starInfo.transform.parent = tableTransform;
            starInfo.transform.localRotation = Quaternion.identity;
            starInfo.transform.Find("Content").Find("Star Name").GetComponent<TMP_Text>().text = starField.stars[starIndexList[i]].proper;
            starInfo.transform.Find("Content").Find("Constellation Name").GetComponent<TMP_Text>().text = StarLoader.IAUtoProperName[starField.stars[starIndexList[i]].con];
        }
    }

    private void Start()
    {
        ClearTable();
    }

    public void QueryStar()
    {
        FindStar(starInputField.text);
        UpdateList();
    }
}
