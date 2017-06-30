﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupController : MonoBehaviour {

    private float lastFallTime;
    private bool canMove = true;
    
    void Start()
    {
        if (MakeEffectiveMove(Vector3.zero)) // 生成后立即更新网格数据, 0移动表示使用当时位置的数据
        {
            canMove = true;
        }
        else
        {
            canMove = false;
            GameController.GameOver();      // 刚生成就不能移动, 即判定为gameover
        }
    }

    void Update ()
    {
        if (canMove)
        {
            InputControl();
            AutoFall();
        }
        else
        {
            if(GameController.gamestate == GameState.running)
            {
                GridController.UpdateGrid(transform);
                GridController.ClearFullRow();
                GameController.SpawnNext();
            }
            enabled = false;
        }
	}

    /// <summary>
    /// 做有效的移动
    /// </summary>
    /// <param name="move"></param>
    /// <returns></returns>
    bool MakeEffectiveMove(Vector3 move)
    {
        Vector3 nowPos = transform.position;
        transform.position += move;
        if (GridController.IsValid(transform))
        {
            GridController.UpdateGrid(transform);
            return true;
        }
        else
        {
            transform.position = nowPos;
            return false;
        }
    }

    /// <summary>
    /// 做有效的旋转
    /// </summary>
    /// <param name="rotation"></param>
    /// <returns></returns>
    bool MakeEffectiveRotate(Vector3 rotation)
    {
        transform.Rotate(rotation);
        if(GridController.IsValid(transform))
        {
            GridController.UpdateGrid(transform);
            return true;
        }
        else
        {
            transform.Rotate(-rotation);
            return false;
        }
    }

    /// <summary>
    /// 玩家控制方块的方向和变形
    /// </summary>
    void InputControl()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MakeEffectiveMove(new Vector3(-1, 0, 0));
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MakeEffectiveMove(new Vector3(1, 0, 0));
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MakeEffectiveMove(new Vector3(0, -1, 0));
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // TO不允许旋转
            if(!gameObject.tag.Equals("TO"))
            {
                MakeEffectiveRotate(new Vector3(0, 0, -90));
            }
        }
    }

    /// <summary>
    /// 控制方块的自由掉落
    /// </summary>
    void AutoFall()
    {
        int time = (int)(Time.time - lastFallTime);
        if (time>=1)
        {
            if(MakeEffectiveMove(new Vector3(0, -1, 0)))
            {
                lastFallTime = Time.time;
            }
            else
            {
                canMove = false;
            }
        }
    }

}
