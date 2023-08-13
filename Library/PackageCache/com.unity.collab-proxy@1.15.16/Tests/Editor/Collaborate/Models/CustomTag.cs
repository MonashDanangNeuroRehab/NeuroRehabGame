using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class CustomTag : MonoBehaviour
{
    [SerializeField]
    private List<string> tags = new List<string>();

    public bool HasTag(string tag)
    {
        return tags.Contains(tag);
    }

    public IEnumerable<string> GetTags()
    {
        return tags;
    }

    public void Rename(int index, string tagName)
    {
        tags[index] = tagName;
    }

    public string GetAtIndex(int index)
    {
        return tags[index];
    }

    public int Count
    {
        get { return tags.Count; }
    }

    [CustomEditor(typeof(CustomTag))]
    public class CustomTagEditor : Editor
    {
        private string[] unityTags;
        SerializedProperty tagsProp;
        private ReorderableList list;

        private void OnEnable()
        {
            unityTags = InternalEditorUtility.tags;
            tagsProp = serializedObject.FindProperty("tags");
            list = new ReorderableList(serializedObject, tagsProp, true, true, true, true);
            list.drawHeaderCallback += DrawHeader;
            list.drawElementCallback += DrawElement;
            list.onAddDropdownCallback += OnAddDropdown;
        }

        private void DrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, new GUIContent("Tags"), EditorStyles.boldLabel);
        }

        private void DrawElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.LabelField(rect, element.stringValue);
        }

        private void OnAddDropdown(Rect buttonRect, ReorderableList list)
        {
            GenericMenu menu = new GenericMenu();

            for (int i = 0; i < unityTags.Length; i++)
            {
                var label = new GUIContent(unityTags[i]);

                // Don't allow duplicate tags to be added.
                if (PropertyContainsString(tagsProp, unityTags[i]))
                    menu.AddDisabledItem(label);
                else
                    menu.AddItem(label, false, OnAddClickHandler, unityTags[i]);
            }

            menu.ShowAsContext();
        }

        private bool PropertyContainsString(SerializedProperty property, string value)
        {
            if (property.isArray)
            {
                for (int i = 0; i < property.arraySize; i++)
                {
                    if (property.GetArrayElementAtIndex(i).stringValue == value)
                        return true;
                }
            }
            else
                return property.stringValue == value;

            return false;
        }

        private void OnAddClickHandler(object tag)
        {
            int index = list.serializedProperty.arraySize;
            list.serializedProperty.arraySize++;
            list.index = index;

            var element = list.serializedProperty.GetArrayElementAtIndex(index);
            element.stringValue = (string)tag;
            serializedObject.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Space(6);
            serializedObject.Update();
            list.DoLayoutList();
            serializedObject.ApplyModifiedProperties();
            GUILayout.Space(3);
        }
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
