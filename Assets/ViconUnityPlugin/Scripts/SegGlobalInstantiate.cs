using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViconDataStreamSDK.CSharp;
using System.Linq;

[System.Serializable]
public class OffsetSegments
{
    public string segToOffset;
    [Tooltip("Leave blank to use an empty object.")]
    public GameObject OffsetPrefab;
    public bool useMiddleOfSegmentMarkers;
    [Tooltip("Use the segment's labelled markers and this will create an offset in the middle of them")]
    public string[] segmentMarkerNames;
    public Vector3 offsetPosition;
}

public class SegGlobalInstantiate : MonoBehaviour
{
    public string[] subjects;
    
    public OffsetSegments[]offsetSegs;
    string currentSegName; 
    public NewModdedDataStreamClient Client;
    [Tooltip("Leave blank to use an empty object.")]
    public GameObject segPrefab;
    public uint maximumSegs = 30;
    private uint segmentCount;
    bool offsetsComplete;
    int instantiatedCount;
    bool allInstantiated;
    int totalSegs;
    public int offsetAttemptsAllowed = 100;
    int offsetAttempts;
    List<Transform> OffsetObjects = new List<Transform>();
    List<Vector3> OffsetVectors = new List<Vector3>();
    
   List<List<GameObject>> segPrefabs = new List<List<GameObject>>();
    public Transform xrrig;
    Vector3 xrRigOffset;

    void CreateOffsets()
    {
        foreach (OffsetSegments offsetsegment in offsetSegs)
        {
            if (offsetsegment.OffsetPrefab == null)
            {
                offsetsegment.OffsetPrefab = new GameObject("EmptyOffsetPrefab");
            }
            if (offsetsegment.useMiddleOfSegmentMarkers)
            {
                Vector3 averagePos = new Vector3();
                foreach(string s in offsetsegment.segmentMarkerNames)
                {
                    GameObject segmentMarker = GameObject.Find(s);
                    if (segmentMarker == null)
                    {
                        Debug.Log(s + "notFound");
                        offsetAttempts++;
                        return;
                    }
                    else 
                    {
                        if (segmentMarker.transform.position.x ==0)
                        {
                            Debug.Log(segmentMarker.name + " not tracking ");
                            return;
                        }
                        averagePos += segmentMarker.transform.position;
                       Debug.Log(segmentMarker.name + " " + averagePos.ToString("F4"));
                    }
                }
                averagePos = averagePos / (float)offsetsegment.segmentMarkerNames.Count();
                Transform originalSeg = this.transform.Find(offsetsegment.segToOffset);
                if (originalSeg == null || originalSeg.position.x == 0) 
                {
                    offsetAttempts++;
                    Debug.Log("Couldn't find original seg to offset");
                    return;
                }
                else
                {
                    
                    GameObject OffsetChild = Instantiate(offsetsegment.OffsetPrefab, averagePos, originalSeg.rotation);
                    
                    Vector3 offset = averagePos + originalSeg.position;
                    OffsetVectors.Add(offset);
                    OffsetObjects.Add(originalSeg);
                    OffsetChild.transform.parent = originalSeg;
                    
                    OffsetChild.name = originalSeg.name + "Offset";
                }
                

            }
            else
            {
                Transform originalSeg = this.transform.Find(offsetsegment.segToOffset);
                GameObject OffsetChild = Instantiate(offsetsegment.OffsetPrefab, offsetsegment.offsetPosition, Quaternion.identity, originalSeg);

            }

        }
        Debug.Log("OffsetsComplete!");
        offsetsComplete = true;

    }
    void Start()
    {
        if (xrrig != null)
        {
            xrRigOffset = xrrig.position;
        }
        else
        {
            Debug.Log("XRRIg notAdded");
        }
        if (segPrefab == null)
        {
            segPrefab = new GameObject("emptySegPrefab");
        }
        
        
    
          
        



        foreach (string s in subjects)
        {
            segPrefabs.Add(new List<GameObject>());
        }
    }
    void DestroyMissingSegs(int index)
    {
        if (segPrefabs.Count > segmentCount)
        {
            for (int i = (int)segmentCount; i < segPrefabs[index].Count; i++)
            {
                Destroy(segPrefabs[index][i]);
               // segPrefabs[index].Remove(segPrefabs[i]);
            }
        }
    }
    void InstantiateNewSegment(Vector3 pos, GameObject prefab, int index)
    {
       
        GameObject segment = Instantiate(prefab, new Vector3 (0,0,0), Quaternion.identity, transform);
        segment.name = currentSegName;
        segPrefabs[index].Add(segment);
        //Debug.Log("Added" + segment.name);
        instantiatedCount++;
       
    }
    Quaternion ConvertToUnity(Quaternion input)  //https://gamedev.stackexchange.com/questions/157946/converting-a-quaternion-in-a-right-to-left-handed-coordinate-system
    {
        return new Quaternion(
             input.y,   // -(  right = -left  )
            -input.z,   // -(     up =  up     )
            -input.x,   // -(forward =  forward)
             input.w
        );
    }

    public void GetFullSegmentCount()
    {
        
        for (int currentSubject = 0; currentSubject < subjects.Count(); currentSubject++)
        {
            Output_GetSegmentCount GetSegmentCount = Client.GetSegmentCount(subjects[currentSubject]);
            totalSegs += (int)GetSegmentCount.SegmentCount;
        }
            
    }
    void Update()
    {
        if (offsetSegs.Length > 0 && !offsetsComplete && offsetAttempts < offsetAttemptsAllowed)
        {
             CreateOffsets();
        }
        if (totalSegs == 0)
        {
           // GetFullSegmentCount();
        }
        for (int currentSubject = 0; currentSubject < subjects.Count(); currentSubject++)
        {
         

            Output_GetSegmentCount GetSegmentCount = Client.GetSegmentCount(subjects[currentSubject]);
            //  DestroyMissingSegs();
            

            if (GetSegmentCount.SegmentCount > 0)  // Test for success with GetSegmentCount.Result == Result.Success but don't think needed
            {
                segmentCount = GetSegmentCount.SegmentCount;
              
                for (int i = 0; i < (int)segmentCount; i++)
                {
                    Output_GetSegmentName OutputGSN = Client.GetSegmentName(subjects[currentSubject], (uint)i);
                    currentSegName = OutputGSN.SegmentName.ToString();
                   // Debug.Log(currentSegName);
                   
                    Output_GetSegmentGlobalTranslation Output = Client.GetSegmentGlobalTranslation(subjects[currentSubject], OutputGSN.SegmentName);

                    Output_GetSegmentGlobalRotationQuaternion rot = Client.Output_GetSegmentGlobalRotationQuaternion(subjects[currentSubject], OutputGSN.SegmentName);


                    Vector3 pos = new Vector3(-(float)Output.Translation[1] * 0.001f, (float)Output.Translation[2] * 0.001f, (float)Output.Translation[0] * 0.001f); //ADDED XR RIG OFFSET to make vicon objects follow the xr rig calibration when it doesn't start at 0,0,0

                    Quaternion TheRot = new Quaternion((float)rot.Rotation[0], (float)rot.Rotation[1], (float)rot.Rotation[2], (float)rot.Rotation[3]);




                    if (segPrefabs[currentSubject].Count < segmentCount)
                    {

                        InstantiateNewSegment(pos, segPrefab, currentSubject);
                       

                    }
                    else
                    {
                        segPrefabs[currentSubject][i].transform.position = pos;

                        segPrefabs[currentSubject][i].transform.rotation = ConvertToUnity(TheRot);


                       
                    }

                }
            }
           

        }
    }
}
