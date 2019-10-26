using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ToggleButton : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI textA;
        [SerializeField] private TextMeshProUGUI textB;
        [SerializeField] private Button button;
        private bool state;

        public System.Action<bool> onStateChanged;

        private void Start()
        {
            this.button.onClick.AddListener(this.OnClick);
            this.state = false;
        }

        private void OnClick()
        {
            this.state = !this.state;
            this.textA.gameObject.SetActive(!this.textA.gameObject.activeInHierarchy);
            this.textB.gameObject.SetActive(!this.textB.gameObject.activeInHierarchy);
            this.onStateChanged?.Invoke(this.state);
        }
    }
}
