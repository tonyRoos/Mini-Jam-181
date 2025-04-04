using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Comportamento do player, incluindo alguns detalhes como zoom de camera */

public enum GameMode
{
    MAIN, PAUSED, CUT_SCENE
}

//Require component evita que o desenvolvedor remova esses componentes importantes do objeto, também adiciona eles caso não existam no objeto que receber esse script
[RequireComponent(typeof(Rigidbody), typeof(Animator), typeof(AudioSource))]
public class Player : MonoBehaviour
{
    public GuiManager guiManager;

    private const float _PLAYER_WALK_SPEED = 2.5f;
    private const float _PLAYER_RUN_SPEED = 4f;
    private const float _PLAYER_RUN_TIME = 5F;
    private float stamina = _PLAYER_RUN_TIME;

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

    // o botão f1 causa a morte do personagem, para fins de testes de telas e caso o personagem fique preso em algum canto ou o puzzle fique travado
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
                if(isTriggered) { handleCollisorInteraction(); }
                break;
            case GameMode.PAUSED:
                break;
            case GameMode.CUT_SCENE:
                //setMenu(true);
                rb.linearVelocity = Vector3.zero;
                break;
        }
    }

    // Usado pela animação de caminhar do personagem
    public void playStepSound()
    {
        audioSrc.Stop();
        audioSrc.pitch = Random.Range(0.75f, 1.25f);
        audioSrc.PlayOneShot(stepSound);
    }

    // Não está em uso até que seja implementado o menu de pause
    private void setMenu(bool state)
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            guiManager.setPauseMenu(state);
            Time.timeScale = state ? 0 : 1;

        }
    }

    // Zoom in caso o personagem esteja em um trigger
    private void updateFieldOfView()
    {
        if (isTriggered)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 40, Time.deltaTime * 2f);
        }
        else
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, 60, Time.deltaTime * 2f);
        }
    }

    // Movimenta o personagem a partir dos inputs, configura escala do sprite para o personagem virar para esquerda ou direita, e alimenta o animator do personagem
    private void move()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), rb.linearVelocity.y, Input.GetAxis("Vertical"));
        rb.linearVelocity = new Vector3(updatePlayerSpeed() * movement.x, rb.linearVelocity.y, updatePlayerSpeed() * movement.z);


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


    // redefine a velocidade atual e consome ou regenera stamina
    private float updatePlayerSpeed()
    {
        if (Input.GetKey(KeyCode.LeftShift) && stamina > 0 &&
            (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
        {
            stamina -= Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0, _PLAYER_RUN_TIME);
            return _PLAYER_RUN_SPEED;
        }
        else
        {
            stamina += Time.deltaTime / 2;
            stamina = Mathf.Clamp(stamina, 0, _PLAYER_RUN_TIME);
            return _PLAYER_WALK_SPEED;
        }
    }

    // Atualiza a barra de stamina na UI - atualmente desativada
    private void updateGui()
    {
        guiManager.updateStaminaBar(stamina / _PLAYER_RUN_TIME);
    }


    // Transfere as interações da colisão para dentro do update para garantir uma consistencia melhor no input por exemplo
    private void handleCollisorInteraction() {
        if (Input.GetKey(KeyCode.Space) && triggerInfo.GetComponent<GetItem>())
        {
            GetItem getItem = (GetItem)triggerInfo.GetComponent<GetItem>();
            inventory.items.Add(getItem.item);
            guiManager.addItem(getItem.item);
            audioSrc.PlayOneShot(getItem.getSound);
            Destroy(triggerInfo.gameObject);
            isTriggered = false;
        }

        if (Input.GetKeyDown(KeyCode.Space) && triggerInfo.GetComponent<Puzzle_Hat>())
        {
            triggerInfo.GetComponent<Puzzle_Hat>().transport(transform);
        }
    }

    private bool isTriggered = false;
    private Collider triggerInfo;
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<GetItem>() || other.GetComponent<Puzzle_Hat>())
        {
            isTriggered = true;
            triggerInfo = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<GetItem>() || other.GetComponent<Puzzle_Hat>())
        {
            isTriggered = false;
        }
    }

    // Essa função de fade out dispara o evento, por exemplo, nesse caso vai ter o fade com a máscara de chave, e posteriormente trocar de cena é chamado pelo fim da coroutine que fica responsável pelo fade
    public void enterSceneChangeDoor(int nextScene)
    {
        guiManager.fadeOut(nextScene, GuiManager.FadeType.KEY);
    }

    // Permite outras classes acessarem o invetário do personagem. (ex.: a porta confere se a chave está presente)
    public Inventory getInventory()
    {
        return inventory;
    }

    // Quando entra em cutscene, posiciona a camera da cutscene na posição da camera main, e vice-versa.
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

    // Essa é uma coroutine que serve para camera de cutscene se deslocar em direção a camera main, na tentativa de suavizar a transição
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

    // Configura a visibilidade através das paredes, as paredes que ficarão "invisiveis" devem receber o shader graph ShaderGraph/WallCutout
    // Casta uma RaySphere, é como um rayCast normal, mas considera um diametro em torno do raio, isso evita a necessidade de múltiplos raios e funciona melhor em algumas ocasiões.
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
        guiManager.fadeOut(0, GuiManager.FadeType.DEATH);
        gameMode = GameMode.CUT_SCENE;
        gameObject.SetActive(false);
    }
}
