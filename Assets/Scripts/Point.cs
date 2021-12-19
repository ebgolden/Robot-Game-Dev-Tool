using UnityEngine;

public class Point
{
    public double x;
    public double y;
    public double z;

    public Point()
    {
        x = 0;
        y = 0;
        z = 0;
    }

    public Point(double x, double y)
    {
        this.x = x;
        this.y = y;
        z = 0;
    }

    public Point(float x, float y)
    {
        this.x = x;
        this.y = y;
        z = 0;
    }

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
        z = 0;
    }

    public Point(double x, double y, double z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Point(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Point(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Point(Vector2 point)
    {
        x = point.x;
        y = point.y;
        z = 0;
    }

    public Point(Vector3 point)
    {
        x = point.x;
        y = point.y;
        z = point.z;
    }

    public double getX()
    {
        return x;
    }

    public double getY()
    {
        return y;
    }

    public double getZ()
    {
        return z;
    }

    public void setX(double x)
    {
        this.x = x;
    }

    public void setY(double y)
    {
        this.y = y;
    }

    public void setZ(double z)
    {
        this.z = z;
    }

    public void setX(float x)
    {
        this.x = x;
    }

    public void setY(float y)
    {
        this.y = y;
    }

    public void setZ(float z)
    {
        this.z = z;
    }

    public void setX(int x)
    {
        this.x = x;
    }

    public void setY(int y)
    {
        this.y = y;
    }

    public void setZ(int z)
    {
        this.z = z;
    }

    public Vector2 toVector2()
    {
        return new Vector2((float)x, (float)y);
    }

    public Vector3 toVector3()
    {
        return new Vector3((float)x, (float)y, (float)z);
    }

    public string toString()
    {
        return toVector3().ToString();
    }
}