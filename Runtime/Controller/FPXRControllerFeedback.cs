namespace FuzzPhyte.XR
{
    using FuzzPhyte.Utility;
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;

    public class FPXRControllerFeedback : MonoBehaviour
    {
        [Header("Config Data")]
        [SerializeField] protected FPXRControllerFeedbackConfig feedbackConfig;
        [SerializeField] protected AudioSource controllerAudioSource;
        [Space]
        [Header("UI References")]
        public bool UseCanvas=false;

        [SerializeField] protected Image buttonCanvasImage;
        [SerializeField] protected SpriteRenderer buttonIconImage;
        [SerializeField] protected TMP_Text buttonLabelText;
        [Space]
        [Header("Not being Used Yet")]
        [Tooltip("This isn't being used yet")]
        [SerializeField] protected SpriteRenderer buttonBackgroundImage;
        [SerializeField] protected Image buttonCanvasBackgroundImage;
        

        private XRHandedness controllerHandedness;

        protected virtual void Awake()
        {
            controllerHandedness = feedbackConfig.ControllerStartHandedness;
        }

        [ContextMenu("Testing Primary, Select")]
        public void PrimaryControllerButtonDown()
        {
            SetButtonActive(XRButton.PrimaryButton, XRInteractionStatus.Select);
        }
        [ContextMenu("Testing Primary A, Unselect")]
        public void PrimaryControllerButtonUp()
        {
            SetButtonActive(XRButton.PrimaryButton, XRInteractionStatus.Unselect);
        }
        [ContextMenu("Testing Secondary, Select")]
        public void SecondaryControllerButtonDown()
        {
            SetButtonActive(XRButton.SecondaryButton, XRInteractionStatus.Select);
        }
        [ContextMenu("Testing Secondary, Unselect")]
        public void SecondaryControllerButtonUp()
        {
            SetButtonActive(XRButton.SecondaryButton, XRInteractionStatus.Unselect);
        }
        public virtual void SetButtonActive(XRButton button, XRInteractionStatus currentButtonStatus)
        {
            ButtonFeedback? feedback = feedbackConfig.feedbacks.Find(f => f.Button == button);
            if (feedback.HasValue)
            {
                var fb = feedback.Value;

                // Update icon
                ButtonLabelState? cachedLabelDetails= fb.ButtonInteractionStates.Find(l => l.XRState == currentButtonStatus);
                if (cachedLabelDetails.HasValue)
                {
                    var label = cachedLabelDetails.Value;
                    if (UseCanvas)
                    {
                        if(buttonCanvasImage != null)
                            buttonCanvasImage.sprite = label.Icon;
                    }
                    else
                    {
                        //update the sprite
                        if (buttonIconImage != null)
                            buttonIconImage.sprite = label.Icon;

                    }

                    // Update Label Text
                    if (buttonLabelText != null)
                    {
                        buttonLabelText.text = label.LabelText;
                        FP_UtilityData.ApplyFontSetting(buttonLabelText, label.LabelFontSetting);
                    }
                    if (controllerAudioSource != null)
                    {
                        controllerAudioSource.clip = label.ButtonSound;
                        controllerAudioSource.Play();
                    }
                }               
            }
        }

        
    }
}
