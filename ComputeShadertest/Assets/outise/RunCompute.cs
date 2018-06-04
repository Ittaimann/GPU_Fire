﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunCompute : MonoBehaviour
{

    private Vector2 cursorPos;

    // struct
    struct Particle
    {
        public Vector3 SpawnPos;
        public Vector3 position;
        public Vector3 velocity;
        public float life;
        public Vector3 Normal;
    }

    /// <summary>
	/// Size in octet of the Particle struct.
    /// since float = 4 bytes...
    /// 4 floats = 16 bytes
	/// </summary>
	//private const int SIZE_PARTICLE = 24;
    private const int SIZE_PARTICLE =52; // since property "life" is added...

    /// <summary>
    /// Number of Particle created in the system.
    /// </summary>
    private int particleCount = 1000000;

    /// <summary>
    /// Material used to draw the Particle on screen.
    /// </summary>
    public Material material;

    /// <summary>
    /// Compute shader used to update the Particles.
    /// </summary>
    public ComputeShader computeShader;

    /// <summary>
    /// Id of the kernel used.
    /// </summary>
    private int mComputeShaderKernelID;

    /// <summary>
    /// Buffer holding the Particles.
    /// </summary>
    ComputeBuffer particleBuffer;

    /// <summary>
    /// Number of particle per warp.
    /// </summary>
    private const int WARP_SIZE = 256; // TODO?

    /// <summary>
    /// Number of warp needed.
    /// </summary>
    private int mWarpCount; // TODO?

    private Mesh mesh;
    //public ComputeShader shader;
    Vector3 prevLoc;
    // Use this for initialization
    void Start()
    {

        InitComputeShader();

    }

    void InitComputeShader()
    {
        mWarpCount = Mathf.CeilToInt((float)particleCount / WARP_SIZE);

        // initialize the particles
        Particle[] particleArray = new Particle[particleCount];
        mesh= GetComponent<MeshFilter>().mesh;
        int verexs= 0;
        List<Vector3> verts = new List<Vector3>();
        mesh.GetVertices(verts);
        List<Vector3> normals = new List<Vector3>();
        mesh.GetNormals(normals);

        for (int i = 0; i < particleCount; i++)
        {
           /* float x = Random.value * 2 - 1.0f;
            float y = Random.value * 2 - 1.0f;
            float z = Random.value * 2 - 1.0f;
            Vector3 xyz = new Vector3(x, y, z);
            xyz.Normalize();
            xyz *= Random.value;
            xyz *= 0.5f;*/
            verts[verexs]=transform.TransformPoint(verts[verexs]);
            normals[verexs]=transform.TransformDirection(normals[verexs]);

            particleArray[i].SpawnPos.x = particleArray[i].position.x = verts[verexs].x;
            particleArray[i].SpawnPos.y = particleArray[i].position.y = verts[verexs].y; 
            particleArray[i].SpawnPos.z = particleArray[i].position.z = verts[verexs].z;

            particleArray[i].Normal.x = normals[verexs].x;
            particleArray[i].Normal.y =normals[verexs].y;
            particleArray[i].Normal.z = normals[verexs].z;

            particleArray[i].velocity.x = 0;
            particleArray[i].velocity.y = 0;
            particleArray[i].velocity.z = 0;

            // Initial life value
            particleArray[i].life = Random.value * 5.0f + 1.0f;
            verexs+=1;
            if(verexs>=mesh.vertexCount)
            {
                verexs=0;
            }
        }

        // create compute buffer
        particleBuffer = new ComputeBuffer(particleCount, SIZE_PARTICLE);

        particleBuffer.SetData(particleArray);

        // find the id of the kernel
        mComputeShaderKernelID = computeShader.FindKernel("CSParticle");

        // bind the compute buffer to the shader and the compute shader
        computeShader.SetBuffer(mComputeShaderKernelID, "particleBuffer", particleBuffer);
        material.SetBuffer("particleBuffer", particleBuffer);
    }

    void OnRenderObject()
    {
        material.SetPass(0);
        Graphics.DrawProcedural(MeshTopology.Points, 1, particleCount);
    }

    void OnDestroy()
    {
        if (particleBuffer != null)
            particleBuffer.Release();
    }

    // Update is called once per frame
    void Update()
    {   
        
    //    Vector3 newPoint=transform.TransformPoint(mesh.vertices[Random.Range(0, mesh.vertexCount)]);
       // Vector3 newNormals = transform.TransformPoint(mesh.normals[Random.Range(0, mesh.vertexCount)]);

//        float[] newPos = {newPoint.x,newPoint.y,newPoint.z};
  //      float[] newNorm = { newNormals.x, newNormals.y, newNormals.z };

        // Mesh mesh = GetComponent<MeshFilter>().mesh;
        // Vector3[] vertices = mesh.vertices;
        if(transform.hasChanged)
        {
            Vector3 changeFromPrev= (transform.position-prevLoc);
            float[] change = { changeFromPrev.x, changeFromPrev.y, changeFromPrev.z };

            computeShader.SetFloats("newPos", change);

        }

        //	while(i<vertices.Length){
        //		float[] verts={vertices[i].x,vertices[i].y, vertices[i].z};
        // Send datas to the compute shader
        computeShader.SetFloat("deltaTime", Time.deltaTime);
        computeShader.SetFloat("totalTime", Time.time);

        //	computeShader.SetFloats("newPos", newPos);
        // computeShader.SetFloats("normals", newNorm);

        // Update the Particles
        //}
        computeShader.Dispatch(mComputeShaderKernelID, mWarpCount, 1, 1);
        prevLoc=transform.position;
    }

    void OnGUI()
    {
        Vector3 p = new Vector3();
        Camera c = Camera.main;
        Event e = Event.current;
        Vector2 mousePos = new Vector2();

        // Get the mouse position from Event.
        // Note that the y position from Event is inverted.
        mousePos.x = e.mousePosition.x;
        mousePos.y = c.pixelHeight - e.mousePosition.y;

        p = c.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, c.nearClipPlane + 14));// z = 3.
		Debug.Log(p);
        cursorPos.x = p.x;
        cursorPos.y = p.y;
        
        GUILayout.BeginArea(new Rect(20, 20, 250, 120));
        GUILayout.Label("Screen pixels: " + c.pixelWidth + ":" + c.pixelHeight);
        GUILayout.Label("Mouse position: " + mousePos);
        GUILayout.Label("World position: " + p.ToString("F3"));
        GUILayout.EndArea();
        
    }
}