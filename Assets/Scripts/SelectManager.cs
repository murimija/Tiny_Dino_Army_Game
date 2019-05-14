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
                if (selectedUnits[i]!=null)
                {
                    selectedUnits[i].GetComponent<DinoController>().GoToNewPlace(hit.point);
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
            if (selectedUnits[i]!=null)
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
}