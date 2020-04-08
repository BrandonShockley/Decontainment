using UnityEngine;

namespace Editor.Help
{
    public class GeneralContentList : ReadOnlyEditorList<GeneralContent>
    {
        [SerializeField]
        private string resourcesPath = null;

        // NOTE: This code could be reused for other ReadOnlyEditorLists
        protected override void CreateListEntry(GeneralContent content, int siblingIndex = -1)
        {
            GameObject listEntryGO = Instantiate(listEntryPrefab, transform);
            TextListEntry listEntry = listEntryGO.GetComponent<TextListEntry>();
            listEntry.Init(content.name, null);
            listEntry.OnSelect += () => HandleSelect(listEntryGO.transform.GetSiblingIndex());
            if (siblingIndex >= 0) {
                listEntry.transform.SetSiblingIndex(siblingIndex);
            }
        }

        protected override void InitList()
        {
            items.AddRange(Resources.LoadAll<GeneralContent>(resourcesPath));
        }
    }
}