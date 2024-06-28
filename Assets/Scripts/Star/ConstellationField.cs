using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ConstellationField : MonoBehaviour
{
    public bool IsConstellationLinesVisible = false;
    [SerializeField]
    private bool _isConstellationNameVisible;
    public bool IsConstellationNameVisible
    {
        get => _isConstellationNameVisible; set
        {
            _isConstellationNameVisible = value;
            foreach(Transform t in transform)
            {
                t.gameObject.SetActive(value);
                Debug.Log(t.name);
            }
        }
    }
    List<string> constellations;
    List<List<(int, int)>> constellationships;
    List<(float, float)> constellationPositions;
    public StarField starField;
    private Camera _cam;
    private Mesh _constellationLineMesh;
    public Material ConstellationMaterial;
    private List<GameObject> _constellationNameObjects = new List<GameObject>();
    public Material ConstellationNameMaterial;

    [Header("Location and Time Manager")]
    public LocationManager LocationManager;
    public TimeManager TimeManager;
    private void Start()
    {
        _cam = Camera.main;
        _constellationLineMesh = Util.CreateQuad();
        constellationships = StarLoader.constellationships;
        constellations = StarLoader.constellations;
        constellationPositions = StarLoader.constellationPositions;
        SetupConstellationNames();
        IsConstellationNameVisible = true;
    }
    private void Update()
    {
        if(IsConstellationLinesVisible) DrawConstellationLines();
        if(IsConstellationNameVisible) DrawConstellationNames();
    }

    public void DrawConstellationLines()
    {
        List<List<Matrix4x4>> matrices = new()
        {
            new List<Matrix4x4>()
        };
        int batch = 0;
        for (int i = 0; i < constellationships.Count; i++)
        {
            for (int j = 0; j < constellationships[i].Count; j++)
            {
                float offset = 3;
                Vector3 pos1 = StarLoader.stars[constellationships[i][j].Item1 - 1].FieldPosition * starField.starFieldScale;
                Vector3 pos2 = StarLoader.stars[constellationships[i][j].Item2 - 1].FieldPosition * starField.starFieldScale;
                Vector3 dir = (pos2 - pos1).normalized * offset;
                float length = (pos2 - pos1).magnitude;
                Vector3 position = (pos1 + pos2) / 2;
                Quaternion rotation = Quaternion.LookRotation(position, Vector3.Cross(position, dir));
                Vector3 scale = new Vector3(length-2*offset, 1, 1);

                Matrix4x4 mat = Matrix4x4.identity;
                mat.SetTRS(position, rotation, scale);

                matrices[batch].Add(mat);
                if (matrices[batch].Count >= 1000)
                {
                    batch++;
                    matrices.Add(new());
                }
                //_constellationLines[i][j].SetPosition(0, pos1 + dir);
                //_constellationLines[i][j].SetPosition(1, pos2 - dir);
            }
        }
        for(int i=0; i<=batch; i++)
        {
            Graphics.DrawMeshInstanced(_constellationLineMesh, 0, ConstellationMaterial, matrices[i]);
        }
    }

    public void SetupConstellationNames()
    {
        for (int i=0; i<constellationPositions.Count; i++)
        {
            float lst = (float) TimeManager.Lst;
            float ha = lst - constellationPositions[i].Item1 * 15;
            if (ha > 180) ha -= 360;
            else if (ha < -180) ha += 360;
            (float, float) altAz = Util.HaDecToAltAz(ha, constellationPositions[i].Item2, LocationManager.latitude);
            Vector3 constellationPos = Util.SphereToRec(altAz.Item2, altAz.Item1);

            GameObject constellationNameGO = new(constellations[i]);
            TMP_Text constellationNameText = constellationNameGO.AddComponent<TextMeshPro>();
            constellationNameGO.transform.parent = transform;
            constellationNameGO.transform.position = constellationPos * starField.starFieldScale;
            constellationNameGO.transform.LookAt(_cam.transform.position);
            constellationNameGO.transform.Rotate(Vector3.up, 180);
            constellationNameText.text = constellations[i];
            constellationNameText.fontSize = 72;
            constellationNameText.enableWordWrapping = false;
            constellationNameText.overflowMode = TextOverflowModes.Overflow;
            _constellationNameObjects.Add(constellationNameGO);
            
        }
    }

    public void DrawConstellationNames()
    {
        for(int i=0; i<_constellationNameObjects.Count; i++)
        {
            float lst = (float)TimeManager.Lst;
            float ha = lst - constellationPositions[i].Item1 * 15;
            if (ha > 180) ha -= 360;
            else if (ha < -180) ha += 360;
            (float, float) altAz = Util.HaDecToAltAz(ha, constellationPositions[i].Item2, LocationManager.latitude);
            Vector3 constellationPos = Util.SphereToRec(altAz.Item2, altAz.Item1);

            _constellationNameObjects[i].transform.position = constellationPos * starField.starFieldScale;
            _constellationNameObjects[i].transform.LookAt(_cam.transform.position);
            _constellationNameObjects[i].transform.Rotate(Vector3.up, 180);
        }
    }

    [ContextMenu("Redraw")]
    public void RedrawConstellationNames()
    {
        int n = transform.childCount;
        for(int i=n-1; i>=0; i--){
            GameObject.Destroy(transform.GetChild(i).gameObject);
        }
        SetupConstellationNames();
        Debug.Log("Redraw");
    }

    public void SetConstellationLinesVisibility(bool isVisible){
        IsConstellationLinesVisible = isVisible;
    }
}
