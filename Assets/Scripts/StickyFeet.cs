using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyFeet : MonoBehaviour
{
    public Transform stepPosition;
    public AnimationCurve verticalStepMovement;
    public AnimationCurve rotationMovement;
    public AnimationCurve stompMovement;
    public Quaternion startingRotation;
    public Vector3 previousPlantedPosition;
    public Quaternion previousPlantedRotation;
    public Vector3 plantedPosition;
    public Quaternion plantedRotation;
    private float timeLength = 0.5f;
    private float timeCurrent = 0.5f;
    private float stompTimeCurrent = 0f;
    private float stompTimeLength = 1;
    BossController boss;
    Transform kneePole;
    public static float moveThreshold = 5;
    public bool isAnimating
    {
        get { return (timeCurrent < timeLength); }
    }
    public bool footHasMoved = false;

    // Start is called before the first frame update
    void Start()
    {
        kneePole = transform.GetChild(0);
        startingRotation = transform.localRotation;
        BossRibsAnimator ribs = GetComponentInParent<BossRibsAnimator>();
        if (ribs == null) boss = null;
        else boss = GetComponentInParent<BossController>();
        //DoRayCast();
        previousPlantedPosition = transform.position;
        plantedPosition = stepPosition.position;
        print(transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        if (boss != null && boss.bossState == BossController.BossStates.Attack)
        {
            stompTimeCurrent += Time.deltaTime;

            float p = stompTimeCurrent / stompTimeLength;

            Vector3 finalPosition = plantedPosition;
            finalPosition.z += stompMovement.Evaluate(p);
            transform.position = finalPosition;
        }
        else
        {
            if (isAnimating)
            {
                timeCurrent += Time.deltaTime;

                float p = timeCurrent / timeLength;

                Vector3 finalPosition = AnimMath.Lerp(previousPlantedPosition, plantedPosition, p);
                finalPosition.y += verticalStepMovement.Evaluate(p);
                transform.position = finalPosition;
                transform.rotation = AnimMath.Lerp(previousPlantedRotation, plantedRotation, p);
                //Quaternion finalRotation = AnimMath.Lerp(previousPlantedRotation, plantedRotation, p);
                //finalRotation.x += rotationMovement.Evaluate(p);
                //transform.localRotation = finalRotation;

                Vector3 vFromCenter = transform.position - transform.parent.position;

                float height = vFromCenter.y;
                vFromCenter.y = 0;
                vFromCenter.Normalize();
                vFromCenter *= 3;
                vFromCenter.y += height;
            }
            else
            { // animation is NOT playing
                transform.position = plantedPosition;
                transform.rotation = plantedRotation;
                //print(gameObject + "Plant feet.");
            }
            stompTimeCurrent = 0;
        }
    }

    public bool TryToStep()
    {
        // if animating, don't try to step
        if (isAnimating) return false;

        if (footHasMoved) return false;

        Vector3 vBetween = transform.position - stepPosition.position;
        // if too close to previous target, don't try to step:
        if (vBetween.sqrMagnitude < moveThreshold * moveThreshold) return false;

        Ray ray = new Ray(stepPosition.position + Vector3.up, Vector3.down);

        Debug.DrawRay(ray.origin, ray.direction * 3);
        if (Physics.Raycast(ray, out RaycastHit hit, 3))
        {
            // setup beginning of animation:
            previousPlantedPosition = transform.position;
            previousPlantedRotation = transform.rotation;

            //set rotation to starting rotation:
            transform.localRotation = startingRotation;

            // setup end of animation:
            plantedPosition = hit.point;
            plantedRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
            print(hit.normal + " - " + transform.up);

            // begin animation:
            timeCurrent = 0;

            footHasMoved = true;

            return true;
        }
        return false;

    }
}
