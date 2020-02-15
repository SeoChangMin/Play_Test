using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    
    
    // 속도 조정 변수
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    [SerializeField]
    private float crouchSpeed;
    [SerializeField]
    private float walkbackSpeed;

    private float applySpeed;

    [SerializeField]
    private float jumpForce;


    // 움직임 상태 변수
    private bool isRun = false;
    private bool isCrouch = false;
    private bool isGround = true;
    private bool isSlow = false;
    



    // 앉았을 때 얼마나 앉을지 결정하는 변수.
    [SerializeField]
    private float crouchPosY;
    private float originPosY;
    private float applyCrouchPosY;



    //달릴 때 얼마나 시야가 벌어지는지 결정하는 변수.
    [SerializeField]
    private float applyDash;





    // 땅 착지 여부
    private CapsuleCollider capsuleCollider;


    // 마우스 민감도
    [SerializeField]
    private float lookSensitivity;


    // 카메라 한계
    [SerializeField]
    private float cameraRotationLimit;
    private float currentCameraRotationX = 0;


    //필요한 컴포넌트
    [SerializeField]
    private Camera theCamera;

    private Rigidbody myRigid;


    // Use this for initialization
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        
        myRigid = GetComponent<Rigidbody>();
        applySpeed = walkSpeed;

        // 초기화.
        originPosY = theCamera.transform.localPosition.y;
        applyCrouchPosY = originPosY;



        //커서 고정
        Cursor.lockState = CursorLockMode.Locked;


        //커서 숨기기
        Cursor.visible = false;

    }




    // Update is called once per frame
    void Update()
    {
        { 


            IsGround();
            TryJump();
            TryRun();
            TryCrouch();
            WalkBack();
            Move();
            CameraRotation();
            CharacterRotation();
            //Camera dash view
            DashView();

        }
    }







    ///-------------------------------동작 시작






    // 앉기 시도
    private void TryCrouch()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGround)
        {
            Crouch();
        }


    }



    // 앉기 동작
    private void Crouch()
    {
        isCrouch = !isCrouch;

        if (isCrouch)
        {
            applySpeed = crouchSpeed;
            applyCrouchPosY = crouchPosY;
        }
        else
        {
            applySpeed = walkSpeed;
            applyCrouchPosY = originPosY;
        }
        

        StartCoroutine(CrouchCoroutine());

    }



    // 앉기 부드럽게
    IEnumerator CrouchCoroutine()
    {

        float _posY = theCamera.transform.localPosition.y;
        int count = 0;

        while (_posY != applyCrouchPosY)
        {
            count++;
            _posY = Mathf.Lerp(_posY, applyCrouchPosY, 0.3f);
            theCamera.transform.localPosition = new Vector3(0, _posY, 0);
            if (count > 13)
                break;
            yield return null;
        }
        theCamera.transform.localPosition = new Vector3(0, applyCrouchPosY, 0f);
    }


    // 지면 체크.
    private void IsGround()
    {
        isGround = Physics.Raycast(transform.position, Vector3.down, capsuleCollider.bounds.extents.y + 0.1f);
    }


    // 점프 시도
    private void TryJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            Jump();
        }
    }


    // 점프
    private void Jump()
    {

        // 앉은 상태에서 점프시 앉은 상태 해제.
        if (isCrouch)
            Crouch();

        myRigid.velocity = transform.up * jumpForce;
    }


    // 달리기 시도
    private void TryRun()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
        {
            Running();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift)&&!isCrouch)
        {
            RunningCancel();
        }
    }


    // 뒤 누를시 달리기 불가
    private void WalkBack()
    {
        if (Input.GetKey(KeyCode.S))
        {
            Slow();
        }
        if (Input.GetKeyUp(KeyCode.S)&&!isCrouch)
        {
            SlowCancel();
        }
    }



    // 달리기 실행
    private void Running()
    {
        if (isCrouch)
            Crouch();

        isRun = true;
        applySpeed = runSpeed;

    }


    // 달리기 취소
    private void RunningCancel()
    {
        isRun = false;
        applySpeed = walkSpeed;
    }



    // 뒤로 걸을때 실행
    private void Slow()
    {
        if (isCrouch)
        {
            isSlow = false;
            applySpeed = crouchSpeed;
        }
        else
        {
            isSlow = true;
            applySpeed = walkbackSpeed;
        }
        
    }


    // 뒤로 걷기 취소
    private void SlowCancel()
    {
        isSlow = false;
        applySpeed = walkSpeed;
    }


    // 움직임 실행
    private void Move()
    {

        float _moveDirX = Input.GetAxisRaw("Horizontal");
        float _moveDirZ = Input.GetAxisRaw("Vertical");

        Vector3 _moveHorizontal = transform.right * _moveDirX;
        Vector3 _moveVertical = transform.forward * _moveDirZ;

        Vector3 _velocity = (_moveHorizontal + _moveVertical).normalized * applySpeed;

        myRigid.MovePosition(transform.position + _velocity * Time.deltaTime);
    }






    ///----------------------------캐릭터 동작끝






    //-----------------------------캐릭터 시야움직임


    // 좌우 캐릭터 회전
    private void CharacterRotation()
    {

        float _yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 _characterRotationY = new Vector3(0f, _yRotation, 0f) * lookSensitivity;
        myRigid.MoveRotation(myRigid.rotation * Quaternion.Euler(_characterRotationY));
    }

    

    // 상하 카메라 회전
    private void CameraRotation()
    {
        float _xRotation = Input.GetAxisRaw("Mouse Y");
        float _cameraRotationX = _xRotation * lookSensitivity;
        currentCameraRotationX -= _cameraRotationX;
        currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

        theCamera.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
    }





    // 달릴시 보이는 시야각 변경

    private void DashView()
    {
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
        {
            theCamera.fieldOfView = 65f;
        }

        else
        {
            theCamera.fieldOfView = 60.2f;
        }

       
    }



}
