using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class CreatorPathController : MonoBehaviour
{
    public PathCreator pathCreation;
    public float speed = 5;
    float distanceTravelled;

    private void Update()
    {
        distanceTravelled += speed * Time.deltaTime;
        transform.position = pathCreation.path.GetPointAtDistance(distanceTravelled);
        transform.rotation = pathCreation.path.GetRotationAtDistance(distanceTravelled);
    }
}
