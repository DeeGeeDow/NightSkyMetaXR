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

    public struct CullFlags
    {
        public const byte CULLED = 0;
        public const byte VISIBLE = 1;
        public const byte PARTIALLY_VISIBLE = 2;
    }

    public static bool IsBehindPlane(ref Plane plane, ref Vector3 p0, ref Vector3 p1, ref Vector3 p2, ref Vector3 p3, ref Vector3 p4, ref Vector3 p5, ref Vector3 p6, ref Vector3 p7)
    {
        bool getside0 = plane.GetSide(p0);
        bool getside1 = plane.GetSide(p1);
        bool getside2 = plane.GetSide(p2);
        bool getside3 = plane.GetSide(p3);
        bool getside4 = plane.GetSide(p4);
        bool getside5 = plane.GetSide(p5);
        bool getside6 = plane.GetSide(p6);
        bool getside7 = plane.GetSide(p7);
        return !(getside0 || getside1 || getside2 || getside3 || getside4 || getside5 || getside6 || getside7);
    }

    public static bool IsPointInsideFrustum(ref Plane plane0, ref Plane plane1, ref Plane plane2, ref Plane plane3, ref Plane plane4, ref Plane plane5, ref Vector3 point)
    {
        bool getside0 = plane0.GetSide(point);
        bool getside1 = plane1.GetSide(point);
        bool getside2 = plane2.GetSide(point);
        bool getside3 = plane3.GetSide(point);
        bool getside4 = plane4.GetSide(point);
        bool getside5 = plane5.GetSide(point);
        return (getside0 && getside1 && getside2 && getside3 && getside4 && getside5);
    }

    public static byte FrustumCullingStatus(ref Plane plane0, ref Plane plane1, ref Plane plane2, ref Plane plane3, ref Plane plane4, ref Plane plane5, ref Bounds bounds)
    {
        Vector3 p0 = bounds.min;
        Vector3 p1 = bounds.min + Vector3.right * bounds.size.x;
        Vector3 p2 = bounds.min + Vector3.forward * bounds.size.z;
        Vector3 p3 = bounds.min + Vector3.right*bounds.size.x + Vector3.forward*bounds.size.z;
        Vector3 p4 = p0 + Vector3.up * bounds.size.y;
        Vector3 p5 = p1 + Vector3.up * bounds.size.y;
        Vector3 p6 = p2 + Vector3.up * bounds.size.y;
        Vector3 p7 = p3 + Vector3.up * bounds.size.y;

        bool cull1 = IsBehindPlane(ref plane0, ref p0, ref p1, ref p2, ref p3, ref p4, ref p5, ref p6, ref p7);
        bool cull2 = IsBehindPlane(ref plane1, ref p0, ref p1, ref p2, ref p3, ref p4, ref p5, ref p6, ref p7);
        bool cull3 = IsBehindPlane(ref plane2, ref p0, ref p1, ref p2, ref p3, ref p4, ref p5, ref p6, ref p7);
        bool cull4 = IsBehindPlane(ref plane3, ref p0, ref p1, ref p2, ref p3, ref p4, ref p5, ref p6, ref p7);
        bool cull5 = IsBehindPlane(ref plane4, ref p0, ref p1, ref p2, ref p3, ref p4, ref p5, ref p6, ref p7);
        bool cull6 = IsBehindPlane(ref plane5, ref p0, ref p1, ref p2, ref p3, ref p4, ref p5, ref p6, ref p7);

        if (cull1 || cull2 || cull3 || cull4 || cull5 || cull6) return CullFlags.CULLED;

        bool partcull0 = IsPointInsideFrustum(ref plane0, ref plane1, ref plane2, ref plane3, ref plane4, ref plane5, ref p0);
        bool partcull1 = IsPointInsideFrustum(ref plane0, ref plane1, ref plane2, ref plane3, ref plane4, ref plane5, ref p1);
        bool partcull2 = IsPointInsideFrustum(ref plane0, ref plane1, ref plane2, ref plane3, ref plane4, ref plane5, ref p2);
        bool partcull3 = IsPointInsideFrustum(ref plane0, ref plane1, ref plane2, ref plane3, ref plane4, ref plane5, ref p3);
        bool partcull4 = IsPointInsideFrustum(ref plane0, ref plane1, ref plane2, ref plane3, ref plane4, ref plane5, ref p4);
        bool partcull5 = IsPointInsideFrustum(ref plane0, ref plane1, ref plane2, ref plane3, ref plane4, ref plane5, ref p5);
        bool partcull6 = IsPointInsideFrustum(ref plane0, ref plane1, ref plane2, ref plane3, ref plane4, ref plane5, ref p6);
        bool partcull7 = IsPointInsideFrustum(ref plane0, ref plane1, ref plane2, ref plane3, ref plane4, ref plane5, ref p7);

        if (!partcull0 || !partcull1 || !partcull2 || !partcull3 || !partcull4 || !partcull5 || !partcull6 || !partcull7) return CullFlags.PARTIALLY_VISIBLE;

        return CullFlags.VISIBLE;
    }

}
