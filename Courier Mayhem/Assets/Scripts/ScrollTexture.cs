using UnityEngine;


/** Script que "mueve" una textura en un objeto, para darle un comportamiento dinámico 
 * a los fondos o paralaxes.
 * Esteban.Hernandez
 */
public class ScrollTexture : MonoBehaviour {

	public Material ScrollableMaterial;
	public string TextureBase = "_MainTex";
	public Vector2 ScrollVectorSpeed = new Vector2( 1.0f, 1.0f );
	private Vector2 uvOffset;

    private void Start()
    {
		uvOffset = ScrollableMaterial.GetTextureOffset(TextureBase);
    }

    void Update() 
	{
		uvOffset += ( ScrollVectorSpeed * Time.deltaTime );
		ScrollableMaterial.SetTextureOffset(TextureBase, uvOffset);
	}
}
