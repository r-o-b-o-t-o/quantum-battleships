using TMPro;
using UnityEngine;

namespace UI
{
    public class TemplatedText : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        private string templatedString;

        private void Awake()
        {
            this.templatedString = this.text.text;
        }

        public void SetArguments(params object[] arguments)
        {
            this.text.text = string.Format(this.templatedString, arguments);
        }
    }
}
