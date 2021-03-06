// MvxFragmentActivity.cs
// (c) Copyright Cirrious Ltd. http://www.cirrious.com
// MvvmCross is licensed using Microsoft Public License (Ms-PL)
// Contributions and inspirations noted in readme.md and license.txt
// 
// Project Lead - Stuart Lodge, @slodge, me@slodge.com

using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Content;
using Cirrious.CrossCore.Droid.Views;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Binding.Droid.BindingContext;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Droid.Views;
using Cirrious.MvvmCross.Droid.Fragging.Fragments.EventSource;

namespace Cirrious.MvvmCross.Droid.Fragging.Fragments
{
    public abstract class MvxFragment
        : MvxEventSourceFragment
          , IMvxAndroidView
          , IMvxParameterValuesContainer
    {
        Activity _fragmentActivity;
        public Activity FragmentActivity
        {
            get { return _fragmentActivity; }
            set { _fragmentActivity = value; }
        }

        int _layoutId;
        public int LayoutId
        {
            get { return _layoutId; }
            set { _layoutId = value; }
        }            

        public IDictionary<string, string> ParameterValues { get; set; }

        public MvxFragment(Activity activity)
        {
            this.FragmentActivity = activity;

            BindingContext = new MvxAndroidBindingContext(FragmentActivity, this);
            this.AddEventListeners();
        }

        public void Init(Activity activity)
        {
            BindingContext = new MvxAndroidBindingContext(FragmentActivity, this);
            this.AddEventListeners();
        }

        public LayoutInflater LayoutInflater
        {
            get {
                return this.Activity.LayoutInflater;
            }
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

        public IMvxBindingContext BindingContext { get; set; }

        public View BindingInflate(int layoutResId)
        {
            var view = this.BindingInflate(layoutResId, null);
            return view;
        }
        
        public View InflateWithBinding(int layoutResId, ViewGroup viewGroup, bool attachToParent) 
        {
            var view = this.BindingInflate(layoutResId, viewGroup, attachToParent);
            return view;
        }

        protected virtual void OnViewModelSet()
        {
        }
    }
}

