// IMvxAndroidViewModelLoader.cs
// (c) Copyright Cirrious Ltd. http://www.cirrious.com
// MvvmCross is licensed using Microsoft Public License (Ms-PL)
// Contributions and inspirations noted in readme.md and license.txt
// 
// Project Lead - Stuart Lodge, @slodge, me@slodge.com

using System;
using Android.Content;
using Cirrious.MvvmCross.ViewModels;
using System.Collections.Generic;

namespace Cirrious.MvvmCross.Droid.Views
{
    public interface IMvxAndroidViewModelLoader
    {
        IMvxViewModel Load(Intent intent, IMvxBundle savedState);
        IMvxViewModel Load(Intent intent, IMvxBundle savedState, Type viewModelTypeHint);
        IMvxViewModel LoadWithParameters(IDictionary<string, string> paramaterValues, IMvxBundle savedState, Type viewModelTypeHint);
    }
}