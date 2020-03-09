using UnityEngine;
public class SpriteSheetNG : MonoBehaviour
{
    public int _uvTileX = 1;
    public int _uvTileY = 1;
    public int _fps = 10;

    private Vector2 _size;
    private Renderer _myRenderer;
    private int _lastIndex = -1;

    void Start()
    {
        _size = new Vector2(1.0f / _uvTileX, 1.0f / _uvTileY);
        _myRenderer = GetComponent<LineRenderer>();
        if (_myRenderer == null)
            enabled = false;
    }
    // Update is called once per frame
    void Update()
    {
        // Calculate index
        int index = (int)(Time.timeSinceLevelLoad * _fps) % (_uvTileX * _uvTileY);
        if (index != _lastIndex)
        {
            // split into horizontal and vertical index
            int uIndex = index % _uvTileX;
            int vIndex = index / _uvTileY;

            // build offset
            // v coordinate is the bottom of the image in opengl so we need to invert.
            Vector2 offset = new Vector2(uIndex * _size.x, 1.0f - _size.y - vIndex * _size.y);

            _myRenderer.material.SetTextureOffset("_MainTex", offset);
            _myRenderer.material.SetTextureScale("_MainTex", _size);

            _lastIndex = index;
        }
    }
}