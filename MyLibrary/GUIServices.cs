using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace MyLibrary
{
    static class GUIServices
    {
        /// <summary>
        ///   A value indicating whether the UI is currently busy
        /// </summary>
        private static bool IsBusy;

        /// <summary>
        /// Sets the BusyState as busy.
        /// </summary>
        public static void SetBusyState()
        {
            SetBusyState(true);
        }

        /// <summary>
        /// Sets the BusyState to busy or not busy.
        /// </summary>
        /// <param name="isBusy">if set to <c>true</c> the application is now busy.</param>
        private static void SetBusyState(bool isBusy)
        {
            if (isBusy != IsBusy)
            {
                IsBusy = isBusy;
                Mouse.OverrideCursor = isBusy ? Cursors.Wait : null;

                if (IsBusy)
                {
                    new DispatcherTimer(TimeSpan.FromSeconds(0), DispatcherPriority.ApplicationIdle,
                        dispatcherTimer_Tick, Application.Current.Dispatcher);
                }
            }
        }

        /// <summary>
        /// Handles the Tick event of the dispatcherTimer control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
        private static void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            var dispatcherTimer = sender as DispatcherTimer;
            if (dispatcherTimer != null)
            {
                SetBusyState(false);
                dispatcherTimer.Stop();
            }
        }

        public static void SetListControlBinding(
            Control uiElement,
            IList list,
            BindingMode mode = BindingMode.OneWay,
            IValueConverter converter = null,
            object converterParameter = null,
            IComparer comparer = null)
        {
            Binding binding = new Binding()
            {
                Source = new ListCollectionView(list),
                Mode = mode,
                Converter = converter,
                ConverterParameter = converterParameter
            };

            if(binding.Source != null)
                (binding.Source as ListCollectionView).CustomSort = comparer;

            uiElement.SetBinding(ItemsControl.ItemsSourceProperty, binding);
        }
    }
}
