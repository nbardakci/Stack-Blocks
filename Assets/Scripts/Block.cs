﻿using System.Collections;
using UnityEngine;

public class Block : MonoBehaviour
{
    [Header(" Settings ")]

    [Tooltip("Block's movement speed to the down.  More, faster")]
    [SerializeField] private float _blockSpeedDownUp = 5f; // more the faster.

    [Header(" Elements ")]
    [SerializeField] private GameObject imageX;
    [SerializeField] private ParticleSystem destroyParticle;

    [HideInInspector] public Vector2Int position, targetPosition;

    [Header("Managers")]
    private Manager _manager;

    private void Awake()
    {
        _manager = FindAnyObjectByType<Manager>();
    }

    public void ImageX(bool value)
    {
        imageX.SetActive(value);
    }

    public void MoveBlokToDown(Vector3 _targetPosition)
    {
        // The block will move. Increase _movingBlockCount by 1.
        _manager._blocksController.movingBlockCount++;

        StartCoroutine(MoveBlokToDownCo(_targetPosition));
    }

    private IEnumerator MoveBlokToDownCo(Vector3 _targetPosition)
    {
        while (transform.position.y > _targetPosition.y + 0.01f) // Since we're using "lerp" we're making a slight change to the target.
        {
            // transform.position = new Vector2(transform.position.x, transform.position.y - _blockSpeed * Time.deltaTime);

            // drop slower to down
            this.transform.position = Vector2.Lerp(transform.position, _targetPosition, _blockSpeedDownUp * Time.deltaTime);

            yield return new WaitForEndOfFrame();
        }

        transform.position = _targetPosition; // Make correction for the position

        // The block completed the movement. Decrease _movingBlockCount by 1
        _manager._blocksController.movingBlockCount--;
    }

    public void DestroyBlock()
    {
        // Activate destroy Effect 
        var main = destroyParticle.main;
        main.startColor = GetComponent<SpriteRenderer>().color; // Set particle start color to block color

        destroyParticle.transform.SetParent(null); // Set the parent to null, so the particle does not destroy during the block's destruction. It will be destroyed by the Stop Action in the particle settings
        destroyParticle.Play(); 

        _manager._audio_Manager.PlayAudio("BlockDestroy");

        Destroy(gameObject); 
    }

}
