using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpinningCircleHelper
{
    public static void DisableSpinningCircleForNumberModel(NumberModel numberModel, bool value)
    {
        if (numberModel != null)
        {
            DisableSpinningCircleForNumberGO(numberModel.NumberObject, value);
        }
    }

    public static void DisableSpinningCircleForNumberGO(GameObject gameObject, bool value)
    {
		Transform childTransform = gameObject.transform.Find("spinningCircle");
		if (childTransform != null)
		{
			var childGameObject = childTransform.gameObject;
			childGameObject.SetActive(value);
		}
	}
}
