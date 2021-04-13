using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyFeet : MonoBehaviour
{
    public Transform stepPosition;
    public AnimationCurve verticalStepMovement;
    public AnimationCurve rotationMovement;
    public Vector3 previousPlantedPosition;
    public Quaternion previousPlantedRotation;
    public Vector3 plantedPosition;
    public Quaternion plantedRotation;
    private float timeLength = 1;
    private float timeCurrent = 1;
    public float offset;
    public StickyFeet parallelFoot;

    // Start is called before the first frame update
    void Start()
    {
        //DoRayCast();
        previousPlantedPosition = transform.position;
        plantedPosition = stepPosition.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (CheckIfCanStep())
        {
            DoRayCast();
            //print(gameObject + "Do ray cast.");
        }
        if (timeCurrent < timeLength)
        {
            timeCurrent += Time.deltaTime;

            float p = timeCurrent / timeLength;

            Vector3 finalPosition = AnimMath.Lerp(previousPlantedPosition, plantedPosition, p);
            finalPosition.y += verticalStepMovement.Evaluate(p);
            transform.position = finalPosition;
            //transform.rotation = AnimMath.Lerp(previousPlantedRotation, plantedRotation, p);
            //print(gameObject + "Pick up feet.");
            //print(gameObject + ": " + timeCurrent);
        }
        else
        {
            transform.position = plantedPosition;
            //transform.rotation = plantedRotation;
            //print(gameObject + "Plant feet.");
        }
        print(timeCurrent);
    }

    bool CheckIfCanStep()
    {
        Vector3 vBetween = transform.position - stepPosition.position;
        float threshold = 20 + offset;
        //print(gameObject + ": " + vBetween);
        return (vBetween.sqrMagnitude > threshold * threshold);
    }

    void DoRayCast()
    {
        Ray ray = new Ray(stepPosition.position + Vector3.up, Vector3.down) ;

        Debug.DrawRay(ray.origin, ray.direction * 3);
        if (Physics.Raycast(ray, out RaycastHit hit, 3))
        {
            previousPlantedPosition = transform.position;
            previousPlantedRotation = transform.rotation;

            plantedPosition = hit.point;
            plantedRotation = Quaternion.FromToRotation(transform.up, hit.normal);

            timeCurrent = 0;
        }
    }
}
