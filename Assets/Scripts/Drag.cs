using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class Drag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public Transform playerTransform;
    public GameManager gm;
    private Camera _camera;
    private Vector3 _startPosition;
    private BoxCollider _collider;
    private Vector3 _offset;
    private float _offsetFactor;
    private float _currentX = -1f;
    private PlayerMovement _playerMovement;
    public UIManager uiManager;
    
    private void Start()
    {
        _camera = Camera.main;
        _offsetFactor = 2.5f;
        _playerMovement = playerTransform.GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        if((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && !gm.isStarted)
            gm.StartPlay();
        
        if (_playerMovement.isOnWall)
        {
            return;
        }
        
        if(Input.GetKey(KeyCode.A))
            playerTransform.localPosition -= new Vector3(gm.sensitivityCoefficient * 5f * Time.deltaTime, 0f, 0f);
        if(Input.GetKey(KeyCode.D))
            playerTransform.localPosition += new Vector3(gm.sensitivityCoefficient * 5f * Time.deltaTime, 0f, 0f);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!gm.isStarted)
        {
            gm.StartPlay();
        }
                     
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10f;
                     
        _offset = _camera.ScreenToWorldPoint(mousePos);
        _offset.x = playerTransform.localPosition.x / _offsetFactor - _offset.x;
    }
                 
    public void OnDrag(PointerEventData eventData)
    {
        if (_playerMovement.isOnWall)
        {
            float forceX = 0f;
            if (eventData.delta.x > 5f && _playerMovement.transform.rotation.eulerAngles.z == 270f)
            {
                forceX = 7f;
                _playerMovement.OnGravity();
                _playerMovement.isOnWall = false;
                _playerMovement.Jump(new Vector3(gm.jumpCoefficient * forceX,gm.jumpCoefficient * 5f,0f));
                _playerMovement.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            } if (eventData.delta.x < -5f && _playerMovement.transform.rotation.eulerAngles.z == 90f)
            {
                forceX = -7f;
                _playerMovement.OnGravity();
                _playerMovement.isOnWall = false;
                _playerMovement.Jump(new Vector3(gm.jumpCoefficient * forceX,gm.jumpCoefficient * 5f,0f));
                _playerMovement.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            return;
        }
        
        if (eventData.delta.x > 3f)
        {
            if(_playerMovement.playerAnimator.GetBool("Jump"))
                Invoke(nameof(NullifyVelocityX), 1f);
            playerTransform.localPosition += new Vector3(gm.sensitivityCoefficient * 15f * Time.deltaTime, 0f, 0f);
        } if (eventData.delta.x < -3f)
        {
            if (_playerMovement.playerAnimator.GetBool("Jump"))
                Invoke(nameof(NullifyVelocityX), 1f);
            playerTransform.localPosition += new Vector3(gm.sensitivityCoefficient * -15f * Time.deltaTime, 0f, 0f);
        }
        //playerTransform.localPosition += new Vector3((eventData.delta.x > 0f ? 10f : -10f) * Time.deltaTime, 0f, 0f);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(_playerMovement.isOnWall || _playerMovement.isScratch || _playerMovement.isDash)
            return;
        
        //_playerMovement.Dash();
    }

    private void NullifyVelocityX() => _playerMovement.NullifyVelocityX();
}
