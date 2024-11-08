using UnityEngine;

namespace FuzzPhyte.XR
{
    public class FPEventWrapperExample : MonoBehaviour, IFPXREventWrapper
    {
        protected IFPXREventBinder eventBinder;

        
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
