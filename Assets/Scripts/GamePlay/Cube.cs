using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cube : BaseComponent
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private List<Sprite> sprites;
    private bool isMove;

    public bool IsMove
    {
        get => isMove;
        set => isMove = value;
    }
    private bool canMove = true;
    // Dictionary lưu trữ màu sắc của từng mặt của khối lập phương
    private Dictionary<BlockFace, ColorType> faceColors = new Dictionary<BlockFace, ColorType>()
    {
        { BlockFace.Top, ColorType.White },
        { BlockFace.Bottom, ColorType.Yellow },
        { BlockFace.Front, ColorType.Red },
        { BlockFace.Back, ColorType.Orange},
        { BlockFace.Right, ColorType.Blue },
        { BlockFace.Left, ColorType.Green }
    };
    
    public ColorType colorTop;
    public ColorType colorBottom;
    public ColorType colorBack;
    public ColorType colorLeft;
    
    protected override void LoadComponent()
    {
        base.LoadComponent();
        if(rb!=null) return;
        if(sr!=null) return;
        if(sprites.Count > 0) return;
        rb = GetComponent<Rigidbody2D>();
        sprites = Resources.LoadAll<Sprite>("Sprites/AllFaceBlock").ToList();
        sr = GetComponent<SpriteRenderer>();
    }

    protected override void Start()
    {
        base.Start();
        UpdateEnum();
    }

    // Update is called once per frame
    private void Update()
    {
        // Kiểm tra nếu có phím nào được nhấn và đối tượng có thể di chuyển
        if (isMove)
        {
            Move();
        }
    }
    private void Move()
    {
        if (canMove)
        {
            Vector3 newPosition;

            if (Input.GetAxisRaw("Horizontal") != 0 )
            {
                if (Input.GetAxisRaw("Horizontal") > 0 ) // Di chuyển sang phải
                {
                    newPosition = transform.position + Vector3.right;
                }
                else // Di chuyển sang trái
                {
                    newPosition = transform.position + Vector3.left;
                }

                if (IsPositionValid(newPosition))
                {
                    AudioManager.Instance.PlaySFX(SoundEffect.RollCube);
                    rb.MovePosition(newPosition);
                    ColorManager.RollBlock(faceColors, Input.GetAxisRaw("Horizontal") > 0 ? 1 : -1, 0);
                    UpdateEnum();
                    canMove = false;
                }
            }
            else if (Input.GetAxisRaw("Vertical") != 0)
            {
                if (Input.GetAxisRaw("Vertical") > 0) // Di chuyển lên trên
                {
                    newPosition = transform.position + Vector3.up;
                }
                else // Di chuyển xuống dưới
                {
                    newPosition = transform.position + Vector3.down;
                }

                if (IsPositionValid(newPosition))
                {
                    AudioManager.Instance.PlaySFX(SoundEffect.RollCube);
                    rb.MovePosition(newPosition);
                    ColorManager.RollBlock(faceColors, 0, Input.GetAxisRaw("Vertical") > 0 ? 1 : -1);
                    UpdateEnum();
                    canMove = false;
                }
            }
        }

        // Phục hồi khả năng di chuyển nếu không có phím nào được nhấn
        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
        {
            canMove = true;
        }
    }

    private bool IsPositionValid(Vector3 position)
    {
        // Kiểm tra nếu vị trí mới nằm trong giới hạn lưới và không có trong danh sách vị trí không được đi
        return position.x >= 0 && position.x < GameManager.Instance.CurrentLevel.width &&
               position.y >= 0 && position.y < GameManager.Instance.CurrentLevel.height &&
               !GameManager.Instance.ListCanNotGo.Contains(position);
    }
    private void UpdateEnum()
    {
        colorTop = faceColors[BlockFace.Top];
        colorBottom = faceColors[BlockFace.Bottom];
        colorBack = faceColors[BlockFace.Back];
        colorLeft = faceColors[BlockFace.Left];
        UpdateSprite();
    }

    private void UpdateSprite()
    {
        string colorStringTop, colorStringBack, colorStringLeft;
        colorStringTop = ColorManager.StringColor(colorTop);
        colorStringBack = ColorManager.StringColor(colorBack);
        colorStringLeft = ColorManager.StringColor(colorLeft);
        sr.sprite = ColorManager.FindSpriteWithColor(colorStringTop + "_" + colorStringBack + "_" + colorStringLeft,sprites);
    }
   
}
