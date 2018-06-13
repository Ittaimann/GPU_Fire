/*
Heavily based on https://gist.github.com/ya7gisa0/742bf24d5edf1e73b971e14a2553ad4e with a 
lot of modifications. It was the only bones for the particle system we were looking for that 
I could find. We removed elements from it were adding others, but becuase the nature of the project
constantly changing (mostly due to ittai constantly trying to experiment) a lot of the original code stayed.
We removed most of what was there to create their effect, but the set up for the compute shader stayed pretty much the same. 
 */


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunCompute : MonoBehaviour
{


    // struct
    struct Particle
    {
        public Vector3 SpawnPos;
        public Vector3 position;
        public Vector3 velocity;
        public float life;
        public Vector3 Normal;
    }

  
    private const int SIZE_PARTICLE =52; // how many bytes an individual particle is. each float is 4 bytes, so 4 float 4's and a float make 52 bytes per particle

  
    private int particleCount = 1000000;

    /// Material with particle shader used to draw the Particle on screen.
    public Material material;

    /// Compute shader used to update the Particles.
    public ComputeShader computeShader;

    /// Id of the kernel used.
    private int mComputeShaderKernelID;

    /// Buffer holding the Particles.
    ComputeBuffer particleBuffer;

    /// Number of particle per warp.
    private const int WARP_SIZE = 256;

    // number of threads? this is from the example code and im not entirely sure about how it relates
    private int mWarpCount; 

    // mesh used as the spawn verticies for the particles
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
          
            verts[verexs]=transform.TransformPoint(verts[verexs]);
            normals[verexs]=transform.TransformDirection(normals[verexs]);

            particleArray[i].SpawnPos.x = particleArray[i].position.x = verts[verexs].x;
            particleArray[i].SpawnPos.y = particleArray[i].position.y = verts[verexs].y; 
            particleArray[i].SpawnPos.z = particleArray[i].position.z = verts[verexs].z;

            // never really used in the program /shrug
            particleArray[i].Normal.x = normals[verexs].x;
            particleArray[i].Normal.y =normals[verexs].y;
            particleArray[i].Normal.z = normals[verexs].z;

            particleArray[i].velocity.x = 0;
            particleArray[i].velocity.y = 0;
            particleArray[i].velocity.z = 0;

            // Initial life value
            particleArray[i].life = Random.value * 2.0f + 1.0f;
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
        
        if(transform.hasChanged)
        {
            Vector3 changeFromPrev= (transform.position-prevLoc);
            float[] change = { changeFromPrev.x, changeFromPrev.y, changeFromPrev.z };

            computeShader.SetFloats("newPos", change);

        }


        computeShader.SetFloat("deltaTime", Time.deltaTime);
        computeShader.SetFloat("totalTime", Time.time);
        float[] ObjPos= {transform.position.x,transform.position.y,transform.position.z};
        computeShader.SetFloats("ObjPos", ObjPos);
       
        computeShader.Dispatch(mComputeShaderKernelID, mWarpCount, 1, 1);
        prevLoc=transform.position;
    }
    
}