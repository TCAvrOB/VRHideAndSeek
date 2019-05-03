using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class LineRendererController : MonoBehaviour {

    VRProxy player;
    [SerializeField]
    GameObject Pointer; //移動位置のTarget
    [SerializeField]
    GameObject MoveTarget;
    [SerializeField]
    GameObject WandEffect;

    [SerializeField]
    float GroundAngle = 30.0f; //角度

    [SerializeField]
    float initialVelocity = 15.0f;
    [ SerializeField]
    float timeResolution = 0.02f;  //点と点の距離
    [SerializeField]
    float MaxTime = 10.0f;  //線の長さ
    [SerializeField]
    int RelayPoint = 15; //中継点
    [SerializeField]
    float Curvature = 0.9f; //キャンバ
    [SerializeField]
    float Delay = 60.0f; //転移時間
    [SerializeField]
    int Move_quantity = 0;

    [ SerializeField]
    Vector3 PositionDiff;
    [SerializeField]
    LayerMask layerMask = -1;

    float DelTime = 0.0f;

    GameObject GetControllerRotation;
    GameObject EyeObject;

    bool Move = false;　//時間判断
    bool Projectile_judge = false; //放物線判断
    bool TargetSetActive = false; //Target表示判断
    bool GroundAngle_judge = false; //地形角度の判断
    bool isWarpInput = false;

    Vector3 Point;
    Vector3 GetPosition = Vector3.zero;

    new Renderer renderer;

    float gravity_add;

    private GameObject PointerInstance;
    private GameObject MoveTargetInstance;
    private LineRenderer lineRenderer;
    private GameObject TrackedObject;
    private bool CanTeleport;

    public Vector3 TargetPoint { get { return Point; } }
    public bool IsWarpInput { get { return isWarpInput; } }
    public int TeleportCount { get { return Move_quantity; } }

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
    }

    void Start() {
        player = GameObject.FindObjectOfType<VRProxy>( );
        TrackedObject = player.Right;
        GetControllerRotation = player.Right;
        EyeObject = player.Camera.gameObject;
        PointerInstance = Instantiate( Pointer, Point, Quaternion.identity );
        renderer = GetComponent<Renderer>( );
    }

    private void OnEnable()
    {
        lineRenderer.enabled = true;
    }

    private void OnDisable()
    {
        lineRenderer.enabled = false;
    }

    void Update( ) {

        //update毎にリセットする物はここに書く
        ResetState( );
        Quaternion ControllerRotation = GetControllerRotation.transform.rotation;

        Vector3 postion = ( PositionDiff.magnitude * TrackedObject.transform.forward.normalized ) + TrackedObject.transform.position;

        //線とTargetの処理
        int index = 0;

        Vector3 velocityVector = TrackedObject.transform.forward * initialVelocity;
        Vector3 currentPosition = postion;

        PointerInstance.transform.position = Point;

        lineRenderer.positionCount = ( int )( MaxTime / timeResolution );

        currentPosition.y = postion.y - 0.01f;

        if (player.Trigger.GetStateDown(SteamVR_Input_Sources.RightHand)) {
            WandEffect.SetActive( true );
        }



        for ( float t = 0.0f; t < MaxTime; t += timeResolution ) {

            if ( index < lineRenderer.positionCount ) {
                lineRenderer.SetPosition( index, currentPosition );
            }

            RaycastHit hit;

            if ( Physics.Raycast( currentPosition, velocityVector, out hit, velocityVector.magnitude * timeResolution, layerMask ) ) {

                lineRenderer.positionCount = index + 2;

                lineRenderer.SetPosition( index + 1, hit.point );


                //VRコントローラの処理
                if (player.Trigger.GetStateDown(SteamVR_Input_Sources.RightHand) && Projectile_judge == false ) {
                    GetPosition = Point;
                    if ( Move == false && MoveTargetInstance == null && CanTeleport ) {
                        MoveTargetInstance = Instantiate( MoveTarget, new Vector3( GetPosition.x, GetPosition.y + 0.1f, GetPosition.z ), Quaternion.identity );
                        MoveTargetInstance.transform.rotation = Quaternion.LookRotation( hit.normal );
                    }

                    TimeDel( );

                }

                if (player.Trigger.GetState(SteamVR_Input_Sources.RightHand) && Projectile_judge == false ) {
                    if ( GetPosition == Vector3.zero ) {
                        GetPosition = Point;
                        if ( MoveTargetInstance == null && CanTeleport ) {
                            MoveTargetInstance = Instantiate( MoveTarget, new Vector3( GetPosition.x, GetPosition.y + 0.1f, GetPosition.z ), Quaternion.identity );
                            MoveTargetInstance.transform.rotation = Quaternion.LookRotation( hit.normal );
                        }
                        TimeDel( );
                    }
                }


                //角度の判断
                PointerInstance.transform.rotation = Quaternion.LookRotation( hit.normal );
                if ( Vector3.Angle( hit.normal, Vector3.up ) >= GroundAngle ) {
                    GroundAngle_judge = true;
                } else {
                    GroundAngle_judge = false;
                }

                Point = hit.point;
                //Point.y = hit.point.y + 0.05f;

                TargetSetActive = false;
                break;

            } else {
                TargetSetActive = true;
            }


            //キャンバシミュレーション
            currentPosition += velocityVector * timeResolution;
            if ( ControllerRotation.eulerAngles.x <= 180 ) {
                gravity_add = ControllerRotation.eulerAngles.x / 5;

                if ( gravity_add >= 9.8f ) {
                    gravity_add = 9.8f;
                }

            } else {
                gravity_add = 0f;
            }

            velocityVector += ControllerRotation * ( Physics.gravity + new Vector3 ( 0, gravity_add, 0 ) ) * timeResolution;

            if ( index >= RelayPoint ) {
                velocityVector *= Curvature;
            }
            index++;
            if ( index >= lineRenderer.positionCount ) {
                index -= 2;
            }

        }

        //コントローラー初期化
        if (player.Trigger.GetStateUp(SteamVR_Input_Sources.RightHand)) {
            Initialized( );
            
        }

        //転移処理
        if (player.Trigger.GetState(SteamVR_Input_Sources.RightHand) && MoveTargetInstance != null  ) {
            DelTime += Time.deltaTime;
            TargetSetActive = true;

            if ( DelTime >= Delay ) {
                if ( Move == true ) {
                    Vector3 diff = EyeObject.transform.localPosition;
                    player.Base.transform.position = GetPosition - player.transform.localRotation * new Vector3( diff.x, 0, diff.z );
                    player.OnTeleport();
                    Move_quantity++;
                    isWarpInput = true;
                    DelTime = 0.0f;
                    Move = false;
                }
            }
        }
        //Targetの判断
        Projectile_judge = ColliderTag(Point);
    }


    private void Initialized( ) {
        WandEffect.SetActive( false );
        ColorControllerON( );
        DelTime = 0f;
        Move = false;
        GetPosition = Vector3.zero;
        Destroy( MoveTargetInstance );
    }

    public void ColorControllerOFF( ) {        
        TargetSetActive = true;
        CanTeleport = false;

    }

    public void ColorControllerON( ) {        
        TargetSetActive = false;
        if ( MoveTargetInstance != null ) {
            Destroy( MoveTargetInstance );
        }
        CanTeleport = true;
    }

    private void TimeDel( ) {
            Move = true;
    }

    private void ResetState( ) {
        isWarpInput = false;
    }

    private bool ColliderTag(Vector3 point) {
        if ( TargetSetActive == true || GroundAngle_judge == true) {
                PointerInstance.SetActive(false);
                return true;
        } else {
                PointerInstance.SetActive(true);
                return false;
        }
    }

    public void DeleteLine() {
        if ( lineRenderer ) {
            lineRenderer.positionCount = 0;
        }
    }

}
