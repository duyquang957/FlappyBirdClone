using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum BirdAnimState
{
    idle,
    fly,
    hurt
}
public class BirdBehaviour : MonoBehaviour
{
    public bool isActive { get; set; }
    
    SpriteRenderer rd;
    public float birdRadius = 0;

    protected float speed = 1;
    public float Speed { get => speed; set => speed = value; }
    public bool speedIncrease { get; set; }
    

    private void Start()
    {
        rd = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!isActive) return;
        Control();
        Gravity();

        if (speedIncrease) speed += Time.deltaTime * 0.02f;
    }

    private void LateUpdate()
    {
        Anim();
        MapClamp();
    }

    float upCounter = 1;
    void Control()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Jump();
        }

    }

    public void Jump()
    {
        upCounter = 0;
        audioFlap.Play();
    }

    [SerializeField] float gravityValue = 9.8f;
    void Gravity()
    {
        upCounter = Mathf.Lerp(upCounter, 1, Time.deltaTime);
        if (upCounter < 0.9f)
        {
            transform.position += Vector3.up * Time.deltaTime * gravityValue * Mathf.Lerp(1.6f, 0, upCounter);
        }

        transform.rotation = Quaternion.AngleAxis(Mathf.Lerp(45, -45, upCounter), Vector3.forward);

        transform.position -= Vector3.up * Time.deltaTime * gravityValue;
    }

    [SerializeField] Vector2 clampY = default;
    void MapClamp()
    {
        var checkedPos = transform.position;
        checkedPos.y = Mathf.Clamp(checkedPos.y, clampY.x, clampY.y);
        transform.position = checkedPos;
    }

    [SerializeField] BirdAnimState animState = BirdAnimState.idle;
    [SerializeField] Sprite[] birdIdle = default;
    [SerializeField] Sprite[] birdFly = default;
    [SerializeField] Sprite[] birdHurt = default;
    [SerializeField] float frameCD = 1;
    float frameDelta = 0;
    int spriteIndex = 0;

    public void SetAnimState(BirdAnimState state)
    {
        animState = state;
        spriteIndex = 0;
        frameDelta = 0;
    }

    void Anim()
    {
        if (animState == BirdAnimState.fly) Animation(birdFly);
        else if (animState == BirdAnimState.hurt) Animation(birdHurt, false);
        else Animation(birdIdle);
    }

    void Animation(Sprite[] sprites, bool loop = true)
    {
        frameDelta += Time.deltaTime;
        if (frameDelta > frameCD)
        {
            frameDelta -= frameCD;
            spriteIndex = (spriteIndex < sprites.Length - 1) ? spriteIndex + 1 : (loop ? 0 : spriteIndex);
            rd.sprite = sprites[spriteIndex];
        }
    }

    public void BirdHit()
    {
        audioHit.Play();
    }

    [SerializeField] AudioSource audioFlap = default;
    [SerializeField] AudioSource audioHit = default;
}
