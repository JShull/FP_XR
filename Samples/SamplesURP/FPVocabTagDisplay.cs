namespace FuzzPhyte.XR
{    
    using UnityEngine;
    using TMPro;
    using FuzzPhyte.Utility.Meta;
    using FuzzPhyte.Utility.EDU;
    using FuzzPhyte.Utility;
    using UnityEngine.Events;
    using System.Collections.Generic;

    /// <summary>
    /// Mono Wrapper Class for FPLabelTag and for connecting various events, visuals etc.
    /// </summary>
    public class FPVocabTagDisplay : MonoBehaviour
    {
        public SpriteRenderer BackDrop;
        [Space]
        [Header("Font Renderers")]
        public TextMeshPro MainTextDisplay;
        public FontSettingLabel MainTextDisplaySetting;
        public TextMeshPro SecondaryTextDisplay;
        public FontSettingLabel SecondaryTextDisplaySetting;
        public TextMeshPro TertiaryTextDisplay;
        public FontSettingLabel TertiaryTextDisplaySetting;
        [Space]
        [Header("Data")]
        public FP_Tag TagData;
        public FP_Vocab VocabData;
        public FP_Theme ThemeData;
        public FP_Language AudioStartLanguage;
        [Tooltip("All renderers associated with this tag")]
        public List<Renderer> RendererList = new List<Renderer>();
        [Space]
        [Header("Layout")]
        public TextAlignmentOptions TagPivotLocation;
        public Transform RootVisual;
        [Tooltip("Will manage a transform for reference across other scripts")]
        public Transform AttachmentLocation;
        protected FPLabelTag labelTag;
        [SerializeField]protected Vector3 pivotLocation;
        [Header("Internal Data Set by Script")]
        [SerializeField] protected AudioClip audioClip;
        public AudioClip VocabTagAudioClip => audioClip;
        [Space]
        [Header("Unity Events Registered with Default Functions")]
        public UnityEvent DisplayTagEvent;
        public UnityEvent DisplayVocabEvent;
        public UnityEvent DisplayTranslationEvent;
        public Vector3 WorldPivotLocation => pivotLocation;
        public virtual void Awake()
        {
            if(MainTextDisplay&&SecondaryTextDisplay)
            {
                //confirm they are in the rendererlist
                var mText = MainTextDisplay.gameObject.GetComponent<Renderer>();
                if (mText)
                {
                    if (!RendererList.Contains(mText))
                    {
                        RendererList.Add(mText);
                    }
                }
                var sText = SecondaryTextDisplay.gameObject.GetComponent<Renderer>();
                if (sText)
                {
                    if (!RendererList.Contains(sText))
                    {
                        RendererList.Add(sText);
                    }
                }
                var tText = TertiaryTextDisplay.gameObject.GetComponent<Renderer>();
                if (tText)
                {
                    if (!RendererList.Contains(tText))
                    {
                        RendererList.Add(tText);
                    }
                }
            }
        }
        public virtual void Start()
        {
            labelTag = new FPLabelTag(TagData, VocabData, ThemeData);
            pivotLocation = ReturnPivotLocation();
            if (AttachmentLocation != null)
            {
                AttachmentLocation.position = pivotLocation;
            }
            if (MainTextDisplay && SecondaryTextDisplay)
            {
                SetupIndividualFontByCategory();
                //SetupTagVocabDefnText();
                SetupVocabDefnText();
                if (AudioStartLanguage != FP_Language.NA)
                {
                    SetupAudioClip(AudioStartLanguage,false,false);
                }
            }
        }
       
        public virtual void DisplayTag()
        {
            DisplayTagEvent.Invoke();
        }
        public virtual void DisplayVocab()
        {
            DisplayVocabEvent.Invoke();
        }
        public virtual void DisplayTranslation()
        {
            DisplayTranslationEvent.Invoke();
        }
        public virtual void UpdateAttachmentLocation(TextAlignmentOptions newPivotLocation)
        {
            TagPivotLocation = newPivotLocation;
            pivotLocation = ReturnPivotLocation();
            if (AttachmentLocation != null)
            {
                AttachmentLocation.position = pivotLocation;
            }
        }
        public virtual string DisplayVocabTranslation(FP_Language choice)
        {
            DisplayTranslationEvent.Invoke();
            return labelTag.ApplyVocabTranslationTextData(SecondaryTextDisplay, choice);
        }
        /// <summary>
        /// Returns the definition or translation based on the language requested
        /// </summary>
        /// <param name="choice"></param>
        /// <returns></returns>
        public virtual string ReturnDefinition(FP_Language choice)
        {
            if(choice==VocabData.Language)
            {
                return VocabData.Definition;
            }
            else
            {
                return labelTag.ReturnTranslation(choice, true);
            }
        }
        /// <summary>
        /// Returns the Vocabulary or the translation based on the language requested
        /// </summary>
        /// <param name="choice"></param>
        /// <returns></returns>
        public virtual string ReturnVocab(FP_Language choice)
        {
            if (choice == VocabData.Language)
            {
                return VocabData.Word;
            }
            else
            {
                return labelTag.ReturnTranslation(choice, false);
            }
        }
        
        /// <summary>
        /// Sets the audio clip and returns it based on the language & definition requested
        /// </summary>
        /// <param name="choice">Language Query</param>
        /// <param name="useDefinition">Definition or Word?</param>
        /// <returns></returns>
        public virtual AudioClip SetAndReturnClip(FP_Language choice, bool useDefinition)
        {
            if (choice == VocabData.Language)
            {
                //don't use translation as these match just use the data and avoid translation data
                return SetupAudioClip(choice, useDefinition, false);
            }
            else
            {
                return SetupAudioClip(choice, useDefinition, true);
            }
        }
        #region Internal Functions
        /// <summary>
        /// Cycles through the Renderer List and activates or deactivates them all
        /// </summary>
        /// <param name="showR">Show renderer?</param>
        protected virtual void HideShowAllRenderers(bool showR)
        {
            foreach (var rend in RendererList)
            {
                rend.enabled = showR;
            }
        }
        protected virtual void SetupIndividualFontByCategory()
        {
            if (MainTextDisplay)
            {
                labelTag.SetupFontByCategory(MainTextDisplay, MainTextDisplaySetting);
            }
            if (SecondaryTextDisplay)
            {
                labelTag.SetupFontByCategory(SecondaryTextDisplay, SecondaryTextDisplaySetting);
            }
            if (TertiaryTextDisplay)
            {
                labelTag.SetupFontByCategory(TertiaryTextDisplay, TertiaryTextDisplaySetting);
            }
        }
        /// <summary>
        /// Assuming a two font configuration
        /// Main = Tag
        /// Secondary = Vocabulary
        /// </summary>
        protected virtual void SetupTagVocabDefnText()
        {
            labelTag.ApplyTagTextData(MainTextDisplay);
            labelTag.ApplyVocabTextData(SecondaryTextDisplay);
            labelTag.ApplyDefinitionTextData(TertiaryTextDisplay);
        }
        protected virtual void SetupVocabDefnText()
        {
            labelTag.ApplyVocabTextData(SecondaryTextDisplay);
            labelTag.ApplyDefinitionTextData(TertiaryTextDisplay);
        }
        /// <summary>
        /// Sets our AudioClip based on the language requested
        /// </summary>
        /// <param name="langRequest"></param>
        /// <param name="useDefn">if we want to use the definition in the data instead of the word</param>
        /// <param name="useTranslation">will not check local word, goes right to translation data</param>
        protected virtual AudioClip SetupAudioClip(FP_Language langRequest, bool useDefn, bool useTranslation)
        {
            AudioType audioT = AudioType.UNKNOWN;
            audioClip = labelTag.ReturnAudio(langRequest, ref audioT, useDefn,useTranslation);
            if (audioT == AudioType.UNKNOWN)
            {
                audioClip = null;
                Debug.LogError($"Audio clip was UNKNOWN for {langRequest} language.");
            }
            return audioClip;
        }
        protected Vector3 ReturnPivotLocation()
        {
            Vector3[] points = labelTag.GetEdgePoints(BackDrop);
            Vector3 pivot = Vector3.zero;
            switch (TagPivotLocation)
            {
                case TextAlignmentOptions.TopLeft:
                    pivot = points[6];
                    break;
                case TextAlignmentOptions.Top:
                    pivot = points[7];
                    break;
                case TextAlignmentOptions.TopRight:
                    pivot = points[8];
                    break;
                case TextAlignmentOptions.Left:
                    pivot = points[3];
                    break;
                case TextAlignmentOptions.Center:
                    pivot = points[4];
                    break;
                case TextAlignmentOptions.Right:
                    pivot = points[5];
                    break;
                case TextAlignmentOptions.BottomLeft:
                    pivot = points[0];
                    break;
                case TextAlignmentOptions.Bottom:
                    pivot = points[1];
                    break;
                case TextAlignmentOptions.BottomRight:
                    pivot = points[2];
                    break;
            }
            return pivot;
        }
        #endregion
    }

}
