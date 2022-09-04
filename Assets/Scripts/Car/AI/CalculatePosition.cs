using System.Collections;
using System.Linq;
using System;
using UnityEngine;

public class CalculatePosition : MonoBehaviour
{
    PositionTracker[] trackers, final;

    // Start is called before the first frame update
    void Start()
    {
        trackers = FindObjectsOfType<PositionTracker>();
        final = new PositionTracker[trackers.Length];
    }

    // Update is called once per frame
    void Update()
    {
        final = trackers.OrderBy(tracker => tracker.distance)
                .OrderByDescending(tracker => tracker.pointOnPath)
                .OrderByDescending(tracker => tracker.currentPath)
                .OrderByDescending(tracker => tracker.checkpoint)
                .OrderByDescending(tracker => tracker.lap).ToArray();

        for(int i = 0; i < final.Length; i++)
        {
            final[i].place = i + 1;
        }
    }
}
