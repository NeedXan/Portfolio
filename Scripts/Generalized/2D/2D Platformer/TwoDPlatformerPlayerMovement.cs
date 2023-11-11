using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(Animator)), RequireComponent(typeof(Collider2D))]
public class TwoDPlatformerPlayerMovement : MonoBehaviour, IDataPersistence
{
    /// <summary>
    ///  ATTACK EVENTS, TO ACCESS THESE EVENTS IN YOUR ATTACK SCRIPT, WRITE THIS:
    ///  private void OnEnable() => TwoDPlatformerPlayerMovement.AttackEvent += Attack;
    ///  private void OnDisable() => TwoDPlatformerPlayerMovement.AttackEvent -= Attack;
    /// </summary>
    public delegate void PlayerAttack();
    public static event PlayerAttack AttackEvent;

    [Header("If you would like preset values, use the context menu!")]
    [SerializeField] private bool _debugMode;

    [Space(3), Header("Required Variables")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private LayerMask _wallLayer;

    private Animator _animator;

    private Rigidbody2D _rigidbody;
    private float _moveVelocity;
    private Transform _groundCheck;
    private Vector2 _facingLeft, _facingRight;
    private float _coyoteTimeCounter;
    private float _jumpBufferCounter;
    private bool _isWallSliding;
    private Transform _wallCheck;
    private bool _isWallJumping;
    private float _wallJumpingDirection;
    private float _wallJumpingCounter;
    [HideInInspector] public bool _doMove = true;

    [Space(3), Header("Customization")]
    [Range(1f, 25f), Tooltip("It is not reasonable to have a speed of over 25 or less than 1")]
    [SerializeField] private float _speed;
    [Range(5f, 30f), Tooltip("It is not reasonable to have a jump force of over 30 or less than 5")]
    [SerializeField] private float _jumpForce;
    [SerializeField] private bool _attack;
    [SerializeField] private bool _wallSlide;
    [Range(1f, 5f)]
    [SerializeField] private float _wallSlideSpeed = 2f;
    [SerializeField] private bool _wallJump;
    [SerializeField] private float _wallJumpingTime = 0.2f;
    [SerializeField] private float _wallJumpingDuration = 0.2f;
    [SerializeField] private Vector2 _wallJumpingPower = new Vector2(8f, 10f);
    [Range(0f, 0.5f)]
    [SerializeField] private float _coyoteTime = 0.5f;
    [Range(0f, 0.5f)]
    [SerializeField] private float _jumpBufferTime = 0.5f;

    [Space(3), Header("Keybinds")] // This is where you add any extra keybindings you need
    public KeyCode jump;
    public KeyCode attack;

    private void Awake() => Initialization();
    private void Initialization()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();

        _facingRight = new Vector2(-transform.localScale.x, transform.localScale.y);
        _facingLeft = new Vector2(transform.localScale.x, transform.localScale.y);

        _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

        foreach (Transform t in transform.GetComponentsInChildren<Transform>())
        {
            if (t.CompareTag("Ground Check"))
                _groundCheck = t;
            else if (t.CompareTag("Wall Check"))
                _wallCheck = t;
        }
    }

    private void Update()
    {
        _animator.SetFloat("yVelocity", _rigidbody.velocity.y);

        if (_wallSlide)
            WallSlide();
        if (_wallJump)
            WallJump();

        if (IsGrounded())
            _coyoteTimeCounter = _coyoteTime;
        else _coyoteTimeCounter -= Time.fixedDeltaTime;

        if (Input.GetKeyDown(jump))
            _jumpBufferCounter = _jumpBufferTime;
        else _jumpBufferCounter -= Time.fixedDeltaTime;

        if (_coyoteTimeCounter > 0 && _jumpBufferCounter > 0f)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _jumpForce);

            _jumpBufferCounter = 0f;
        }

        if (Input.GetKeyUp(jump) && _rigidbody.velocity.y > 0f)
        {
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _rigidbody.velocity.y * 0.25f);
            _coyoteTimeCounter = 0;
        }

        _moveVelocity = Input.GetAxisRaw("Horizontal") * _speed;

        if (_rigidbody.velocity.x < 0 && Input.GetAxisRaw("Horizontal") < 0)
                transform.localScale = _facingLeft;
        else if (_rigidbody.velocity.x > 0 && Input.GetAxisRaw("Horizontal") > 0)
                    transform.localScale = _facingRight;

        if (Input.GetAxisRaw("Horizontal") != 0)
            _animator.SetBool("Moving", true);
        else _animator.SetBool("Moving", false);

        if (_wallJump && !_isWallJumping && _doMove)
            _rigidbody.velocity = new Vector2(_moveVelocity, _rigidbody.velocity.y);
        else if (!_wallJump && _doMove)
            _rigidbody.velocity = new Vector2(_moveVelocity, _rigidbody.velocity.y);

        if (_attack)
            if (Input.GetKeyDown(attack) && (!IsWalled() || (IsWalled() && IsGrounded())))
                if (AttackEvent != null)
                    AttackEvent();

        if (IsGrounded())
        {
            _animator.SetBool("IsGrounded", true);
            _animator.SetBool("Jump", false);
            _animator.SetBool("Wall Slide", false);
        }
        else
        {
            _animator.SetBool("IsGrounded", false);
            _animator.SetBool("Jump", true);
            _animator.SetBool("Wall Slide", false);
        }

        if (IsWalled() && !IsGrounded())
        {
            _animator.SetBool("Wall Slide", true);
            _animator.SetBool("Jump", false);
        }
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(_groundCheck.position, 0.4f, _groundLayer);
    }

    public bool IsWalled()
    {
        return Physics2D.OverlapCircle(_wallCheck.position, 0.2f, _wallLayer);
    }

    private void WallSlide()
    {
        if (IsWalled() && !IsGrounded() && Input.GetAxisRaw("Horizontal") != 0)
        {
            _isWallSliding = true;
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, Mathf.Clamp(_rigidbody.velocity.y, -_wallSlideSpeed, float.MaxValue));
        }
        else _isWallSliding = false;
    }

    private void WallJump()
    {
        if (_isWallSliding)
        {
            _isWallJumping = false;
            if (-transform.localScale.x < 0)
                _wallJumpingDirection = 1;
            else if (-transform.localScale.x > 0)
                _wallJumpingDirection = -1;
            _wallJumpingCounter = _wallJumpingTime;

            CancelInvoke(nameof(StopWallJumping));
        }
        else
            _wallJumpingCounter -= Time.fixedDeltaTime;

        if (Input.GetKeyDown(jump) && _wallJumpingCounter > 0f)
        {
            _isWallJumping = true;
            _rigidbody.velocity = new Vector2(_wallJumpingDirection * _wallJumpingPower.x, _wallJumpingPower.y);
            _wallJumpingCounter = 0f;

            if (transform.localScale.x != _wallJumpingDirection && _wallJumpingDirection < 0)
                transform.localScale = _facingLeft;
            else if (transform.localScale.x != _wallJumpingDirection && _wallJumpingDirection > 0)
                transform.localScale = _facingRight;

            Invoke(nameof(StopWallJumping), _wallJumpingDuration);
        }
    }

    private void StopWallJumping() => _isWallJumping = false;

    #region Customizable Functions

    #endregion

    #region Context Menu Functions

    [ContextMenu("Debug Mode")] public void DebugMode() { _debugMode = true; }
    [ContextMenu("Pure Platformer Defaults")] private void PurePlatformerDefaults() { _speed = 5f; _jumpForce = 10f; _attack = false; GetComponent<Rigidbody2D>().gravityScale = 1f; }
    [ContextMenu("Metroidvania Defaults")] private void MetroidvaniaDefaults() { _speed = 5f; _jumpForce = 10f; _attack = true; GetComponent<Rigidbody2D>().gravityScale = 1.75f; }

    #endregion

    #region Save-Load

    void IDataPersistence.LoadData(GameData data)
    {
        if (_debugMode) return;

        transform.position = data.playerPosition;
        if (data.isFacingLeft)
            transform.localScale = _facingLeft;
    }

    void IDataPersistence.SaveData(ref GameData data)
    {
        if (_debugMode) return;

        data.playerPosition = transform.position;
        if (transform.localScale.Equals(_facingLeft))
            data.isFacingLeft = true;
    }

    #endregion
}
