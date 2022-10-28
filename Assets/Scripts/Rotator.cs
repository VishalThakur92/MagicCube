using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    //[SerializeField]
    //bool rotating = false;

    //[SerializeField]
    //float lerpDuration = 5f;

    //#region Methods

    //public void Update()
    //{
    //    if (rotating)
    //        return;

    //    if (Input.GetKeyUp(KeyCode.A))
    //        Rotate(Vector3.left);

    //    if (Input.GetKeyUp(KeyCode.D))
    //        Rotate(Vector3.right);

    //    if (Input.GetKeyUp(KeyCode.W))
    //        Rotate(Vector3.up);

    //    if (Input.GetKeyUp(KeyCode.S))
    //        Rotate(Vector3.down);

    //}
    //public void Rotate(Vector3 direction)
    //{
    //    if (direction == Vector3.left)
    //        StartCoroutine(RotationBehaviour(transform.rotation * Quaternion.Euler(0, 90, 0)));
    //    else if (direction == Vector3.right)
    //        StartCoroutine(RotationBehaviour(transform.rotation * Quaternion.Euler(0, -90, 0)));
    //    if (direction == Vector3.up)
    //        StartCoroutine(RotationBehaviour(transform.rotation * Quaternion.Euler(0, 0, 90)));
    //    else if (direction == Vector3.down)
    //        StartCoroutine(RotationBehaviour(transform.rotation * Quaternion.Euler(0, 0, -90)));
    //}

    //IEnumerator RotationBehaviour(Quaternion targetRotation) {
    //    rotating = true;
    //    float timeElapsed = 0;
    //    Quaternion startRotation = transform.rotation;

    //    while (timeElapsed < lerpDuration)
    //    {
    //        transform.rotation = Quaternion.Slerp(startRotation, targetRotation, timeElapsed / lerpDuration);
    //        timeElapsed += Time.deltaTime;
    //        yield return null;
    //    }
    //    transform.rotation = targetRotation;
    //    rotating = false;
    //    Debug.LogError("Finish");
    //}
    //#endregion
}
