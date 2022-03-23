using UnityEngine;

[ExecuteInEditMode]
public class BezierCurveCreator : MonoBehaviour
{
    [SerializeField] private Transform startPoint, cornerPoint, finishPoint;
    [SerializeField] private GameObject pointPrefab;
    [SerializeField] private int divisionCount = 10;
    private Vector3 abPos, bcPos;
    
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 70, 150, 50), "CreateBezierPoints"))
            CreateBezierPoints(divisionCount);
    }

    // Update is called once per frame
    void CreateBezierPoints(int divisionCount)
    {
        Vector3 lerpedPos;
        Quaternion lerpedRot;
        float lerpStep;
        
        for (int i = 1; i < divisionCount+1; i++)
        {
            lerpStep = (float)i / divisionCount; 
            Debug.Log("LerpStep : "+lerpStep);
            abPos = Vector3.Lerp(startPoint.position, cornerPoint.position, lerpStep);
            bcPos = Vector3.Lerp(cornerPoint.position, finishPoint.position, lerpStep);
            lerpedPos = Vector3.Lerp(abPos, bcPos, lerpStep);

            lerpedRot = Quaternion.Lerp(startPoint.rotation,finishPoint.rotation,lerpStep);

            Instantiate(pointPrefab, lerpedPos, lerpedRot).name = "cornerPoint" + i;
        }
        
    }
}
