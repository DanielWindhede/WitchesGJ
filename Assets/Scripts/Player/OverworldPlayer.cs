using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Fami.Overworld
{
    [RequireComponent(typeof(OverworldController))]
    public class OverworldPlayer : MonoBehaviour
    {
        private Input _input;
        private OverworldController _controller;

        void Awake()
        {
            _controller = GetComponent<OverworldController>();

            _input = new Input();
            _input.Overworld.Jump.performed += x => DoJump();
            _input.Overworld.Special.performed += x => Interact();
        }

        private void Interact()
        {
            print("Interact");
        }

        void Update()
        {
            _controller.DoUpdate();
        }

        private void DoJump()
        {
            _controller.velocity = -_controller.GravityDirection * 10;
        }

        private void OnEnable()
        {
            _input.Enable();
        }

        private void OnDisable()
        {
            _input.Disable();
        }


        //private void LoadResources()
        //{
        //    _knife = Resources.Load<Knife>("FPS/Prefabs/Knife");
        //    _knife = GetFPSResource("Prefabs/Knife") as Knife;
        //}

        //private Object GetFPSResource(string path)
        //{
        //    return Fami.ResourceHelper.GetResource("folder", path);
        //}

        //[SerializeField] Volume volume;
        //LensDistortion lensDistortion;
        //LoadResources();
        //volume.profile.TryGet<LensDistortion>(out lensDistortion);
    }
}