using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;

public class PermissionManager : MonoBehaviour
{
    private string targetPermission = Permission.FineLocation;


    private void Start()
    {
        RequestsWithCallbacks();
    }

    public void RequestsWithCallbacks()
    {
        string[] permissions = new string[]
        {
            Permission.FineLocation,
            Permission.Camera,
        };

        PermissionCallbacks callbacks = new();
        Permission.RequestUserPermissions(permissions, callbacks);

        callbacks.PermissionGranted += t =>
        {
            SceneManager.LoadScene(1);
        };
        callbacks.PermissionDenied += t =>
        {
            Application.Quit();
        };

    }

    public void RequestWithCallbacks()
    {
        PermissionCallbacks callbacks = new();
        Permission.RequestUserPermission(targetPermission, callbacks);

        callbacks.PermissionGranted += t =>
        {
            SceneManager.LoadScene(1);
        };
        callbacks.PermissionDenied += t =>
        {
            Application.Quit();
        };

    }

}
