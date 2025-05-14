using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class LauncherSystem : MonoBehaviour, IGameSystem
{
    [FormerlySerializedAs("projectilePrefab")] [SerializeField]
    public GameObject bulletPrefab;

    [SerializeField] public Transform launchPoint;
    [SerializeField] public float launchPower = 30f;
    [SerializeField] public int launchCount = 50;
    [SerializeField] public float launchInterval = 0.05f;

    [SerializeField] public GameObject aimDotPrefab;
    [SerializeField] public int aimDotSkipCount = 2;
    [SerializeField] public float aimDotBaseScale = 0.08f;
    [SerializeField] public float aimDotIntervalTime = 0.1f;
    [SerializeField] public float maxPullDistance = 5f;

    [SerializeField] public int minPreviewLinePoints = 5;
    [SerializeField] public int maxPreviewLinePoints = 20;

    private bool _isAiming;
    private SpriteRenderer[] _aimDots;
    private Vector2 _currentTouchPosition;
    private Camera _mainCamera;
    private PlayerControls _inputActions;
    private Coroutine _launchCoroutine;


    //当玩家发射
    public event UnityAction OnPlayerLaunch;
    public int LaunchedCount { get; private set; }

    private bool _isLaunchEnabled = true;

    public bool IsLaunchEnabled
    {
        get => _isLaunchEnabled;
        set
        {
            _isLaunchEnabled = value;
            if (_isLaunchEnabled)
            {
                _inputActions.Enable();
            }
            else
            {
                _inputActions.Disable();
            }
        }
    }

    void Awake()
    {
        _mainCamera = Camera.main;

        InitializeInputActions();
        InitializeAimDots();
    }

    private void InitializeInputActions()
    {
        _inputActions = new PlayerControls();
        _inputActions.Gameplay.Touch.performed += OnMovePerformed;
        _inputActions.Gameplay.Press.performed += OnPressPerformed;
        _inputActions.Gameplay.Press.canceled += OnPressCanceled;
    }

    private void InitializeAimDots()
    {
        _aimDots = new SpriteRenderer[maxPreviewLinePoints];
        for (int i = 0; i < maxPreviewLinePoints; i++)
        {
            _aimDots[i] = Instantiate(aimDotPrefab).GetComponent<SpriteRenderer>();
            _aimDots[i].enabled = false;
        }
    }

    private void OnPressCanceled(InputAction.CallbackContext obj)
    {
        _isAiming = false;
        if (_launchCoroutine != null)
        {
            StopCoroutine(_launchCoroutine);
        }

        OnPlayerLaunch?.Invoke();

        _launchCoroutine = StartCoroutine(LaunchBalls(CalculateForce(launchPoint.position, _currentTouchPosition)));
    }

    private void OnPressPerformed(InputAction.CallbackContext obj)
    {
        _isAiming = true;
    }

    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        _currentTouchPosition = _mainCamera.ScreenToWorldPoint(context.ReadValue<Vector2>());
    }

    void OnEnable()
    {
        _inputActions.Enable();
    }

    void OnDisable()
    {
        _inputActions.Disable();
    }

    void Update()
    {
        DrawAimLine();
    }

    private void DrawAimLine()
    {
        if (_isAiming)
        {
            Vector2 startPoint = launchPoint.position;
            Vector2 force = CalculateForce(startPoint, _currentTouchPosition);
            float pullDistance = Vector2.Distance(startPoint, _currentTouchPosition);
            int dynamicPreviewPoints =
                Mathf.Clamp(
                    Mathf.FloorToInt(Mathf.Lerp(minPreviewLinePoints, maxPreviewLinePoints,
                        pullDistance / maxPullDistance)), minPreviewLinePoints, maxPreviewLinePoints);
            float totalAimDotTime = aimDotIntervalTime * dynamicPreviewPoints;
            Vector2 acceleration = Physics2D.gravity;

            for (int i = 0; i < dynamicPreviewPoints; i++)
            {
                float time = (i + aimDotSkipCount) * aimDotIntervalTime;
                Vector2 previewPos = startPoint + force * time + 0.5f * acceleration * time * time;
                var aimDot = _aimDots[i];
                aimDot.transform.position = previewPos;
                float scale = Mathf.Lerp(aimDotBaseScale, aimDotBaseScale * 2f, time / totalAimDotTime);
                aimDot.transform.localScale = new Vector3(scale, scale, 1);
                float alpha = Mathf.Lerp(1f, 0f, time / totalAimDotTime);
                Color color = aimDot.material.color;
                color.a = alpha;
                aimDot.material.color = color;
                aimDot.enabled = true;
            }

            for (int i = dynamicPreviewPoints; i < maxPreviewLinePoints; i++)
            {
                _aimDots[i].enabled = false;
            }
        }
        else
        {
            HideAimDots();
        }
    }

    private void HideAimDots()
    {
        foreach (var aimDot in _aimDots)
        {
            aimDot.enabled = false;
        }
    }

    private IEnumerator LaunchBalls(Vector2 force)
    {
        for (int i = 0; i < launchCount; i++)
        {
            var projectile = Instantiate(bulletPrefab, launchPoint.position, Quaternion.identity);
            projectile.GetComponent<Rigidbody2D>()?.AddForce(force, ForceMode2D.Impulse);

            LaunchedCount++;

            yield return new WaitForSeconds(launchInterval);
        }
    }

    private Vector2 CalculateForce(Vector2 startPoint, Vector2 touchPosition)
    {
        float pullDistance = Vector2.Distance(startPoint, touchPosition);
        float adjustedStrength = Mathf.Lerp(0, launchPower, pullDistance / maxPullDistance);
        // Debug.Log($"startPoint: {startPoint}, pullDistance: {pullDistance}, adjustedStrength: {adjustedStrength} ({pullDistance / maxPullDistance * 100}%)");
        return (touchPosition - startPoint).normalized * adjustedStrength;
    }
}
