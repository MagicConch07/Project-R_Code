using DG.Tweening;
using GM.Inputs;
using System.Threading;
using Unity.Cinemachine;
using UnityEngine;

namespace GM
{
    public class MoveCamera : MonoBehaviour
    {
        [SerializeField] private InputReaderSO input;

        [Header("Setting")]
        [SerializeField] private float minZoomSize = 5f;
        [SerializeField] private float maxZoomSize = 20f;
        [SerializeField] private float widthClamp = 20f; 
        [SerializeField] private float heightClamp = 20f; 

        private CinemachineCamera cam;
        private CameraInput camInput;
        private bool isMove = false;
        private Vector3 startPos;
        private Vector3 originPos;

        private void Awake()
        {
            cam = GetComponent<CinemachineCamera>();

            camInput = input.GetInput<CameraInput>();
            camInput.OnMouseWheel += HandleMouseWheel;
            camInput.OnMouseClick += HandleMouseClick;

            originPos = transform.position;
        }

        private void Update()
        {
            if (isMove)
            {
                Vector3 curPos = Camera.main.ScreenToWorldPoint(camInput.MousePos);
                Vector3 deltaPos = startPos - curPos;

                Vector3 move = transform.position + deltaPos;
                move.x = Mathf.Clamp(move.x, originPos.x - widthClamp, originPos.x + widthClamp);
                move.y = Mathf.Clamp(move.y, originPos.y - heightClamp, originPos.y + heightClamp);
                move.z = Mathf.Clamp(move.z, originPos.z - widthClamp, originPos.z + widthClamp);
                transform.position = move;
            }
        }

        private void HandleMouseWheel(int wheel)
        {
            cam.Lens.OrthographicSize = Mathf.Clamp(cam.Lens.OrthographicSize - wheel, minZoomSize, maxZoomSize);
        }

        private void HandleMouseClick(bool value)
        {
            if (value)
            {
                startPos = Camera.main.ScreenToWorldPoint(camInput.MousePos);
            }

            isMove = value;
        }

        private void OnDestroy()
        {
            input.GetInput<CameraInput>().OnMouseWheel -= HandleMouseWheel;
        }
    }
}
