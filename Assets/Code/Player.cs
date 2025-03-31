using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum GameMode
{
    MAIN, PAUSED, CUT_SCENE
}

public class Player : MonoBehaviour
{
    public GuiManager guiManager;

    private const float _PLAYER_WALK_SPEED = 2.5f;
    private const float _PLAYER_RUN_SPEED = 4f;
    private const float _PLAYER_RUN_TIME = 5F;
    private float stamina = 5;
    private float totalSpeed;

    private Camera cam, cutsceneCam;
    public GameMode gameMode = GameMode.CUT_SCENE;

    private Inventory inventory = new Inventory();

    private Animator anim;
    private Rigidbody rb;
    private AudioSource audioSrc;
    [SerializeField] AudioClip stepSound;


    private void Awake()
    {
        inventory.init();
        cam = Camera.main;
        if (GameObject.FindGameObjectWithTag("CutsceneCamera"))
        {
            cutsceneCam = GameObject.FindGameObjectWithTag("CutsceneCamera").GetComponent<Camera>();
            cutsceneCam.gameObject.SetActive(false);
        }
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        audioSrc = GetComponent<AudioSource>();
        anim.SetFloat("speed", 0);
    }

    private void Update()
    {
        switch (gameMode)
        {
            case GameMode.MAIN:
                move();
                updateGui();
                //setMenu(true);
                updateFieldOfView();
                updateVisibility();
                if(Input.GetKeyDown(KeyCode.F1) || transform.position.y < -4) { Die(); }
                break;
            case GameMode.PAUSED:
                break;
            case GameMode.CUT_SCENE:
                //setMenu(true);
                break;
        }
    }

    public void playStepSoun()
    {
        audioSrc.Stop();
        audioSrc.pitch = Random.Range(0.75f, 1.25f);
        audioSrc.PlayOneShot(stepSound);
    }

    private void setMenu(bool state)
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            guiManager.setPauseMenu(state);
            Time.timeScale = state ? 0 : 1;

        }
    }

    private void updateFieldOfView()
    {
        if (isCloseToItem)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 40, Time.deltaTime * 2f);
        }
        else
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 60, Time.deltaTime * 2f);
        }
    }

    private void move()
    {
        updatePlayerSpeed();
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), rb.linearVelocity.y/totalSpeed, Input.GetAxis("Vertical"));
        rb.linearVelocity = totalSpeed * movement;
        
        if (movement.x != 0)
        {
            transform.localScale = new Vector3((movement.x > 0 ? 1 : -1) * 0.3f, 0.3f, 1);
        }
        if(movement.z != 0)
        {
            anim.SetBool("isUp", movement.z > 0);
        }

        anim.SetFloat("speed", rb.linearVelocity.magnitude);
    }

    private void updatePlayerSpeed()
    {
        if (Input.GetKey(KeyCode.LeftShift) && stamina > 0 &&
            (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)))
        {
            stamina -= Time.deltaTime;
            totalSpeed = _PLAYER_RUN_SPEED;
        }
        else
        {
            stamina += Time.deltaTime / 2;
            totalSpeed = _PLAYER_WALK_SPEED;
        }


        stamina = Mathf.Clamp(stamina, 0, _PLAYER_RUN_TIME);
    }

    private void updateGui()
    {
        guiManager.updateStaminaBar(stamina / _PLAYER_RUN_TIME);
    }


    private void OnTriggerStay(Collider other)
    {

        if (Input.GetKey(KeyCode.Space) && other.GetComponent<GetItem>())
        {
            GetItem getItem = (GetItem)other.GetComponent<GetItem>();
            inventory.items.Add(getItem.item);
            guiManager.addItem(getItem.item);
            audioSrc.PlayOneShot(getItem.getSound);
            Destroy(other.gameObject);
            isCloseToItem = false;
        }

        if (Input.GetKeyDown(KeyCode.Space) && other.GetComponent<Puzzle_Hat>())
        {
            other.GetComponent<Puzzle_Hat>().transport(transform);
        }
    }

    private bool isCloseToItem = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<GetItem>() || other.GetComponent<Puzzle_Hat>())
        {
            isCloseToItem = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<GetItem>() || other.GetComponent<Puzzle_Hat>())
        {
            isCloseToItem = false;
        }
    }

    public void enterSceneChangeDoor(int nextScene)
    {
        guiManager.fadeOut(nextScene);
    }

    public Inventory getInventory()
    {
        return inventory;
    }

    public void onCutscene(bool active)
    {
        gameMode = active ? GameMode.CUT_SCENE : GameMode.MAIN;
        if (active)
        {
            cutsceneCam.transform.position = cam.transform.position;
            cam.gameObject.SetActive(false);
            cutsceneCam.gameObject.SetActive(true);
        }
        else
        {
            cam.gameObject.SetActive(true);
            StartCoroutine("goToCameraPos");
        }
    }

    private IEnumerator goToCameraPos()
    {
        Vector3 initialPosition = cutsceneCam.transform.position;
        Quaternion initialRotation = cutsceneCam.transform.rotation;

        Vector3 targetPosition = cam.transform.position;
        Quaternion targetRotation = cam.transform.rotation;


        float elapsedTime = 0f;

        while (elapsedTime < 2)
        {
            cutsceneCam.transform.position = Vector3.Lerp(initialPosition, targetPosition, elapsedTime / 2);
            cutsceneCam.transform.rotation = Quaternion.Lerp(initialRotation, targetRotation, elapsedTime / 2);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        cutsceneCam.transform.position = targetPosition;
        cutsceneCam.transform.rotation = targetRotation;
        cutsceneCam.gameObject.SetActive(false);
    }


    [SerializeField] private LayerMask wallMask;
    [SerializeField] private float sphereRadius = 1.5f;
    private List<Renderer> lastHitRenderers = new List<Renderer>();
    private void updateVisibility()
    {
        foreach (Renderer renderer in lastHitRenderers)
        {
            if (renderer != null)
            {
                foreach (Material mat in renderer.materials)
                {
                    mat.SetFloat("_CutoutSize", 0f);
                }
            }
        }
        lastHitRenderers.Clear();

        Vector2 cutoutPos = cam.WorldToViewportPoint(transform.position);
        cutoutPos.y /= (Screen.width / Screen.height);

        Vector3 camToChar = transform.position - cam.transform.position;
        float playerDistance = camToChar.magnitude;

        RaycastHit[] hitObjects = Physics.SphereCastAll(cam.transform.position, sphereRadius, camToChar.normalized, playerDistance, wallMask);

        for (int i = 0; i < hitObjects.Length; ++i)
        {
            Renderer renderer = hitObjects[i].transform.GetComponent<Renderer>();
            if (renderer != null)
            {

                if (hitObjects[i].point.z < transform.position.z 
                    && hitObjects[i].transform.localEulerAngles.y != 90 
                    && hitObjects[i].transform.localEulerAngles.y != -90)
                {
                    Material[] materials = renderer.materials;

                    for (int m = 0; m < materials.Length; ++m)
                    {
                        materials[m].SetVector("_CutoutPos", cutoutPos);
                        materials[m].SetFloat("_CutoutSize", 0.16f);
                        materials[m].SetFloat("_FalloffSize", 0.08f);
                    }

                    lastHitRenderers.Add(renderer);
                } else
                {
                    Material[] materials = renderer.materials;

                    for (int m = 0; m < materials.Length; ++m)
                    {
                        materials[m].SetFloat("_CutoutSize", 0f);
                    }

                    lastHitRenderers.Add(renderer);
                }
            }
        }
    }
    
    private void Die()
    {
        guiManager.setDeathScreen(true);
        gameMode = GameMode.CUT_SCENE;
        gameObject.SetActive(false);
    }
}
