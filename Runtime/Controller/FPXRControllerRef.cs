namespace FuzzPhyte.XR
{
    using FuzzPhyte.Utility;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    
    /// <summary>
    /// Mono class to hold references for UI item associated with controller overlay UI/audio data
    /// </summary>
    public class FPXRControllerRef : MonoBehaviour
    {
        protected bool useCanvas = false;
        public Vector3 OffsetAmount;
        protected Vector3 storedLocalScaleAdj = Vector3.one;
        [Header("Setup")]
        [Tooltip("Root Item Ref Locally on Prefab - itself probably")]
        [SerializeField] protected Transform buttonRootParent;
        [Tooltip("Local Root ref - probably the first child")]
        [SerializeField] protected Transform localButtonRootParent;
        [SerializeField] protected Image buttonCanvasImage;
        [SerializeField] protected SpriteRenderer buttonIconImage;
        [SerializeField] protected TMP_Text buttonLabelText;
        [SerializeField] protected AudioSource worldAudioRef;
        [SerializeField] protected bool setupComplete;
        [Space]
        [Header("Not being Used Yet")]
        [Tooltip("This isn't being used yet")]
        [SerializeField] protected SpriteRenderer buttonBackgroundImage;
        [SerializeField] protected Image buttonCanvasBackgroundImage;
       
        /// <summary>
        /// Setup UI item
        /// </summary>
        /// <param name="iconRef"></param>
        /// <param name="textRef"></param>
        /// <param name="fontRef"></param>
        /// <param name="audioRef"></param>
        /// <param name="audioSourceRef"></param>
        /// <param name="UseCanvas"></param>
        public void SetupUI(Vector3 localScale,Sprite iconRef, string textRef, FontSetting fontRef, AudioClip audioRef=null,AudioSource audioSourceRef=null,bool UseCanvas = false)
        {
            //scale accordingly
            buttonRootParent.transform.localScale = localScale;
            storedLocalScaleAdj=localScale;
            if (useCanvas)
            {
                if (buttonCanvasImage != null)
                    buttonCanvasImage.sprite = iconRef;
            }
            else
            {
                //update the sprite
                if (buttonIconImage != null)
                    buttonIconImage.sprite = iconRef;

            }

            // Update Label Text
            if (buttonLabelText != null)
            {
                buttonLabelText.text = textRef;
                FP_UtilityData.ApplyFontSetting(buttonLabelText, fontRef);
            }
            if (audioSourceRef != null && audioRef !=null)
            {
                worldAudioRef = audioSourceRef;
                worldAudioRef.clip = audioRef;
            }
            setupComplete = true;
        }
        
        /// <summary>
        /// Apply and play UI Changes
        /// </summary>
        /// <param name="iconRef"></param>
        /// <param name="textRef"></param>
        /// <param name="fontRef"></param>
        /// <param name="audioRef"></param>
        /// <param name="UseCanvas"></param>
        public bool ApplyUIChanges(Sprite iconRef, string textRef, FontSetting fontRef, AudioClip audioRef=null, bool UseCanvas = false, bool useOffset = false)
        {
            if (!setupComplete)
            {
                Debug.LogError($"Have you called setup yet?");
                return false;
            }
            if (useCanvas)
            {
                if (buttonCanvasImage != null)
                    buttonCanvasImage.sprite = iconRef;
            }
            else
            {
                //update the sprite
                if (buttonIconImage != null)
                    buttonIconImage.sprite = iconRef;

            }

            // Update Label Text
            if (buttonLabelText != null)
            {
                buttonLabelText.text = textRef;
                FP_UtilityData.ApplyFontSetting(buttonLabelText, fontRef);
            }
            //update audio
            if (worldAudioRef!=null && audioRef!=null)
            {
                worldAudioRef.clip = audioRef;
                worldAudioRef.Play();
            }
            if (useOffset)
            {
                AdjustDownLocation();
            }
            else
            {
                AdjustOriginalLocation();
            }
                return true;
        }
        protected void AdjustDownLocation()
        {
            localButtonRootParent.localPosition = new Vector3(OffsetAmount.x* storedLocalScaleAdj.x,OffsetAmount.y*storedLocalScaleAdj.y,OffsetAmount.z*storedLocalScaleAdj.z);
        }
        protected void AdjustOriginalLocation()
        {
            localButtonRootParent.localPosition = Vector3.zero;
        }
    }
}
