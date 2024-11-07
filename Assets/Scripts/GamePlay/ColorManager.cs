using System;
using System.Collections.Generic;
using UnityEngine;

public enum ColorType
{
    White,
    Green,
    Blue,
    Red,
    Yellow,
    Orange,
    None
}
public enum BlockFace
{
    Top,    
    Bottom, 
    Left,  
    Right,  
    Front,  
    Back    
}
public static class ColorManager
{
    // Hàm để cập nhật màu sắc khi lăn theo trục X hoặc Y
    public static void RollBlock(Dictionary<BlockFace,ColorType> faceColors,int x, int y)
    {
        // Tạm thời lưu trữ màu hiện tại trước khi lăn
        ColorType topColor = faceColors[BlockFace.Top];
        ColorType bottomColor = faceColors[BlockFace.Bottom];
        ColorType frontColor = faceColors[BlockFace.Front];
        ColorType backColor = faceColors[BlockFace.Back];
        ColorType leftColor = faceColors[BlockFace.Left];
        ColorType rightColor = faceColors[BlockFace.Right];

        // Nếu di chuyển theo trục X (lăn về trước hoặc sau)
        if (x > 0) // Lăn về phía trước (dọc theo trục X)
        {
            faceColors[BlockFace.Top] = backColor;
            faceColors[BlockFace.Bottom] = frontColor;
            faceColors[BlockFace.Front] = topColor;
            faceColors[BlockFace.Back] = bottomColor;
        }
        else if(x < 0) // Lăn về phía sau (dọc theo trục X)
        {
            faceColors[BlockFace.Top] = frontColor;
            faceColors[BlockFace.Bottom] = backColor;
            faceColors[BlockFace.Front] = bottomColor;
            faceColors[BlockFace.Back] = topColor;
        }

        // Nếu di chuyển theo trục Y (lăn sang trái hoặc phải)
        if (y > 0) // Lăn sang trái (dọc theo trục Y)
        {
            faceColors[BlockFace.Top] = leftColor;
            faceColors[BlockFace.Bottom] = rightColor;
            faceColors[BlockFace.Left] = bottomColor;
            faceColors[BlockFace.Right] = topColor;
        }
        else if (y < 0) // Lăn sang phải (dọc theo trục Y)
        {
            faceColors[BlockFace.Top] = rightColor;
            faceColors[BlockFace.Bottom] = leftColor;
            faceColors[BlockFace.Left] = topColor;
            faceColors[BlockFace.Right] = bottomColor;
        }
       
    }
    public static string StringColor(ColorType color)
    {
        return Enum.GetName(color.GetType(), color);
    }

    public static Sprite FindSpriteWithColor(String nameColor,List<Sprite> sprites)
    {
        Sprite newSprite = null;
        foreach (var sprite in sprites)
        {
            if (sprite.name.Equals(nameColor))
            {
                newSprite = sprite;
            }
        }
        return newSprite;
    }
}


