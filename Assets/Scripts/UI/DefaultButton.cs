using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DefaultButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	private Color32 _originalColor = GlobalVars.DEFAULT_GREEN_BUTTON_COLOR;
	private Image _imgComp;

	void Awake()
	{
		_imgComp = GetComponent<Image>();
	}
	public void OnPointerEnter(PointerEventData eventData)
	{
		_imgComp.color = new Color(_imgComp.color.r * GlobalVars.DARKER_MULTIPLYER, 
			_imgComp.color.g * GlobalVars.DARKER_MULTIPLYER, 
			_imgComp.color.b * GlobalVars.DARKER_MULTIPLYER, 
			_imgComp.color.a);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		_imgComp.color = _originalColor;
	}
	public void OnPointerDown(PointerEventData eventData)
	{
		Debug.Log("Touch started (finger down or mouse down)");
		transform.localScale = Vector3.one * 0.95f; // "Pressed" effect
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		Debug.Log("Touch ended (finger lifted or mouse up)");
		transform.localScale = Vector3.one; // Reset scale
		//imgComp.color = _originalColor;
	}

	public void SetPressed()
	{
		gameObject.GetComponent<Image>().color = GlobalVars.DARKER_GREEN_BUTTON_COLOR;
		GetComponent<Button>().interactable = false;
		enabled = false;
	}

	public void SetDisabled()
	{
		gameObject.GetComponent<Image>().color = Color.grey;
		GetComponent<Button>().interactable = false;
		enabled = false;
	}
}
