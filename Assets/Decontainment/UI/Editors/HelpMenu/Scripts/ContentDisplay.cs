using TMPro;
using UnityEngine;

namespace Editor.Help
{
    public abstract class ContentDisplay<T, TL> : MonoBehaviour
    where T : class
    where TL : ReadOnlyEditorList<T>
    {
        [SerializeField]
        protected GameObject titlePrefab = null;
        [SerializeField]
        protected GameObject headerPrefab = null;
        [SerializeField]
        protected GameObject textPrefab = null;
        [SerializeField]
        protected GameObject smallSpacerPrefab = null;
        [SerializeField]
        protected GameObject bigSpacerPrefab = null;
        [SerializeField]
        protected GameObject contentImagePrefab = null;
        [SerializeField]
        protected TL contentList = null;

        protected void Start()
        {
            FullReset();
            contentList.OnItemSelected += (oldIndex) => FullReset();
        }

        protected void GenerateText(GameObject textPrefab, string text, string goName = null)
        {
            GameObject go = Instantiate(textPrefab, transform);
            go.GetComponent<TMP_Text>().text = text;
            if (goName != null) {
                go.name = goName;
            }
        }

        protected void GenerateImage(Sprite sprite, Vector2 size, string goName = null)
        {
            GameObject go = Instantiate(contentImagePrefab, transform);
            go.GetComponent<ContentImage>().Init(sprite, size);
            if (goName != null) {
                go.name = goName;
            }
        }

        protected abstract void Generate();

        private void FullReset()
        {
            Clear();
            Generate();
        }

        private void Clear()
        {
            foreach (Transform child in transform) {
                Destroy(child.gameObject);
            }
        }
    }
}