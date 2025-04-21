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
    public class FPVocabTagDisplay : MonoBehaviour, IFPXRLabel
    {
        public SpriteRenderer BackDrop;
        [Space]
        [Header("Font Renderers")]
        public TMP_Text MainTextDisplay;
        public FontSettingLabel MainTextDisplaySetting;
        public TMP_Text SecondaryTextDisplay;
        public FontSettingLabel SecondaryTextDisplaySetting;
        public TMP_Text TertiaryTextDisplay;
        public FontSettingLabel TertiaryTextDisplaySetting;
        [Space]
        [Header("Data")]
        public FP_Tag TagData;
        public FP_Vocab VocabData;
        public List<XRVocabSupportData> SupportData;
        public FP_Theme ThemeData;
        public bool UseCombinedVocabData = false;
        public FP_Language AudioStartLanguage;
        public bool SetupOnStart = true;
        public bool HideOnStart = false;
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
        [SerializeField] protected AudioClip[] audioClipArray;
        public AudioClip VocabTagAudioClip => audioClip;
        public AudioClip[] VocabTagAudioClipArray => audioClipArray;
        [Space]
        [Header("Unity Events Registered with Default Functions")]
        public UnityEvent DisplayTagEvent;
        public UnityEvent DisplayVocabEvent;
        public UnityEvent DisplayTranslationEvent;
        [Space]
        public bool DisplayJustVocab = true;
        public bool UseThemeBackdrop = true;
        public bool UseThemeBackdropColor = false;
        [Space]
        public bool RenderersActive = true;
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
            if (SetupOnStart)
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
                    SetupBackdropByTheme();
                    //SetupTagVocabDefnText();
                    if (DisplayJustVocab)
                    {
                        SetupJustVocabText();
                    }
                    else
                    {
                        SetupVocabDefnText();
                    }
                    
                    if (AudioStartLanguage != FP_Language.NA)
                    {
                        if(SupportData.Count > 0)
                        {
                            audioClipArray = new AudioClip[SupportData.Count];
                            audioClipArray = SetupAudioClipArray(AudioStartLanguage, false, false);
                        }
                        SetupAudioClip(AudioStartLanguage, false, false);
                    }
                }
            }

            HideShowAllRenderers(!HideOnStart);
        }
        #region Interface Requirements
        public void SetupLabelData(XRDetailedLabelData data, FP_Language startingLanguage, bool startActive, bool useCombinedVocab)
        {
            UseCombinedVocabData = useCombinedVocab;
            Setup(data.TagData, data.VocabData, data.ThemeData, startingLanguage, data.SupportVocabData,startActive);
        }
        public virtual string DisplayVocabTranslation(FP_Language choice)
        {
            return DisplayVocabTranslation(SecondaryTextDisplay, choice,UseCombinedVocabData);
        }
        public virtual void ShowAllRenderers(bool status)
        {
            HideShowAllRenderers(status);
        }
        #endregion
        protected virtual void Setup(FP_Tag tag, FP_Vocab vocab, FP_Theme theme, FP_Language startLanguage, List<XRVocabSupportData> supportVocabData, bool display =false)
        {
            TagData = tag;
            VocabData = vocab;
            ThemeData = theme;
            SupportData = new List<XRVocabSupportData>();
            SupportData.AddRange(supportVocabData);
            AudioStartLanguage = startLanguage;
            labelTag = new FPLabelTag(TagData, VocabData, ThemeData, SupportData);
            pivotLocation = ReturnPivotLocation();
            if (AttachmentLocation != null)
            {
                AttachmentLocation.position = pivotLocation;
            }
            if (MainTextDisplay && SecondaryTextDisplay)
            {
                SetupIndividualFontByCategory();
                SetupBackdropByTheme();
                //SetupTagVocabDefnText();
                if (DisplayJustVocab)
                {
                    SetupJustVocabText();
                }
                else
                {
                    SetupVocabDefnText();
                }
                if (AudioStartLanguage != FP_Language.NA)
                {
                    if (SupportData.Count > 0)
                    {
                        audioClipArray = new AudioClip[SupportData.Count];
                        audioClipArray = SetupAudioClipArray(AudioStartLanguage, false, false);
                    }
                    SetupAudioClip(AudioStartLanguage, false, false);
                }
            }
            HideShowAllRenderers(display);
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
        
        public virtual string DisplayVocabTranslation(TMP_Text textDisplay,FP_Language choice, bool useCombinedVocab)
        {
            DisplayTranslationEvent.Invoke();
            return labelTag.ApplyVocabTranslationTextData(textDisplay, choice, useCombinedVocab);
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
                if (UseCombinedVocabData)
                {
                    return labelTag.ReturnCombinedWords(true);
                }
                else
                {
                    return VocabData.Definition;
                }
                    
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
            if (UseCombinedVocabData)
            {
                if (choice == VocabData.Language)
                {
                    return labelTag.ReturnCombinedWords(false);
                    
                }
                else
                {
                    return labelTag.ReturnTranslation(choice, false,UseCombinedVocabData);
                }
            }
            else
            {
                if (choice == VocabData.Language)
                {
                    return VocabData.Word;
                }
                else
                {
                    return labelTag.ReturnTranslation(choice, false,false);
                }
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
            if (UseCombinedVocabData)
            {
                Debug.LogError($"This is a singular clip request but you have combined vocab data set to true. This will only return the vocab data - call the other function for the array list.");
            }
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
        /// <summary>
        /// Sets the audioclip array and returns the array based on the language & definition requested
        /// </summary>
        /// <param name="choice"></param>
        /// <param name="useDefinition"></param>
        /// <returns></returns>
        public virtual AudioClip[] SetAndReturnClipArray(FP_Language choice, bool useDefinition)
        {
            if (choice == VocabData.Language)
            {
                //don't use translation as these match just use the data and avoid translation data
                return SetupAudioClipArray(choice, useDefinition, false);
            }
            else
            {
                return SetupAudioClipArray(choice, useDefinition, true);
            }
        }
        #region Internal Functions
        /// <summary>
        /// Cycles through the Renderer List and activates or deactivates them all
        /// </summary>
        /// <param name="showR">Show renderer?</param>
        protected virtual void HideShowAllRenderers(bool showR)
        {
            RenderersActive = showR;
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
        protected virtual void SetupBackdropByTheme()
        {
            if (BackDrop && UseThemeBackdrop)
            {
                labelTag.SetupBackdrop(BackDrop, UseThemeBackdropColor);
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
            TertiaryTextDisplay.gameObject.SetActive(true);
            labelTag.ApplyVocabTextData(SecondaryTextDisplay);
            labelTag.ApplyDefinitionTextData(TertiaryTextDisplay);
        }
        protected virtual void SetupJustVocabText()
        {
            labelTag.ApplyVocabTextData(SecondaryTextDisplay,UseCombinedVocabData);
            TertiaryTextDisplay.gameObject.SetActive(false);
            MainTextDisplay.gameObject.SetActive(false);
        }
        /// JOHN FINISH SETUP with AudioClip Array and accessing this information externally via other requests
        /// <summary>
        /// Sets our AudioClip based on the language requested
        /// </summary>
        /// <param name="langRequest"></param>
        /// <param name="useDefn">if we want to use the definition in the data instead of the word</param>
        /// <param name="useTranslation">will not check local word, goes right to translation data</param>
        protected virtual AudioClip SetupAudioClip(FP_Language langRequest, bool useDefn, bool useTranslation)
        {
            AudioType audioT = AudioType.UNKNOWN;
            if (SupportData.Count > 0)
            {
                Debug.LogWarning($"You're requesting a single audio clip but you have multiple support data items. This will only return the vocab data - call the other function for the array list.");
            }
            audioClip = labelTag.ReturnAudio(langRequest, ref audioT, useDefn,useTranslation);
            if (audioT == AudioType.UNKNOWN)
            {
                audioClip = null;
                Debug.LogError($"Audio clip was UNKNOWN for {langRequest} language.");
            }
            return audioClip;
        }
        protected virtual AudioClip[] SetupAudioClipArray(FP_Language langRequest,bool useDefn, bool useTranslation)
        {
            AudioType audioT = AudioType.UNKNOWN;
            (bool success, AudioClip[] audioClips) = labelTag.ReturnAudioArray(langRequest, ref audioT, useDefn, useTranslation);
            if (success && audioT !=AudioType.UNKNOWN)
            {
                return audioClips;
            }
            else
            {
                Debug.LogError($"Something happened to the audio clips for {langRequest} language | {audioT.ToString()}");
                return null;
            }
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
