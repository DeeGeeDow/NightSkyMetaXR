using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Oculus.Interaction.DebugTree;
using UnityEngine.UI;
using Unity.Jobs;
using Unity.Collections;

public class StarField : MonoBehaviour
{
    [Range(0, 50)]
    [SerializeField] private float starSizeMaxClearSky = 10f;
    [SerializeField] private float starSizeMax = 10f;
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
    public SkyBoxController SkyBoxController;

    private Camera _cam;
    private Plane[] frustumPlanes = new Plane[6];

    private NativeArray<Vector3> positionNA;
    private NativeArray<Vector3> scaleNA;
    private NativeArray<byte> cullResultsNA;
    private NativeArray<float> magnitudesNA;

    private Vector3[] positions;
    private byte[] cullResults;

    private JobHandle cullingJobHandle;

    private void Awake()
    {
        StarLoader.LoadData(starFieldScale);
        stars = StarLoader.stars;
        starPositions = new();
    }

    private void OnEnable()
    {
        int n = stars.Count;
        positionNA = new NativeArray<Vector3>(n, Allocator.Persistent, NativeArrayOptions.ClearMemory);
        scaleNA = new NativeArray<Vector3>(n, Allocator.Persistent, NativeArrayOptions.ClearMemory);
        cullResultsNA = new NativeArray<byte>(n, Allocator.Persistent, NativeArrayOptions.ClearMemory);
        magnitudesNA = new NativeArray<float>(n,Allocator.Persistent, NativeArrayOptions.ClearMemory);
        for(int i=0; i<n; i++)
        {
            magnitudesNA[i] = stars[i].mag;
            if (stars[i].mag < maxMagnitudeVisible)
            {
                stars[i].CalculatePosition((float)TimeManager.Lst, LocationManager.latitude);
                positionNA[i] = stars[i].FieldPosition;
                scaleNA[i] = Vector3.one * stars[i].size * starSizeMax;
            }
        }
        cullResults = new byte[n];
        positions = new Vector3[n];
    }

    private void OnDisable()
    {
        cullingJobHandle.Complete();
        if(positionNA.IsCreated) { positionNA.Dispose(); }
        if(scaleNA.IsCreated) {  scaleNA.Dispose(); }
        if(magnitudesNA.IsCreated) { magnitudesNA.Dispose(); }
        if(cullResultsNA.IsCreated) {  cullResultsNA.Dispose(); }
    }
    private void Start()
    {
        _cam = Camera.main;
        _constellationStars = StarLoader.constellationStars;

        mesh = Util.CreateQuad();
        //RenderStar();

        //CreateConstellation();
        Debug.Log(Time.realtimeSinceStartup);
    }

    private void Update()
    {
        if (cullingJobHandle.IsCompleted)
        {
            cullingJobHandle.Complete();
            cullResultsNA.CopyTo(cullResults);
            GeometryUtility.CalculateFrustumPlanes(_cam, frustumPlanes);
            positionNA.CopyFrom(positions);
            StarRenderingJob starRenderingJob = new StarRenderingJob()
            {
                positionNA = this.positionNA,
                scaleNA = this.scaleNA,
                cullResultsNA = this.cullResultsNA,
                magnitudesNA = this.magnitudesNA,
                plane0 = frustumPlanes[0],
                plane1 = frustumPlanes[1],
                plane2 = frustumPlanes[2],
                plane3 = frustumPlanes[3],
                plane4 = frustumPlanes[4],
                plane5 = frustumPlanes[5],
                maxMagnitudeVisible = this.maxMagnitudeVisible
            };
            
            cullingJobHandle = starRenderingJob.Schedule(stars.Count, 100);
            RenderStarJob();
        }
        //DrawConstellation();
    }

    //private void RenderStar()
    //{
    //    int n = stars.Count;
    //    matrices = new();
    //    colors = new();
    //    sizes = new();
    //    blocks = new();
    //    batch = 0;

    //    matrices.Add(new List<Matrix4x4>());
    //    colors.Add(new List<Vector4>());
    //    sizes.Add(new List<float>());
    //    blocks.Add(new MaterialPropertyBlock());
    //    int constellationStarId = 0;
    //    for (int i = 0; i < n; i++)
    //    {
    //        //stars[i].CalculatePosition((float)TimeManager.Lst, LocationManager.latitude);
    //        Matrix4x4 mat = Matrix4x4.identity;
    //        if (stars[i].mag < maxMagnitudeVisible)
    //        {
    //            stars[i].CalculatePosition((float)TimeManager.Lst, LocationManager.latitude);
    //            Vector3 position = stars[i].FieldPosition * starFieldScale;
    //            Quaternion rotation = Quaternion.LookRotation(position, Vector3.up);
    //            Vector3 scale = Vector3.one * stars[i].size * starSizeMax;

    //            _bounds.center = position;
    //            _bounds.size = scale;
    //            byte cullFlag = Util.FrustumCullingStatus(ref frustumPlanes[0], ref frustumPlanes[1], ref frustumPlanes[2], ref frustumPlanes[3], ref frustumPlanes[4], ref frustumPlanes[5], ref _bounds);
    //            if(cullFlag != Util.CullFlags.CULLED)
    //            {
    //                mat.SetTRS(position, rotation, scale);
    //                matrices[batch].Add(mat);
    //                colors[batch].Add(stars[i].color);
    //                sizes[batch].Add(stars[i].size * starSizeMax);
    //                if (matrices[batch].Count >= 1000)
    //                {
    //                    blocks[batch].SetFloatArray("_Size", sizes[batch]);
    //                    blocks[batch].SetVectorArray("_Color", colors[batch]);
    //                    batch++;
    //                    matrices.Add(new List<Matrix4x4>());
    //                    colors.Add(new List<Vector4>());
    //                    sizes.Add(new List<float>());
    //                    blocks.Add(new MaterialPropertyBlock());
    //                }

    //            }

    //            if (constellationStarId < _constellationStars.Count && _constellationStars[constellationStarId] == stars[i].id)
    //            {
    //                constellationStarId++;
    //            }
    //        }
    //        else if (constellationStarId < _constellationStars.Count && _constellationStars[constellationStarId] == stars[i].id)
    //        {
    //            stars[i].CalculatePosition((float)TimeManager.Lst, LocationManager.latitude);
    //            constellationStarId++;
    //        }
    //    }
    //    //blocks[batch].SetFloatArray("_Size", sizes[batch]);
    //    blocks[batch].SetVectorArray("_Color", colors[batch]);

    //    for (int i = 0; i <= batch; i++)
    //    {
    //        Graphics.DrawMeshInstanced(mesh, 0, starMaterial, matrices[i].ToArray(), matrices[i].Count, blocks[i]);
    //    }
    //}

    private void RenderStarJob()
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
            if (cullResults[i] != Util.CullFlags.CULLED)
            {
                stars[i].CalculatePosition((float)TimeManager.Lst, LocationManager.latitude);
                Vector3 position = stars[i].FieldPosition * starFieldScale;
                Quaternion rotation = Quaternion.LookRotation(position, Vector3.up);
                Vector3 scale = Vector3.one * stars[i].size * starSizeMax;

                positions[i] = position;
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
        SkyBoxController.SetIntensity(maxMagnitudeVisible);
        starSizeMax = starSizeMaxClearSky*Mathf.Pow(1.25f, (maxMagnitudeVisible - 8)/2);
    }

    public void onBortleScaleDropdownChanged(int dropdownValue)
    {
        // Dropdown index started from 0
        BortleScaleChanged(dropdownValue+1);
    }
}

public struct StarRenderingJob: IJobParallelFor
{
    public NativeArray<Vector3> positionNA;
    public NativeArray<Vector3> scaleNA;
    public NativeArray<byte> cullResultsNA;
    public NativeArray<float> magnitudesNA;
    public Plane plane0, plane1, plane2, plane3, plane4, plane5;

    public float maxMagnitudeVisible;

    public void Execute(int i)
    {
        if (magnitudesNA[i] < maxMagnitudeVisible)
        {
            Bounds bounds = new Bounds(positionNA[i], scaleNA[i]);
            cullResultsNA[i] = Util.FrustumCullingStatus(ref plane0, ref plane1, ref plane2, ref plane3, ref plane4, ref plane5, ref bounds);
        }
        else
        {
            cullResultsNA[i] = Util.CullFlags.CULLED;
        }
    }
}
