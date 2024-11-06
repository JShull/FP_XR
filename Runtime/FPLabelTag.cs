namespace FuzzPhyte.XR
{
    using FuzzPhyte.Utility.Meta;
    using FuzzPhyte.Utility.EDU;
    using FuzzPhyte.Utility;
    using UnityEngine;

    public class FPLabelTag
    {
        protected FP_Tag dataTag;
        protected FP_Vocab vocabData;
        public FPLabelTag(FP_Tag dataTag)
        {
            this.dataTag = dataTag;
        }
        public FPLabelTag(FP_Tag dataTag, FP_Vocab vocabData): this(dataTag)
        {
            this.vocabData = vocabData;
        }
        
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
        
        /// <summary>
        /// Return the vocabulary word by matching language
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public string ReturnTranslation(FP_Language language)
        {
            for(int i = 0; i < vocabData.Translations.Count; i++)
            {
                if(vocabData.Translations[i].Language == language)
                {
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
        public AudioClip ReturnAudio(FP_Language language,ref AudioType audioType)
        {
            //go through my own language and my translations for a match
            if(vocabData.Language == language)
            {
                audioType = vocabData.WordAudio.URLAudioType;
                return vocabData.WordAudio.AudioClip;
            }
            for(int i = 0; i < vocabData.Translations.Count; i++)
            {
                if(vocabData.Translations[i].Language == language)
                {
                    audioType = vocabData.Translations[i].WordAudio.URLAudioType;
                    return vocabData.Translations[i].WordAudio.AudioClip;
                }
            }
            audioType = AudioType.UNKNOWN;
            return null;
        }
    }
}
