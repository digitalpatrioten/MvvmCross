// MvxFragmentActivity.cs
// (c) Copyright Cirrious Ltd. http://www.cirrious.com
// MvvmCross is licensed using Microsoft Public License (Ms-PL)
// Contributions and inspirations noted in readme.md and license.txt
// 
// Project Lead - Stuart Lodge, @slodge, me@slodge.com

using Android.Content;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Droid.BindingContext;
using Cirrious.MvvmCross.Droid.Views;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Droid.Fragging.EventSource;
using System.Collections.Generic;

namespace Cirrious.MvvmCross.Droid.Fragging
{
    public class MvxFragmentActivity
        : MvxEventSourceFragmentActivity
          , IMvxAndroidView
          , IMvxBindingDescriptionContainer
    {
        protected MvxFragmentActivity()
        {
            BindingContext = new MvxAndroidBindingContext(this, this);
            this.AddEventListeners();
        }

        public object DataContext
        {
            get { return BindingContext.DataContext; }
            set { BindingContext.DataContext = value; }
        }

        public IMvxViewModel ViewModel
        {
            get { return DataContext as IMvxViewModel; }
            set
            {
                DataContext = value;
                OnViewModelSet();
            }
        }

        public void MvxInternalStartActivityForResult(Intent intent, int requestCode)
        {
            base.StartActivityForResult(intent, requestCode);
        }

        protected virtual void OnViewModelSet()
        {
        }

        public IMvxBindingContext BindingContext { get; set; }

        public override void SetContentView(int layoutResId)
        {
            var view = this.BindingInflate(layoutResId, null);
            SetContentView(view);
        }

        #region IMvxBindingDescriptionContainer implementation
        public void Bind(int viewId, string bindingDescription)
        {
            throw new System.NotImplementedException();
        }
        public Dictionary<int, string> _bindingDescriptions;
        public Dictionary<int, string> BindingDescriptions
        {
            get
            {
                if (_bindingDescriptions == null)
                {
                    _bindingDescriptions = new Dictionary<int, string>();
                }
                return _bindingDescriptions;
            }
        }
        #endregion

        /*
         * When the ActionBar home button is pressed, the bindings are not reloaded
         * on the parent activity, this override forces the ActionBar home button
         * to trigger the same lifecycle behavior as the hardware button
         */
        public override bool OnOptionsItemSelected(Android.Views.IMenuItem item)
        {
            switch (item.ItemId) {
                // Respond to the action bar's Up/Home button
                case Android.Resource.Id.Home:
                    OnBackPressed();
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}