using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace XamCam
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        #region Fields
        bool isInternetConnectionActive;
        #endregion

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Properties
        public bool IsInternetConnectionActive
        {
            get { return isInternetConnectionActive; }
            set { SetProperty(ref isInternetConnectionActive, value); }
        }
        #endregion

        #region Methods
        protected void SetProperty<T>(ref T backingStore, T value, [CallerMemberName]string propertyName = "", Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return;
            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
        }

        void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
    }
}