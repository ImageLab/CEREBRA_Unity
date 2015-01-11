using UnityEngine;
using System.Collections;

public class AnimatedTextureExtendedUV : MonoBehaviour
{
    public int _fps = 3;
    public float[] intensities;

    private Vector2 _size;
    private Renderer _myRenderer;
    private int _lastIndex = 0;

    private int currentIndex = 0;
    private int nextIndex = 1;
    private float currentCoord;

    void Start()
    {
        _myRenderer = renderer;
        if (_myRenderer == null)
            enabled = false;

        currentCoord = intensities[currentIndex];
    }
    // Update is called once per frame
    void Update()
    {
        if( (intensities[nextIndex] < intensities[currentIndex] && currentCoord <= intensities[nextIndex]) ||
            (intensities[nextIndex] > intensities[currentIndex] && currentCoord >= intensities[nextIndex])) {

            currentCoord = intensities[nextIndex];

            currentIndex = nextIndex;
            nextIndex = (currentIndex+1) % intensities.Length;
        }

        currentCoord += (intensities[nextIndex] - intensities[currentIndex])/200;
        Vector2 offset = new Vector2(0, currentCoord - intensities[currentIndex]);

        _myRenderer.material.SetTextureOffset("_MainTex", offset);

        // Calculate index
        //int index = (int)(Time.timeSinceLevelLoad * _fps) % (intensities.Length);

        //if (index != _lastIndex)
        //{

        //    // build offset
        //    // v coordinate is the bottom of the image in opengl so we need to invert.
        //    Vector2 offset = new Vector2(0, intensities[index] - intensities[_lastIndex]);

        //    _myRenderer.material.SetTextureOffset("_MainTex", offset);

        //    _lastIndex = index;
        //}
    }
}