using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using DevExpress.Mvvm;
using DevExpress.Mvvm.UI;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shell;
using System.Windows.Data;
using System.Collections.ObjectModel;
using DevExpress.Mvvm.UI.Interactivity;
using System.Windows.Controls;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Native;

namespace DevExpress.Mvvm.UI {
    [TargetTypeAttribute(typeof(UserControl))]
    [TargetTypeAttribute(typeof(Window))]
    public class TaskbarButtonService : ServiceBase, ITaskbarButtonService {
        #region
        public static readonly DependencyProperty ProgressStateProperty =
            DependencyProperty.Register("ProgressState", typeof(TaskbarItemProgressState), typeof(TaskbarButtonService), new PropertyMetadata(TaskbarItemProgressState.None,
                (d, e) => ((TaskbarButtonService)d).OnProgressStateChanged(e)));
        [IgnoreDependencyPropertiesConsistencyCheckerAttribute]
        static readonly DependencyProperty ItemInfoProgressStateProperty =
            DependencyProperty.Register("ItemInfoProgressState", typeof(TaskbarItemProgressState), typeof(TaskbarButtonService), new PropertyMetadata(TaskbarItemProgressState.None,
                (d, e) => ((TaskbarButtonService)d).OnItemInfoProgressStateChanged(e)));
        public static readonly DependencyProperty ProgressValueProperty =
            DependencyProperty.Register("ProgressValue", typeof(double), typeof(TaskbarButtonService), new PropertyMetadata(0.0,
                (d, e) => ((TaskbarButtonService)d).OnProgressValueChanged(e)));
        [IgnoreDependencyPropertiesConsistencyCheckerAttribute]
        static readonly DependencyProperty ItemInfoProgressValueProperty =
            DependencyProperty.Register("ItemInfoProgressValue", typeof(double), typeof(TaskbarButtonService), new PropertyMetadata(0.0,
                (d, e) => ((TaskbarButtonService)d).OnItemInfoProgressValueChanged(e)));
        public static readonly DependencyProperty OverlayIconProperty =
            DependencyProperty.Register("OverlayIcon", typeof(ImageSource), typeof(TaskbarButtonService), new PropertyMetadata(null,
                (d, e) => ((TaskbarButtonService)d).OnOverlayIconChanged(e)));
        [IgnoreDependencyPropertiesConsistencyCheckerAttribute]
        static readonly DependencyProperty ItemInfoOverlayIconProperty =
            DependencyProperty.Register("ItemInfoOverlayIcon", typeof(ImageSource), typeof(TaskbarButtonService), new PropertyMetadata(null,
                (d, e) => ((TaskbarButtonService)d).OnItemInfoOverlayIconChanged(e)));
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(TaskbarButtonService), new PropertyMetadata("",
                (d, e) => ((TaskbarButtonService)d).OnDescriptionChanged(e)));
        [IgnoreDependencyPropertiesConsistencyCheckerAttribute]
        static readonly DependencyProperty ItemInfoDescriptionProperty =
            DependencyProperty.Register("ItemInfoDescription", typeof(string), typeof(TaskbarButtonService), new PropertyMetadata("",
                (d, e) => ((TaskbarButtonService)d).OnItemInfoDescriptionChanged(e)));
        public static readonly DependencyProperty ThumbButtonInfosProperty =
            DependencyProperty.Register("ThumbButtonInfos", typeof(TaskbarThumbButtonInfoCollection), typeof(TaskbarButtonService), new PropertyMetadata(null,
                (d, e) => ((TaskbarButtonService)d).OnThumbButtonInfosChanged(e)));
        [IgnoreDependencyPropertiesConsistencyCheckerAttribute]
        static readonly DependencyProperty ItemInfoThumbButtonInfosProperty =
            DependencyProperty.Register("ItemInfoThumbButtonInfos", typeof(ThumbButtonInfoCollection), typeof(TaskbarButtonService), new PropertyMetadata(null,
                (d, e) => ((TaskbarButtonService)d).OnItemInfoThumbButtonInfosChanged(e)));
        public static readonly DependencyProperty ThumbnailClipMarginProperty =
            DependencyProperty.Register("ThumbnailClipMargin", typeof(Thickness), typeof(TaskbarButtonService), new PropertyMetadata(new Thickness(),
                (d, e) => ((TaskbarButtonService)d).OnThumbnailClipMarginChanged(e)));
        [IgnoreDependencyPropertiesConsistencyCheckerAttribute]
        static readonly DependencyProperty ItemInfoThumbnailClipMarginProperty =
            DependencyProperty.Register("ItemInfoThumbnailClipMargin", typeof(Thickness), typeof(TaskbarButtonService), new PropertyMetadata(new Thickness(),
                (d, e) => ((TaskbarButtonService)d).OnItemInfoThumbnailClipMarginChanged(e)));
        public static readonly DependencyProperty WindowProperty =
            DependencyProperty.Register("Window", typeof(Window), typeof(TaskbarButtonService), new PropertyMetadata(null,
                (d, e) => ((TaskbarButtonService)d).OnWindowChanged(e)));
        static readonly DependencyPropertyKey ActualWindowPropertyKey =
            DependencyProperty.RegisterReadOnly("ActualWindow", typeof(Window), typeof(TaskbarButtonService), new PropertyMetadata(null,
                (d, e) => ((TaskbarButtonService)d).OnActualWindowChanged(e)));
        public static readonly DependencyProperty ActualWindowProperty = ActualWindowPropertyKey.DependencyProperty;
        [IgnoreDependencyPropertiesConsistencyCheckerAttribute]
        static readonly DependencyProperty WindowItemInfoProperty =
            DependencyProperty.Register("WindowItemInfo", typeof(TaskbarItemInfo), typeof(TaskbarButtonService), new PropertyMetadata(null,
                (d, e) => ((TaskbarButtonService)d).OnWindowItemInfoChanged(e)));
        public static readonly DependencyProperty ThumbnailClipMarginCallbackProperty =
            DependencyProperty.Register("ThumbnailClipMarginCallback", typeof(Func<Size, Thickness>), typeof(TaskbarButtonService), new PropertyMetadata(null,
                (d, e) => ((TaskbarButtonService)d).OnThumbnailClipMarginCallbackChanged(e)));
        #endregion

        TaskbarItemInfo itemInfo;
        bool associatedObjectIsLoaded = false;

        public TaskbarButtonService() {
            ItemInfo = new TaskbarItemInfo();
        }
        public TaskbarItemProgressState ProgressState { get { return (TaskbarItemProgressState)GetValue(ProgressStateProperty); } set { SetValue(ProgressStateProperty, value); } }
        public double ProgressValue { get { return (double)GetValue(ProgressValueProperty); } set { SetValue(ProgressValueProperty, value); } }
        public ImageSource OverlayIcon { get { return (ImageSource)GetValue(OverlayIconProperty); } set { SetValue(OverlayIconProperty, value); } }
        public string Description { get { return (string)GetValue(DescriptionProperty); } set { SetValue(DescriptionProperty, value); } }
        public TaskbarThumbButtonInfoCollection ThumbButtonInfos { get { return (TaskbarThumbButtonInfoCollection)GetValue(ThumbButtonInfosProperty); } set { SetValue(ThumbButtonInfosProperty, value); } }
        public Thickness ThumbnailClipMargin { get { return (Thickness)GetValue(ThumbnailClipMarginProperty); } set { SetValue(ThumbnailClipMarginProperty, value); } }
        public Func<Size, Thickness> ThumbnailClipMarginCallback { get { return (Func<Size, Thickness>)GetValue(ThumbnailClipMarginCallbackProperty); } set { SetValue(ThumbnailClipMarginCallbackProperty, value); } }
        public Window Window { get { return (Window)GetValue(WindowProperty); } set { SetValue(WindowProperty, value); } }
        public Window ActualWindow { get { return (Window)GetValue(ActualWindowProperty); } private set { SetValue(ActualWindowPropertyKey, value); } }
        public void UpdateThumbnailClipMargin() {
            if(ActualWindow != null && ThumbnailClipMarginCallback != null)
                ThumbnailClipMargin = ThumbnailClipMarginCallback(new Size(ActualWindow.Width, ActualWindow.Height));
        }
        protected override Freezable CreateInstanceCore() { return this; }
        protected virtual void OnProgressStateChanged(DependencyPropertyChangedEventArgs e) {
            ItemInfo.ProgressState = (TaskbarItemProgressState)e.NewValue;
        }
        protected virtual void OnProgressValueChanged(DependencyPropertyChangedEventArgs e) {
            ItemInfo.ProgressValue = (double)e.NewValue;
        }
        protected virtual void OnOverlayIconChanged(DependencyPropertyChangedEventArgs e) {
            ItemInfo.Overlay = (ImageSource)e.NewValue;
        }
        protected virtual void OnDescriptionChanged(DependencyPropertyChangedEventArgs e) {
            ItemInfo.Description = (string)e.NewValue;
        }
        protected virtual void OnThumbnailClipMarginChanged(DependencyPropertyChangedEventArgs e) {
            ItemInfo.ThumbnailClipMargin = (Thickness)e.NewValue;
        }
        protected virtual void OnThumbnailClipMarginCallbackChanged(DependencyPropertyChangedEventArgs e) {
            UpdateThumbnailClipMargin();
        }
        protected virtual void OnWindowSizeChanged(object sender, SizeChangedEventArgs e) {
            UpdateThumbnailClipMargin();
        }
        protected override void OnAttached() {
            base.OnAttached();
            AssociatedObject.Loaded += OnAssociatedObjectLoaded;
            AssociatedObject.Unloaded += OnAssociatedObjectUnloaded;
            if(AssociatedObject.IsLoaded)
                OnAssociatedObjectLoaded(AssociatedObject, null);
        }
        protected override void OnDetaching() {
            base.OnDetaching();
            AssociatedObject.Loaded -= OnAssociatedObjectLoaded;
            AssociatedObject.Unloaded -= OnAssociatedObjectUnloaded;
            if(AssociatedObject.IsLoaded)
                OnAssociatedObjectUnloaded(AssociatedObject, null);
        }
        TaskbarItemInfo ItemInfo {
            get { return itemInfo; }
            set {
                if(itemInfo == value) return;
                itemInfo = value;
                BindingOperations.SetBinding(this, ItemInfoProgressStateProperty, new Binding("ProgressState") { Source = itemInfo, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                BindingOperations.SetBinding(this, ItemInfoProgressValueProperty, new Binding("ProgressValue") { Source = itemInfo, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                BindingOperations.SetBinding(this, ItemInfoOverlayIconProperty, new Binding("Overlay") { Source = itemInfo, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                BindingOperations.SetBinding(this, ItemInfoDescriptionProperty, new Binding("Description") { Source = itemInfo, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                BindingOperations.SetBinding(this, ItemInfoThumbnailClipMarginProperty, new Binding("ThumbnailClipMargin") { Source = itemInfo, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                BindingOperations.SetBinding(this, ItemInfoThumbButtonInfosProperty, new Binding("ThumbButtonInfos") { Source = itemInfo, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            }
        }
        void OnItemInfoProgressStateChanged(DependencyPropertyChangedEventArgs e) {
            ProgressState = (TaskbarItemProgressState)e.NewValue;
        }
        void OnItemInfoProgressValueChanged(DependencyPropertyChangedEventArgs e) {
            ProgressValue = (double)e.NewValue;
        }
        void OnItemInfoOverlayIconChanged(DependencyPropertyChangedEventArgs e) {
            OverlayIcon = (ImageSource)e.NewValue;
        }
        void OnItemInfoDescriptionChanged(DependencyPropertyChangedEventArgs e) {
            Description = (string)e.NewValue;
        }
        protected virtual void OnThumbButtonInfosChanged(DependencyPropertyChangedEventArgs e) {
            TaskbarThumbButtonInfoCollection collection = (TaskbarThumbButtonInfoCollection)e.NewValue;
            ItemInfo.ThumbButtonInfos = collection.InternalCollection;
        }
        void OnItemInfoThumbButtonInfosChanged(DependencyPropertyChangedEventArgs e) {
            ThumbButtonInfos = new TaskbarThumbButtonInfoCollection((ThumbButtonInfoCollection)e.NewValue);
        }
        void OnItemInfoThumbnailClipMarginChanged(DependencyPropertyChangedEventArgs e) {
            ThumbnailClipMargin = (Thickness)e.NewValue;
        }
        void OnWindowChanged(DependencyPropertyChangedEventArgs e) {
            UpdateActualWindow();
        }
        void OnActualWindowChanged(DependencyPropertyChangedEventArgs e) {
            Window oldWindow = (Window)e.OldValue;
            if(oldWindow != null)
                oldWindow.SizeChanged -= OnWindowSizeChanged;
            Window window = (Window)e.NewValue;
            if(window == null) {
                BindingOperations.ClearBinding(this, WindowItemInfoProperty);
                ItemInfo = new TaskbarItemInfo();
                return;
            }
            if(window.TaskbarItemInfo == null) {
                window.TaskbarItemInfo = ItemInfo;
            } else {
                window.TaskbarItemInfo.ProgressState = ItemInfo.ProgressState;
                window.TaskbarItemInfo.ProgressValue = ItemInfo.ProgressValue;
                window.TaskbarItemInfo.Description = ItemInfo.Description;
                window.TaskbarItemInfo.Overlay = ItemInfo.Overlay;
                window.TaskbarItemInfo.ThumbButtonInfos = ItemInfo.ThumbButtonInfos;
                window.TaskbarItemInfo.ThumbnailClipMargin = ItemInfo.ThumbnailClipMargin;
                ItemInfo = window.TaskbarItemInfo;
            }
            BindingOperations.SetBinding(this, WindowItemInfoProperty, new Binding("TaskbarItemInfo") { Source = window, Mode = BindingMode.TwoWay, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            window.SizeChanged -= OnWindowSizeChanged;
            window.SizeChanged += OnWindowSizeChanged;
            OnWindowSizeChanged(window, null);
        }
        void OnWindowItemInfoChanged(DependencyPropertyChangedEventArgs e) {
            if(ActualWindow == null) return;
            TaskbarItemInfo itemInfo = (TaskbarItemInfo)e.NewValue;
            if(itemInfo == null) {
                itemInfo = new TaskbarItemInfo();
                ActualWindow.TaskbarItemInfo = itemInfo;
            }
            ItemInfo = itemInfo;
        }
        void OnAssociatedObjectLoaded(object sender, RoutedEventArgs e) {
            associatedObjectIsLoaded = true;
            UpdateActualWindow();
        }
        void OnAssociatedObjectUnloaded(object sender, RoutedEventArgs e) {
            associatedObjectIsLoaded = false;
            UpdateActualWindow();
        }
        void UpdateActualWindow() {
            ActualWindow = Window ?? (!associatedObjectIsLoaded || AssociatedObject == null ? null : Window.GetWindow(AssociatedObject));
        }
        IList<TaskbarThumbButtonInfo> ITaskbarButtonService.ThumbButtonInfos { get { return ThumbButtonInfos; } }
    }
}