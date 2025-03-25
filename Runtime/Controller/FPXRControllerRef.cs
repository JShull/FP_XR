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
        [Header("Additional Visual Item")]
        [SerializeField] protected SpriteRenderer buttonAdditionalImage;
        [SerializeField] protected Image buttonAdditionalCanvasImage;

        #region Main Utility Functions for UI Updates
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
        
        /// <summary>
        /// Apply UI changes for secondary visual
        /// </summary>
        /// <param name="iconRef">sprite icon we want to utilize</param>
        /// <returns></returns>
        public bool ApplyUISecondaryVisual(Sprite iconRef)
        {
            if (!setupComplete)
            {
                Debug.LogError($"Have you called setup yet?");
                return false;
            }
            if (useCanvas)
            {
                if (buttonAdditionalCanvasImage != null)
                {
                    buttonAdditionalCanvasImage.sprite = iconRef;
                }
            }
            else
            {
                if (buttonAdditionalImage != null)
                {
                    buttonAdditionalImage.sprite = iconRef;
                }
            }
            return true;
        }
        #endregion
        #region Show Hide Public Functions
        /// <summary>
        /// Will hide/show visuals based on parameter
        /// </summary>
        /// <param name="visualsActive">Show visuals?</param>
        public void ShowORHideVisuals(bool visualsActive)
        {
            if (!setupComplete)
            {
                return;
            }
            if (useCanvas)
            {
                if (buttonCanvasImage != null)
                {
                    buttonCanvasImage.enabled = visualsActive;
                }
            }
            else
            {
                //update the sprite
                if (buttonIconImage != null)
                {
                    buttonIconImage.enabled = visualsActive;
                }
            }
        }
        /// <summary>
        /// Show or hide secondary visual
        /// </summary>
        /// <param name="visualsActive"></param>
        public void ShowORHideSecondaryVisual(bool visualsActive)
        {
            if (!setupComplete)
            {
                return;
            }
            if (useCanvas)
            {
                if (buttonAdditionalCanvasImage != null)
                {
                    buttonAdditionalCanvasImage.enabled = visualsActive;
                }
            }
            else
            {
                if (buttonAdditionalImage != null)
                {
                    buttonAdditionalImage.enabled = visualsActive;
                }
            }
        }
        /// <summary>
        /// Show or hide text based on the parameter passed
        /// </summary>
        /// <param name="textActive">Show Text?</param>
        public void ShowORHideText(bool textActive)
        {
            if (!setupComplete)
            {
                return;
            }
            if (buttonLabelText != null)
            {
                buttonLabelText.enabled = textActive;
            }   
        }
        /// <summary>
        /// Changes the value of the text draw order on the renderer component for the text
        /// </summary>
        /// <param name="passedDrawOrder">Order?</param>
        public void ChangeTextDrawOrder(int passedDrawOrder)
        {
            if (buttonLabelText != null)
            {
                var renderer = buttonLabelText.gameObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.sortingOrder = passedDrawOrder;
                }
            }
        }
        /// <summary>
        /// Changes the value of the sprite draw order on the renderer component
        /// </summary>
        /// <param name="passedDrawOrder">Order?</param>
        public void ChangeVisualDrawOrder(int passedDrawOrder)
        {
            
            if (useCanvas)
            {
                if (buttonCanvasImage != null)
                {
                    var renderer = buttonCanvasImage.gameObject.GetComponent<Renderer>();
                    if(renderer != null)
                    {
                        renderer.sortingOrder = passedDrawOrder;
                    }
                }
            }
            else
            {
                //update the sprite
                if (buttonIconImage != null)
                {
                    var renderer = buttonIconImage.gameObject.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.sortingOrder = passedDrawOrder;
                    }
                }
            }
        }
        /// <summary>
        /// Changes the value of the sprite draw order on the secondary renderer component
        /// </summary>
        /// <param name="passedDrawOrder">Order?</param>
        public void ChangeSecondaryVisualDrawOrder(int passedDrawOrder)
        {

            if (useCanvas)
            {
                if (buttonAdditionalCanvasImage != null)
                {
                    var renderer = buttonAdditionalCanvasImage.gameObject.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.sortingOrder = passedDrawOrder;
                    }
                }
            }
            else
            {
                //update the sprite
                if (buttonAdditionalImage != null)
                {
                    var renderer = buttonAdditionalImage.gameObject.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        renderer.sortingOrder = passedDrawOrder;
                    }
                }
            }
        }
        #endregion
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