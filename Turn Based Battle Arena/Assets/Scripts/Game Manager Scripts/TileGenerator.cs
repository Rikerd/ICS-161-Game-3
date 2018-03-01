using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileGenerator : MonoBehaviour {

    [Range(0, 100)]
    public int initChance;

    [Range(0, 8)]
    public int birthLimit;

    [Range(0, 8)]
    public int deathLimit;

    public Vector3Int tileMapSize;

    // Determine the type of tile and the characteristics
    public Tilemap map;
    public Tile tile;

    // Size of map to be used
    int width;
    int height;

    // 1 is alive, 0 is not filled
    private int[,] terrainMap;
    
    // Main method
    public void doSimulation(int numberOfIterations)
    {
        clearMaps(false);

        // Init the size of the map
        width = tileMapSize.x;
        height = tileMapSize.y;

        // If map is empty create a map
        if (terrainMap == null)
        {
            terrainMap = new int[width, height];

            initTerrainMap();
        }

        for (int i = 0; i < numberOfIterations; i++)
        {
            terrainMap = generateTilePosition(terrainMap);
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (terrainMap[x, y] == 1)  
                {
                    // To not start at 0, 0
                    map.SetTile(new Vector3Int(-x + width / 2, -y + height / 2, 0), tile);
                }
            }
        }
    }

    // Changes terrainMap accordingly with algorithm to make it more procedural
    public int[,] generateTilePosition(int[,] oldMap)
    {
        int[,] newMap = new int[width, height];
        int neighbor;
        BoundsInt neighborBoundaries = new BoundsInt(-1, -1, 0, 3, 3, 1); // Virtual rectangle of x, y around our position

        // Neightbor check (the algorithm)
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                neighbor = 0;
                
                // Loop through each neighbor around
                foreach (var n in neighborBoundaries.allPositionsWithin)
                {
                    // Our current position and continue
                    if (n.x == 0 && n.y == 0)
                    {
                        continue;
                    }

                    // Check if in bound and adds neighbor counter to current neighbor
                    if (x + n.x >= 0 && x + n.x < width && y + n.y >= 0 && y + n.y < height)
                    {
                        neighbor += oldMap[x + n.x, y + n.y];
                    } else // Border of the map in order to generate a border
                    {
                        neighbor++;
                    }
                }

                if (oldMap[x, y] == 1)
                {
                    if (neighbor < deathLimit)
                    {
                        newMap[x, y] = 0;
                    } else
                    {
                        newMap[x, y] = 1;
                    }
                }

                if (oldMap[x, y] == 0)
                {
                    if (neighbor > birthLimit)
                    {
                        newMap[x, y] = 1;
                    }
                    else
                    {
                        newMap[x, y] = 0;
                    }
                }
            }
        }

        return newMap;
    }

    // Populates terrainMap with appropriate values
    public void initTerrainMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Checks if within chances to generate certain tile and generates proper tile
                terrainMap[x, y] = Random.Range(1, 101) < initChance ? 1 : 0;
            }
        }
    }

    // Clear map for a new simulation
    public void clearMaps(bool complete)
    {
        map.ClearAllTiles();

        if (complete)
        {
            terrainMap = null;
        }
    }
}
