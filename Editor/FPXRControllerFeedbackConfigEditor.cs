namespace FuzzPhyte.XR.Editor
{
    using UnityEditor;
    using UnityEngine;
    using FuzzPhyte.XR;
    using System.Collections.Generic;

    [CustomEditor(typeof(FPXRControllerFeedbackConfig))]
    public class FPXRControllerFeedbackConfigEditor:Editor
    {

        private SerializedProperty feedbacksProp;
        private Vector2 scrollPosition;

        private void OnEnable()
        {
            feedbacksProp = serializedObject.FindProperty("feedbacks");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Draw Default Inspector UI Above
            DrawPropertiesExcluding(serializedObject, "feedbacks");

            EditorGUILayout.Space();

            // Custom Styling for Feedbacks Section
            EditorGUILayout.LabelField("Feedbacks", EditorStyles.boldLabel);

            // Scrollable Area for the List
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(Screen.height * 0.8f));

            for (int i = 0; i < feedbacksProp.arraySize; i++)
            {
                SerializedProperty feedbackElement = feedbacksProp.GetArrayElementAtIndex(i);
                SerializedProperty buttonEnum = feedbackElement.FindPropertyRelative("Button"); // Get the Button enum

                // 🔹 Fix: Convert the Enum Value Properly
                string displayName = "Unnamed";
                if (buttonEnum != null && buttonEnum.propertyType == SerializedPropertyType.Enum)
                {
                    displayName = buttonEnum.enumNames[buttonEnum.enumValueIndex].Replace("_", " ");
                }

                EditorGUILayout.BeginVertical("box"); // Adds a subtle border around each element
                EditorGUILayout.LabelField(displayName, EditorStyles.boldLabel); // Display Enum Name
                EditorGUILayout.PropertyField(feedbackElement, true);
                EditorGUILayout.EndVertical();

                GUILayout.Space(10); // Adds space between elements
            }

            EditorGUILayout.EndScrollView();

            serializedObject.ApplyModifiedProperties();
        }
    }
}
