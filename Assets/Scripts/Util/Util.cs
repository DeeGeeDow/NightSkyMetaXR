using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static Vector3 SphereToRec(float hor, float ver)
    {
        // ver : angle from z+ to y+
        // hor : angle from z+ to x+
        // NOTE : Unity is using left-handed coordinate with y axis going up, all right-handed y and z axis must be swapped before using this
        float sin_hor = Mathf.Sin(hor);
        float cos_hor = Mathf.Cos(hor);
        float sin_ver = Mathf.Sin(ver);
        float cos_ver = Mathf.Cos(ver);

        float x = sin_hor * cos_ver;
        float z = cos_hor * cos_ver;
        float y = sin_ver;
        return new(x,y,z);
    }
    public static (float, float) HaDecToAltAz(float ha, float dec, float lat)
    {
        // THIS METHOD WILL RETURN (alt, az) in rad
        ha *= Mathf.Deg2Rad;
        dec *= Mathf.Deg2Rad;
        lat *= Mathf.Deg2Rad;

        float sin_dec = Mathf.Sin(dec);
        float cos_dec = Mathf.Cos(dec);
        float sin_lat = Mathf.Sin(lat);
        float cos_lat = Mathf.Cos(lat);
        float cos_ha = Mathf.Cos(ha);
        float sin_alt = sin_dec * sin_lat + cos_dec * cos_lat * cos_ha;
        float alt = Mathf.Asin(sin_alt);
        float cos_alt = Mathf.Cos(alt);
        float cos_az = (sin_dec - sin_alt * sin_lat) / (cos_alt * cos_lat);
        float az = Mathf.Acos(cos_az);

        if (ha > 0) az = Mathf.PI*2 - az;
        //if(alt > 0) Debug.Log((alt, az));
        return new(alt, az);
    }

    public static Mesh CreateQuad(float width = 1f, float height = 1f)
    {
        // Create a quad mesh.
        var mesh = new Mesh();

        float w = width * .5f;
        float h = height * .5f;
        var vertices = new Vector3[4] {
            new Vector3(-w, -h, 0),
            new Vector3(w, -h, 0),
            new Vector3(-w, h, 0),
            new Vector3(w, h, 0)
        };

        var tris = new int[6] {
            // lower left tri.
            0, 2, 1,
            // lower right tri
            2, 3, 1
        };

        var normals = new Vector3[4] {
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
            -Vector3.forward,
        };

        var uv = new Vector2[4] {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
        };

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.normals = normals;
        mesh.uv = uv;

        return mesh;
    }


}
