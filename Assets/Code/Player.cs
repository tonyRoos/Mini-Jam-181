using System.Collections;
using UnityEngine;

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
        anim.SetFloat("speed", 0);
    }

    private void Update()
    {
        switch(gameMode)
        {
            case GameMode.MAIN:
                calculateSpeed();
                move();
                updateGui();
                setMenu(true);
                break;
            case GameMode.PAUSED:
                setMenu(false);
                break;
            case GameMode.CUT_SCENE:
                setMenu(true);
                break;
        }
    }

    private void setMenu(bool state)
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            guiManager.setPauseMenu(state);
            Time.timeScale = state ? 0 : 1;
        }
    }

    private void move()
    {
        updatePlayerSpeed();
        if (Input.GetKey(KeyCode.W))
        {
            anim.SetBool("isUp", true);
            transform.Translate(0, 0, Time.deltaTime * totalSpeed, Space.World);

        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.localScale = new Vector3(-0.3f, 0.3f, 1);
            transform.Translate(Time.deltaTime * -totalSpeed, 0, 0, Space.World);
        }
        if (Input.GetKey(KeyCode.S))
        {
            anim.SetBool("isUp", false);
            transform.Translate(0, 0, Time.deltaTime * -totalSpeed, Space.World);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.localScale = new Vector3( 0.3f, 0.3f, 1);
            transform.Translate(Time.deltaTime * totalSpeed, 0, 0, Space.World);
        }
        anim.SetFloat("speed", speed);
    }

    private float speed;
    private Vector3 lastPosition;
    private void calculateSpeed()
    {
        speed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = transform.position;
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
        guiManager.updateStaminaBar(stamina/ _PLAYER_RUN_TIME);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<GetItem>())
        {
            cam.fieldOfView = 40;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKey(KeyCode.Space) && other.GetComponent<GetItem>())
        {
            GetItem getItem = (GetItem)other.GetComponent<GetItem>();
            inventory.items.Add(getItem.item);
            guiManager.addItem(getItem.item);
            Destroy(other.gameObject);
            cam.fieldOfView = 60;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        cam.fieldOfView = 60;
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
        if(active)
        {
            cutsceneCam.transform.position = cam.transform.position;
            cam.gameObject.SetActive(false);
            cutsceneCam.gameObject.SetActive(true);
        } else
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
}
