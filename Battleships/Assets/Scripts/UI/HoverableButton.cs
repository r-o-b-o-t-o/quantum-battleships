using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace UI
{
    public class HoverableButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Sprite normal;
        [SerializeField] private Sprite hovered;
        [SerializeField] private Button button;

        private Image image;

        private void Start()
        {
            this.button.image.sprite = this.normal;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (this.button.interactable)
            {
                this.button.image.sprite = this.hovered;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.button.image.sprite = this.normal;
        }
    }
}
