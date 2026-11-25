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

    [SerializeField] private GameObject player;

    public AlertStage alertStage;

    public void Awake()
    {
        alertStage = AlertStage.Searching; //All Elves begin the game searching for player
    }

    public void Update()
    {
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
