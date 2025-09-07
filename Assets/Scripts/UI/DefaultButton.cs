using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Class for default styling behavior for all buttons in the game (except DSA screen buttons).
/// </summary>
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
		transform.localScale = Vector3.one * 0.95f; // "Pressed" effect
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		transform.localScale = Vector3.one; // Reset scale
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
