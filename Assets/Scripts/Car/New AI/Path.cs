using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path : MonoBehaviour
{
    public Color color;
    [Range(0, 10)]public float cubeSize;

    private List<Transform> nodes = new();

    private void OnDrawGizmos()
    {
        Gizmos.color = color;

        Transform[] pathTransforms = GetComponentsInChildren<Transform>();
        nodes = new();

        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        }

        if (nodes.Count > 1)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                Vector3 currentNode = nodes[i].position;
                Vector3 previousNode;
                if (i == 0)
                {
                    previousNode = nodes[nodes.Count - 1].position;
                }
                else
                {
                    previousNode = nodes[i - 1].position;
                }

                Gizmos.DrawLine(currentNode, previousNode);
                Gizmos.DrawWireCube(currentNode, new Vector3(cubeSize, cubeSize, cubeSize));
            }
        }
    }
}
