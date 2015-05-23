using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

[AddComponentMenu("UI/Effects/Animate UV0 Effect")]
[ExecuteInEditMode]
public class AnimateUV0Effect : MonoBehaviour {

    /// <summary>
    /// Repetitions per second 
    /// </summary>
    public Vector2 speed = new Vector2(0, 0);
    private RawImage _image;
    private void Start()
    {
    }

    private void Update()
    {
        if (!_image) _image = GetComponent<RawImage>();
        float aspect = (float)_image.texture.height / _image.texture.width;
 
        _image.uvRect = new Rect(
            _image.uvRect.x + speed.x * Time.deltaTime,
            _image.uvRect.y + speed.y * Time.deltaTime,
            _image.rectTransform.rect.width / _image.rectTransform.rect.height * aspect * _image.uvRect.height,
            _image.uvRect.height
        );
            
        //_image.uvRect.
    }
}
