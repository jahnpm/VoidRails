using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField] float speed = 5;
    public RailProperties currentRail;
    public GameObject XROrigin;
    float positionOnCurrentRail;

    // Start is called before the first frame update
    void Start()
    {
        positionOnCurrentRail = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentRail == null)
            return;

        (Vector3 point, float angle) pointAngle = currentRail.GetPointAndAngleOnRail(positionOnCurrentRail, 0);

        transform.position = pointAngle.point + 0.3f * Vector3.up;
        transform.rotation = Quaternion.Euler(0, pointAngle.angle, 0) * currentRail.transform.rotation;

        positionOnCurrentRail += speed * Time.deltaTime;

        if (positionOnCurrentRail > currentRail.length)
        {
            positionOnCurrentRail -= currentRail.length;
            currentRail = currentRail.nextRail;
        }

        Camera.main.transform.position = transform.position - 0.25f * transform.forward + transform.up;
        Camera.main.transform.rotation = transform.rotation;

        XROrigin.transform.position = transform.position - 0.25f * transform.forward + transform.up;
    }
}
