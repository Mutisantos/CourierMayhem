using UnityEngine;

/** Script apoyado en el Shader que permite modificar el Matiz de un objeto de manera
 * programática. Util para darle variedad a las entidades en juego.
 * Esteban.Hernandez
 */
public class HueSpriteShifter : MonoBehaviour
{

    [Range(-0.5f, 0.5f)]
    public float hueShiftValue = 0;

    public SpriteRenderer Renderer;

    public string texturePropertyName = "_HueDiff";

    void Start()
    {
        Renderer = GetComponent<SpriteRenderer>();
        Renderer.material.SetFloat(texturePropertyName, hueShiftValue);
    }

    public void ShiftHue(float value)
    {
        Renderer.material.SetFloat(texturePropertyName, value);
    }
}
