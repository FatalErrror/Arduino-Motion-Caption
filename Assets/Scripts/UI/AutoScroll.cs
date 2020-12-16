using UnityEngine;
using UnityEngine.UI;

public class AutoScroll : MonoBehaviour
{
    public ScrollRect ScrollRect;
    private Rect _content;

    // Start is called before the first frame update
    void Start()
    {
        _content = ScrollRect.content.rect;
    }

    private void Update()
    {
        if (ScrollRect.content.rect != _content)
        {
            ScrollRect.verticalScrollbar.value = 0;
            _content = ScrollRect.content.rect;
        }
    }
}
