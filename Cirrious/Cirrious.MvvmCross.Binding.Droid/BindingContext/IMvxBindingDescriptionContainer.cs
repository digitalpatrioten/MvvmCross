// IMvxEventSourceActivity.cs
// (c) Copyright Cirrious Ltd. http://www.cirrious.com
// MvvmCross is licensed using Microsoft Public License (Ms-PL)
// Contributions and inspirations noted in readme.md and license.txt
// 
// Project Lead - Stuart Lodge, @slodge, me@slodge.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Cirrious.MvvmCross.Binding.Droid.BindingContext
{
    public interface IMvxBindingDescriptionContainer
    {
        Dictionary<int, String> BindingDescriptions { get; }
        void Bind(int viewId, string bindingDescription);
    }
}

