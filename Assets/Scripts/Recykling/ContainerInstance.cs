using UnityEngine;

namespace Recykling {
    public enum ContainerType
    {
        Paper,
        PlasticAndMetal,
        Glass,
        Bio,
        Other
    }
    public class ContainerInstance : MonoBehaviour
    {
        [Header("ContainerType")]
        public ContainerType containerType;

        [Header("Sprites")]
        public SpriteRenderer spriteRenderer;
        public Sprite paperSprite;
        public Sprite plasticAndMetalSprite;
        public Sprite glassSprite;
        public Sprite bioSprite;
        public Sprite otherSprite;

        public RecyklingGameScript rgs;

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();

            switch (containerType)
            {
                case ContainerType.Paper:
                    spriteRenderer.sprite = paperSprite;
                    break;
                case ContainerType.PlasticAndMetal:
                    spriteRenderer.sprite = plasticAndMetalSprite;
                    break;
                case ContainerType.Glass:
                    spriteRenderer.sprite = glassSprite;
                    break;
                case ContainerType.Bio:
                    spriteRenderer.sprite = bioSprite;
                    break;
                case ContainerType.Other:
                    spriteRenderer.sprite = otherSprite;
                    break;
                default:
                    break;
            }
        }

        void Update()
        {

        }

    }
}