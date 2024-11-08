using UnityEngine;

namespace FuzzPhyte.XR
{
    public class FPEventWrapperExample : MonoBehaviour, IFPXREventWrapper
    {
        protected IFPXREventBinder eventBinder;

        // on an extension class I would create a IFPXREventBinder tied to OVR
        // I would then in start/awake call Initialize with the OVR event binder
        /*
         * public void Start()
         * {
         * Initialize(new OVREventBinder(this));
         * }
         */
        public virtual void Initialize(IFPXREventBinder eventBinder)
        {
            this.eventBinder = eventBinder;

            eventBinder.BindHover(OnHover);
            eventBinder.BindUnhover(OnUnhover);
            eventBinder.BindSelect(OnSelect);
            eventBinder.BindUnselect(OnUnselect);
            eventBinder.BindInteractorViewAdded(OnInteractorViewAdded);
            eventBinder.BindInteractorViewRemoved(OnInteractorViewRemoved);
            eventBinder.BindSelectingInteractorViewAdded(OnSelectingInteractorViewAdded);
            eventBinder.BindSelectingInteractorViewRemoved(OnSelectingInteractorViewRemoved);
        }
        public virtual void OnDestroy()
        {
            if (eventBinder != null)
            {
                eventBinder.UNBindHover(OnHover);
                eventBinder.UNBindUnHover(OnUnhover);
                eventBinder.UNBindSelect(OnSelect);
                eventBinder.UNBindUnselect(OnUnselect);
                eventBinder.UNBindInteractorViewAdded(OnInteractorViewAdded);
                eventBinder.UNBindInteractorViewRemoved(OnInteractorViewRemoved);
                eventBinder.UNBindSelectingInteractorViewAdded(OnSelectingInteractorViewAdded);
                eventBinder.UNBindSelectingInteractorViewRemoved(OnSelectingInteractorViewRemoved);
            }
        }
        public virtual void OnHover()
        {
            Debug.Log("Hovered");
        }
        public virtual void UNHover()
        {
            Debug.Log("Unhovered");
        }
        public virtual void OnUnhover()
        {
            Debug.Log("Unhovered");
        }
        public virtual void OnSelect()
        {
            Debug.Log("Selected");
        }
        public virtual void OnUnselect()
        {
            Debug.Log("Unselected");
        }
        public virtual void OnInteractorViewAdded()
        {
            Debug.Log("InteractorViewAdded");
        }
        public virtual void OnInteractorViewRemoved()
        {
            Debug.Log("InteractorViewRemoved");
        }
        public virtual void OnSelectingInteractorViewAdded()
        {
            Debug.Log("SelectingInteractorViewAdded");
        }
        public virtual void OnSelectingInteractorViewRemoved()
        {
            Debug.Log("SelectingInteractorViewRemoved");
        }
    }
}
