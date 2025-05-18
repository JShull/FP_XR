namespace FuzzPhyte.XR
{
    using FuzzPhyte.Utility.Meta;
    using FuzzPhyte.Utility.EDU;
    using FuzzPhyte.Utility;
    using UnityEngine;
    using TMPro;
    using System.Linq;
    using System.Collections.Generic;

    // https://github.com/Antoshidza/NSprites
    // https://github.com/Fribur/TextMeshDOTS
    public class FPLabelTag
    {
        protected FP_Tag dataTag;
        protected FP_Vocab vocabData;
        protected FP_Theme themeData;
        protected List<XRVocabSupportData> vocabSupportData;
        // our combined vocab list based on XRVocabSupportData and by language requirements
        protected List<FP_Vocab> allVocabCombined;
        //protected bool useSupportData = false;
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
        public FPLabelTag(FP_Tag dataTag, FP_Vocab vocabData,FP_Theme theme, List<XRVocabSupportData> vocabSupportData) : this(dataTag, vocabData, theme)
        {
            this.vocabSupportData = new List<XRVocabSupportData>();
            this.vocabSupportData.AddRange(vocabSupportData);
            SetupCombinedVocabulary(vocabData.Language);
        }
        /// <summary>
        /// Only returns the first matching support category
        /// </summary>
        /// <param name="language"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public AudioClip ReturnFirstMatchSupportAudio(FP_Language language, FP_VocabSupport category)
        {
            for(int i=0;i< vocabSupportData.Count; i++)
            {
                var curVocabCombined = vocabSupportData[i];
                if (category == curVocabCombined.SupportCategory)
                {
                    //find correct language
                    (bool success,FP_Vocab data) = curVocabCombined.SupportData.ReturnTranslatedFPVocab(language);
                    if (success)
                    {
                        return data.WordAudio.AudioClip;
                    }
                }
            }
            return null;
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
        protected void SetupCombinedVocabulary(FP_Language language)
        {
            // rough out order then
            allVocabCombined = new List<FP_Vocab>();
            var categoryOrder = new Dictionary<FP_VocabSupport, int>();
            FPLanguageAdjectiveRules rules = null;
            switch (language)
            {
                case FP_Language.USEnglish:
                    rules = FPLanguageUtility.USEnglishRules;
                    categoryOrder = rules.SortOrder
                        .Select((cat, index) => new { cat, index })
                        .ToDictionary(x => x.cat, x => x.index);
                    var sorted = vocabSupportData
                        .OrderBy(v => categoryOrder.GetValueOrDefault(v.SupportCategory, int.MaxValue))
                        .Select(v => v.SupportData);

                    allVocabCombined.AddRange(sorted);
                    allVocabCombined.Add(vocabData);
                    break;
                case FP_Language.Spanish:
                    rules = FPLanguageUtility.SpanishRules;
                    categoryOrder = rules.SortOrder
                        .Select((cat, index) => new { cat, index })
                        .ToDictionary(x => x.cat, x => x.index);
                    var preES = vocabSupportData
                        .Where(v => rules.PreNounCategories.Contains(v.SupportCategory))
                        .OrderBy(v => categoryOrder.GetValueOrDefault(v.SupportCategory, int.MaxValue))
                        .Select(v => v.SupportData);

                    var postES = vocabSupportData
                        .Where(v => !rules.PreNounCategories.Contains(v.SupportCategory))
                        .OrderBy(v => categoryOrder.GetValueOrDefault(v.SupportCategory, int.MaxValue))
                        .Select(v => v.SupportData);

                    allVocabCombined.AddRange(preES);
                    allVocabCombined.Add(vocabData);
                    allVocabCombined.AddRange(postES);
                    break;
                case FP_Language.French:
                    rules = FPLanguageUtility.FrenchRules;
                    categoryOrder = rules.SortOrder
                        .Select((cat, index) => new { cat, index })
                        .ToDictionary(x => x.cat, x => x.index);

                    var preFR = vocabSupportData
                        .Where(v => rules.PreNounCategories.Contains(v.SupportCategory))
                        .OrderBy(v => categoryOrder.GetValueOrDefault(v.SupportCategory, int.MaxValue))
                        .Select(v => v.SupportData);

                    var postFR = vocabSupportData
                        .Where(v => !rules.PreNounCategories.Contains(v.SupportCategory))
                        .OrderBy(v => categoryOrder.GetValueOrDefault(v.SupportCategory, int.MaxValue))
                        .Select(v => v.SupportData);

                    allVocabCombined.AddRange(preFR);
                    allVocabCombined.Add(vocabData);
                    allVocabCombined.AddRange(postFR);
                    break;
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
        public string ReturnTranslation(FP_Language language, bool useDefin=false, bool useCombined = false)
        {
            for(int i = 0; i < vocabData.Translations.Count; i++)
            {
                if(vocabData.Translations[i].Language == language)
                {
                    if (useCombined)
                    {
                        string combinedWords = string.Empty;
                        SetupCombinedVocabulary(language);
                        for (int j = 0; j < allVocabCombined.Count; j++)
                        {
                            if (useDefin)
                            {
                                if (j == allVocabCombined.Count - 1)
                                {
                                    var curFPWord = allVocabCombined[j];
                                    (bool success, FP_Vocab returnWord) = curFPWord.ReturnTranslatedFPVocab(language);
                                    if (success)
                                    {
                                        combinedWords += returnWord.Definition;
                                    }
                                    else
                                    {
                                        Debug.LogError($"Missing translation for {allVocabCombined[j]} for {language}");
                                        combinedWords += " ... ";
                                    }
                                }
                                else
                                {
                                    var curFPWord = allVocabCombined[j];
                                    (bool success, FP_Vocab returnWord) = curFPWord.ReturnTranslatedFPVocab(language);
                                    if (success)
                                    {
                                        combinedWords += returnWord.Definition + " ";
                                    }
                                    else
                                    {
                                        Debug.LogError($"Missing translation for {allVocabCombined[j]} for {language}");
                                        combinedWords += " ... ";
                                    }
                                }
                            }
                            else
                            {
                                if (j == allVocabCombined.Count - 1)
                                {
                                    var curFPWord = allVocabCombined[j];
                                    (bool success, FP_Vocab returnWord) = curFPWord.ReturnTranslatedFPVocab(language);
                                    if (success)
                                    {
                                        combinedWords += returnWord.Word;
                                    }
                                    else
                                    {
                                        Debug.LogError($"Missing translation for {allVocabCombined[j]} for {language}");
                                        combinedWords += " ... ";
                                    }
                                }
                                else
                                {
                                    var curFPWord = allVocabCombined[j];
                                    (bool success, FP_Vocab returnWord) = curFPWord.ReturnTranslatedFPVocab(language);
                                    if (success)
                                    {
                                        combinedWords += returnWord.Word + " ";
                                    }
                                    else
                                    {
                                        Debug.LogError($"Missing translation for {allVocabCombined[j]} for {language}");
                                        combinedWords += " ... ";
                                    }
                                }
                            }
                            
                        }
                        return combinedWords;
                    }
                    else
                    {
                        if (useDefin)
                        {
                            return vocabData.Translations[i].Definition;
                        }
                        return vocabData.Translations[i].Word;
                    }
                    
                }
            }
            return null;
        }
        /// <summary>
        /// Return the combined words based on the current language
        /// </summary>
        /// <param name="useDefin"></param>
        /// <returns></returns>
        public string ReturnCombinedWords(bool useDefin)
        {
            string combinedWords = string.Empty;
            SetupCombinedVocabulary(vocabData.Language);
            for (int j = 0; j < allVocabCombined.Count; j++)
            {
                if (useDefin)
                {
                    if (j == allVocabCombined.Count - 1)
                    {
                        combinedWords += allVocabCombined[j].Definition;
                    }
                    else
                    {
                        combinedWords += allVocabCombined[j].Definition + " ";
                    }
                }
                else
                {
                    if (j == allVocabCombined.Count - 1)
                    {
                        combinedWords += allVocabCombined[j].Word;
                    }
                    else
                    {
                        combinedWords += allVocabCombined[j].Word + " ";
                    }
                }
            }
            return combinedWords;
        }
        
        /// <summary>
        /// Return Audio Clip by matching language
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public AudioClip ReturnAudio(FP_Language language,ref AudioType audioType, bool useDefinition=false, bool useTranslation=false, bool useCombined=false)
        {
            //go through my own language and my translations for a match
            if (useCombined)
            {
                audioType = AudioType.UNKNOWN;
                return null;
            }
            if (!useTranslation)
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
            
            Debug.LogError($"No match found: current vocab {vocabData.Word}, returning null for {language} language with user passed settings| Use Defn:{useDefinition} and use Translation:{useTranslation}.");
            audioType = AudioType.UNKNOWN;
            return null;
        }
        /// <summary>
        /// Will return the AudioClip Array based on language and your parameters
        /// </summary>
        /// <param name="language"></param>
        /// <param name="audioType"></param>
        /// <param name="useDefinition"></param>
        /// <param name="useTranslation"></param>
        /// <returns></returns>
        public (bool,AudioClip[]) ReturnAudioArray(FP_Language language, ref AudioType audioType, bool useDefinition = false,bool useTranslation = false)
        {
            //build order by language
            if(allVocabCombined == null)
            {
                SetupCombinedVocabulary(language);
            }
            AudioClip[] audioClips = new AudioClip[allVocabCombined.Count];
           
            if (!useTranslation)
            {
                SetupCombinedVocabulary(vocabData.Language);
                if (allVocabCombined.Count > 0)
                {
                    audioType = allVocabCombined[0].WordAudio.URLAudioType;
                    if (useDefinition)
                    {
                        for (int i = 0; i < allVocabCombined.Count; i++)
                        {
                            audioClips[i] = allVocabCombined[i].DefinitionAudio.AudioClip;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < allVocabCombined.Count; i++)
                        {
                            audioClips[i] = allVocabCombined[i].WordAudio.AudioClip;
                        }
                        
                    }
                    return (true,audioClips);
                }
                else
                {
                    return (false,null);
                }
            }
            else
            {
                //use translation
                for (int i = 0; i < vocabData.Translations.Count; i++)
                {
                    if (vocabData.Translations[i].Language == language)
                    {
                        //found match need to translate and setupCombinedVocab by language
                        SetupCombinedVocabulary(language);
                        if (allVocabCombined.Count > 0)
                        {
                            audioType = allVocabCombined[0].WordAudio.URLAudioType;
                            if (useDefinition)
                            {
                                for (int j = 0; j < allVocabCombined.Count; j++)
                                {
                                    (bool success, FP_Vocab curTranslationFPVocab) = allVocabCombined[j].ReturnTranslatedFPVocab(language);
                                    //translate
                                    if (success)
                                    {
                                        audioClips[j] = curTranslationFPVocab.DefinitionAudio.AudioClip;
                                    }
                                    else
                                    {
                                        Debug.LogError($"Missing defn audio/translation for {allVocabCombined[j].Definition} missing the {language} language translation");
                                    }
                                }
                            }
                            else
                            {
                                for (int j = 0; j < allVocabCombined.Count; j++)
                                {
                                    (bool success, FP_Vocab curTranslationFPVocab) = allVocabCombined[j].ReturnTranslatedFPVocab(language);
                                    if (success)
                                    {
                                        audioClips[j] = curTranslationFPVocab.WordAudio.AudioClip;
                                    }
                                    else
                                    {
                                        Debug.LogError($"Missing word audio/translation for {allVocabCombined[j].Word} missing the {language} language translation");
                                    }
                                }
                            }
                            return (true,audioClips);
                        }
                        else
                        {
                            return (false,null);
                        }
                    }
                }
            }
            return (false, null);
        }
        public string ApplyTagTextData(TMP_Text fontTagDisplay)
        {
            fontTagDisplay.text = dataTag.TagName;
            return fontTagDisplay.text;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vocabDisplay"></param>
        /// <param name="useCombined">If we want to combine our support items e.g. color/size</param>
        /// <returns></returns>
        public string ApplyVocabTextData(TMP_Text vocabDisplay, bool useCombined=false)
        {
            if (useCombined)
            {
                string combinedWords = string.Empty;
                //rebuild combined list by language
                SetupCombinedVocabulary(vocabData.Language);
                for (int j = 0; j < allVocabCombined.Count; j++)
                {
                    if (j == allVocabCombined.Count - 1)
                    {
                        combinedWords += allVocabCombined[j].Word;
                    }
                    else
                    {
                        combinedWords += allVocabCombined[j].Word + " ";
                    }
                }
                return combinedWords;
            }
            else
            {
                vocabDisplay.text = vocabData.Word;
                return vocabDisplay.text;
            } 
        }
        public string ApplyDefinitionTextData(TMP_Text defnDisplay, bool useCombined=false)
        {
            if (useCombined)
            {
                string combinedWords = string.Empty;
                //rebuild combined list by language
                SetupCombinedVocabulary(vocabData.Language);
                for (int j = 0; j < allVocabCombined.Count; j++)
                {
                    if (j == allVocabCombined.Count - 1)
                    {
                        combinedWords += allVocabCombined[j].Definition;
                    }
                    else
                    {
                        combinedWords += allVocabCombined[j].Definition + " ";
                    }
                }
                return combinedWords;
            }
            else
            {
                defnDisplay.text = vocabData.Definition;
                return defnDisplay.text;
            }
                
        }
        public string ApplyVocabTranslationTextData(TMP_Text vocabDisplay,FP_Language language, bool useCombined)
        {
            vocabDisplay.text = ReturnTranslation(language,false,useCombined);
            return vocabDisplay.text;
        }
        public string ApplyDefnTranslationTextData(TMP_Text defnDisplay,FP_Language language, bool useCombined)
        {
            defnDisplay.text = ReturnTranslation(language, true,useCombined);
            return defnDisplay.text;
        }
        #endregion
    }
}
