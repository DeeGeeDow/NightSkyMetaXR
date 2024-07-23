using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Star
{
    public readonly int id;

    public readonly string hip;
    public readonly string hd;
    public readonly string hr;
    public readonly string gl;
    public readonly string bf;
    public readonly string con;
    public readonly string bayer;
    public readonly string flam;
    public readonly string proper;
    public readonly double ra;
    public readonly double dec;
    public readonly string dist;
    public readonly double x0;
    public readonly double y0;
    public readonly double z0;
    public readonly float mag;
    public readonly float absmag;
    public readonly float ci;
    public readonly float pm_ra;
    public readonly float pm_dec;
    public readonly float vx;
    public readonly float vy;
    public readonly float vz;
    public readonly string spect;

    public float size;
    public Color color;

    public float alt;
    public float az;
    public Vector3 FieldPosition; // poisition with radius 1

    public List<string> names = new();


    //Constructor
    public Star(string[] items)
    {
        id = int.Parse(items[0]);
        hip = items[1];
        hd = items[2];
        hr = items[3];
        gl = items[4];
        bayer = items[27];
        flam = items[28];
        con = items[29];
        proper = items[6];
        ra = double.Parse(items[7]);
        dec = double.Parse(items[8]);
        dist = items[9];
        // HYG dataset's coordinate system is right-handed, with z is vertical axis
        // meanwhile the Unity coordinate system is left-handed, with y is vertical axis
        x0 = double.Parse(items[17]);
        y0 = double.Parse(items[19]);
        z0 = double.Parse(items[18]);

        mag = float.Parse(items[13]);
        absmag = float.Parse(items[14]);
        ci = items[16] == "" ? 0 : float.Parse(items[16]);
        pm_ra =  float.Parse(items[10]);
        pm_dec = float.Parse(items[11]);
        vx = float.Parse(items[20]);
        vy = float.Parse(items[21]);
        vz = float.Parse(items[22]);
        spect = items[15];

        size = SetSize(mag);
        color = BvToRgb(ci);
    }

    private float SetSize(float magnitude)
    {
        float sirius_mag = -1.44f;
        return Mathf.Pow(2.512f, (sirius_mag - magnitude)/2);
    }
    private Color BvToRgb(float bv)
    {
        double t;
        double r = 0.0;
        double g = 0.0;
        double b = 0.0;

        if (bv < -0.4) bv = -0.4f;
        if (bv > 2.0) bv = 2.0f;

        if ((bv >= -0.40) && (bv < 0.00)) { t = (bv + 0.40) / (0.00 + 0.40); r = 0.61 + (0.11 * t) + (0.1 * t * t); }
        else if ((bv >= 0.00) && (bv < 0.40)) { t = (bv - 0.00) / (0.40 - 0.00); r = 0.83 + (0.17 * t); }
        else if ((bv >= 0.40) && (bv < 2.10)) { t = (bv - 0.40) / (2.10 - 0.40); r = 1.00; }

        if ((bv >= -0.40) && (bv < 0.00)) { t = (bv + 0.40) / (0.00 + 0.40); g = 0.70 + (0.07 * t) + (0.1 * t * t); }
        else if ((bv >= 0.00) && (bv < 0.40)) { t = (bv - 0.00) / (0.40 - 0.00); g = 0.87 + (0.11 * t); }
        else if ((bv >= 0.40) && (bv < 1.60)) { t = (bv - 0.40) / (1.60 - 0.40); g = 0.98 - (0.16 * t); }
        else if ((bv >= 1.60) && (bv < 2.00)) { t = (bv - 1.60) / (2.00 - 1.60); g = 0.82 - (0.5 * t * t); }

        if ((bv >= -0.40) && (bv < 0.40)) { t = (bv + 0.40) / (0.40 + 0.40); b = 1.00; }
        else if ((bv >= 0.40) && (bv < 1.50)) { t = (bv - 0.40) / (1.50 - 0.40); b = 1.00 - (0.47 * t) + (0.1 * t * t); }
        else if ((bv >= 1.50) && (bv < 1.94)) { t = (bv - 1.50) / (1.94 - 1.50); b = 0.63 - (0.6 * t * t); }

        return new Color((float)r, (float)g, (float)b);
    }

    public void CalculatePosition(float lst, float lat)
    {
        float ha = lst - (float)ra * 15;
        if (ha > 180) ha -= 360;
        else if (ha < -180) ha += 360;
        (float, float) altAz = Util.HaDecToAltAz(ha, (float)dec, lat);
        alt = altAz.Item1;
        az = altAz.Item2;
        FieldPosition = Util.SphereToRec(altAz.Item2, altAz.Item1);
    }
}
