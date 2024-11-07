using UnityEngine;

public class Tele : MonoBehaviour
{
    private Vector2 endPoint;

    public Vector2 EndPoint
    {
        set
        {
            this.endPoint = value;
        }
        get { return endPoint; }
    }
}
