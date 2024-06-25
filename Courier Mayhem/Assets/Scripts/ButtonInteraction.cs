using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

/** Script para darle mayor interactividad a los botones
 * Esteban.Hernandez
 */
public class ButtonInteraction : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{

    public AudioClip HoverAudioClip;
    public AudioClip ClickAudioClip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.instance.PlayOnce(HoverAudioClip);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SoundManager.instance.PlayOnce(ClickAudioClip);
    }
}
