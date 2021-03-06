﻿using UnityEngine;

namespace Tetris
{
    namespace Main
    {
        public class GridController : MonoBehaviour
        {
            private static Transform[,] gridDate;
            private static int width;
            private static int height;
            private static int leftBorder;
            private static int rightBorder;

            private static GridController instance;
            public static GridController Instance
            {
                get
                {
                    return instance;
                }
            }
            
            private void Awake()
            {   
                // 适用于MonoBehavior的单例模式
                instance = this;
            }

            private void Start()
            {
                width = MainSceneManager.Instance.Width;
                height = MainSceneManager.Instance.Height;
                leftBorder = MainSceneManager.Instance.LeftBorder;
                rightBorder = MainSceneManager.Instance.RightBorder;
                gridDate = new Transform[width, height];
            }

            /// <summary>
            /// 清除未移动前的数据
            /// </summary>
            /// <param name="group"></param>
            private static void ClearOldDate(Transform group)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = 0; y < height; y++)
                    {
                        if (gridDate[x, y] && gridDate[x, y].parent == group)
                        {
                            gridDate[x, y] = null;
                        }
                    }
                }
            }

            /// <summary>
            /// 更新网格数据
            /// </summary>
            /// <param name="group"></param>
            public void UpdateGrid(Transform group)
            {
                ClearOldDate(group);
                foreach (Transform child in group)
                {
                    int x = (int)Mathf.Round(child.position.x);
                    int y = (int)Mathf.Round(child.position.y);
                    gridDate[x, y] = child;
                }
            }

            /// <summary>
            /// 判断是否是有效位置
            /// </summary>
            /// <param name="group"></param>
            /// <param name="offset"></param>
            /// <returns></returns>
            public bool IsValid(Transform group)
            {
                return IsInBorder(group) && IsEffectiveGrid(group);
            }

            /// <summary>
            /// 判断所有的方格是否在边界中
            /// </summary>
            /// <param name="pos"></param>
            /// <returns></returns>
            private bool IsInBorder(Transform group)
            {
                foreach (Transform child in group)
                {
                    if (!((int)child.position.x >= leftBorder &&
                          (int)child.position.x <= rightBorder &&
                          (int)child.position.y >= 0))
                    {
                        return false;
                    }
                }
                return true;
            }

            /// <summary>
            /// 判断所有的方格是否在有效网格中
            /// </summary>
            /// <param name="group"></param>
            /// <param name="offset"></param>
            /// <returns></returns>
            private bool IsEffectiveGrid(Transform group)
            {
                foreach (Transform child in group)
                {
                    //这两句四舍五入让我纠结了半天, 不这样做的话, 会出现方块凌空的问题, 一开始我以为是数据更新的错,
                    //后来纠结了半天, 发现原来是这里的问题. 一开始我用的是int的强转, 后来改成这个才ok,
                    //估计unity内部表示坐标时, 会出现1表示为0.99999这样的情况.
                    int x = Mathf.RoundToInt(child.position.x);
                    int y = Mathf.RoundToInt(child.position.y);
                    if (!(gridDate[x, y] == null || gridDate[x, y].parent == group))
                    {
                        return false;
                    }
                }
                return true;
            }

            /// <summary>
            /// 清除已经满的行
            /// </summary>
            public void ClearFullRow()
            {
                //从顶部逆向循环检查是否有满行的存在
                for (int row = height - 1; row >= 0; row--)
                {
                    bool isFull = true;
                    for (int x = 0; x < width; x++)
                    {
                        if (gridDate[x, row] == null)
                        {
                            isFull = false;
                            break;
                        }
                    }
                    if (isFull)
                    {
                        ClearRow(row);
                        MoveRowDownOne(row);
                        MainSceneManager.Instance.AddSorce();
                    }
                }
            }

            /// <summary>
            /// 清除某一行的数据
            /// </summary>
            /// <param name="row"></param>
            private static void ClearRow(int row)
            {
                for (int x = 0; x < width; x++)
                {
                    Destroy(gridDate[x, row].gameObject);
                }
            }

            /// <summary>
            /// 将该行以上的数据都下移一格
            /// </summary>
            /// <param name="row"></param>
            private static void MoveRowDownOne(int whichRow)
            {
                for (int x = 0; x < width; x++)
                {
                    for (int y = whichRow; y < height - 1; y++)
                    {
                        //移动方格的Transform
                        if (gridDate[x, y + 1])
                        {
                            gridDate[x, y + 1].position += new Vector3(0, -1, 0);
                        }
                        //移动方格内的数据
                        gridDate[x, y] = gridDate[x, y + 1];
                    }
                }
            }


        }


    }

}

