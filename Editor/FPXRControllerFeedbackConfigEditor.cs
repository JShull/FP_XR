namespace FuzzPhyte.XR.Editor
{
    using UnityEditor;
    using UnityEngine;
    using FuzzPhyte.XR;
    using FuzzPhyte.Utility.Editor;
    //using Unity.Profiling;
    [CustomEditor(typeof(FPXRControllerFeedbackConfig))]
    public class FPXRControllerFeedbackConfigEditor:Editor
    {
        //private static readonly ProfilerMarker feedbackEditorMarker = new ProfilerMarker(ProfilerCategory.Vr,"FPXREditor.ControllerFeedback.DrawInspector");
        private SerializedProperty feedbacksProp;
        private Vector2 scrollPosition;

        private void OnEnable()
        {
            feedbacksProp = serializedObject.FindProperty("feedbacks");
        }

        public override void OnInspectorGUI()
        {
            //feedbackEditorMarker.Begin();
            
                

            serializedObject.Update();
            #region Interior Block Code for Serialized Inspector
            // Draw Default Inspector UI Above
            DrawPropertiesExcluding(serializedObject, "feedbacks");

            EditorGUILayout.Space();
            // Custom Styling for Feedbacks Section
            #region Scroll View Begins
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(Screen.height * 0.8f));
            #region First Vertical Section
            FP_Utility_Editor.BeginColoredVertical(FP_Utility_Editor.UnityEditorDarkGrey, 2);
            EditorGUILayout.LabelField("Controller Data", FP_Utility_Editor.ReturnHeaderOne(Color.white, FontStyle.Normal, TextAnchor.UpperLeft), GUILayout.Height(30));
            //EditorGUILayout.LabelField("Feedbacks", EditorStyles.boldLabel);

            // Scrollable Area for the List
            // scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(Screen.height * 0.8f));
            #region Horizontal View Outside
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            #region Fourth Color Vertical
            FP_Utility_Editor.BeginColoredVertical(FP_Utility_Editor.UnityEditorDarkGrey, 2);
            //EditorGUILayout.BeginVertical("box");
            //if we want the button at the top
            for (int i = 0; i < feedbacksProp.arraySize; i++)
            {
                SerializedProperty feedbackElement = feedbacksProp.GetArrayElementAtIndex(i);
                SerializedProperty buttonEnum = feedbackElement.FindPropertyRelative("Button"); // Get the Button enum
                SerializedProperty statesProp = feedbackElement.FindPropertyRelative("ButtonInteractionStates");

                string displayName = "Unnamed";
                if (buttonEnum != null && buttonEnum.propertyType == SerializedPropertyType.Enum)
                {
                    displayName = buttonEnum.enumNames[buttonEnum.enumValueIndex].Replace("_", " ");
                }
                #region Interior Third Color Vertical
                FP_Utility_Editor.BeginColoredVertical(FP_Utility_Editor.UnityEditorGrey, 5);
                //EditorGUILayout.BeginVertical("box"); // Adds a subtle border around each element
                #region Horizontal
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(displayName, FP_Utility_Editor.ReturnHeaderTwo(Color.white, FontStyle.Normal, TextAnchor.UpperLeft), GUILayout.Height(25));
                EditorGUILayout.Space(10);

                EditorGUILayout.EndHorizontal();
                #endregion
                // secondary list
                EditorGUILayout.PropertyField(buttonEnum);
                if (statesProp != null)
                {
                    EditorGUILayout.LabelField("Button States", EditorStyles.boldLabel);
                    #region Horizontal
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space(10);
                    #region Interior Second Color Vertical
                    FP_Utility_Editor.BeginColoredVertical(FP_Utility_Editor.UnityEditorMiddleGrey, 4);
                    // Add state button
                    //"+ Add Button Feedback",FP_UtilityData.ReturnButtonStyle(FP_UtilityData.ReturnColorByStatus(SequenceStatus.Finished),FontStyle.Bold,TextAnchor.UpperCenter))

                    for (int j = 0; j < statesProp.arraySize; j++)
                    {
                        var stateElement = statesProp.GetArrayElementAtIndex(j);
                        var stateEnum = stateElement.FindPropertyRelative("XRState");

                        string stateLabel = $"Element {j}";
                        if (stateEnum != null && stateEnum.propertyType == SerializedPropertyType.Enum)
                        {
                            stateLabel = stateEnum.enumNames[stateEnum.enumValueIndex];
                        }

                        //EditorGUILayout.BeginVertical("box");


                        FP_Utility_Editor.DrawUILine(FP_Utility_Editor.OkayColor, 0, 0);
                        #region Interior Color Vertical
                        FP_Utility_Editor.BeginColoredVertical(FP_Utility_Editor.UnityEditorMiddleGrey, 4);
                        #region Horizontal
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(stateLabel, FP_Utility_Editor.ReturnHeaderThree(Color.white, FontStyle.Normal, TextAnchor.UpperLeft), GUILayout.Height(20));
                        //EditorGUILayout.LabelField(stateLabel, EditorStyles.boldLabel);
                        // Remove button

                        EditorGUILayout.EndHorizontal();
                        #endregion
                        #region Horizontal
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(15);
                        EditorGUILayout.PropertyField(stateElement);
                        GUILayout.EndHorizontal();
                        #endregion
                        EditorGUILayout.Space(5);
                        #region Horizontal
                        EditorGUILayout.BeginHorizontal();
                        GUILayout.Space(Screen.width * .5f);
                        if (GUILayout.Button("+ State", FP_Utility_Editor.ReturnButtonStyle(FP_Utility_Editor.OkayColor, FontStyle.Bold, TextAnchor.UpperCenter)))
                        {
                            statesProp.InsertArrayElementAtIndex(j);
                            break;
                        }
                        GUILayout.Space(4);
                        if (GUILayout.Button($"− State", FP_Utility_Editor.ReturnButtonStyle(FP_Utility_Editor.WarningColor, FontStyle.Bold, TextAnchor.UpperCenter)))
                        {
                            statesProp.DeleteArrayElementAtIndex(j);
                            break; // Important: exit loop immediately after mutation
                        }
                        EditorGUILayout.EndHorizontal();
                        #endregion
                        FP_Utility_Editor.EndColoredVertical();
                        #endregion
                        //EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.Space(5);
                    FP_Utility_Editor.EndColoredVertical();
                    #endregion
                    EditorGUILayout.EndHorizontal();
                    #endregion

                }
                if (statesProp.arraySize == 0)
                {
                    if (GUILayout.Button("+ Add Button State", FP_Utility_Editor.ReturnButtonStyle(FP_Utility_Editor.OkayColor, FontStyle.Bold, TextAnchor.UpperCenter)))
                    {
                        statesProp.arraySize++;
                        break;
                    }
                }
                FP_Utility_Editor.EndColoredVertical();
                #endregion
                //EditorGUILayout.EndVertical();
                GUILayout.Space(05); // Adds space between elements
            }
            #region Horizontal
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(Screen.width * .05f);
            if (GUILayout.Button("+ Feedback", FP_Utility_Editor.ReturnButtonStyle(FP_Utility_Editor.OkayColor, FontStyle.Bold, TextAnchor.UpperCenter)))
            {
                feedbacksProp.arraySize++;
            }
            if (feedbacksProp.arraySize > 0)
            {
                GUILayout.Space(Screen.width * .05f);
                if (GUILayout.Button("− Feedback", FP_Utility_Editor.ReturnButtonStyle(FP_Utility_Editor.WarningColor, FontStyle.Bold, TextAnchor.UpperCenter)))
                {
                    feedbacksProp.arraySize--;
                    if (feedbacksProp.arraySize < 0)
                    {
                        feedbacksProp.arraySize = 0;
                    }
                }
            }

            GUILayout.Space(Screen.width * .05f);
            EditorGUILayout.EndHorizontal();
            #endregion
            //EditorGUILayout.EndVertical();
            GUILayout.Space(5);
            FP_Utility_Editor.EndColoredVertical();
            #endregion
            EditorGUILayout.EndHorizontal();
            #endregion
            FP_Utility_Editor.EndColoredVertical();
            #endregion
            EditorGUILayout.EndScrollView();
            #endregion
            #endregion
            serializedObject.ApplyModifiedProperties();

            //feedbackEditorMarker.End();
            
        }
    }
}
