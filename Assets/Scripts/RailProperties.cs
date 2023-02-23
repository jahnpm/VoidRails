using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RailProperties : MonoBehaviour
{
    public int RailID;
    public Vector3 End;
    public float angle;
    public float nextPlankPosition;
    public float nextInnerBarPosition;
    public float nextOuterBarPosition;
    public RailProperties previousRail;
    public RailProperties nextRail;
    public float length;
    public bool rightCurve;

    [SerializeField] GameObject PlankPrefab;
    [SerializeField] GameObject BarPrefab;

    float radius;
    bool curved;
    float innerLength;
    float outerLength;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Generate(float nextPlankPos, float nextInnerBarPos, float nextOuterBarPos)
    {
        int roll = Random.Range(0, 6);
        if (roll < 4)
        {
            curved = false;

            //if (roll == 0)
                //transform.rotation = Quaternion.AngleAxis(Random.Range(-45f, 45f), transform.right) * transform.rotation;
        }
        else
            curved = true;

        if (curved)
        {
            if (Random.Range(0, 2) == 0)
                rightCurve = true;
            else
                rightCurve = false;

            angle = Random.Range(10f, 180f);
            radius = Random.Range(2f, 20f);
            length = 2 * radius * Mathf.PI * (angle / 360f);
            innerLength = 2 * (radius - 0.3f) * Mathf.PI * (angle / 360f);
            outerLength = 2 * (radius + 0.3f) * Mathf.PI * (angle / 360f);

            if (!rightCurve)
                angle *= -1;
        }
        else
        {
            rightCurve = false;
            angle = 0;
            radius = -1;
            length = Random.Range(2f, 20f);
            innerLength = length;
            outerLength = length;
        }

        End = GetPointAndAngleOnRail(length, 0).point;

        if (previousRail != null)
            if (previousRail.rightCurve ^ rightCurve)
            {
                float temp = nextInnerBarPos;
                nextInnerBarPos = nextOuterBarPos;
                nextOuterBarPos = temp;
            }

        SpawnPlanks(nextPlankPos);
        SpawnInnerBars(nextInnerBarPos);
        SpawnOuterBars(nextOuterBarPos);
    }

    void SpawnPlanks(float nextPlankPos)
    {
        float currentSpawnDistance = nextPlankPos;

        while (currentSpawnDistance <= length)
        {
            (Vector3 point, float angle) pointAngle = GetPointAndAngleOnRail(currentSpawnDistance, 0);

            GameObject plank = Instantiate(PlankPrefab, transform, true);
            plank.transform.position = pointAngle.point;
            plank.transform.rotation = Quaternion.Euler(0, pointAngle.angle, 0) * transform.rotation;

            currentSpawnDistance += 0.5f;
        }

        nextPlankPosition = currentSpawnDistance - length;
    }

    void SpawnInnerBars(float nextInnerBarPos)
    {
        float currentSpawnDistance = nextInnerBarPos;

        while (currentSpawnDistance <= innerLength)
        {
            (Vector3 point, float angle) pointAngle = GetPointAndAngleOnRail(currentSpawnDistance, -0.3f);

            GameObject bar = Instantiate(BarPrefab, transform, true);
            bar.GetComponent<BarProperties>().positionOnRail = currentSpawnDistance;
            bar.transform.position = pointAngle.point + 0.0375f * transform.up;
            bar.transform.rotation = Quaternion.Euler(0, pointAngle.angle, 0) * transform.rotation;

            currentSpawnDistance += 0.25f;
        }

        nextInnerBarPosition = currentSpawnDistance - innerLength;
    }

    void SpawnOuterBars(float nextOuterBarPos)
    {
        float currentSpawnDistance = nextOuterBarPos;

        while (currentSpawnDistance <= outerLength)
        {
            (Vector3 point, float angle) pointAngle = GetPointAndAngleOnRail(currentSpawnDistance, 0.3f);

            GameObject bar = Instantiate(BarPrefab, transform, true);
            bar.GetComponent<BarProperties>().positionOnRail = currentSpawnDistance;
            bar.transform.position = pointAngle.point + 0.0375f * transform.up;
            bar.transform.rotation = Quaternion.Euler(0, pointAngle.angle, 0) * transform.rotation;

            currentSpawnDistance += 0.25f;
        }

        nextOuterBarPosition = currentSpawnDistance - outerLength;
    }

    public (Vector3 point, float angle) GetPointAndAngleOnRail(float distance, float offset)
    {
        if (curved)
        {
            float offsetRadius = radius + offset;
            float offsetLength = Mathf.Abs(2 * offsetRadius * Mathf.PI * (angle / 360f));

            float angleAtPoint = (distance / offsetLength) * angle;

            float x = offsetRadius * Mathf.Sin(Mathf.Deg2Rad * angleAtPoint);
            float y = offsetRadius * Mathf.Cos(Mathf.Deg2Rad * angleAtPoint);

            if (rightCurve)
                return (transform.position + x * transform.forward + (offsetRadius - offset - y) * transform.right, angleAtPoint);
            else
                return (transform.position - x * transform.forward - (offsetRadius - offset - y) * transform.right, angleAtPoint);
        }
        else
        {
            return (transform.position + offset * transform.right + distance * transform.forward, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
