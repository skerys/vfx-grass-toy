using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

[VFXType(VFXTypeAttribute.Usage.GraphicsBuffer)]
struct HitInfo
{
    public Vector3 position;
    public Vector3 normal;
}

public class SpawnParticleOnMouseClick : MonoBehaviour
{
    Camera myCam;

    public VisualEffect vfx;

    public float inaccuracyAngle = 10f;
    public int shotCount = 10;

    GraphicsBuffer positionBuffer;

    void Start()
    {
        myCam = GetComponent<Camera>();
    }

    void Update()
    {
        if(Input.GetMouseButton(0))
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
                if(Physics.Raycast(inaccurateRay, out hit, 10000f))
                {
                    HitInfo info;
                    info.position = hit.point + hit.normal * 0.0001f;
                    info.normal = hit.normal;
                    Debug.Log(info.normal);
                    hits.Add(info);
                }
            }

            if(hits.Count > 0)
            {
                if(positionBuffer != null)
                {
                    positionBuffer.Release();
                }
                positionBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, hits.Count, sizeof(float) * 6);
                positionBuffer.SetData(hits);

                vfx.SetGraphicsBuffer("PositionBuffer", positionBuffer);
                vfx.SendEvent("SpawnBurst");
            }

            
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
