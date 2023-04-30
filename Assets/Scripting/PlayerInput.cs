using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [Header("Keys")]
    [SerializeField] KeyCode[] leftMoveKeys = null;
    [SerializeField] KeyCode[] rightMoveKeys = null;
    [SerializeField] KeyCode[] upMoveKeys = null;
    [SerializeField] KeyCode[] downMoveKeys = null;

    [SerializeField] KeyCode[] brakeKeys = null;

    public static PlayerInput Instance;

    public static KeyCode[] LeftMoveKeys { get { return Instance.leftMoveKeys; } }
    public static KeyCode[] RightMoveKeys { get { return Instance.rightMoveKeys; } }
    public static KeyCode[] UpMoveKeys { get { return Instance.upMoveKeys; } }
    public static KeyCode[] DownMoveKeys { get { return Instance.downMoveKeys; } }
    public static KeyCode[] BrakeKeys { get { return Instance.brakeKeys; } }

    private void OnEnable()
    {
        if (Instance != null && Instance != this)
        {
            gameObject.SetActive(false);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public static bool IsKeyArrayDown(KeyCode[] key, Action OnKeyDown = null)
    {
        for (int i = 0; i < key.Length; i++)
        {
            if (Input.GetKey(key[i]))
            {
                OnKeyDown?.Invoke();
                return true;
            }
        }

        return false;
    }
}
