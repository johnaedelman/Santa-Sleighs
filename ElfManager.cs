using UnityEngine;


public enum AlertStage
{
    Searching,
    Alerted
}
public class ElfManager : MonoBehaviour
{

    public float fov = 15f; //FOV distance
    [Range(0, 360)] public float fovAngle; //FOV range
    private bool hasLineOfSight;

    private float moveSpeed = 5f;

    [SerializeField] private GameObject player;

    public AlertStage alertStage;

    public void Awake()
    {
        alertStage = AlertStage.Searching; //All Elves begin the game searching for player
    }

    public void Update()
    {
        if (alertStage == AlertStage.Alerted)
        {
            //Turns towards Player when spotted
            Vector3 direction = player.transform.position - transform.position;
            direction.y = 0f;
            if (direction != Vector3.zero)
            {
                Quaternion lookrotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Euler(0f, lookrotation.eulerAngles.y, 0f);
            }
            //Moves towards player until within a radius of 10 
            //TODO: Add an "Elf Count" using targetsInFov Collider[] to attack when a certain number of elves are nearby
            if (Vector3.Distance(transform.position, player.transform.position) > 10)
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
        }
    bool playerInFov = false;
        Collider[] targetsInFov = Physics.OverlapSphere(transform.position, fov);  //List of objects in Elf's FOV 360 degrees
        foreach (Collider c in targetsInFov)
        {
            if (c.CompareTag("Player"))
            {
                //Makes sure Player is in the chosen FOV arch, not outside of it
                float signedAngle = Vector3.Angle(transform.forward, c.transform.position - transform.position);
                if (Mathf.Abs(signedAngle) < fovAngle / 2)
                    playerInFov = true;
                break;
            }
        }

        //Sends Raycast to see if any trees are between Elf and Player
        if (playerInFov)
        {
            RaycastHit ray;
            if (Physics.Raycast(transform.position, player.transform.position - transform.position, out ray))
            {
                hasLineOfSight = ray.collider.CompareTag("Player");
                _UpdateAlertState(hasLineOfSight);
            }
        }
    }
    private void _UpdateAlertState(bool hasLineOfSight)
    {
        switch (alertStage)
        {
            case AlertStage.Searching:
                if (hasLineOfSight)
                    alertStage = AlertStage.Alerted;
                break;
            case AlertStage.Alerted:
                if (!hasLineOfSight)
                    alertStage = AlertStage.Searching;
                break;
        }
    }

}
