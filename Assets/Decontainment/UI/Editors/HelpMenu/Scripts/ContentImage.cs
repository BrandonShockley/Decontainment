using UnityEngine;
using UnityEngine.UI;

namespace Editor.Help
{
    public class ContentImage : MonoBehaviour
    {
        [SerializeField]
        private Image realImage = null;

        public void Init(Sprite sprite, Vector2 size)
        {
            realImage.sprite = sprite;
            realImage.GetComponent<RectTransform>().sizeDelta = size;
        }
    }
}