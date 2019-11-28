using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPath
{
    public List<MapNode> nodes;
    public float pathLength;
    public float additionalCost;
    public float crowFliesDistance;
    public float diagonalness1, diagonalness2;
    public MapPath(List<MapNode> nodes, float pathLength, float crowFliesDistance, float additionalCost, float diagonalness1, float diagonalness2)
    {
        this.nodes = nodes;
        this.pathLength = pathLength;
        this.crowFliesDistance = crowFliesDistance;
        this.additionalCost = additionalCost;
        this.diagonalness1 = diagonalness1;
        this.diagonalness2 = diagonalness2;
    }

    public float Cost => pathLength + crowFliesDistance + additionalCost + diagonalness1 + diagonalness2;

    
}