using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    public bool isActive { get; set; }
    public BirdBehaviour bird { get; set; }
    [SerializeField] Transform block = default;
    [SerializeField] float blockWidth = 0.48f;
    [SerializeField] int blockLength = 5;
    [SerializeField] float blockSpaceHeigh = 1.4f;
    [SerializeField] float blockSpaceWidth = 2f;
    [SerializeField] Vector2 bottomBlockPosYClamp = default;

    [Header("BG")]
    [SerializeField] Transform floor = default;
    [SerializeField] int floorSpriteLength = 16;
    [SerializeField] float floorSpriteWidth = 0.4f;
    [SerializeField] Transform grass = default;
    [SerializeField] int grassSpriteLength = 10;
    [SerializeField] float grassSpriteWidth = 1;
    [SerializeField] Transform cloud = default;
    [SerializeField] Sprite[] cloudSprites = default;
    [SerializeField] float cloudMaxY = default;
    [SerializeField] int cloudSpriteLength = 10;
    [SerializeField] float cloudSpriteWidth = 0.8f;
    [SerializeField] Transform tree = default;
    [SerializeField] Sprite[] treeSprites = default;
    [SerializeField] int treeSpriteLength = 8;
    [SerializeField] float treeSpriteWidth = 1f;
    [SerializeField] Transform bg = default;

    private void Start()
    {
        SyncScreenWidth();
        GenerateBG(floor, floorSpriteLength, floorSpriteWidth);
        GenerateBG(grass, grassSpriteLength, grassSpriteWidth);
        GenerateBGRamdomYRandomSprite(cloud, cloudSpriteLength, cloudSpriteWidth, cloudMaxY, cloudSprites);
        GenerateBlock(block, blockLength, blockSpaceWidth, blockSpaceHeigh);
        GenerateBGRamdomYRandomSprite(tree, treeSpriteLength, treeSpriteWidth, 0, treeSprites);
    }

    void SyncScreenWidth()
    {
        var defaultScreen = new Vector2(1080, 1920);
        var amount = (Screen.width / (float)Screen.height) / (defaultScreen.x / (float)defaultScreen.y);
        bg.localScale = new Vector3(bg.localScale.x * amount, bg.localScale.y, bg.localScale.z);

        amount = Mathf.Max(1, amount);

        blockLength = blockLength * (int)amount;
        floorSpriteLength = floorSpriteLength * (int)amount;
        grassSpriteLength = grassSpriteLength * (int)amount;
        cloudSpriteLength = cloudSpriteLength * (int)amount;
        treeSpriteLength = treeSpriteLength * (int)amount;
    }

    private void Update()
    {
        if (!isActive) return;
        if ( CheckTriggerBlock(bird.transform.position)) GameManager.instance.GameOver();
        BlockLoop(block, 1.2f * bird.Speed, blockLength * blockSpaceWidth, blockSpaceWidth, blockSpaceHeigh);
        BgLoop(floor, 1.2f * bird.Speed, floorSpriteLength * floorSpriteWidth, floorSpriteWidth);
        BgLoop(grass, 0.5f * bird.Speed, grassSpriteLength * grassSpriteWidth, grassSpriteWidth);
        BgLoopAndRamdomYRandomSprite(cloud, 0.1f * bird.Speed, cloudSpriteLength * cloudSpriteWidth, cloudSpriteWidth, cloudMaxY, cloudSprites);
        BgLoopAndRamdomYRandomSprite(tree, 1f * bird.Speed, treeSpriteLength * treeSpriteWidth, treeSpriteWidth, 0, treeSprites);
    }
    Transform blockCache = null;
    public bool CheckTriggerBlock(Vector3 pos)
    {
        var blockRadius = blockWidth * 0.5f;
        var blockSpaceHeighRadius = blockSpaceHeigh * 0.5f;
        foreach (Transform child in block)
        {
            if (Mathf.Abs(child.position.x - pos.x) < blockRadius + bird.birdRadius)
            {
                if (Mathf.Abs(child.GetChild(1).GetChild(0).position.y - (blockRadius + blockSpaceHeighRadius) - pos.y) > blockSpaceHeighRadius - bird.birdRadius) return true;
                if (pos.x > child.position.x)
                {
                    if (blockCache != child)
                    {
                        blockCache = child;
                        GameManager.instance.Score++;
                    }
                }
            }
        }
        if (pos.y < block.position.y + bird.birdRadius) return true;
        return false;
    }

    void GenerateBlock(Transform parent, int length, float width, float heigh)
    {
        var indexPosX = length * width * 1.5f;
        for (int i = 0; i < length; i++)
        {
            var temp = (i > 0) ? Instantiate(parent.GetChild(0), parent) : parent.GetChild(0);

            indexPosX -= width;
            temp.localPosition = Vector3.right * indexPosX;
            GenerateWall(temp.GetChild(0), temp.GetChild(1), temp.position.y, heigh, Random.Range(bottomBlockPosYClamp.x, bottomBlockPosYClamp.y));
        }
    }

    void BlockLoop(Transform parent, float speed, float groundWidth, float sizeWidth, float heigh)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            parent.GetChild(i).localPosition -= Vector3.right * Time.deltaTime * speed;
            if (parent.GetChild(i).localPosition.x < -groundWidth / 2f)
            {
                parent.GetChild(i).localPosition = parent.GetChild(0).localPosition + Vector3.right * sizeWidth;
                parent.GetChild(i).SetSiblingIndex(0);
                GenerateWall(parent.GetChild(i).GetChild(0), parent.GetChild(i).GetChild(1), parent.GetChild(i).position.y, heigh, Random.Range(bottomBlockPosYClamp.x, bottomBlockPosYClamp.y));
            }
        }
    }

    void GenerateWall(Transform bottom, Transform top, float groudPosY, float spaceHeigh, float buttomBlockYPos)
    {
        for (int i = 1; i < bottom.childCount; i++) Destroy(bottom.GetChild(i).gameObject);
        for (int i = 1; i < top.childCount; i++) Destroy(top.GetChild(i).gameObject);

        var indexPosY = blockWidth / 2f;
        var bottomLength = (int)((buttomBlockYPos - groudPosY) / blockWidth);
        for (int i = 0; i < bottomLength; i++)
        {
            if (i > 0)
            {
                var temp = Instantiate(bottom.GetChild(0), bottom);
                temp.localPosition = Vector3.up * indexPosY;
            }
            else bottom.GetChild(0).localPosition = Vector3.up * indexPosY;

            indexPosY += blockWidth;
        }

        indexPosY += spaceHeigh;
        top.GetChild(0).localPosition = Vector3.up * indexPosY;
        indexPosY += blockWidth;
        while(indexPosY < 10)
        {
            var temp = Instantiate(top.GetChild(0), top);
            temp.localPosition = Vector3.up * indexPosY;
            indexPosY += blockWidth;
        }
    }

    void GenerateBG(Transform parent, int length, float width)
    {
        var indexPosX = (length * width) / 2f;
        for (int i = 0; i < length; i++)
        {
            var sprite = (i > 0) ? Instantiate(parent.GetChild(0), parent) : parent.GetChild(0);

            indexPosX -= width;
            sprite.localPosition = Vector3.right * indexPosX;
        }
    }

    void BgLoop(Transform parent, float speed, float groundWidth, float sizeWidth)
    {
        for(int i = 0; i < parent.childCount; i++)
        {
            parent.GetChild(i).localPosition -= Vector3.right * Time.deltaTime * speed;
            if (parent.GetChild(i).localPosition.x < -groundWidth / 2f)
            {
                parent.GetChild(i).localPosition = parent.GetChild(0).localPosition + Vector3.right * sizeWidth;
                parent.GetChild(i).SetSiblingIndex(0);
            }
        }
    }

    void GenerateBGRamdomYRandomSprite(Transform parent, int length, float width, float maxY, Sprite[] sprites)
    {
        var indexPosX = (length * width) / 2f;
        for (int i = 0; i < length; i++)
        {
            var sprite = (i > 0) ? Instantiate(parent.GetChild(0), parent) : parent.GetChild(0);

            indexPosX -= width;
            sprite.localPosition = Vector3.right * indexPosX + Vector3.up * Random.Range(0, maxY);
            parent.GetChild(i).GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
        }
    }

    void BgLoopAndRamdomYRandomSprite(Transform parent, float speed, float groundWidth, float sizeWidth, float maxY, Sprite[] sprites)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            parent.GetChild(i).localPosition -= Vector3.right * Time.deltaTime * speed;
            if (parent.GetChild(i).localPosition.x < -groundWidth / 2f)
            {
                parent.GetChild(i).localPosition = Vector3.right * parent.GetChild(0).localPosition.x + Vector3.right * sizeWidth + Vector3.up * Random.Range(0, maxY);
                parent.GetChild(i).SetSiblingIndex(0);
                parent.GetChild(i).GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
            }
        }
    }

#if UNITY_EDITOR
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(Vector3.right + Vector3.up * bottomBlockPosYClamp.x, Vector3.right + Vector3.up * bottomBlockPosYClamp.y);
        var blockRadius = (blockLength * blockSpaceWidth) / 2f;
        Gizmos.DrawLine(block.position - Vector3.right * blockRadius, block.position + Vector3.right * blockRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(cloud.position, cloud.position + Vector3.up * cloudMaxY);
        var cloudRadius = (cloudSpriteLength * cloudSpriteWidth) / 2f;
        Gizmos.DrawLine(cloud.position - Vector3.right * cloudRadius, cloud.position + Vector3.right * cloudRadius);

        Gizmos.color = Color.green;
        var grassRadius = (grassSpriteLength * grassSpriteWidth) / 2f;
        Gizmos.DrawLine(grass.position - Vector3.right * grassRadius, grass.position + Vector3.right * grassRadius);
        Gizmos.color = Color.blue;
        var floorRadius = (floorSpriteLength * floorSpriteWidth) / 2f;
        Gizmos.DrawLine(floor.position - Vector3.right * floorRadius, floor.position + Vector3.right * floorRadius);
    }
#endif
}
