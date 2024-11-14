namespace FuzzPhyte.XR
{
    using FuzzPhyte.Utility;
    using System.Collections;
    using TMPro;
    using UnityEngine;

    public class FPTypingText : MonoBehaviour
    {
        public FPVocabTagDisplay TypingData;
        public AudioSource AudioSource;
        public AnimationCurve TypingCurve;
        protected Coroutine typingCoroutine;
        public FP_Language CurrentLanguageSet;
        public bool TextTypeReplace = false;

        public void OnDisable()
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
            }
        }
        public void StartTypingEffectVocabEnglish() => StartTypingEffectVocab(FP_Language.USEnglish);
        public void StartTypingEffectVocabSpanish() => StartTypingEffectVocab(FP_Language.Spanish);
        public void StartTypingEffectVocabFrench() => StartTypingEffectVocab(FP_Language.French);
        public void UIStartTypingEffectDefinition()
        {
            StartTypingEffectDefinition(CurrentLanguageSet);
        }
        public void UIStartTypingEffectVocab()
        {
            StartTypingEffectVocab(CurrentLanguageSet);
        }
        /// <summary>
        /// Accessor from a Unity Event
        /// </summary>
        /// <param name="lang"></param>
        public void StartTypingEffectDefinition(FP_Language lang)
        {
            StartTypingEffect(lang, true);
        }
        /// <summary>
        /// Accessor from a Unity Event
        /// </summary>
        /// <param name="lang"></param>
        public void StartTypingEffectVocab(FP_Language lang)
        {
            StartTypingEffect(lang, false);
        }
        public void StartTypingEffect(FP_Language lang, bool useDefinition = false)
        {
            if (TypingData != null)
            {
                StartTypingEffect(TypingData, lang, useDefinition, TextTypeReplace);
                return;
            }
            else
            {
                //check if we are on the same component as one
                if (gameObject.GetComponent<FPVocabTagDisplay>())
                {
                    TypingData = gameObject.GetComponent<FPVocabTagDisplay>();
                    StartTypingEffect(TypingData,lang, useDefinition, TextTypeReplace);
                    return;
                }
            }
            Debug.LogError($"Typing Data is null, cannot start typing effect.");
        }
        /// <summary>
        /// If we have no information about anything - shouldn't use this unless we have a reference to the data
        /// </summary>
        /// <param name="typingData">VocabTag Data</param>
        /// <param name="lang">Language Request</param>
        /// <param name="useDefinition">Vocab or definition</param>
        public void StartTypingEffect(FPVocabTagDisplay typingData,FP_Language lang,bool useDefinition=false, bool replaceFontVisual=false)
        {
            TypingData = typingData;
            var theClip = TypingData.SetAndReturnClip(lang, useDefinition);
            string txtContent = string.Empty;
            string rplFont = string.Empty;
            if (useDefinition)
            {
                txtContent = TypingData.ReturnDefinition(lang);
                if (replaceFontVisual)
                {
                    rplFont = TypingData.TertiaryTextDisplay.text;
                }
                StartTypingEffect(TypingData.TertiaryTextDisplay, txtContent, theClip, rplFont);
            }
            else
            {
                txtContent= TypingData.ReturnVocab(lang);
                if (replaceFontVisual)
                {
                    rplFont = TypingData.SecondaryTextDisplay.text;
                }
                StartTypingEffect(TypingData.SecondaryTextDisplay, txtContent, theClip, rplFont);
            }
        }
        public void StartTypingEffect(TMP_Text TxtComponent, string Txt, AudioClip audioFile, string startingTxt = "")
        {
            if (!TypingData.RenderersActive)
            {
                return;
            }
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine); // Stop if already running
            }
            typingCoroutine = StartCoroutine(TypeText(TxtComponent,Txt,audioFile, startingTxt));
        }
        protected IEnumerator TypeText(TMP_Text textComponent,string fullText,AudioClip aFile, string startingText="")
        {
            AudioSource.clip = aFile;
            AudioSource.Play();
            yield return new WaitForEndOfFrame();
            float clipLength = aFile.length;
            Debug.LogWarning($"Clip Length: {clipLength}");
            while (AudioSource.isPlaying)
            {
                float normalizedTime = AudioSource.time / clipLength;
                //Debug.Log($"normalized time: {normalizedTime}");
                float charIndexPosition = TypingCurve.Evaluate(normalizedTime);

                int characterCount = Mathf.FloorToInt(charIndexPosition * fullText.Length);
                int startingTextCount = Mathf.FloorToInt(charIndexPosition * startingText.Length);
                var startStringPartial = startingText.Substring(startingTextCount, startingText.Length-startingTextCount);
               
                textComponent.text = fullText.Substring(0, characterCount) + startStringPartial;

                yield return new WaitForEndOfFrame();
            }

            textComponent.text = fullText; // Display the full text once the audio ends
            typingCoroutine = null; // Reset the coroutine reference
        }
    }
}