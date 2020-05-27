using UnityEngine;
using System.Collections.Generic;

public class SplineManager : MonoBehaviour
{
    public static SplineManager instance;
    private List<BezierSpline> splines;

    private void Awake()
    {
        // Singleton setup
        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }
        instance = this;
        splines = new List<BezierSpline>();
    }

    public static int AddSpline(BezierSpline spline)
    {
        instance.splines.Add(spline);
        return instance.splines.Count - 1;
    }

    public static BezierSpline GetSpline(int index)
    {
        // C# Assert? Just explode if someone does something stupid...
        return instance.splines[index];
    }
}
