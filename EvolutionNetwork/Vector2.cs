using System;
using System.Collections.Generic;
using System.Text;


public struct Vector2
{
    public float x;
    public float y;

    public Vector2(float x, float y)
    {
        this.x = x;
        this.y = y;
    }

    public static Vector2 operator -(Vector2 a, Vector2 b)
    {
        return new Vector2( a.x - b.x, a.y - b.y);
    }

    public static Vector2 operator +(Vector2 a, Vector2 b)
    {
        return new Vector2(b.x + a.x, b.y + a.y);
    }

    public static Vector2 operator *(Vector2 a, float con)
    {
        return new Vector2(a.x * con, a.y * con);
    }
    public static Vector2 operator *(float con, Vector2 a)
    {
        return new Vector2(a.x * con, a.y * con);
    }

}
