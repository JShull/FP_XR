namespace FuzzPhyte.XR
{
    using FuzzPhyte.Utility;
    using System.Collections.Generic;
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
        public bool UseCombinedVocabData = false;
        [Tooltip("Gap time to add between clips")]
        public float TimeBetweenClips = 0.1f;
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
        protected virtual void StartTypingEffect(FP_Language lang, bool useDefinition = false)
        {
            //check typingData min cool down
            if (TypingData != null)
            {
                //StartTypingEffect(TypingData, lang, useDefinition, TextTypeReplace, UseCombinedVocabData);
                if (TypingData.CheckMinCooldownTime())
                {
                    TypingData.ForceShowRenderer();
                    StartTypingEffect(TypingData, lang, useDefinition, TextTypeReplace, UseCombinedVocabData);
                }
                return;
            }
            else
            {
                //check if we are on the same component as one
                if (gameObject.GetComponent<FPVocabTagDisplay>())
                {
                    TypingData = gameObject.GetComponent<FPVocabTagDisplay>();
                    if(TypingData!=null)
                    {
                    //StartTypingEffect(TypingData,lang, useDefinition, TextTypeReplace, UseCombinedVocabData);
                        if (TypingData.CheckMinCooldownTime())
                        {
                            TypingData.ForceShowRenderer();
                            StartTypingEffect(TypingData, lang, useDefinition, TextTypeReplace,UseCombinedVocabData);
                        } 
                    }
                    
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
        public void StartTypingEffect(FPVocabTagDisplay typingData,FP_Language lang,bool useDefinition=false, bool replaceFontVisual=false, bool useSupportData=false)
        {
            TypingData = typingData;
            AudioClip[] theClip = null;
            //TypingData.SetAndReturnClip(lang, useDefinition);
            string txtContent = string.Empty;
            string rplFont = string.Empty;
            //update our last language
            CurrentLanguageSet = lang;
            // use support language or not
            if (useSupportData)
            {
                Debug.LogWarning($"we are setting the combined data to true on your behalf now");
                TypingData.UseCombinedVocabData = true;
            }
            if (useDefinition)
            {
                txtContent = TypingData.ReturnDefinition(lang);
                if (replaceFontVisual)
                {
                    rplFont = TypingData.TertiaryTextDisplay.text;
                }
                if (useSupportData)
                {
                    theClip = TypingData.SetAndReturnClipArray(lang, true);
                }
                else
                {
                    theClip = new AudioClip[1];
                    theClip[0] = TypingData.SetAndReturnClip(lang, true);
                }
                StartTypingEffect(TypingData.TertiaryTextDisplay, txtContent, theClip, rplFont);
            }
            else
            {
                txtContent = TypingData.ReturnVocab(lang);
                if (replaceFontVisual)
                {
                    rplFont = TypingData.SecondaryTextDisplay.text;
                }
                if (useSupportData)
                {
                    theClip = TypingData.SetAndReturnClipArray(lang, false);
                }
                else
                {
                    theClip = new AudioClip[1];
                    theClip[0] = TypingData.SetAndReturnClip(lang, false);
                }
                StartTypingEffect(TypingData.SecondaryTextDisplay, txtContent, theClip, rplFont);
            }
        }
        public void StartTypingEffect(TMP_Text TxtComponent, string Txt, AudioClip[] audioFile, string startingTxt = "")
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
        protected IEnumerator TypeText(TMP_Text textComponent,string fullText,AudioClip[] aFile, string startingText="")
        {
            //build out an estimate of length
            var estimateLength = 0f;
            var runningLoopTime = 0f;
            AudioSource.clip = aFile[0];
            List<float>clipStartTimes = new List<float>();
            List<AudioClip> clipArrays = new List<AudioClip>();
            clipStartTimes.Add(0f);

            if (aFile.Length > 1)
            {
                for(int i=0; i < aFile.Length; i++)
                {
                    estimateLength += aFile[i].length;
                    var clipStartTime = aFile[i].length;
                    clipArrays.Add(aFile[i]);
                    if (i < aFile.Length - 1)
                    {
                        //add in gap time
                        estimateLength+=TimeBetweenClips;
                        clipStartTimes.Add(clipStartTime+TimeBetweenClips);
                    }
                }
            }
            else
            {
                if (aFile.Length == 1)
                {
                    Debug.LogWarning($"Clip length: 1");
                    estimateLength = aFile[0].length;
                    clipArrays.Add(aFile[0]);
                }
                else
                {
                    yield break;
                }
            }
            Debug.LogWarning($"ClipStart Times Count: {clipStartTimes.Count} with ClipArrays at {clipArrays.Count}");
            
            yield return new WaitForEndOfFrame();
            // float clipLength = estimateLength;
            Debug.LogWarning($"Clip Length: {estimateLength}");
            while(runningLoopTime < estimateLength)
            {
                runningLoopTime += Time.deltaTime;
                float normalizedTime = runningLoopTime / estimateLength;
                float charIndexPosition = TypingCurve.Evaluate(normalizedTime);
                int characterCount = Mathf.FloorToInt(charIndexPosition * fullText.Length);
                int startingTextCount = Mathf.FloorToInt(charIndexPosition * startingText.Length);
                var startStringPartial = startingText.Substring(startingTextCount, startingText.Length - startingTextCount);
                textComponent.text = fullText.Substring(0, characterCount) + startStringPartial;
                if(clipStartTimes.Count>0)
                {
                    if(runningLoopTime >= clipStartTimes[0])
                    {
                        if (clipArrays.Count > 0)
                        {
                            AudioSource.clip = clipArrays[0];
                            AudioSource.Play();
                            clipStartTimes.RemoveAt(0);
                            clipArrays.RemoveAt(0);
                        }
                    }
                }
                
                yield return new WaitForEndOfFrame();
            }

            textComponent.text = fullText; // Display the full text once the audio ends
            typingCoroutine = null; // Reset the coroutine reference
        }
    }
}