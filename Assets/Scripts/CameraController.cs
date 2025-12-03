using UnityEngine;


    public class CameraController : MonoBehaviour
    {
        [Header("Player To Follow")]
        public Transform APRRoot;
    
        [Header("Follow Properties")]
        public float distance = 10.0f;
        public float smoothness = 0.15f;
    
        [Header("Rotation Properties")]
        public bool rotateCamera = true;
        public float rotateSpeed = 5.0f;
        public float minAngle = -45.0f;
        public float maxAngle = -10.0f;
    
        // TAMBAHAN: Untuk standby camera
        [HideInInspector] public bool isStandby = false;
    
        private Camera cam;
        private float currentX = 0.0f;
        private float currentY = 0.0f;
        private Quaternion rotation;
        private Vector3 dir;
        private Vector3 offset;

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        
            cam = Camera.main;
            offset = cam.transform.position;
        }

        void Update()
        {
            // MODIFIKASI: Hanya proses input jika tidak standby
            if (!isStandby)
            {
                currentX = currentX + Input.GetAxis("Mouse X") * rotateSpeed;
                currentY = currentY + Input.GetAxis("Mouse Y") * rotateSpeed;
                currentY = Mathf.Clamp(currentY, minAngle, maxAngle);
            }
        }
    
        void FixedUpdate()
        {
            // MODIFIKASI: Hanya update posisi jika tidak standby
            if (isStandby)
                return;

            if(rotateCamera)
            {
                dir = new Vector3(0, 0, -distance);
                rotation = Quaternion.Euler(-currentY, currentX, 0);
                cam.transform.position = Vector3.Lerp(cam.transform.position, APRRoot.position + rotation * dir, smoothness);
                cam.transform.LookAt(APRRoot.position);
            }
        
            if(!rotateCamera)
            {
                var targetRotation = Quaternion.LookRotation(APRRoot.position - cam.transform.position);
                cam.transform.position = Vector3.Lerp(cam.transform.position, APRRoot.position + offset, smoothness);
                cam.transform.rotation = Quaternion.Slerp(cam.transform.rotation, targetRotation, smoothness);
            }
        }

        // TAMBAHAN: Fungsi untuk kontrol standby
        public void SetStandby(bool standby)
        {
            isStandby = standby;
        }
    }
