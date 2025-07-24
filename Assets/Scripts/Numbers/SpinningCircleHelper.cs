using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SpinningCircleHelper
{
    public static void SetSpinningCircleForNumberModel(NumberModel numberModel, bool toSetActive, bool destroyThisGO = false)
    {
        if (numberModel != null)
        {
            SetSpinningCircleForGO(numberModel.NumberObject, toSetActive, destroyThisGO);
        }
    }

    public static void SetSpinningCircleForGO(GameObject gameObject, bool toSetActive, bool destroyThisGO)
    {
		Transform childTransform = gameObject.transform.Find("spinningCircle");
		if (childTransform != null)
		{
			var childGameObject = childTransform.gameObject;
			childGameObject.SetActive(toSetActive);

            if (destroyThisGO)
            {
                UnityEngine.Object.Destroy(childGameObject);
            }
		}
	}
}
