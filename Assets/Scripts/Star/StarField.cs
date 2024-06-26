using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.DebugTree;
using UnityEngine.UI;

public class StarField : MonoBehaviour
{
    [Range(0, 50)]
    [SerializeField] private float starSizeMax = 5f;
    public List<Star> stars;
    // private List<GameObject> starObjects;
    private List<Vector3> starPositions;
    [SerializeField] private float maxMagnitudeVisible = 6f;
    public readonly int starFieldScale = 400;
    public Material starMaterial;
    private List<MaterialPropertyBlock> blocks;
    private Mesh mesh;
    private List<List<Matrix4x4>> matrices;
    private List<List<Vector4>> colors;
    private List<List<float>> sizes;
    private int batch = 0;

    [Header("Location and Time based")]
    public LocationManager LocationManager;
    public TimeManager TimeManager;

    private List<string> _constellations = new();
    private List<List<(int, int)>> _constellationships = new();
    private List<List<LineRenderer>> _constellationLines = new();
    private List<int> _constellationStars = new();
    public Material ConstellationLineMaterial;

    private Camera _cam;
    private void Awake()
    {
        StarLoader.LoadData(starFieldScale);
    }
    private void Start()
    {
        _cam = Camera.main;
        stars = StarLoader.stars;
        _constellationStars = StarLoader.constellationStars;
        starPositions = new();

        mesh = Util.CreateQuad();
        RenderStar();

        //CreateConstellation();
        Debug.Log(Time.realtimeSinceStartup);
    }

    private void Update()
    {
        RenderStar();
        //DrawConstellation();
    }

    private void RenderStar()
    {
        int n = stars.Count;
        matrices = new();
        colors = new();
        sizes = new();
        blocks = new();
        batch = 0;

        matrices.Add(new List<Matrix4x4>());
        colors.Add(new List<Vector4>());
        sizes.Add(new List<float>());
        blocks.Add(new MaterialPropertyBlock());
        int constellationStarId = 0;
        for (int i = 0; i < n; i++)
        {
            //stars[i].CalculatePosition((float)TimeManager.Lst, LocationManager.latitude);
            Matrix4x4 mat = Matrix4x4.identity;
            if (stars[i].mag < maxMagnitudeVisible)
            {
                stars[i].CalculatePosition((float)TimeManager.Lst, LocationManager.latitude);
                Vector3 position = stars[i].FieldPosition * starFieldScale;
                Quaternion rotation = Quaternion.LookRotation(position, Vector3.up);
                Vector3 scale = Vector3.one * stars[i].size * starSizeMax;

                mat.SetTRS(position, rotation, scale);

                matrices[batch].Add(mat);
                colors[batch].Add(stars[i].color);
                sizes[batch].Add(stars[i].size * starSizeMax);
                if (matrices[batch].Count >= 1000)
                {
                    blocks[batch].SetFloatArray("_Size", sizes[batch]);
                    blocks[batch].SetVectorArray("_Color", colors[batch]);
                    batch++;
                    matrices.Add(new List<Matrix4x4>());
                    colors.Add(new List<Vector4>());
                    sizes.Add(new List<float>());
                    blocks.Add(new MaterialPropertyBlock());
                }
                if (constellationStarId < _constellationStars.Count && _constellationStars[constellationStarId] == stars[i].id)
                {
                    constellationStarId++;
                }
            }
            else if (constellationStarId < _constellationStars.Count && _constellationStars[constellationStarId] == stars[i].id)
            {
                stars[i].CalculatePosition((float)TimeManager.Lst, LocationManager.latitude);
                constellationStarId++;
            }
        }
        //blocks[batch].SetFloatArray("_Size", sizes[batch]);
        blocks[batch].SetVectorArray("_Color", colors[batch]);

        for (int i = 0; i <= batch; i++)
        {
            Graphics.DrawMeshInstanced(mesh, 0, starMaterial, matrices[i].ToArray(), matrices[i].Count, blocks[i]);
        }
    }

    private void CreateConstellation(){
        // Read the txt file
        const string filename = "constellationship";
        TextAsset textAsset = Resources.Load(filename) as TextAsset;
        StringReader reader = new StringReader(textAsset.text);

        // TO COMMENT
        GameObject constellationsHolder = new("Constellation Holder");
        constellationsHolder.transform.parent = transform;

        while (reader.Peek() != -1)
        {
            string line = reader.ReadLine();
            string[] items = line.Split(' ', System.StringSplitOptions.RemoveEmptyEntries);
            _constellations.Add(items[0]);
            int n = int.Parse(items[1]);
            List<(int, int)> constellationship = new();

            // TO COMMENT
            GameObject constellationHolder = new(items[0]);
            constellationHolder.transform.parent = constellationsHolder.transform;
            List<LineRenderer> constellationLine = new();

            for (int i = 1; i <= n; i++)
            {
                // TO COMMENT
                GameObject lineGO = new("Line");
                lineGO.transform.parent = constellationHolder.transform;
                LineRenderer lineRenderer = lineGO.AddComponent<LineRenderer>();
                lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
                lineRenderer.material = ConstellationLineMaterial;
                lineRenderer.useWorldSpace = false;
                

                int a = int.Parse(items[2 * i]);
                int b = int.Parse(items[2 * i + 1]);

                // TO COMMENT
                Vector3 pos1 = stars[a - 1].FieldPosition * starFieldScale;
                Vector3 pos2 = stars[b - 1].FieldPosition * starFieldScale;
                // Offset them so they don't occlude the stars, 3 chosen by trial and error.
                Vector3 dir = (pos2 - pos1).normalized * 3;
                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, pos1 + dir);
                lineRenderer.SetPosition(1, pos2 - dir);

                constellationship.Add((a, b));

                // TO COMMENT
                constellationLine.Add(lineRenderer);

            }
            _constellationships.Add(constellationship);
            _constellationLines.Add(constellationLine);
        }
    }

    private void DrawConstellation()
    {
        for(int i=0; i<_constellationships.Count; i++)
        {
            for(int j=0; j< _constellationships[i].Count; j++)
            {
                Vector3 pos1 = stars[_constellationships[i][j].Item1 - 1].FieldPosition * starFieldScale;
                Vector3 pos2 = stars[_constellationships[i][j].Item2 - 1].FieldPosition * starFieldScale;
                Vector3 dir = (pos2 - pos1).normalized * 3;
                _constellationLines[i][j].SetPosition(0, pos1 + dir);
                _constellationLines[i][j].SetPosition(1, pos2 - dir);
            }
        }
    }

    public void BortleScaleChanged(int BortleScale)
    {
        switch(BortleScale)
        {
            case 1: 
                maxMagnitudeVisible = 8f; break;
            case 2:
                maxMagnitudeVisible = 7.5f; break;
            case 3:
                maxMagnitudeVisible = 7.0f; break;
            case 4:
                maxMagnitudeVisible = 6.5f; break;
            case 5:
                maxMagnitudeVisible = 6.2f; break;
            case 6:
                maxMagnitudeVisible = 6.0f; break;
            case 7:
                maxMagnitudeVisible = 5.0f; break;
            case 8:
                maxMagnitudeVisible = 4.5f; break;
            case 9:
                maxMagnitudeVisible = 4.0f; break;
        }
    }

    public void onBortleScaleDropdownChanged(int dropdownValue)
    {
        // Dropdown index started from 0
        BortleScaleChanged(dropdownValue+1);
    }
}
