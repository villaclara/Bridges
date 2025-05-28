using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpinningCircleHelper
{
    public static void DisableSpinningCircle(NumberModel numberModel, bool value)
    {
        if (numberModel != null)
        {
            Transform childTransform = numberModel.NumberObject.transform.Find("spinningCircle");
            if (childTransform != null)
            {
                var childGameObject = childTransform.gameObject;
                childGameObject.SetActive(value);
            }
        }
    }
}
