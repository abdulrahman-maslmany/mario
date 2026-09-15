using UnityEngine;

namespace MyGame
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform target; // اسحب اللاعب هنا في الإنسpector
        public Vector3 offset = new Vector3(0, 0, -10); // مسافة الكاميرا عن اللاعب (Z للكاميرا عادة -10)
        public float smoothSpeed = 0.125f; // سرعة التليين (اختياري لتتبع سلس)

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}