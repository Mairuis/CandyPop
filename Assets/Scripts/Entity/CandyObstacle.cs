using UnityEngine;
using UnityEngine.U2D;
using System;
using Random = UnityEngine.Random;

public class CandyObstacle : Obstacle
{
    //TODO from configuration
    [SerializeField] private int hp = 100;
    [SerializeField] private SpriteAtlas candyAtlas;
    [SerializeField] private PhysicsMaterial2D _candyPhysicsMaterial;


    private Candy _candy;
    private int _candyId;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    public event Action<int> OnHpChanged;

    public Candy Candy => _candy;

    public int HP
    {
        get => hp;
        private set
        {
            int oldHp = hp;
            hp = value;
            if (oldHp != hp)
            {
                OnHpChanged?.Invoke(hp);
            }
        }
    }

    private void Awake()
    {
        _candy = transform.GetComponentInChildren<Candy>();
        _animator = _candy.GetComponent<Animator>();
        _spriteRenderer = _candy.GetComponent<SpriteRenderer>();

        SetCandyId(Random.Range(1, 18));
    }

    void OnEnable()
    {
        // Subscribe to candy events
        SubscribeToCandyEvents();
    }

    void OnDisable()
    {
        UnsubscribeFromCandyEvents();
    }

    private void SubscribeToCandyEvents()
    {
        if (_candy != null)
        {
            _candy.OnCandyHit += HandleCandyHit;
        }
    }

    private void UnsubscribeFromCandyEvents()
    {
        if (_candy != null)
        {
            _candy.OnCandyHit -= HandleCandyHit;
        }
    }

    private void HandleCandyHit(Collision2D other)
    {
        if (!other.gameObject.CompareTag(StringPool.Projectile))
        {
            return;
        }

        if (_animator != null)
        {
            _animator.Play("CandyShock");
        }

        SetHp(hp - 1);
    }

    public void SetHp(int newHp)
    {
        HP = newHp;

        if (hp <= 0)
        {
            //销毁自身
            Destroy(gameObject);
        }
    }


    private void SetCandyId(int id)
    {
        _candyId = id;

        if (candyAtlas != null && _spriteRenderer != null)
        {
            string spriteName = "Candy_" + _candyId;
            Sprite candySprite = candyAtlas.GetSprite(spriteName);

            if (candySprite != null)
            {
                _spriteRenderer.sprite = candySprite;

                // Update collider
                GameObject candyObject = _candy.gameObject;
                PolygonCollider2D polygonCollider = candyObject.GetComponent<PolygonCollider2D>();
                if (polygonCollider != null)
                {
                    Destroy(polygonCollider);
                }

                polygonCollider = candyObject.AddComponent<PolygonCollider2D>();
                polygonCollider.sharedMaterial = _candyPhysicsMaterial;
            }
            else
            {
                Debug.LogWarning("Sprite not found in atlas: " + spriteName);
            }
        }
        else
        {
            Debug.LogError("Candy Atlas or SpriteRenderer is not assigned.");
        }
    }

    public void SetCandy(Candy newCandy)
    {
        // Unsubscribe from old candy events
        UnsubscribeFromCandyEvents();

        //Destroy old candy
        Destroy(_candy);

        // Set new candy
        _candy = newCandy;

        if (_candy == null) return;

        _animator = _candy.GetComponent<Animator>();
        _spriteRenderer = _candy.GetComponent<SpriteRenderer>();

        _candy.transform.parent = transform;
        _candy.transform.position = Vector3.zero;

        SubscribeToCandyEvents();
    }
}
