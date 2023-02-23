using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackGenerator : MonoBehaviour
{
    [SerializeField] GameObject RailPrefab;
    [SerializeField] Transform Track;
    [SerializeField] GameObject CarPrefab;
    [SerializeField] GameObject XROrigin;

    RailProperties lastRail;

    CarController car;

    // Start is called before the first frame update
    void Start()
    {
        lastRail = Instantiate(RailPrefab, Vector3.zero, Quaternion.identity, Track).GetComponent<RailProperties>();
        lastRail.RailID = 0;
        lastRail.Generate(0, 0, 0);

        for (int i = 0; i < 20; i++)
        {
            RailProperties newRail = CreateRail();

            if (i == 9)
            {
                car = Instantiate(CarPrefab).GetComponent<CarController>();
                car.currentRail = newRail;
                car.XROrigin = XROrigin;
            }
        }
    }

    RailProperties CreateRail()
    {
        RailProperties newRail = Instantiate(RailPrefab, lastRail.End, Quaternion.Euler(0, lastRail.angle, 0) *
                Quaternion.Euler(0, lastRail.transform.rotation.eulerAngles.y, 0), Track).GetComponent<RailProperties>();
        newRail.RailID = lastRail.RailID + 1;

        lastRail.nextRail = newRail;
        newRail.previousRail = lastRail;

        newRail.Generate(lastRail.nextPlankPosition, lastRail.nextInnerBarPosition, lastRail.nextOuterBarPosition);

        lastRail = newRail;

        return newRail;
    }

    // Update is called once per frame
    void Update()
    {
        int firstID = Track.GetChild(0).GetComponent<RailProperties>().RailID;
        int lastID = Track.GetChild(Track.childCount - 1).GetComponent<RailProperties>().RailID;

        if (car.currentRail.RailID - firstID > 10)
            Destroy(Track.GetChild(0).gameObject);

        if (lastID - car.currentRail.RailID < 10)
            CreateRail();
    }
}
