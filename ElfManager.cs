using UnityEditor.Toolbars;
using UnityEngine;

//MAIN TODO'S:
//TODO: TEST FOR WHEN THE PLAYER RUNS AWAY AND ALL ELVES LOSE SIGHT OF PLAYER
//TODO: IMPLEMENT PATROL SYSTEM
//TODO: MAKE ELVES AWARE OF TREES AND MOVE AROUND THEM
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
    public float jumpEmFov = 30; //The radius within which the elf searches for other elves before attacking

    public static bool playerFound;
    

    [SerializeField] private GameObject player;

    public AlertStage alertStage;

    public void Awake()
    {
        alertStage = AlertStage.Searching; //All Elves begin the game searching for player
    }

    public void Update()
    {
        if (alertStage == AlertStage.Alerted || playerFound == true)
        {
            //Turns towards Player when spotted
            Vector3 direction = player.transform.position - transform.position;
            direction.y = 0f;
            if (direction != Vector3.zero)
            {
                Quaternion lookrotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Euler(0f, lookrotation.eulerAngles.y, 0f);
            }
            //Counts the # of elves within a radius, if number is 5 or more then instead of waiting at a distance of 10, the 5 elves attack all at once
            Collider[] elfGang = Physics.OverlapSphere(transform.position, jumpEmFov);
            int elfGangNum = 0;
            foreach (Collider c in elfGang)
            {
                if (c.CompareTag("Elf"))
                {
                    elfGangNum++;
                }
            }
            if (Vector3.Distance(transform.position, player.transform.position) > 10 && elfGangNum < 5)
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
            else if (elfGangNum >= 5)
            {
                transform.position = Vector3.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
            }
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
                {
                    alertStage = AlertStage.Alerted;
                    playerFound = true;
                }
                break;
            case AlertStage.Alerted:
                if (!hasLineOfSight)
                {
                    alertStage = AlertStage.Searching;
                    playerFound = false;
                }
                break;
        }
    }

}
