using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//use a namespace
public class ResetPlayerAfterDeath : MonoBehaviour
{
    private Transform player;
    private LerpBetweenTwoPoint lerp;
    private Transform mcamera;
    [SerializeField]
    private CheckPointSO checkPoint;
    [SerializeField]
    private BossScript boss;
    [SerializeField]
    private List<float> waitForSecond = new List<float>();
    [SerializeField]
    private float time = 0;
    [SerializeField]
    private float resettimer = 0;

    //You don't need to, but switching to an Action is easier to read
    public delegate void Levelreset();
    public static event Levelreset OnLevelReset;

    // Start is called before the first frame update
    void Start()
    {
        //Do anything you can to get rid of these Find statements
        //A Find statment is not reliable and goes through every single object in the scene before returning the first hit
        if (FindObjectOfType<BossScript>() != null)  
        {
          boss = GameObject.FindGameObjectWithTag("Boss").GetComponentInChildren<BossScript>();
        }

        if (GameObject.FindGameObjectWithTag("Player") != null) 
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }

        if (FindObjectOfType<LerpBetweenTwoPoint>() != null) {

            lerp = FindObjectOfType<LerpBetweenTwoPoint>();
        }

        if (GameObject.FindGameObjectWithTag("MainCamera") != null)
        {
            mcamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }

        OnLevelReset += PassCheckPointSoDataBack;
        checkPoint = Resources.Load<CheckPointSO>("Scritable Objects/CheckPoint_SO");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Don't do this in FixedUpdate
        //FixedUpdate is for physics calls and you don't need this here
        time -=Time.deltaTime;

        //This is pretty cool asignment/conditional
        if (PlayerState.state == PlayerState.PlayerStates.Dead && time < 0) {
            time = resettimer;
            StartCoroutine(ResetLevel());
        }
    }

    IEnumerator ResetLevel() {
        OnLevelReset();
        yield return new WaitForSeconds(waitForSecond[0]);
        FadingManger.StartFading();

        yield return new WaitForSeconds(waitForSecond[1]);
        if (GameController.instance.bossLevel == true)
        {
            DestroyInBoss();
        }
        else
        {
            DestroyInLevel();
        }

        yield return new WaitForSeconds(waitForSecond[2]);

        if (GameController.instance.bossLevel == true)
        {
            boss.AnimationChange();
        }
        PlayerState.state = PlayerState.PlayerStates.Alive;
        FadingManger.StopFading();
    }

    private void DestroyInLevel()
    {

        DestroyGamesObjects.OnObjectDestroy();
    }

    private void DestroyInBoss()
    {

        boss.StopAllCoroutines();
        boss.currentstate = BossScript.bossState.Idle;
        boss.hp = boss.enemyTemple.hp;
    }

    private void PassCheckPointSoDataBack() 
    {
        player.position = checkPoint.playerCheckpointPosition;
        mcamera.position = checkPoint.camaraCheckpointPosition;
        //Try to find a different way to get the references
        if (FindObjectOfType<LerpBetweenTwoPoint>() != null) {
            lerp.distCovered = checkPoint.distCovered;
            lerp.journeyLength = checkPoint.journeyLength;
            lerp.nextPosition = checkPoint.nextPosition;
        }
    }
}
