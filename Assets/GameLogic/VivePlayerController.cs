using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using UnityEngine;

public class VivePlayerController : PlayerController
{
    public SteamVR_ControllerManager ControllerManager;

    private SteamVR_Controller.Device _leftController;
    private SteamVR_Controller.Device _rightController;

    public GameObject PreviewObject;

    public VivePlayerController()
    {
    }

    public override void NotifyMove(Move move, int moveNumber)
    {
    }

    public void Start()
    {
    }

    private void FireLaser(GameObject controller, SteamVR_Controller.Device device)
    {
        print("firin mah lazar");

        RaycastHit hit;

        var ray = new Ray(controller.transform.position, controller.transform.forward);
        UnityEngine.Debug.DrawRay(ray.origin, ray.direction);

        if (Physics.Raycast(ray, out hit))
        {
            var board = hit.transform.GetComponent<GoBoard>();

            if (board != null)
            {
                GoBoard.Coord point = board.TranslateToBoardCoord(hit.point);
                print(point.x + " - " + point.y);

                PreviewObject.transform.localPosition = board.GetPosition(point.x, point.y);

                if (device.GetPress(Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger) && GameController.CurrentPlayer == ControlledPlayer)
                {
                    GameController.PlayMove(new Move() { player = ControlledPlayer, x = point.x, y = point.y });
                }
            }
        }
    }

    public void Update()
    {
        if (ControllerManager == null)
            return;

        if (_leftController == null)
        {
            if (ControllerManager.left != null)
            {
                var leftIndex = ControllerManager.left.GetComponent<SteamVR_TrackedObject>().index;
                if (leftIndex != SteamVR_TrackedObject.EIndex.None)
                    _leftController = SteamVR_Controller.Input((int)leftIndex);
            }
        }
        else
        {
            FireLaser(ControllerManager.left, _leftController);
        }

        if (_rightController == null)
        {
            if (ControllerManager.right != null)
            {
                var rightIndex = ControllerManager.right.GetComponent<SteamVR_TrackedObject>().index;
                if (rightIndex != SteamVR_TrackedObject.EIndex.None)
                    _rightController = SteamVR_Controller.Input((int)rightIndex);
            }
        }
        else
        {
            FireLaser(ControllerManager.right, _rightController);
        }
    }

    public override void Destroy()
    {
        Destroy(gameObject);
    }
}