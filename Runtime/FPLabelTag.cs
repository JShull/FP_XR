namespace FuzzPhyte.XR
{
    using FuzzPhyte.Utility.Meta;
    using FuzzPhyte.Utility.EDU;
    using FuzzPhyte.Utility;
    using UnityEngine;
    using TMPro;
    using System.Collections.Generic;

    // https://github.com/Antoshidza/NSprites
    // https://github.com/Fribur/TextMeshDOTS
    public class FPLabelTag
    {
        protected FP_Tag dataTag;
        protected FP_Vocab vocabData;
        protected FP_Theme themeData;
        #region Setup
        public FPLabelTag(FP_Tag dataTag)
        {
            this.dataTag = dataTag;
        }
        public FPLabelTag(FP_Tag dataTag, FP_Vocab vocabData, FP_Theme theme): this(dataTag)
        {
            this.vocabData = vocabData;
            themeData = theme;
        }
        /// <summary>
        /// Send the list of fonts for the display system you're using
        /// This function will make sure they have the correct settings by theme by FontSetingLabel based on order H1-H7
        /// </summary>
        /// <param name="FontsInOrder">In order of Header 1 thru</param>
        public void SetupFonts(List<TMP_Text> HeaderFontsInOrder)
        {
            if (themeData.FontSettings.Count > 0)
            {
                //we have font settings - lets update our visuals
                for (int i = 0; i < themeData.FontSettings.Count; i++)
                {
                    var curThemeFont = themeData.FontSettings[i];
                    for (int j = 0; j < HeaderFontsInOrder.Count; j++)
                    {
                        if (curThemeFont.Label == (FontSettingLabel)j)
                        {
                            HeaderFontsInOrder[j].font = curThemeFont.Font;
                            HeaderFontsInOrder[j].fontSize = curThemeFont.MinSize;
                            HeaderFontsInOrder[j].color = curThemeFont.FontColor;
                            HeaderFontsInOrder[j].fontStyle = curThemeFont.FontStyle;
                            HeaderFontsInOrder[j].alignment = curThemeFont.FontAlignment;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Setup Individual Fonts by a Category
        /// </summary>
        /// <param name="font"></param>
        /// <param name="category"></param>
        public void SetupFontByCategory(TMP_Text font,FontSettingLabel category)
        {
            for (int i = 0; i < themeData.FontSettings.Count; i++)
            {
                var curThemeFont = themeData.FontSettings[i];
                if (curThemeFont.Label == category)
                {
                    font.font = curThemeFont.Font;
                    font.fontSize = curThemeFont.MinSize;
                    font.color = curThemeFont.FontColor;
                    font.fontStyle = curThemeFont.FontStyle;
                    font.alignment = curThemeFont.FontAlignment;
                    return;
                }
            }
        }
        /// <summary>
        /// This will use the theme backdrop for the passed sprite renderer
        /// </summary>
        /// <param name="backdrop"></param>
        /// <param name="useThemeColor"></param>
        public void SetupBackdrop(SpriteRenderer backdrop, bool useThemeColor = false)
        {
            backdrop.sprite = themeData.BackgroundImage;
            if (useThemeColor)
            {
                backdrop.color = themeData.MainColor;
            }
        }
        #endregion
        #region Layout Based
        /// <summary>
        /// Get Edge Points of the BackDrop SpriteRenderer
        /// </summary>
        /// <returns></returns>
        public Vector3[] GetEdgePoints(SpriteRenderer BackDrop)
        {
            Vector3[] points = new Vector3[9];

            if (BackDrop == null)
            {
                Debug.LogError("BackDrop SpriteRenderer is not assigned.");
                return points;
            }

            Bounds bounds = BackDrop.bounds;
            Vector3 min = bounds.min;
            Vector3 max = bounds.max;
            Vector3 center = bounds.center;

            // Calculate the 9 points: corners, midpoints, and center
            points[0] = min; // Bottom-left corner
            points[1] = new Vector3(center.x, min.y, max.z); // Bottom-center
            points[2] = new Vector3(max.x, min.y, min.z); // Bottom-right

            points[3] = new Vector3(min.x, center.y, min.z); // Mid-left
            points[4] = center; // Center
            points[5] = new Vector3(max.x, center.y, min.z); // Mid-right

            points[6] = new Vector3(min.x, max.y, min.z); // Top-left
            points[7] = new Vector3(center.x, max.y, max.z); // Top-center
            points[8] = max; // Top-right

            return points;
        }
        #endregion
        /// <summary>
        /// Return the vocabulary word by matching language
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        #region Returns and Visuals
        public string ReturnTranslation(FP_Language language, bool useDefin=false)
        {
            for(int i = 0; i < vocabData.Translations.Count; i++)
            {
                if(vocabData.Translations[i].Language == language)
                {
                    if (useDefin)
                    {
                        return vocabData.Translations[i].Definition;
                    }
                    return vocabData.Translations[i].Word;
                }
            }
            return null;
        }
        /// <summary>
        /// Return Audio Clip by matching language
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public AudioClip ReturnAudio(FP_Language language,ref AudioType audioType, bool useDefinition=false, bool useTranslation=false)
        {
            //go through my own language and my translations for a match

            if (!useTranslation)
            {
                if (vocabData.Language == language)
                {
                    if (useDefinition)
                    {
                        audioType = vocabData.DefinitionAudio.URLAudioType;
                        return vocabData.DefinitionAudio.AudioClip;
                    }
                    else
                    {
                        audioType = vocabData.WordAudio.URLAudioType;
                        return vocabData.WordAudio.AudioClip;
                    }

                }
            }
            else
            {
                for (int i = 0; i < vocabData.Translations.Count; i++)
                {
                    if (vocabData.Translations[i].Language == language)
                    {
                        if (useDefinition)
                        {
                            audioType = vocabData.Translations[i].DefinitionAudio.URLAudioType;
                            return vocabData.Translations[i].DefinitionAudio.AudioClip;
                        }
                        else
                        {
                            audioType = vocabData.Translations[i].WordAudio.URLAudioType;
                            return vocabData.Translations[i].WordAudio.AudioClip;
                        }

                    }
                }
            }
            
            Debug.LogError($"No match found, returning null for {language} language with user passed settings| Use Defn:{useDefinition} and use Translation:{useTranslation}.");
            audioType = AudioType.UNKNOWN;
            return null;
        }
        public string ApplyTagTextData(TMP_Text fontTagDisplay)
        {
            fontTagDisplay.text = dataTag.TagName;
            return fontTagDisplay.text;
        }
        public string ApplyVocabTextData(TMP_Text vocabDisplay)
        {
            vocabDisplay.text = vocabData.Word;
            return vocabDisplay.text;
        }
        public string ApplyDefinitionTextData(TMP_Text defnDisplay)
        {
            defnDisplay.text = vocabData.Definition;
            return defnDisplay.text;
        }
        public string ApplyVocabTranslationTextData(TMP_Text vocabDisplay,FP_Language language)
        {
            vocabDisplay.text = ReturnTranslation(language,false);
            return vocabDisplay.text;
        }
        public string ApplyDefnTranslationTextData(TMP_Text defnDisplay,FP_Language language)
        {
            defnDisplay.text = ReturnTranslation(language, true);
            return defnDisplay.text;
        }
        #endregion
    }
}
