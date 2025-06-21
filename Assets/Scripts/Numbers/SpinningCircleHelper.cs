using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpinningCircleHelper
{
    public static void DisableSpinningCircleForNumberModel(NumberModel numberModel, bool toSetActive)
    {
        if (numberModel != null)
        {
            DisableSpinningCircleForNumberGO(numberModel.NumberObject, toSetActive);
        }
    }

    public static void DisableSpinningCircleForNumberGO(GameObject gameObject, bool toSetActive)
    {
		Transform childTransform = gameObject.transform.Find("spinningCircle");
		if (childTransform != null)
		{
			var childGameObject = childTransform.gameObject;
			childGameObject.SetActive(toSetActive);
		}
	}
}
