using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Animator playerAnimator;
    public float forwardSpeed;
    public float maxSpeed;
    public GameManager gm;
    public GameObject smokeFallingParticlePref;
    public GameObject smokeJumpingParticlePref;
    public GameObject slicedObj;
    public GameObject skinMeshRenderer;
    public GameObject bloodEffect;
    public ParticleSystem runParticles;
    public CutObject[] swords;
    public Transform katanaTransform1;
    public Transform katanaTransform2;
    public Transform fakeKatanaTransform1;
    public Transform fakeKatanaTransform2;
    public Transform trailTransform1;
    public Transform trailTransform2;
    public bool isOnWall;
    public bool isMovingOnWall;
    public bool isJumped;
    public bool isJumpedPlatform;
    private Transform _cameraTransform;
    private Rigidbody _rb;
    private BoxCollider _bc;
    private RoadTop _currentRoadTop;
    public bool isScratch;
    public bool checkGround;
    private float _rendererZ;
    private float _rotateY;
    private float _rotateZ;
    private float _wallX;
    public LayerMask groundLayer;
    public bool isDash;
    private Transform lastTransform;
    private Vector3 _nextWallPos;
    private Vector3 _nextPos;
    public bool canWall = true;
    public bool canDie = true;
    public bool canScretch = true;
    public bool canTopRun = true;
    public bool canLanding;
    public SkinnedMeshRenderer playerSkinMesh;
    public MeshRenderer playerCuttedMesh;
    
    private void Start()
    {
        OffFakeKatana();
        runParticles.Stop();
        checkGround = true;
        _rb = GetComponent<Rigidbody>();
        _bc = GetComponent<BoxCollider>();
        _cameraTransform = Camera.main.transform;
        maxSpeed *= gm.maxSpeedCoefficient;
        forwardSpeed *= gm.speedCoefficient;
        for (int i = 0; i < swords.Length; i++)
        {
            swords[i].gm = gm;
        }
    }

    private void Update()
    {
        if (!gm.isStarted)
            return;
        
        if (slicedObj.activeSelf)
            enabled = false;
        
        // _cameraTransform.rotation = Quaternion.RotateTowards(_cameraTransform.rotation, 
        //     Quaternion.Euler(new Vector3(_cameraTransform.rotation.eulerAngles.x, _rotateY, 0f)), 
        //     200f * Time.deltaTime); 

        if (isMovingOnWall)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                Quaternion.Euler(0f, _rotateY, _rotateZ), 300f * Time.deltaTime);
            Vector3 needTransform = new Vector3(_wallX, transform.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, needTransform, 10f*Time.deltaTime);
            if (transform.rotation == Quaternion.Euler(0f, _rotateY, _rotateZ))
            {
                isMovingOnWall = false;
            } 
            if (!canWall)
            {
                transform.rotation = Quaternion.identity;
                isMovingOnWall = false;
            }
        }

        if(isOnWall && !isMovingOnWall)
            transform.position = Vector3.Lerp(transform.position, _nextWallPos, Time.deltaTime * 0.4f);

        if (isJumped && !isScratch)
            transform.position = Vector3.Lerp(transform.position, _nextPos, Time.deltaTime * 2.2f);

        //if (checkGround)
        //{
        //    if (Physics.Raycast(transform.position, Vector3.down, 0.5f, groundLayer))
        //    {
        //        checkGround = false;
        //        playerAnimator.SetBool("Jump", false);
        //        playerAnimator.SetBool("IsGround", true);
        //        Instantiate(smokeFallingParticlePref, transform.position, Quaternion.identity);
        //    }
        //}
    }

    void FixedUpdate()
    {
        //if (_rb.velocity.magnitude >= 5f && runParticles.isStopped)
        //    runParticles.Play();
        //if (_rb.velocity.magnitude < 5f && runParticles.isPlaying)
        //    runParticles.Stop();

        if (_rb.velocity.magnitude < maxSpeed && gm.isStarted && !isScratch && !isOnWall)
        {
            _rb.AddForce(new Vector3(0f, 0f, forwardSpeed * Time.fixedDeltaTime), ForceMode.Impulse);
        }

        if (isScratch && gm.isStarted)
        {
            _rb.AddForce(new Vector3(0f, forwardSpeed * Time.fixedDeltaTime, 0f), ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //SCRATCH
        if (other.gameObject.layer == 17)
        {
            if (isScratch || !canScretch || isOnWall)
                return;

            canDie = false;
            isScratch = true;
            isOnWall = false;
            isJumped = false;
            other.gameObject.GetComponent<BuildingTop>().EnableColliders(0f);
            //RemoveDash();
            _currentRoadTop?.SetIdleAllEnemies();
            //transform.position = new Vector3(transform.position.x, transform.position.y, other.transform.GetChild(0).position.z);
            //transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
            checkGround = false;
            _rb.velocity = Vector3.zero;
            playerAnimator.SetBool("Jump", false);
            playerAnimator.SetBool("Scratch", true);
            OffFakeKatana();
        }

        //JUMP FROM WALL OR SCRATCH
        if (other.gameObject.layer == 4)
        {
            if (!isScratch && !isOnWall)
                return;

            isOnWall = false;
            isScratch = false;
            canDie = true;
            _rb.useGravity = true;
            if(other.GetComponent<JumpPlatform>().posTransform)
                Jump(other.GetComponent<JumpPlatform>().posTransform);
            else
                Jump(other.GetComponent<JumpPlatform>().forces);
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            //playerAnimator.SetBool("IsSlideWall", false);
            //other.gameObject.SetActive(false);
            //_cameraTransform.rotation = Quaternion.Euler(_cameraTransform.rotation.eulerAngles.x, 0f, 0f);
            //playerAnimator.SetBool("Jump", true);
            //Invoke(nameof(CheckGround), 0.2f);
        }

        //JUMP PLATFORM
        if (other.gameObject.layer == 8)
        {
            if (isScratch || checkGround || isJumpedPlatform)
                return;

            isOnWall = false;
            isScratch = false;
            isJumpedPlatform = true;
            _currentRoadTop?.SetIdleAllEnemies();
            if (other.GetComponent<JumpPlatform>().posTransform)
                Jump(other.GetComponent<JumpPlatform>().posTransform);
            else
                Jump(other.GetComponent<JumpPlatform>().forces);
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            FalseJumpPlatform(0.2f);
        }
        
        //WALL RUN
        if (other.gameObject.layer == 23)
        {
            if (!canWall)
                return;

            if(IsInvoking(nameof(OnGravity)))
                CancelInvoke(nameof(OnGravity));
            _rb.useGravity = false;
        }
        
        //WIN
        if (other.gameObject.layer == 18)
        {
            gm.Finish(this);
            other.GetComponent<BoxCollider>().enabled = false;
        }

        //LOSE
        if (other.gameObject.layer == 19)
        {
            if (!canDie)
                return;

            OffFakeKatana();
            transform.position = lastTransform.position;
            transform.rotation = Quaternion.Euler(Vector3.zero);
            isOnWall = false;
            canWall = false;
            isJumped = false;
            if(playerAnimator.GetBool("Jump"))
                TrueCanWall();
            else
                TrueCanWallDelay(0.2f);
            isMovingOnWall = false;
            isScratch = false;
            //_rotateY = 0f;
            //_rotateZ = 0f;
        }

        //GLASS
        if (other.gameObject.layer == 24)
        {
            gm.PlaySoundAfterCreating(gm.glassSound);
            other.GetComponent<Glass>().destroyGlass.SetActive(true);
            other.GetComponent<Glass>().glass.SetActive(false);
            Destroy(other.gameObject, 4f);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        //RIGHT RUN
        if (other.gameObject.layer == 16)
        {
            if (isScratch || isOnWall || !canWall || !checkGround)
                return;
            
            _rotateY = -45f;//-45f
            _rotateZ = 90f;
            _currentRoadTop?.SetIdleAllEnemies();
            SetToWall(other, 90f, -45f);
        }

        //LEFT RUN
        if (other.gameObject.layer == 15)
        {
            if (isScratch || isOnWall || !canWall || !checkGround)
                return;

            _rotateY = 45f;//45f
            _rotateZ = -90f;
            _currentRoadTop?.SetIdleAllEnemies();
            SetToWall(other, -90f, 45f);
        }

        //ROAD TOP
        if (other.gameObject.layer == 22 && checkGround && gm.isStarted)
        {
            if (isOnWall || !canWall)
                return;

            if (!playerAnimator.GetBool("Run") && gm.isStarted)
                playerAnimator.SetBool("Run", true);
            other.gameObject.GetComponent<BuildingTop>().EnableColliders(0f);
            _currentRoadTop = other.gameObject.GetComponent<RoadTop>();
            lastTransform = _currentRoadTop.lastTransform;
            if (_currentRoadTop)
                _currentRoadTop.SetRunAllEnemies();

            checkGround = false;
            isJumped = false;
            if (!gm.isStarted)
                return;
            
            OffTrail();
            playerAnimator.SetBool("Jump", false);
            playerAnimator.SetBool("IsGround", true);
            if (!IsInvoking(nameof(OnFakeKatana)))
                Invoke(nameof(OnFakeKatana), 1f);
            if (!IsInvoking(nameof(OnTrail)))
                Invoke(nameof(OnTrail), 0.2f);

            if (!canLanding)
            {
                canLanding = true;
                return;
            }
            gm.PlaySoundAfterCreating(gm.landingSound);
            Instantiate(smokeFallingParticlePref, transform.position, Quaternion.identity);
        }

        //SCRATCH
        if (other.gameObject.layer == 17 && canScretch && !isOnWall)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, other.transform.GetChild(0).position.z);
            transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
        }
    }

    private void SetToWall(Collider other, float rotateZ, float rotateY)
    {
        isMovingOnWall = true;
        isOnWall = true;
        isJumped = false;
        //isScratch = false;
        //RemoveDash();
        _rb.useGravity = false;
        other.GetComponent<BuildingTop>().EnableColliders(0f);
        _wallX = other.transform.GetChild(0).position.x;
        _nextWallPos = new Vector3(other.transform.GetChild(0).position.x, transform.position.y, other.transform.GetChild(0).position.z);
        //transform.position = new Vector3(other.transform.GetChild(0).position.x, transform.position.y, transform.position.z);
        //transform.rotation = Quaternion.Euler(0f, rotateY, rotateZ);
        playerAnimator.SetBool("Jump", false);
        playerAnimator.SetBool("OnWall", true);
        _rb.velocity = Vector3.zero;
        _rb.AddForce(new Vector3(0f, 0f, 2f), ForceMode.Impulse);
        //_nextWallPos = other.GetComponent<BuildingTop>()
        Invoke(nameof(OnGravity), 1.5f);
        OffFakeKatana();
    }

    private void OnTriggerExit(Collider other)
    {
        //UP SCRATCH EXIT
        if (other.gameObject.layer == 11)
        {
            if (playerAnimator.GetBool("IsGround"))
                return;

            canDie = true;
            isScratch = false;
            playerAnimator.SetBool("Scratch", false);
            _rb.velocity = Vector3.zero;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            Jump(other.GetComponent<JumpPlatform>().forces);
            //playerAnimator.SetBool("Jump", true);
            //Invoke(nameof(CheckGround), 0.2f);
        }

        //WALL RUN
        if (other.gameObject.layer == 23 && _rb.useGravity)
        {
            if (IsInvoking(nameof(OnGravity)))
                CancelInvoke(nameof(OnGravity));
            _rb.useGravity = false;
            Invoke(nameof(OnGravity), 0.5f);
        }
    }

    public GameObject Cut()
    {
        if(PlayerPrefs.GetInt("IsVibration") != 0)
            Handheld.Vibrate();
        runParticles.Stop();
        Destroy(Instantiate(bloodEffect, transform.position + new Vector3(0f, 0.3f, 0.7f), Quaternion.identity), 1f);
        skinMeshRenderer.SetActive(false);
        slicedObj.SetActive(true);
        _currentRoadTop.SetIdleAllEnemies();
        Invoke(nameof(Lose),1f);
        return slicedObj;
    }

    public void Jump(Vector3 forces)
    {
        Jump();
        _rb.AddForce(gm.jumpCoefficient * forces, ForceMode.Impulse);
    }

    public void Jump(Transform posTransform)
    {
        Jump();
        _nextPos = posTransform.position;
        isJumped = true;
    }

    private void Jump()
    {
        //_rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
        gm.PlaySoundAfterCreating(gm.jumpSound);
        _rb.velocity = Vector3.zero;
        canTopRun = false;
        canScretch = false;
        canWall = false;
        if (IsInvoking(nameof(NullifyVelocityX)))
            CancelInvoke(nameof(NullifyVelocityX));
        if (IsInvoking(nameof(OnFakeKatana)))
            CancelInvoke(nameof(OnFakeKatana));
        Invoke(nameof(CheckGround), 0.2f);
        playerAnimator.SetBool("Jump", true);
        playerAnimator.SetBool("IsGround", false);
        Instantiate(smokeJumpingParticlePref, transform.position, Quaternion.identity);
        TrueTopRun(0.3f);
        TrueCanWallDelay(0.5f);
        TrueScretch(0.3f);
        OffFakeKatana();
    }

    public void ChangeColor()
    {
        playerCuttedMesh.material.color = gm.playerColorsBySkin[gm.currentPlayerSkin];
        playerSkinMesh.material.color = gm.playerColorsBySkin[gm.currentPlayerSkin];
    }

    public void SetSwords()
    {
        katanaTransform1 = gm.swordSkins[gm.currentSwordSkin].GetComponent<Skin>().skins[0].GetComponent<Skin>().skins[0].transform;
        katanaTransform2 = gm.swordSkins[gm.currentSwordSkin].GetComponent<Skin>().skins[1].GetComponent<Skin>().skins[0].transform;
        fakeKatanaTransform1 = gm.swordSkins[gm.currentSwordSkin].GetComponent<Skin>().skins[2].GetComponent<Skin>().skins[0].transform;
        fakeKatanaTransform2 = gm.swordSkins[gm.currentSwordSkin].GetComponent<Skin>().skins[3].GetComponent<Skin>().skins[0].transform;
        trailTransform1 = gm.swordSkins[gm.currentSwordSkin].GetComponent<Skin>().skins[0].GetComponent<Skin>().skins[1].transform;
        trailTransform2 = gm.swordSkins[gm.currentSwordSkin].GetComponent<Skin>().skins[1].GetComponent<Skin>().skins[1].transform;
        OffFakeKatana();
    }

    private void CheckGround() => checkGround = true;
    
    public void OnGravity() => _rb.useGravity = true;
    
    public void NullifyVelocityX()
    {
        if(_rb.velocity.x != 0f && !isOnWall)
            _rb.velocity = new Vector3(0f, _rb.velocity.y, _rb.velocity.z);
    }

    public void Dash()
    {
        if(IsInvoking(nameof(OnFakeKatana)))
            CancelInvoke(nameof(OnFakeKatana));
        if(playerAnimator.GetBool("OnWall"))
            playerAnimator.SetBool("OnWall", false);
        OffFakeKatana();
        isDash = true;
        Instantiate(smokeJumpingParticlePref, transform.position, Quaternion.identity);
        _rb.AddForce(new Vector3(0f, 0f,gm.dashCoefficient*5f), ForceMode.Impulse);
        playerAnimator.SetTrigger("Dash");
        Invoke(nameof(RemoveDash), 0.8f);
    }

    public void RemoveDash()
    {
        isDash = false;
        if (!playerAnimator.GetBool("Jump"))
        {
            if(IsInvoking(nameof(OnFakeKatana)))
                CancelInvoke(nameof(OnFakeKatana));
            Invoke(nameof(OnFakeKatana), 0.5f);
        }
    }

    private void Lose() => gm.Lose();

    public void Freeze()
    {
        _rb.useGravity = false;
        _rb.isKinematic = true;
        _rb.freezeRotation = true;
    }

    public void TrueCanWallDelay(float time) => Invoke(nameof(TrueCanWall), time);
    
    public void TrueCanWall() => canWall = true;
    
    public void TrueTopRun(float time) => Invoke(nameof(AllowTopRun), time);

    private void AllowTopRun() => canTopRun = true;

    public void TrueScretch(float time) => Invoke(nameof(AllowScretch), time);
    
    private void AllowScretch() => canScretch = true;

    public void FalseJump(float time) => Invoke(nameof(DontAllowJump), time);

    private void DontAllowJump() => isJumped = false;

    public void FalseJumpPlatform(float time) => Invoke(nameof(DontAllowJumpPlatform), time);

    private void DontAllowJumpPlatform() => isJumpedPlatform = false;

    public void OnOffFakeKatana(bool isFakeTrailOn)
    {
        if(!katanaTransform1.gameObject.activeSelf && isFakeTrailOn)
            return;

        katanaTransform1.gameObject.SetActive(!isFakeTrailOn);
        katanaTransform2.gameObject.SetActive(!isFakeTrailOn);
        fakeKatanaTransform1.gameObject.SetActive(isFakeTrailOn);
        fakeKatanaTransform2.gameObject.SetActive(isFakeTrailOn);
    }

    public void OnOffTrail(bool isTrailOn)
    {
        trailTransform1.gameObject.SetActive(!isTrailOn);
        trailTransform2.gameObject.SetActive(!isTrailOn);
    }

    public void OnFakeKatana() => OnOffFakeKatana(true);

    public void OffFakeKatana() => OnOffFakeKatana(false);

    public void OnTrail() => OnOffTrail(true);

    public void OffTrail() => OnOffTrail(true);
}