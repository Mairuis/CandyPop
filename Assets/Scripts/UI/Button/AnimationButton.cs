using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class AnimationButton : MonoBehaviour,IPointerDownHandler, IPointerUpHandler
{
    public static readonly int IsPressed = Animator.StringToHash("IsPressed");

    private Animator animator;

    private Button button;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.button = GetComponent<Button>();
        this.animator = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        this.animator.SetBool(IsPressed, true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        this.animator.SetBool(IsPressed, false);
    }
}
