using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CurvedUIElement : MonoBehaviour
{
    public void Update()
    {
        CurvedUICanvas canvas = GetComponentInParent<CurvedUICanvas>();
        if (canvas == null)
            return;

        float zPos;
        float yRot;
        canvas.GetElementPosition(GetComponent<RectTransform>(), out zPos, out yRot);
        Vector3 pos = transform.localPosition;
        pos.z = zPos;
        transform.localPosition = pos;

        Vector3 rot = transform.localRotation.eulerAngles;
        rot.y = yRot;
        transform.localRotation = Quaternion.Euler(rot);
    }
}
