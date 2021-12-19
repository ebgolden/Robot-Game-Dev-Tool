using UnityEngine;

public class Dimension
{
    public double width;
    public double height;
    public double depth;

    public Dimension()
    {
        width = 0;
        height = 0;
        depth = 0;
    }

    public Dimension(double width, double height)
    {
        this.width = width;
        this.height = height;
        depth = 0;
    }

    public Dimension(float width, float height)
    {
        this.width = width;
        this.height = height;
        depth = 0;
    }

    public Dimension(int width, int height)
    {
        this.width = width;
        this.height = height;
        depth = 0;
    }

    public Dimension(double width, double height, double depth)
    {
        this.width = width;
        this.height = height;
        this.depth = depth;
    }

    public Dimension(float width, float height, float depth)
    {
        this.width = width;
        this.height = height;
        this.depth = depth;
    }

    public Dimension(int width, int height, int depth)
    {
        this.width = width;
        this.height = height;
        this.depth = depth;
    }

    public Dimension(Vector2 dimension)
    {
        width = dimension.x;
        height = dimension.y;
        depth = 0;
    }

    public Dimension(Vector3 dimension)
    {
        width = dimension.x;
        height = dimension.y;
        depth = dimension.z;
    }

    public double getWidth()
    {
        return width;
    }

    public double getHeight()
    {
        return height;
    }

    public double getDepth()
    {
        return depth;
    }

    public void setWidth(double width)
    {
        this.width = width;
    }

    public void setHeight(double height)
    {
        this.height = height;
    }

    public void setDepth(double depth)
    {
        this.depth = depth;
    }

    public void setWidth(float width)
    {
        this.width = width;
    }

    public void setHeight(float height)
    {
        this.height = height;
    }

    public void setDepth(float depth)
    {
        this.depth = depth;
    }

    public void setWidth(int width)
    {
        this.width = width;
    }

    public void setHeight(int height)
    {
        this.height = height;
    }

    public void setDepth(int depth)
    {
        this.depth = depth;
    }

    public Vector2 toVector2()
    {
        return new Vector2((float)width, (float)height);
    }

    public Vector3 toVector3()
    {
        return new Vector3((float)width, (float)height, (float)depth);
    }

    public string toString()
    {
        return toVector3().ToString();
    }
}