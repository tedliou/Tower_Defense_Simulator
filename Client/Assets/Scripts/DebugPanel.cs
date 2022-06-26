using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum MouseEdgeState
{
    Unknown,
    Left, Right, Top, Down,
    TopLeft, TopRight,
    DownLeft, DownRight
}

public class DebugPanel : MonoBehaviour
{
    public Transform screenSize;
    public Transform mousePos;
    public Transform mouseEdge;

    public new Transform camera;
    public Vector3 cameraLimit;
    [Range(0, 1)] public float cameraMoveSpeed;

    private TMP_Text _screenSizeVal;
    private TMP_Text _mousePosVal;
    private TMP_Text _mouseEdgeVal;

    private readonly string _value = "Value";

    private void Start()
    {
        _screenSizeVal = GetText(screenSize);
        _mousePosVal = GetText(mousePos);
        _mouseEdgeVal = GetText(mouseEdge);
    }

    private void Update()
    {
        UpdateUserCamera();    
    }

    private void LateUpdate()
    {
        var mouseX = Mathf.RoundToInt(Input.mousePosition.x);
        var mouseY = Mathf.RoundToInt(Input.mousePosition.y);
        _screenSizeVal.text = $"({Screen.width}, {Screen.height})";
        _mousePosVal.text = $"({mouseX}, {mouseY})";
        _mouseEdgeVal.text = GetMouseEdgeState().ToString();
    }

    private TMP_Text GetText(Transform source)
    {
        return source.Find(_value).GetComponent<TMP_Text>();
    }

    private MouseEdgeState GetMouseEdgeState()
    {
        var mouseX = Mathf.RoundToInt(Input.mousePosition.x);
        var mouseY = Mathf.RoundToInt(Input.mousePosition.y);
        if (mouseX >= Screen.width)
        {
            if (mouseY >= Screen.height)
            {
                return MouseEdgeState.TopRight;
            }
            else if (mouseY <= 0)
            {
                return MouseEdgeState.DownRight;
            }
            else
            {
                return MouseEdgeState.Right;
            }
        }
        else if (mouseX <= 0)
        {
            if (mouseY >= Screen.height)
            {
                return MouseEdgeState.TopLeft;
            }
            else if (mouseY <= 0)
            {
                return MouseEdgeState.DownLeft;
            }
            else
            {
                return MouseEdgeState.Left;
            }
        }
        else
        {
            if (mouseY >= Screen.height)
            {
                return MouseEdgeState.Top;
            }
            else if (mouseY <= 0)
            {
                return MouseEdgeState.Down;
            }
            else
            {
                return MouseEdgeState.Unknown;
            }
        }
    }

    private void UpdateUserCamera()
    {
        var mode = GetMouseEdgeState();
        if (mode == MouseEdgeState.Unknown) return;
        var direction = Vector3.zero;

        if (mode == MouseEdgeState.Top)
        {
            direction += Vector3.forward;
        }
        else if (mode == MouseEdgeState.TopRight)
        {
            direction += Vector3.forward;
            direction += Vector3.right;
        }
        else if (mode == MouseEdgeState.TopLeft)
        {
            direction += Vector3.forward;
            direction += Vector3.left;
        }
        else if (mode == MouseEdgeState.Down)
        {
            direction += Vector3.back;
        }
        else if (mode == MouseEdgeState.DownRight)
        {
            direction += Vector3.back;
            direction += Vector3.right;
        }
        else if (mode == MouseEdgeState.DownLeft)
        {
            direction += Vector3.back;
            direction += Vector3.left;
        }
        else if (mode == MouseEdgeState.Right)
        {
            direction += Vector3.right;
        }
        else if (mode == MouseEdgeState.Left)
        {
            direction += Vector3.left;
        }
        camera.transform.position += Vector3.ClampMagnitude(direction, 1) * cameraMoveSpeed;
        var cameraPos = camera.transform.position;
        camera.transform.position = new Vector3(Mathf.Clamp(cameraPos.x, -cameraLimit.x, cameraLimit.x), cameraPos.y, Mathf.Clamp(cameraPos.z, -cameraLimit.z, cameraLimit.z));
    }
}
