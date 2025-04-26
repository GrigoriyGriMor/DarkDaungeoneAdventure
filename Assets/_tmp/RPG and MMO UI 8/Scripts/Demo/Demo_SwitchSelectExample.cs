using UnityEngine;

namespace DuloGames.UI
{
    public class Demo_SwitchSelectExample : MonoBehaviour
    {
        [SerializeField] private UISwitchSelect m_SwitchSelect;

        private void OnEnable()
        {
            if (this.m_SwitchSelect != null)
                this.m_SwitchSelect.onChange.AddListener(OnOptionChange);
        }

        private void OnDisable()
        {
            if (this.m_SwitchSelect != null)
                this.m_SwitchSelect.onChange.RemoveListener(OnOptionChange);
        }

        public void OnOptionChange(int index, string option)
        {
            Debug.Log("Switch Select option changed to: " + index + " " + option);
        }
    }
}