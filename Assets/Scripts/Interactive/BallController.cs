using UnityEngine;
using System.Collections.Generic;
using System;

public class BallController : MonoBehaviour
{
    private const float soundTimeout = 0.2f;

    public List<AudioSource> hitSounds;
    public Renderer ground1;
    public Renderer ground2;

    private ActionsContoller actions;

    private bool isTouchedGround = false;
    private Rigidbody ballRigidbody;
    private float lastSoundAt = 0f;
    private readonly Vector3 defaultPosition = new Vector3(0, 5, 0);
    private const float highPitchAdditionalVelocity = 2f;
    private const float highPitchThreshold = 6f;

    private void Start()
    {
        ActionsContoller.OnRoundEnd += FreezeAndResetPosition;
        ActionsContoller.OnRoundStart += UnfreezeAndResetGround;

        actions = ActionsContoller.GetActions();
        ballRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (ballRigidbody.isKinematic)
        {
            return;
        }

        var position = gameObject.transform.position;

        if (
            Math.Abs(position.x) > 5.5f || Math.Abs(position.z) > 5.5f || position.y > 10 || position.y < -2
        )
        {
            RoundRestart();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        bool isFirstTeamScore = collision.gameObject.name == "Ground 1";
        bool isSecondTeamScore = collision.gameObject.name == "Ground 2";

        PlayHitSound();

        if (!isTouchedGround && (isFirstTeamScore || isSecondTeamScore))
        {
            isTouchedGround = true;
            UpdateGroundColor(isFirstTeamScore, isSecondTeamScore);            
            UpdateScore(isFirstTeamScore, isSecondTeamScore);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (
            ballRigidbody.velocity.y + highPitchAdditionalVelocity < highPitchThreshold
            && collision.transform.GetComponent<PlayerController>() != null
        )
        {
            ballRigidbody.velocity += new Vector3(0, highPitchAdditionalVelocity, 0);
        }
    }

    public void RoundRestart()
    {
        actions.RoundRestart();
    }

    void UpdateScore(bool isFirstTeamScore, bool isSecondTeamScore) {
        actions.UpdateScore(isFirstTeamScore, isSecondTeamScore);
    }

    void UpdateGroundColor(bool isFirstTeamScore, bool isSecondTeamScore) {

        if (isFirstTeamScore)
        {
            ground1.material.color = new Color32(255, 0, 0, 1);
        }
        else if (isSecondTeamScore)
        {
            ground2.material.color = new Color32(255, 0, 0, 1);
        }
    }

    public void FreezeAndResetPosition()
    {
        ballRigidbody.transform.position = defaultPosition;
        ballRigidbody.position = defaultPosition;
        ballRigidbody.velocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
        ballRigidbody.isKinematic = true;
    }

    public void UnfreezeAndResetGround()
    {
        ballRigidbody.isKinematic = false;
        isTouchedGround = false;
        ground1.material.color = new Color32(255, 160, 129, 1);
        ground2.material.color = new Color32(107, 255, 130, 1);
    }

    private void PlayHitSound()
    {
        if (Time.time - lastSoundAt > soundTimeout)
        {
            lastSoundAt = Time.time;
            hitSounds[UnityEngine.Random.Range(0, hitSounds.Count)].Play();
        }
    }
}
