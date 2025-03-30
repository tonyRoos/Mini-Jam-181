using UnityEngine;

public class LookAt : MonoBehaviour
{
    [SerializeField] private bool follow = true;
    [SerializeField] private bool lateralMovement = true;
    private Transform player;
    private const float _MIN_DISTANCE = 8f;
    private const float _MAX_DISTANCE = 10f;
    private const float _MAX_LATERAL_DISTANCE = 2F;
    

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private bool isPlayerLeftToCamera;
    private void Update()
    {
        transform.LookAt(player);
        if (!follow) return;

        if(Mathf.Abs((transform.position.z - player.position.z)) > _MAX_DISTANCE)
        {
            transform.Translate( 0, 0, Time.deltaTime * 4, Space.World);
        } else if (Mathf.Abs((transform.position.z - player.position.z)) < _MIN_DISTANCE)
        {
            transform.Translate(0, 0, Time.deltaTime * -4, Space.World);
        }

        if(lateralMovement && Mathf.Abs((transform.position.x - player.position.x)) > _MAX_LATERAL_DISTANCE)
        {
            isPlayerLeftToCamera = (transform.position.x - player.position.x) > 0;
            transform.Translate(Time.deltaTime * 4 * (isPlayerLeftToCamera ? -1 : 1), 0, 0, Space.World);
        }
    }
}
