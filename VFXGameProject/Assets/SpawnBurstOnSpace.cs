using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SpawnBurstOnSpace : MonoBehaviour
{
    public VisualEffect vfx;

    public List<Vector3> positions;

    void Start()
    {
        positions = new List<Vector3>();

        for(int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                positions.Add(new Vector3(i, 0, j));
            }
        }
    }

    GraphicsBuffer positionBuffer;


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            if(positionBuffer != null)
            {
                positionBuffer.Release();
            }

            positionBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, positions.Count, sizeof(float) * 3);
            positionBuffer.SetData(positions.ToArray());

            vfx.SetGraphicsBuffer("PositionBuffer", positionBuffer);
            vfx.SendEvent("SpawnBurst");
            //positionBuffer.Dispose();
        }
    }

    void OnDisable()
    {
        if(positionBuffer != null)
        {
            positionBuffer.Release();
        }
    }
}
