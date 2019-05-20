using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SelectManager : MonoBehaviour
{
    RaycastHit hit;
    public List<GameObject> selectedUnits = new List<GameObject>();
    private bool isDragging = false;
    private Vector3 mousePosition;

    [SerializeField] private GameObject plaseToGoPref;

    private void OnGUI()
    {
        if (isDragging)
        {
            var rect = ScreenHelper.GetScreenRect(mousePosition, Input.mousePosition);
            ScreenHelper.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.1f));
            ScreenHelper.DrawScreenRectBorder(rect, 4, Color.blue);
        }
    }

    private void Start()
    {
        CreatePointToGoMatrix();
    }

    // Update is called once per frame
    void Update()
    {
        //Detect if mouse is down
        if (Input.GetMouseButtonDown(0))
        {
            mousePosition = Input.mousePosition;
            //Create a ray from the camera to our space
            var camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Shoot that ray and get the hit data
            if (Physics.Raycast(camRay, out hit))
            {
                //Do something with that data 
                //Debug.Log(hit.transform.tag);
                if (hit.transform.CompareTag("PlayersDino"))
                {
                    SelectUnit(hit.transform, Input.GetKey(KeyCode.LeftShift));
                }
                else if (hit.transform.CompareTag("Enemy"))
                {
                    return;
                }
                else
                {
                    isDragging = true;
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
            {
                DeselectUnits();
                var type = FindObjectsOfType<DinoController>();
                for (var i = 0; i < type.Length; i++)
                {
                    var selectableObject = type[i];
                    if (IsWithinSelectionBounds(selectableObject.transform))
                    {
                        SelectUnit(selectableObject.transform, true);
                    }
                }

                isDragging = false;
            }
        }

        if (Input.GetMouseButtonUp(1) && selectedUnits.Count != 0)
        {
            Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit);
            for (var i = 0; i < selectedUnits.Count; i++)
            {
                if (selectedUnits[i] != null)
                {
                    selectedUnits[i].GetComponent<DinoController>().GoToNewPlace(hit.point + pointToGoMatrix[i]);
                }
            }

            Destroy(Instantiate(plaseToGoPref, hit.point, Quaternion.identity), 1f);
        }
    }


    private void SelectUnit(Component unit, bool isMultiSelect = false)
    {
        if (!isMultiSelect)
        {
            DeselectUnits();
        }

        if (unit != null)
        {
            selectedUnits.Add(unit.gameObject);
            unit.GetComponent<DinoController>().SetHighlightState(true);
        }
    }

    private void DeselectUnits()
    {
        for (int i = 0; i < selectedUnits.Count; i++)
        {
            if (selectedUnits[i] != null)
            {
                selectedUnits[i].transform.GetComponent<DinoController>().SetHighlightState(false);
            }
        }

        selectedUnits.Clear();
    }

    private bool IsWithinSelectionBounds(Transform transform)
    {
        if (!isDragging)
        {
            return false;
        }

        var camera = Camera.main;
        var viewportBounds = ScreenHelper.GetViewportBounds(camera, mousePosition, Input.mousePosition);
        return viewportBounds.Contains(camera.WorldToViewportPoint(transform.position));
    }

    #region "Creation of Point To Go Grid"

    [SerializeField] private GameObject pointToGoPref;

    private Vector3[] pointToGoMatrix = new Vector3[61];

    private readonly float numOfRings = 2;
    private readonly float diamOfHex = 0.2f;

    private int pointCounter;

    private void CreatePointToGoMatrix()
    {
        pointCounter = 0;
        pointToGoMatrix[pointCounter] = Vector3.zero;

        var north = new Vector3(0, 0, 1);
        var northEast = new Vector3(Mathf.Cos(Mathf.PI / 6) * 1, 0, Mathf.Sin(Mathf.PI / 6) * 1);
        var southEast = new Vector3(Mathf.Cos(Mathf.PI / 6) * 1, 0, -Mathf.Sin(Mathf.PI / 6) * 1);

        for (var i = 1; i <= numOfRings; i++)
        {
            buildRayOfHex(north, southEast, i);
            buildRayOfHex(northEast, -north, i);
            buildRayOfHex(southEast, -northEast, i);
            buildRayOfHex(-north, -southEast, i);
            buildRayOfHex(-northEast, north, i);
            buildRayOfHex(-southEast, northEast, i);
        }
    }

    private void buildRayOfHex(Vector3 startDirection, Vector3 direction, int numOfCurrentRing)
    {
        var spawnPosition = startDirection * diamOfHex * numOfCurrentRing;
        for (int i = 1; i <= numOfCurrentRing; i++)
        {
            pointCounter++;
            pointToGoMatrix[pointCounter] = spawnPosition;
            spawnPosition += direction * diamOfHex;
        }
    }

    #endregion
}