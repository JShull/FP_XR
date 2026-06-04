// Copyright (c) 2026 John B. Shull
// FuzzPhyte LLC is a company associated with John B. Shull
// This file is part of FP_XR Package.
//
// Public license: GNU GPLv3-or-later.
// Commercial/proprietary use requires a separate license from John B. Shull.
//
// See LICENSE.md COMMERCIAL-LICENSE.md, and NOTICE.md.

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
