using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshairs : MonoBehaviour
{
    public LayerMask targetMask;
    public SpriteRenderer dot;
    public Color dotHeightlightColor;
    Color originalDotColor;
    public float rotateSpeed = 40;

    private void Start()
    {
        Cursor.visible = false;
        originalDotColor = dot.color;
    }
    void Update()
    {
        transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime); 
    }

    public void DetectTargets(Ray ray)
    {
        if (Physics.Raycast(ray, 100, targetMask))
        {
            //如果射线检测到敌人，就把准星颜色改成红色
            dot.color = dotHeightlightColor;
        }
        else
        {
            //如果射线没有检测到敌人，就把准星颜色改成白色
            dot.color = originalDotColor;
        }
    }
}
