using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum lizardState
{
    Wander,
    Run,
    Dead
};

public class LostSoulScript : MonoBehaviour
{
    Rigidbody2D rb;
    //Animator animator;
    //AudioSource audioSource;

    public Vector3 defaultStats; // hp, attack, speed

    #region MOVE RELATED
    public float moveSpeed;
    Vector2 position;
    bool firstMove = false;
    #endregion

    #region ATTACK RELATED
    public float firerate;
    float timer;
    public GameObject acidPrefab;
    public Transform firePoint;
    public float shotDamage;
    #endregion

    #region AI RELATED
    GameObject player;
    public lizardState currState = lizardState.Wander;
    public float range = 20;
    bool chooseDir = false;
    Vector3 randomDir;
    #endregion

    #region idk RELATED
    public float health;
    //GameObject door;
    //LevelManagement LM;
    #endregion

    #region DROPS RELATED
    //public GameObject drop1Prefab;
    //public GameObject drop2Prefab;
    #endregion

    #region  AUDIO
    //public AudioClip acidClip;
    #endregion

    void Start()
    {
        player = GameObject.Find("_Player");
        //door = GameObject.Find("Door");
        rb = GetComponent<Rigidbody2D>();
        timer = firerate;
        //LM = door.GetComponent<LevelManagement>();
        //animator = GetComponent<Animator>();
        //audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0 && isPlayerInRange(range) == true)
        {
            shoot();
            timer = firerate;
        }

        Vector2 Look = player.GetComponent<Rigidbody2D>().position - (Vector2)firePoint.position;
        Look.Normalize();

        //animator.SetFloat("Look X", Look.x);
        //animator.SetFloat("Look Y", Look.y);

    }

    void FixedUpdate()
    {

        switch (currState)
        {
            case (lizardState.Wander):
                Wander();
                break;
            case (lizardState.Run):
                Run();
                break;
            case (lizardState.Dead):
                Die();
                break;
        }

        if (isPlayerInRange(range) && currState != lizardState.Dead)
        {
            currState = lizardState.Run;
        }
        else if (!isPlayerInRange(range) && currState != lizardState.Dead)
        {
            currState = lizardState.Wander;
        }
    }

    private IEnumerator chooseDirection()
    {
        chooseDir = true;
        if (!firstMove)
        {
            yield return new WaitForSeconds(0.1f);
            //animator.SetBool("Moving", true);
            firstMove = true;
        }
        else
        {
            yield return new WaitForSeconds(Random.Range(1f, 2f));
        }
        randomDir = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        randomDir.Normalize();
        chooseDir = false;
    }

    void Wander()
    {

        if (!chooseDir)
        {
            StartCoroutine(chooseDirection());
        }
        Vector2 position = rb.position;

        position.x = position.x + moveSpeed * randomDir.x * Time.deltaTime;
        position.y = position.y + moveSpeed * randomDir.y * Time.deltaTime;

        rb.MovePosition(position);
    }

    void Run()
    {
        // animator.SetBool("Moving", true);
        StopCoroutine(chooseDirection());
        Vector2 position = rb.position;
        Vector2 moveDir = rb.position - player.GetComponent<Rigidbody2D>().position;
        moveDir.Normalize();

        position.x = position.x + moveSpeed * moveDir.x * Time.deltaTime;
        position.y = position.y + moveSpeed * moveDir.y * Time.deltaTime;

        rb.MovePosition(position);
    }

    void shoot()
    {
        Vector2 aimDirection = player.GetComponent<Rigidbody2D>().position - (Vector2)firePoint.position;

        aimDirection.Normalize();

        GameObject acidObject = Instantiate(acidPrefab, firePoint.position, Quaternion.identity);
        SoulBullet sBullet = acidObject.GetComponent<SoulBullet>();
        sBullet.shoot(aimDirection, 10, shotDamage);
        //AcidShotController asController = acidObject.GetComponent<AcidShotController>();
        //asController.shoot(aimDirection, 10);
        //audioSource.PlayOneShot(acidClip);
    }

    private bool isPlayerInRange(float range)
    {

        return Vector3.Distance(transform.position, player.transform.position) <= range;
    }

    public void takeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            currState = lizardState.Dead;
        }
    }

    void Die()
    {
        Debug.Log("Se murio definitivamente");
        //int opcDrop;
        //for (int i = 1; i <= Random.Range(1, 3); i++)
        //{
        //    opcDrop = Random.Range(1, 3);

        //    if (opcDrop == 1)
        //    {
        //        Vector2 objectPos = transform.position;
        //        objectPos.x += Random.Range(-1f, 1f);
        //        objectPos.y += Random.Range(-1f, 1f);
        //        //GameObject dropObject = Instantiate(drop1Prefab, objectPos, Quaternion.identity);
        //    }
        //    else if (opcDrop == 2)
        //    {
        //        Vector2 objectPos = transform.position;
        //        objectPos.x += Random.Range(-1f, 1f);
        //        objectPos.y += Random.Range(-1f, 1f);
        //        //GameObject dropObject = Instantiate(drop2Prefab, objectPos, Quaternion.identity);
        //    }
        //}
        //LM.enemyCount -= 1;
        Destroy(gameObject);
    }
}