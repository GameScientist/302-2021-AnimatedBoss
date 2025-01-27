using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class StickyFeetLab : MonoBehaviour
{
    /// <summary>
    /// How far away to allow the foot to slide.
    /// </summary>
    public static float moveThreshold = 2;
    public Transform stepPosition;
    public AnimationCurve verticalStepMovement;
    public AnimationCurve rotationMovement;
    public AnimationCurve stompMovement;
    private Quaternion startingRotation;
    private Vector3 previousPlantedPosition;
    private Quaternion previousPlantedRotation;
    private Vector3 plantedPosition;
    private Quaternion plantedRotation;
    private Vector3 stompPosition;
    private float timeLength = 0.5f;
    private float timeCurrent = 0.5f;
    private float stompTimeCurrent = 0f;
    private float stompTimeLength = 1;
    public float offset;
    BossController boss;
    Transform kneePole;
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
    }

    // Update is called once per frame
    void Update()
    {
        {
            if (isAnimating)
            {
                timeCurrent += Time.deltaTime;

                float p = timeCurrent / timeLength;

                Vector3 finalPosition = AnimMath.Lerp(previousPlantedPosition, plantedPosition, p);
                finalPosition.y += verticalStepMovement.Evaluate(p);
                transform.position = finalPosition;
                transform.rotation = AnimMath.Lerp(previousPlantedRotation, plantedRotation, p);

                Vector3 vFromCenter = transform.position - transform.parent.position;

                vFromCenter.y = 0;
                vFromCenter.Normalize();
                vFromCenter *= 3;
                vFromCenter.y += 2.5f;
                //vFromCenter += transform.position;

                //print(gameObject + "Pick up feet.");
                //print(gameObject + ": " + timeCurrent);
                kneePole.position = vFromCenter + transform.position;
            }
            else
            { // animation is NOT playing
                transform.position = plantedPosition;
                transform.rotation = plantedRotation;
                //print(gameObject + "Plant feet.");
            }
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

            // begin animation:
            timeCurrent = 0;

            footHasMoved = true;

            return true;
        }
        return false;

    }
}
