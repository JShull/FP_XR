namespace FuzzPhyte.XR
{
    using FuzzPhyte.Utility;
    using FuzzPhyte.Utility.EDU;
    using TMPro;
    using UnityEngine;
    using System.Collections.Generic;

    public class FPXRInteractableLabel : MonoBehaviour, IFPXRLabel
    {
        public GameObject ParentLabelRef;
        public TMP_Text LabelText;
        protected FPLabelTag labelTag;
        public bool UseCombinedVocabData = false;
        public FontSettingLabel LabelFontSettingMatch;
        [SerializeField]
        protected FP_Vocab VocabData;
        protected List<XRVocabSupportData> supportData;
        [SerializeField]protected FP_Theme ThemeData;
        [SerializeField]protected FP_Language StartLanguage;
        public string DisplayVocabTranslation(FP_Language language = FP_Language.USEnglish)
        {
            return labelTag.ApplyVocabTranslationTextData(LabelText, language,UseCombinedVocabData);
        }
        public string ReturnVocabTranslation(FP_Language language = FP_Language.USEnglish)
        {
            return labelTag.GetVocabTranslationTextData(language, UseCombinedVocabData);
        }

        public void SetupLabelData(XRDetailedLabelData data, FP_Language startingLanguage, bool startActive = true, bool useCombined=false)
        {
            //TagData = tag;
            supportData = data.SupportVocabData;
            UseCombinedVocabData =useCombined;
            VocabData = data.VocabData;
            ThemeData = data.ThemeData;
            StartLanguage = startingLanguage;
            labelTag = new FPLabelTag(data.TagData, VocabData, ThemeData,supportData);
            if (ParentLabelRef != null)
            {
                ParentLabelRef.SetActive(startActive);
            }
            labelTag.ApplyVocabTextData(LabelText,UseCombinedVocabData);
        }

        public virtual bool ShowAllRenderers(bool status)
        {
            //not sure on this item
            return false;
        }
        public virtual void ForceShowRenderer()
        {

        }
        public virtual void ForceHideRenderer()
        {

        }

        public GameObject ReturnGameObject()
        {
            return this.gameObject;
        }

        public FPLabelTag ReturnDataObject()
        {
            return this.labelTag;
        }

       
    }
}
