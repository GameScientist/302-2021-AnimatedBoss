using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Used to simulate stepping when the boss moves.
/// </summary>
public class StickyFeet : MonoBehaviour
{
    /// <summary>
    /// This component is checked to see if the boss is attacking or not.
    /// </summary>
    private BossController boss;
    /// <summary>
    /// The amount of time that has been taken to take a step.
    /// </summary>
    private float stepTimeCurrent = 0.5f;
    /// <summary>
    /// The amount of time it takes for the boss to raise its foot into the air.
    /// </summary>
    private readonly float stepTimeLength = 0.5f;
    /// <summary>
    /// The amount of time it has been taken to stomp on the player.
    /// </summary>
    private float stompTimeCurrent = 0f;
    /// <summary>
    /// The amount of time it takes to stomp on the player.
    /// </summary>
    private readonly float stompTimeLength = 1;
    /// <summary>
    /// Controls the verticality of the foot while stomping.
    /// </summary>
    public AnimationCurve stompMovement;
    /// <summary>
    /// Controls the verticality of the foot while stepping.
    /// </summary>
    public AnimationCurve verticalStepMovement;
    /// <summary>
    /// The point that the boss will try to step on when it is time for it to take a step.
    /// </summary>
    public Transform stepPosition;
    /// <summary>
    /// Tracks whether the foot has already moved to help determine if it is time to take a step.
    /// </summary>
    public bool footHasMoved = false;
    /// <summary>
    /// Controls the rotation of a foot after taking a step.
    /// </summary>
    public Quaternion plantedRotation;
    /// <summary>
    /// Tracks the rotation of the foot before taking a step.
    /// </summary>
    public Quaternion previousPlantedRotation;
    /// <summary>
    /// The original rotation of the foot at runtime.
    /// </summary>
    public Quaternion startingRotation;
    /// <summary>
    /// The position of a foot before a step has been taken.
    /// </summary>
    public Vector3 previousPlantedPosition;
    /// <summary>
    /// The position of a foot after a step has been taken.
    /// </summary>
    public Vector3 plantedPosition;
    public bool IsAnimating
    {
        get { return stepTimeCurrent < stepTimeLength; }
    }

    // Start is called before the first frame update
    void Start()
    {
        BossRibsAnimator ribs = GetComponentInParent<BossRibsAnimator>();
        if (ribs == null) boss = null;
        else boss = GetComponentInParent<BossController>();
        startingRotation = transform.localRotation;
        previousPlantedPosition = transform.position;
        plantedPosition = stepPosition.position;
        plantedRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (boss != null && boss.bossState == BossController.BossStates.Attack)Stomp();
        else CheckForAnimation();
    }
    
    private void Stomp()
    {
        stompTimeCurrent += Time.deltaTime;

        float p = stompTimeCurrent / stompTimeLength;

        Vector3 finalPosition = plantedPosition;
        finalPosition.z += stompMovement.Evaluate(p);
        transform.position = finalPosition;
    }

    private void CheckForAnimation()
    {
        if (IsAnimating) Step();
        else
        { // animation is NOT playing
            transform.position = plantedPosition;
            transform.rotation = plantedRotation;
        }
        stompTimeCurrent = 0;
    }

    private void Step()
    {
        stepTimeCurrent += Time.deltaTime;

        float p = stepTimeCurrent / stepTimeLength;

        Vector3 finalPosition = AnimMath.Lerp(previousPlantedPosition, plantedPosition, p);
        finalPosition.y += verticalStepMovement.Evaluate(p);
        transform.position = finalPosition;
        transform.rotation = AnimMath.Lerp(previousPlantedRotation, plantedRotation, p);

        Vector3 vFromCenter = transform.position - transform.parent.position;

        float height = vFromCenter.y;
        vFromCenter.y = 0;
        vFromCenter.Normalize();
        vFromCenter *= 3;
        vFromCenter.y += height;
    }

    public bool TryToStep()
    {
        // if animating, don't try to step
        if (IsAnimating) return false;

        if (footHasMoved) return false;

        Vector3 vBetween = transform.position - stepPosition.position;
        // if too close to previous target, don't try to step:
        if (vBetween.sqrMagnitude < 5 * 5) return false;

        Ray ray = new Ray(stepPosition.position + Vector3.up, Vector3.down);

        Debug.DrawRay(ray.origin, ray.direction * 50);
        if (Physics.Raycast(ray, out RaycastHit hit, 50))
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
            stepTimeCurrent = 0;

            footHasMoved = true;

            return true;
        }
        return false;

    }
}
