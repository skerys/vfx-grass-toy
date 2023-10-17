using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

[VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)]
struct HitInfo
{
    public Vector3 position;
    public Vector3 normal;
}

public class SpawnParticleOnMouseClick : MonoBehaviour
{
    public enum VfxMode {Grass, Flower}
    
    
    Camera myCam;

    public VisualEffect grassVfx;
	public VisualEffect flowerVfx;

	private VisualEffect currentVfx;

    public float inaccuracyAngle = 10f;
    private int shotCount;

    GraphicsBuffer positionBuffer;

    void Start()
    {
        myCam = GetComponent<Camera>();
        ChangeActiveVfx(VfxMode.Grass);
    }

    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            SpawnGrass();
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeActiveVfx(VfxMode.Grass);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeActiveVfx(VfxMode.Flower);
        }
    }

    private void SpawnGrass()
    {
        Ray mouseRay = myCam.ScreenPointToRay(Input.mousePosition);
        List<HitInfo> hits = new List<HitInfo>();

        for (int i = 0; i < shotCount; i++)
        {
            RaycastHit hit;

            Ray inaccurateRay = mouseRay;

            Vector2 randomInCircle = Random.insideUnitCircle * inaccuracyAngle;

            Quaternion xRot = Quaternion.AngleAxis(randomInCircle.x, transform.right);
            Quaternion yRot = Quaternion.AngleAxis(randomInCircle.y, transform.up);

            inaccurateRay.direction = xRot * yRot * inaccurateRay.direction;
            if (Physics.Raycast(inaccurateRay, out hit, 10000f))
            {
                HitInfo info;
                info.position = hit.point + hit.normal * 0.0001f;
                info.normal = hit.normal;
                hits.Add(info);
            }
        }

        if (hits.Count > 0)
        {
            if (positionBuffer != null)
            {
                positionBuffer.Release();
            }

            positionBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, hits.Count, sizeof(float) * 6);
            positionBuffer.SetData(hits);
            
            Debug.Log($"send buffer of size {positionBuffer.count}");

            currentVfx.SetGraphicsBuffer("PositionBuffer", positionBuffer);
            currentVfx.SendEvent("SpawnBurst");
        }
    }

    void ChangeActiveVfx(VfxMode mode)
    {
        switch (mode)
        {
            case VfxMode.Grass:
                currentVfx = grassVfx;
                shotCount = 50;
                break;
            case VfxMode.Flower:
                currentVfx = flowerVfx;
                shotCount = 10;
                break;
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
