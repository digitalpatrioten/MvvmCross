﻿// MvxAndroidViewsContainer.cs
// (c) Copyright Cirrious Ltd. http://www.cirrious.com
// MvvmCross is licensed using Microsoft Public License (Ms-PL)
// Contributions and inspirations noted in readme.md and license.txt
// 
// Project Lead - Stuart Lodge, @slodge, me@slodge.com

using System;
using Android.Content;
using Cirrious.CrossCore.Exceptions;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Views;
using System.Collections.Generic;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentActivity = Android.Support.V4.App.FragmentActivity;
using FragmentTransaction = Android.Support.V4.App.FragmentTransaction;
using Android.App;

namespace Cirrious.MvvmCross.Droid.Views
{
    public class MvxAndroidViewsContainer
        : MvxViewsContainer
         , IMvxAndroidViewsContainer
    {
        private const string ExtrasKey = "MvxLaunchData";
        private const string SubViewModelKey = "MvxSubViewModelKey";

        private readonly Context _applicationContext;

        public MvxAndroidViewsContainer(Context applicationContext)
        {
            _applicationContext = applicationContext;
        }

        #region Implementation of IMvxAndroidViewModelRequestTranslator

        public virtual IMvxViewModel Load(Intent intent, IMvxBundle savedState)
        {
            return Load(intent, null, null);
        }

        public virtual IMvxViewModel LoadWithParameters(IDictionary<string, string> parameterValue, IMvxBundle savedState, Type viewModelTypeHint)
        {
            return CreateViewModelFromType(parameterValue, viewModelTypeHint);
        }

        public virtual IMvxViewModel Load(Intent intent, IMvxBundle savedState, Type viewModelTypeHint)
        {                
            if (intent.Action == Intent.ActionMain) {
                MvxTrace.Trace("Creating ViewModel for ActionMain");
                return DirectLoad(savedState, viewModelTypeHint);
            }

            if (intent.Extras == null) {
                MvxTrace.Trace("Null Extras seen on Intent when creating ViewModel - have you tried to navigate to an MvvmCross View directly? Will try direct load");
                return DirectLoad(savedState, viewModelTypeHint);
            }

            IMvxViewModel mvxViewModel;
            if (TryGetEmbeddedViewModel(intent, out mvxViewModel)) {
                MvxTrace.Trace("Embedded ViewModel used");
                return mvxViewModel;
            }

            MvxTrace.Trace("Attempting to load new ViewModel from Intent with Extras");
            var toReturn = CreateViewModelFromIntent(intent, savedState);
            if (toReturn != null)
                return toReturn;

            MvxTrace.Trace("ViewModel not loaded from Extras - will try DirectLoad");
            return DirectLoad(savedState, viewModelTypeHint);
        }

        protected virtual IMvxViewModel DirectLoad(IMvxBundle savedState, Type viewModelTypeHint)
        {
            if (viewModelTypeHint == null)
            {
                Mvx.Error("Unable to load viewmodel - no type hint provided");
                return null;
            }

            var loaderService = Mvx.Resolve<IMvxViewModelLoader>();
            var viewModelRequest = MvxViewModelRequest.GetDefaultRequest(viewModelTypeHint);
            var viewModel = loaderService.LoadViewModel(viewModelRequest, savedState);
            return viewModel;
        }

        protected virtual IMvxViewModel CreateViewModelFromType(IDictionary<string, string> parameterValue, Type type)
        {
            MvxViewModelRequest viewModelRequest = new MvxViewModelRequest();
            viewModelRequest.ParameterValues = parameterValue;
            viewModelRequest.PresentationValues = null;
            viewModelRequest.RequestedBy = null;
            viewModelRequest.ViewModelType = type;

            return ViewModelFromRequest(viewModelRequest, null);
        }

        protected virtual IMvxViewModel CreateViewModelFromIntent(Intent intent, IMvxBundle savedState)
        {
            var extraData = intent.Extras.GetString(ExtrasKey);
            if (extraData == null)
                return null;

            var converter = Mvx.Resolve<IMvxNavigationSerializer>();
            var viewModelRequest = converter.Serializer.DeserializeObject<MvxViewModelRequest>(extraData);

            return ViewModelFromRequest(viewModelRequest, savedState);
        }

        protected virtual IMvxViewModel ViewModelFromRequest(MvxViewModelRequest viewModelRequest, IMvxBundle savedState)
        {
            var loaderService = Mvx.Resolve<IMvxViewModelLoader>();
            var viewModel = loaderService.LoadViewModel(viewModelRequest, savedState);
            return viewModel;
        }

        protected virtual bool TryGetEmbeddedViewModel(Intent intent, out IMvxViewModel mvxViewModel)
        {
            var embeddedViewModelKey = intent.Extras.GetInt(SubViewModelKey);
            if (embeddedViewModelKey != 0)
            {
                {
                    mvxViewModel = Mvx.Resolve<IMvxChildViewModelCache>().Get(embeddedViewModelKey);
                    return true;
                }
            }
            mvxViewModel = null;
            return false;
        }

        public ViewDetails IdentifyView(MvxViewModelRequest request)
        {
            ViewDetails details = new ViewDetails();

            var viewType = GetViewType(request.ViewModelType);
            if (typeof(Fragment).IsAssignableFrom(viewType))
            {
                details.category = ViewCategory.Fragment;
            }
            else if (typeof(Activity).IsAssignableFrom(viewType) || typeof(FragmentActivity).IsAssignableFrom(viewType))
            {
                details.category = ViewCategory.Activity;
            }
            details.type = viewType;

            return details;
        }

        public virtual Intent GetIntentFor(MvxViewModelRequest request)
        {
            var viewType = GetViewType(request.ViewModelType);
            if (viewType == null)
            {
                throw new MvxException("View Type not found for " + request.ViewModelType);
            }

            var converter = Mvx.Resolve<IMvxNavigationSerializer>();
            var requestText = converter.Serializer.SerializeObject(request);

            var intent = new Intent(_applicationContext, viewType);
            intent.PutExtra(ExtrasKey, requestText);

            AdjustIntentForPresentation(intent, request);

            return intent;
        }

        protected virtual void AdjustIntentForPresentation(Intent intent, MvxViewModelRequest request)
        {
            intent.AddFlags(ActivityFlags.NewTask);

#warning we want to do things here... clear top, remove history item, etc
//#warning ClearTop is not enough :/ Need to work on an Intent based scheme like http://stackoverflow.com/questions/3007998/on-logout-clear-activity-history-stack-preventing-back-button-from-opening-l
//            if (request.ClearTop)
//                intent.AddFlags(ActivityFlags.ClearTop);
        }

        public virtual Tuple<Intent, int> GetIntentWithKeyFor(IMvxViewModel viewModel)
        {
            var request = MvxViewModelRequest.GetDefaultRequest(viewModel.GetType());
            var intent = GetIntentFor(request);

            var key = Mvx.Resolve<IMvxChildViewModelCache>().Cache(viewModel);
            intent.PutExtra(SubViewModelKey, key);

            return new Tuple<Intent, int>(intent, key);
        }

        public void RemoveSubViewModelWithKey(int key)
        {
            Mvx.Resolve<IMvxChildViewModelCache>().Remove(key);
        }

        #endregion
    }
}