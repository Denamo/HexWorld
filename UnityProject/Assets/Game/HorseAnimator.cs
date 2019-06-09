using UnityEngine;

public class HorseAnimator : MonoBehaviour
{

    [SerializeField]
    private Animator animator;
    
    // Use this for initialization
    void Start()
    {
       previousPosition = transform.position;
    }

    // Update is called once per frame
    private Vector3 previousPosition;
    void FixedUpdate()
    {
        if (animator != null)
        {
            float speed = (transform.position - previousPosition).magnitude / Time.fixedDeltaTime;
            previousPosition = transform.position;
            animator.SetFloat("velocity", speed);
        }
    }
}
