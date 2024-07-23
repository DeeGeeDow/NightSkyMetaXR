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
    public bool isStarSelected = false;
    public PointerController PointerController;
    public GameObject dialog;
    public void FindStar(string name)
    {
        starIndexList = new List<int>();
        for(int i=0; i<starField.stars.Count; i++)
        {
            for(int j=0; j < starField.stars[i].names.Count; j++)
            {
                if (starField.stars[i].names[j].ToLower().Contains(name.ToLower()))
                {
                    starIndexList.Add(i);
                }
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
            starInfo.transform.GetComponent<StarInfoController>().Star = starField.stars[starIndexList[i]];
            starInfo.transform.GetComponent<StarInfoController>().PointerController = PointerController;
            starInfo.transform.GetComponent<StarInfoController>().dialog = dialog;
            starInfo.transform.GetComponent<StarInfoController>().starIndex = i;
            starInfo.transform.Find("Content").Find("Star Name").GetComponent<TMP_Text>().text = starField.stars[starIndexList[i]].names[0];
            starInfo.transform.Find("Content").Find("Constellation Name").GetComponent<TMP_Text>().text = StarLoader.IAUtoProperName[starField.stars[starIndexList[i]].con];
        }
    }

    private void Start()
    {
        ClearTable();
    }

    public void QueryStar()
    {
        if(starInputField.text.Length >= 3)
        {
            FindStar(starInputField.text);
            UpdateList();
        }
        else
        {
            starIndexList.Clear();
            UpdateList();
        }
    }

    public void CloseDialog()
    {
        starIndexList.Clear();
        UpdateList();
        dialog.SetActive(false);
        PointerController.IsShowing = false;
    }
}
