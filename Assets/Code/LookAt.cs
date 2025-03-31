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
    private Vector2 cameraSpeed;
    private void Update()
    {
        cameraSpeed = new Vector2((Mathf.Abs((transform.position.x - player.position.x)) - _MAX_LATERAL_DISTANCE),
            Mathf.Abs((transform.position.z - player.position.z)) - _MAX_DISTANCE);
        transform.LookAt(player);
        if (!follow) return;

        if(Mathf.Abs((transform.position.z - player.position.z)) > _MAX_DISTANCE)
        {
            transform.Translate( 0, 0, Time.deltaTime * cameraSpeed.y, Space.World);
        } else if (Mathf.Abs((transform.position.z - player.position.z)) < _MIN_DISTANCE)
        {
            transform.Translate(0, 0, Time.deltaTime * -4f, Space.World);
        }

        if(lateralMovement && Mathf.Abs((transform.position.x - player.position.x)) > _MAX_LATERAL_DISTANCE)
        {
            isPlayerLeftToCamera = (transform.position.x - player.position.x) > 0;
            transform.Translate(Time.deltaTime * cameraSpeed.x * (isPlayerLeftToCamera ? -1 : 1), 0, 0, Space.World);
        }
    }
}
