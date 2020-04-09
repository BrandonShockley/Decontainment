using UnityEngine;

namespace Editor.Help
{
    public class GeneralContentDisplay : ContentDisplay<GeneralContent, GeneralContentList>
    {
        protected override void Generate()
        {
            if (contentList.SelectedItem == null) {
                return;
            }

            // Title
            GenerateText(titlePrefab, contentList.SelectedItem.name, "Title");
            Instantiate(bigSpacerPrefab, transform);

            foreach (GeneralContent.Block block in contentList.SelectedItem.blocks) {
                switch (block.type)
                {
                    case GeneralContent.Block.Type.HEADER:
                        GenerateText(headerPrefab, block.text, "Header");
                        break;
                    case GeneralContent.Block.Type.TEXT:
                        GenerateText(textPrefab, block.text, "Text");
                        break;
                    case GeneralContent.Block.Type.BIG_SPACER:
                        Instantiate(bigSpacerPrefab, transform);
                        break;
                    case GeneralContent.Block.Type.SMALL_SPACER:
                        Instantiate(smallSpacerPrefab, transform);
                        break;
                    case GeneralContent.Block.Type.IMAGE:
                        GenerateImage(block.image, block.imageSize, "Image");
                        break;
                }
            }
        }
    }
}