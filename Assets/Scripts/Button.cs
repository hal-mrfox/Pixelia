using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pigeon
{
    public class Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [HideInInspector] public bool hovering;
        [HideInInspector] public bool clicking;

        public bool ignoreEvents;
        public Button eventButton;

        public float hoverSpeed = 1f;

        public EaseFunctions.EvaluateMode easingFunctionHover;
        [SerializeField] protected EaseFunctions.EaseMode easingModeHover;
        public EaseFunctions.EaseMode EasingModeHover
        {
            get => easingModeHover;
            set
            {
                easingModeHover = value;
                easingFunctionHover = EaseFunctions.SetEaseMode(value);
            }
        }

        [Space(10f)]
        public float clickSpeed = 1f;

        public EaseFunctions.EvaluateMode easingFunctionClick;
        [SerializeField] protected EaseFunctions.EaseMode easingModeClick;
        public EaseFunctions.EaseMode EasingModeClick
        {
            get => easingModeClick;
            set
            {
                easingModeClick = value;
                easingFunctionClick = EaseFunctions.SetEaseMode(value);
            }
        }

        public IEnumerator hoverCoroutine;
        public IEnumerator clickCoroutine;

        [HideInInspector] public UnityEvent OnHoverEnter;
        [HideInInspector] public UnityEvent OnHoverExit;
        [HideInInspector] public UnityEvent OnClickDown;
        [HideInInspector] public UnityEvent OnClickUp;

        protected virtual void OnValidate()
        {
            EasingModeHover = easingModeHover;
            EasingModeClick = easingModeClick;
        }

        public virtual void Awake()
        {
            if (eventButton)
            {
                eventButton.OnHoverEnter.AddListener(() => OnPointerEnter(null));
                eventButton.OnHoverExit.AddListener(() => OnPointerExit(null));
                eventButton.OnClickDown.AddListener(() => OnPointerDown(null));
                eventButton.OnClickUp.AddListener(() => OnPointerUp(null));
            }
        }

        public void SetHover(bool hover)
        {
            if (hover && !hovering)
            {
                OnPointerEnter(null);
            }
            else if (!hover && hovering)
            {
                OnPointerExit(null);
            }
        }

        public void SetClick(bool click)
        {
            if (click && !clicking)
            {
                OnPointerDown(null);
            }
            else if (!click && clicking)
            {
                OnPointerUp(null);
            }
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (ignoreEvents && eventData != null)
            {
                return;
            }

            hovering = true;

            OnHoverEnter?.Invoke();
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (ignoreEvents && eventData != null)
            {
                return;
            }

            hovering = false;

            OnHoverExit?.Invoke();
        }

        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (ignoreEvents && eventData != null)
            {
                return;
            }

            clicking = true;

            OnClickDown?.Invoke();
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            if (ignoreEvents && eventData != null)
            {
                return;
            }

            clicking = true;

            OnClickUp?.Invoke();
        }

        public virtual void SetToNull(IEnumerator coroutine)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }
    }
}

#if UNITY_EDITOR
    [CustomEditor(typeof(Pigeon.Button), true)]
    [CanEditMultipleObjects]
    public class ButtonEditor : Editor
    {
        public bool showEvents;

        SerializedProperty OnHoverEnter;
        SerializedProperty OnHoverExit;
        SerializedProperty OnClickDown;
        SerializedProperty OnClickUp;

        void OnEnable()
        {
            OnHoverEnter = serializedObject.FindProperty("OnHoverEnter");
            OnHoverExit = serializedObject.FindProperty("OnHoverExit");
            OnClickDown = serializedObject.FindProperty("OnClickDown");
            OnClickUp = serializedObject.FindProperty("OnClickUp");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();

            showEvents = EditorGUILayout.Foldout(showEvents, "Events", true);

            if (showEvents)
            {
                EditorGUILayout.PropertyField(OnHoverEnter);
                EditorGUILayout.PropertyField(OnHoverExit);
                EditorGUILayout.PropertyField(OnClickDown);
                EditorGUILayout.PropertyField(OnClickUp);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif