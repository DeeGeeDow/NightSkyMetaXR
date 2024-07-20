using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class StarInfoController : MonoBehaviour
{
    private Star _star;
    public PointerController PointerController;
    public Star Star
    {
        get => _star;
        set
        {
            _star = value;
            names = new List<string>();
            if (value.proper is not null && value.proper.Length>0)
            {
                names.Add(value.proper);
            }
            if(value.bayer is not null && value.bayer.Length > 0)
            {
                string[] baySplit = value.bayer.Split('/');
                Debug.Log($"value.bayer : {value.bayer.Length}");
                string fullBayerPrefix = Util.FullGreek[baySplit[0]] + ((baySplit.Length == 1) ? "" : $"-{baySplit[1]}");
                names.Add($"{fullBayerPrefix} {StarLoader.IAUtoGenitive[value.con]}");
            }
            if(value.flam is not null && value.flam.Length > 0)
            {
                names.Add($"{value.flam} {StarLoader.IAUtoGenitive[value.con]}");
            }
            if(value.hip is not null && value.hip.Length > 0)
            {
                names.Add($"HIP {value.hip}");
            }            
            if(value.hd is not null && value.hd.Length > 0)
            {
                names.Add($"HD {value.hd}");
            }
            if(value.hr is not null && value.hr.Length > 0)
            {
                names.Add($"HR {value.hr}");
            }
            if(value.gl is not null && value.gl.Length > 0)
            {
                names.Add(value.gl);
            }
        }
    }
    private List<string> names;



    public void OnClick()
    {
        GameObject dialog = GameObject.Find("Dialog");
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

        starname.GetComponent<TMP_Text>().text = names[0];
        consname.GetComponent<TMP_Text>().text = StarLoader.IAUtoProperName[Star.con];
        string aliasesText = "";
        for(int i=1; i<names.Count; i++)
        {
            aliasesText += names[i];
            if(i != names.Count-1)
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
    }
}
