using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.CullingGroup;

public class Player : MonoBehaviour
{
    public SPUM_Prefabs _prefabs;
    public Vector2 inputVec;
    public float speed;

    Rigidbody2D rigid;
    Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        if (_prefabs == null)
        {
            _prefabs = transform.GetChild(0).GetComponent<SPUM_Prefabs>();
        }
    }

    void Update()
    {
        inputVec.x = Input.GetAxisRaw("Horizontal");
        inputVec.y = Input.GetAxisRaw("Vertical");

        if (inputVec.x != 0 || inputVec.y != 0)
        {
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }
    }

    void FixedUpdate()
    {
        Vector2 nextVec = inputVec.normalized * speed * Time.fixedDeltaTime;
        rigid.MovePosition(rigid.position + nextVec);
    }

    void LateUpdate()
    {
        if (inputVec.x > 0)
        {
            _prefabs.transform.localScale = new Vector3(-1, 1, 1);
        }
        else if  (inputVec.x < 0)
        {
            _prefabs.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (GameManager.instance.isLive)
            return;

        GameManager.instance.health -= Time.deltaTime * 10;

        if (GameManager.instance.health < 0) { }
    }
}
