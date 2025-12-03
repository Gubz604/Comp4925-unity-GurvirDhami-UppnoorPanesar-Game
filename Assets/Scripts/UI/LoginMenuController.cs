using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LoginMenuController : MonoBehaviour
{
    [SerializeField] private Button registerButton;
    [SerializeField] private Button loginButton;

    private void Start()
    {
        if (EventSystem.current != null && loginButton != null)
        {
            EventSystem.current.SetSelectedGameObject(loginButton.gameObject);
        }
    }

    private void Update()
    {
        if (EventSystem.current == null) return;

        GameObject current = EventSystem.current.currentSelectedGameObject;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (current == null || current == loginButton.gameObject)
                SelectButton(registerButton);
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (current == null || current == registerButton.gameObject)
                SelectButton(loginButton);
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (current != null)
            {
                var button = current.GetComponent<UnityEngine.UI.Button>();
                if (button != null)
                    button.onClick.Invoke();

                var fx = current.GetComponent<ButtonParticleFX>();
                if (fx != null)
                    fx.Pulse();
            }
        }
    }

    private void SelectButton(Button button)
    {
        if (button == null) return;
        EventSystem.current.SetSelectedGameObject(button.gameObject);
    }
}
