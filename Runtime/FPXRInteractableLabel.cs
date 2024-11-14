using FuzzPhyte.Utility;
using FuzzPhyte.Utility.EDU;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms;

namespace FuzzPhyte.XR
{
    public class FPXRInteractableLabel : MonoBehaviour, IFPXRLabel
    {
        public GameObject ParentLabelRef;
        public TMP_Text LabelText;
        protected FPLabelTag labelTag;
        public FontSettingLabel LabelFontSettingMatch;
        [SerializeField]
        protected FP_Vocab VocabData;
        [SerializeField]protected FP_Theme ThemeData;
        [SerializeField]protected FP_Language StartLanguage;
        public string DisplayVocabTranslation(FP_Language language = FP_Language.USEnglish)
        {
            return labelTag.ApplyVocabTranslationTextData(LabelText, language);
        }

        public void SetupLabelData(XRDetailedLabelData data, FP_Language startingLanguage, bool startActive = true)
        {
            //TagData = tag;
            VocabData = data.VocabData;
            ThemeData = data.ThemeData;
            StartLanguage = startingLanguage;
            labelTag = new FPLabelTag(data.TagData, VocabData, ThemeData);
            if (ParentLabelRef != null)
            {
                ParentLabelRef.SetActive(startActive);
            }
            labelTag.ApplyVocabTextData(LabelText);
        }

        public void ShowAllRenderers(bool status)
        {
            //not sure on this item
        }
    }
}
