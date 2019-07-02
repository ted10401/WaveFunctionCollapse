using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class EvenSimplerTiledModel : MonoBehaviour
{
    public enum TileType
    {
        Red,
        Green,
        Blue,
    }

    public enum DirectionType
    {
        Up,
        Down,
        Left,
        Right
    }

    [Serializable]
    public class TiledModelRule
    {
        public TileType tileTypeA;
        public DirectionType directionType;
        public TileType tileTypeB;
    }

    public class TileState
    {
        public int x;
        public int y;
        public TileType tileType;
        public bool observed = false;

        public TileState(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public GameObject[] tiles;
    public Vector2Int mapSize = new Vector2Int(10, 10);
    public TiledModelRule[] tiledModelRules;
    private TileState[,] m_tileStates;
    private List<TileState> m_checkingTileStates = new List<TileState>();
    private List<GameObject> m_tiles = new List<GameObject>();

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Generate();
        }
    }

    private void Generate()
    {
        m_tileStates = new TileState[mapSize.x, mapSize.y];
        for(int x = 0; x < mapSize.x; x++)
        {
            for(int y = 0; y < mapSize.y; y++)
            {
                m_tileStates[x, y] = new TileState(x, y);
            }
        }

        m_checkingTileStates.Clear();
        m_checkingTileStates.Add(m_tileStates[Random.Range(0, mapSize.x), Random.Range(0, mapSize.y)]);

        int random;
        TileState tileState;
        List<TileState> unobservedTileStates;

        while(m_checkingTileStates.Count != 0)
        {
            random = Random.Range(0, m_checkingTileStates.Count);
            tileState = m_checkingTileStates[random];
            GetValidTileType(tileState);

            if(tileState.observed)
            {
                m_checkingTileStates.Remove(tileState);

                unobservedTileStates = GetUnobservedTileStates(tileState);
                if (unobservedTileStates.Count > 0)
                {
                    for (int i = 0; i < unobservedTileStates.Count; i++)
                    {
                        if (m_checkingTileStates.Contains(unobservedTileStates[i]))
                        {
                            continue;
                        }

                        m_checkingTileStates.Add(unobservedTileStates[i]);
                    }
                }
            }
            else
            {
                break;
            }
        }

        GenerateTiles();
    }

    private void GenerateTiles()
    {
        for(int i = 0; i < m_tiles.Count; i++)
        {
            GameObject.Destroy(m_tiles[i]);
        }
        m_tiles.Clear();

        GameObject cacheGameObject;
        for (int x = 0; x < mapSize.x; x++)
        {
            for(int y = 0; y < mapSize.y; y++)
            {
                cacheGameObject = GameObject.Instantiate(tiles[(int)m_tileStates[x, y].tileType]);
                cacheGameObject.transform.position = new Vector3(x - mapSize.x * 0.5f + 0.5f, y - mapSize.y * 0.5f + 0.5f, 0);
                m_tiles.Add(cacheGameObject);
            }
        }
    }

    private void GetValidTileType(TileState tileState)
    {
        List<TileType> validTileTypes = new List<TileType>();
        validTileTypes.Add(TileType.Red);
        validTileTypes.Add(TileType.Green);
        validTileTypes.Add(TileType.Blue);

        List<TileType> invalidTileTypes = new List<TileType>();

        if (tileState.y < mapSize.y - 1 && validTileTypes.Count != 0)
        {
            if(m_tileStates[tileState.x, tileState.y + 1].observed)
            {
                invalidTileTypes.Clear();
                invalidTileTypes.Add(TileType.Red);
                invalidTileTypes.Add(TileType.Green);
                invalidTileTypes.Add(TileType.Blue);

                for (int i = 0; i < tiledModelRules.Length; i++)
                {
                    if (tiledModelRules[i].tileTypeA == m_tileStates[tileState.x, tileState.y + 1].tileType &&
                    tiledModelRules[i].directionType == DirectionType.Down)
                    {
                        invalidTileTypes.Remove(tiledModelRules[i].tileTypeB);
                    }
                }

                if(invalidTileTypes.Count > 0)
                {
                    for(int i = 0; i < invalidTileTypes.Count; i++)
                    {
                        validTileTypes.Remove(invalidTileTypes[i]);
                    }
                }
            }
        }

        if (tileState.y > 0 && validTileTypes.Count != 0)
        {
            if (m_tileStates[tileState.x, tileState.y - 1].observed)
            {
                invalidTileTypes.Clear();
                invalidTileTypes.Add(TileType.Red);
                invalidTileTypes.Add(TileType.Green);
                invalidTileTypes.Add(TileType.Blue);

                for (int i = 0; i < tiledModelRules.Length; i++)
                {
                    if (tiledModelRules[i].tileTypeA == m_tileStates[tileState.x, tileState.y - 1].tileType &&
                    tiledModelRules[i].directionType == DirectionType.Up)
                    {
                        invalidTileTypes.Remove(tiledModelRules[i].tileTypeB);
                    }
                }

                if (invalidTileTypes.Count > 0)
                {
                    for (int i = 0; i < invalidTileTypes.Count; i++)
                    {
                        validTileTypes.Remove(invalidTileTypes[i]);
                    }
                }
            }
        }

        if (tileState.x > 0 && validTileTypes.Count != 0)
        {
            if (m_tileStates[tileState.x - 1, tileState.y].observed)
            {
                invalidTileTypes.Clear();
                invalidTileTypes.Add(TileType.Red);
                invalidTileTypes.Add(TileType.Green);
                invalidTileTypes.Add(TileType.Blue);

                for (int i = 0; i < tiledModelRules.Length; i++)
                {
                    if (tiledModelRules[i].tileTypeA == m_tileStates[tileState.x - 1, tileState.y].tileType &&
                    tiledModelRules[i].directionType == DirectionType.Right)
                    {
                        invalidTileTypes.Remove(tiledModelRules[i].tileTypeB);
                    }
                }

                if (invalidTileTypes.Count > 0)
                {
                    for (int i = 0; i < invalidTileTypes.Count; i++)
                    {
                        validTileTypes.Remove(invalidTileTypes[i]);
                    }
                }
            }
        }

        if (tileState.x < mapSize.x - 1 && validTileTypes.Count != 0)
        {
            if (m_tileStates[tileState.x + 1, tileState.y].observed)
            {
                invalidTileTypes.Clear();
                invalidTileTypes.Add(TileType.Red);
                invalidTileTypes.Add(TileType.Green);
                invalidTileTypes.Add(TileType.Blue);

                for (int i = 0; i < tiledModelRules.Length; i++)
                {
                    if (tiledModelRules[i].tileTypeA == m_tileStates[tileState.x + 1, tileState.y].tileType &&
                    tiledModelRules[i].directionType == DirectionType.Left)
                    {
                        invalidTileTypes.Remove(tiledModelRules[i].tileTypeB);
                    }
                }

                if (invalidTileTypes.Count > 0)
                {
                    for (int i = 0; i < invalidTileTypes.Count; i++)
                    {
                        validTileTypes.Remove(invalidTileTypes[i]);
                    }
                }
            }
        }

        if(validTileTypes.Count > 0)
        {
            tileState.observed = true;
            tileState.tileType = validTileTypes[Random.Range(0, validTileTypes.Count)];
        }
    }

    private List<TileState> GetUnobservedTileStates(TileState tileState)
    {
        List<TileState> unobservedTileStates = new List<TileState>();
        TileState cacheTileState;

        if (tileState.y < mapSize.y - 1)
        {
            cacheTileState = m_tileStates[tileState.x, tileState.y + 1];

            if (!cacheTileState.observed)
            {
                unobservedTileStates.Add(cacheTileState);
            }
        }

        if (tileState.y > 0)
        {
            cacheTileState = m_tileStates[tileState.x, tileState.y - 1];

            if (!cacheTileState.observed)
            {
                unobservedTileStates.Add(cacheTileState);
            }
        }

        if (tileState.x > 0)
        {
            cacheTileState = m_tileStates[tileState.x - 1, tileState.y];

            if (!cacheTileState.observed)
            {
                unobservedTileStates.Add(cacheTileState);
            }
        }

        if (tileState.x < mapSize.x - 1)
        {
            cacheTileState = m_tileStates[tileState.x + 1, tileState.y];

            if (!cacheTileState.observed)
            {
                unobservedTileStates.Add(cacheTileState);
            }
        }

        return unobservedTileStates;
    }
}
