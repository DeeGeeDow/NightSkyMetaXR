using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class StarInfoController : MonoBehaviour
{
    private Star _star;
    [HideInInspector] public int starIndex;
    [HideInInspector] public PointerController PointerController;
    [HideInInspector] public GameObject dialog;
    public Star Star;



    public void OnClick()
    {
        dialog.SetActive(true);
        Transform content = dialog.transform.Find("Content");
        Transform title = content.Find("Title");
        Transform starname = title.Find("Star name");
        Transform consname = title.Find("Constellation name");
        Transform mainInfo = content.Find("Main Info");
        Transform aliases = mainInfo.Find("Aliases");
        Transform magnitude = mainInfo.Find("Magnitude");
        Transform spectralType = mainInfo.Find("Spectral type");
        Transform ra = mainInfo.Find("RA");
        Transform dec = mainInfo.Find("Dec");
        Transform alt = mainInfo.Find("Alt");
        Transform az = mainInfo.Find("Az");

        starname.GetComponent<TMP_Text>().text = Star.names[0];
        consname.GetComponent<TMP_Text>().text = StarLoader.IAUtoProperName[Star.con];
        string aliasesText = "";
        for(int i=1; i<Star.names.Count; i++)
        {
            aliasesText += Star.names[i];
            if(i != Star.names.Count-1)
            {
                aliasesText += ", ";
            }
        }
        aliases.GetComponent<TMP_Text>().text = $"Aliases : {aliasesText}";
        magnitude.GetComponent<TMP_Text>().text = $"Magnitude : {Star.mag}";
        spectralType.GetComponent<TMP_Text>().text = $"Spectral type: {Star.spect}";
        ra.GetComponent<TMP_Text>().text = $"Right Ascension : {Util.Deg2DegMinSec((float)Star.ra)}";
        dec.GetComponent<TMP_Text>().text = $"Declination : {Util.Deg2DegMinSec((float)Star.dec)}";
        alt.GetComponent<TMP_Text>().text = $"Altitude : {Util.Deg2DegMinSec(Star.alt)}";
        az.GetComponent<TMP_Text>().text = $"Azimuth : {Util.Deg2DegMinSec(Star.az)}";

        PointerController.star = Star;
        PointerController.IsShowing = true;
        PointerController.starIndex = starIndex;
    }
}
