using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public enum AttackDir
    {
        Up = 1,
        Down = 2,
        Side = 3
    }
    private AttackDir _currentAttackDir;

    private Animator _animator;
    private float kbTime = .4f;
    private float kbCounter;
    private bool inKB;

    [SerializeField] private GameObject _upSwing;
    [SerializeField] private GameObject _downSwing;
    [SerializeField] private GameObject _sideSwing;

    [Space(3), SerializeField]
    private float _knockbackAmount;
    [SerializeField] private float _pogoAmount;
    public int attackDamage;

    private void OnEnable() { TwoDPlatformerPlayerMovement.AttackEvent += Attack; }
    private void OnDisable() { TwoDPlatformerPlayerMovement.AttackEvent -= Attack; }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        GetDir();

        if (inKB)
            kbCounter -= Time.fixedDeltaTime;
        else
            kbCounter = kbTime;

        if (kbCounter <= 0)
        {
            gameObject.GetComponent<TwoDPlatformerPlayerMovement>()._doMove = true;
            inKB = false;
        }
    }

    void GetDir()
    {
        if (Input.GetAxisRaw("Vertical") > 0)
            _currentAttackDir = AttackDir.Up;
        else if (Input.GetAxisRaw("Vertical") < 0 && !GetComponent<TwoDPlatformerPlayerMovement>().IsGrounded())
            _currentAttackDir = AttackDir.Down;
        else
            _currentAttackDir = AttackDir.Side;
    }

    void Attack()
    {
        _animator.SetFloat("AttackDir", (int)_currentAttackDir);
        _animator.SetTrigger("Attack");
    }

    public void SwingAnim(AttackDir dir)
    {
        switch (dir)
        {
            case AttackDir.Up:
                Instantiate(_upSwing, transform.position + (Vector3.up * 2.5f), Quaternion.identity, transform);
                break;
            case AttackDir.Down:
                Instantiate(_downSwing, transform.position + (Vector3.down * 2), Quaternion.identity, transform);
                break;
            case AttackDir.Side:
                if (transform.localScale.x > 0)
                    Instantiate(_sideSwing, new Vector2(transform.position.x - 1.5f, transform.position.y), Quaternion.identity, transform);
                if (transform.localScale.x < 0)
                    Instantiate(_sideSwing, new Vector2(transform.position.x + 1.5f, transform.position.y), Quaternion.identity, transform);
                break;
        }
    }

    public void Hit(IHitable hitable, int dir)
    {
        if (dir == 3)
            Knockback();
    }

    public void Hit(IPogoable pogoable, int dir)
    {
        if (dir == 3)
            Knockback();
        else if (dir == 2)
            Pogo();
    }

    public void Hit(IDamageable damageable, int dir)
    {
        damageable.TakeDamage(attackDamage);

        if (dir == 3)
            Knockback();
        else if (dir == 2)
            Pogo();
    }

    void Knockback()
    {
        inKB = true;
        gameObject.GetComponent<TwoDPlatformerPlayerMovement>()._doMove = false;
        if (transform.localScale.x > 0)
            GetComponent<Rigidbody2D>().velocity = new Vector2(_knockbackAmount, GetComponent<Rigidbody2D>().velocity.y);
        if (transform.localScale.x < 0)
            GetComponent<Rigidbody2D>().velocity = new Vector2(-_knockbackAmount, GetComponent<Rigidbody2D>().velocity.y);
    }

    void Pogo()
    {
        GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, _pogoAmount);
    }
}
