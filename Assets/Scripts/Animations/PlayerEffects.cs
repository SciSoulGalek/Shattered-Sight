using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerEffects : MonoBehaviour
{
    [Header("Animation state names")]
    public string spawnStateName = "Player_Spawn";
    public string deathStateName = "Player_Death";
    public string finishStateName = "Player_Finish";

    [Header("Durations")]
    public float spawnAnimDuration = 0.3f;
    public float deathAnimDuration = 0.3f;
    public float finishAnimDuration = 0.3f;

    private Animator anim;
    private PlayerController2D controller;
    private PlayerRespawn respawn;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<PlayerController2D>();
        respawn = GetComponent<PlayerRespawn>();
    }

    private void Start()
    {
        PlaySpawn();
    }

    public void PlaySpawn()
    {
        if (anim == null) return;
        anim.Play(spawnStateName, 0, 0f);
    }

    public void PlayFinish()
    {
        if (anim == null) return;
        anim.Play(finishStateName, 0, 0f);
    }

    public void PlayDeath()
    {
        if (anim == null || string.IsNullOrEmpty(deathStateName)) return;
        anim.Play(deathStateName, 0, 0f);
    }
}
