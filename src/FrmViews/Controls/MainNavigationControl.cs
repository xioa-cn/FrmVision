using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using FrmServices.ViewModel;

namespace FrmViews.Controls
{
    public partial class MainNavigationControl : UserControl
    {
        private readonly HashSet<ObservableCollection<NavigationMenuItemViewModel>> _observedCollections =
            new HashSet<ObservableCollection<NavigationMenuItemViewModel>>();
        private readonly HashSet<NavigationMenuItemViewModel> _observedMenuItems =
            new HashSet<NavigationMenuItemViewModel>();
        private ObservableCollection<NavigationMenuItemViewModel> _menuItems;

        public MainNavigationControl()
        {
            InitializeComponent();
            stopToolButton.Click += RaiseWorkflowPauseRequested;
            refreshToolButton.Click += RaiseWorkflowRestartRequested;
            toolTip.SetToolTip(stopToolButton, "暂停任务流");
            toolTip.SetToolTip(refreshToolButton, "重启任务流");
            SetActivePage(0);
            Disposed += (sender, args) => DetachMenuSubscriptions();
        }

        public event EventHandler HomeRequested;
        public event EventHandler RecordsRequested;
        public event EventHandler ParametersRequested;
        public event EventHandler WorkflowPauseRequested;
        public event EventHandler WorkflowRestartRequested;
        public event EventHandler LogoutRequested;
        public event EventHandler OpenConfigDirectoryRequested;

        public void BindMenuItems(ObservableCollection<NavigationMenuItemViewModel> menuItems)
        {
            if (menuItems == null) throw new ArgumentNullException(nameof(menuItems));
            if (ReferenceEquals(_menuItems, menuItems)) return;

            DetachMenuSubscriptions();
            _menuItems = menuItems;
            AttachMenuSubscriptions(_menuItems);
            RenderMenuItems();
        }

        public void SetActivePage(int pageIndex)
        {
            SetToolButtonSelected(homeToolButton, pageIndex == 0);
            SetToolButtonSelected(settingsToolButton, pageIndex == 1);
        }

        public void ApplyPermissions(bool isLoggedIn, bool canOperate,
            bool canConfigure)
        {
            stopToolButton.Enabled = canOperate;
            refreshToolButton.Enabled = canOperate;
            settingsToolButton.Enabled = canOperate;

            logoutToolButton.Enabled = isLoggedIn;
            openConfigDirectoryToolButton.Enabled = canConfigure;
        }

        private static void SetToolButtonSelected(Button button, bool selected)
        {
            button.BackColor = selected ? UiTheme.Primary : UiTheme.SurfaceMuted;
            button.FlatAppearance.BorderColor = button.BackColor;
            button.FlatAppearance.MouseOverBackColor = selected
                ? UiTheme.PrimaryHover
                : Color.FromArgb(239, 243, 248);
            button.FlatAppearance.MouseDownBackColor = selected
                ? Color.FromArgb(24, 68, 190)
                : Color.FromArgb(225, 232, 242);
            button.ForeColor = selected ? Color.White : UiTheme.Primary;
        }

        private void AttachMenuSubscriptions(
            ObservableCollection<NavigationMenuItemViewModel> menuItems)
        {
            if (!_observedCollections.Add(menuItems)) return;
            menuItems.CollectionChanged += MenuCollectionOnChanged;

            foreach (var menuItem in menuItems)
            {
                if (_observedMenuItems.Add(menuItem))
                    menuItem.PropertyChanged += MenuItemOnPropertyChanged;
                AttachMenuSubscriptions(menuItem.Children);
            }
        }

        private void DetachMenuSubscriptions()
        {
            foreach (var menuItems in _observedCollections)
                menuItems.CollectionChanged -= MenuCollectionOnChanged;
            foreach (var menuItem in _observedMenuItems)
                menuItem.PropertyChanged -= MenuItemOnPropertyChanged;

            _observedCollections.Clear();
            _observedMenuItems.Clear();
        }

        private void MenuCollectionOnChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            DetachMenuSubscriptions();
            if (_menuItems != null) AttachMenuSubscriptions(_menuItems);
            RenderMenuItems();
        }

        private void MenuItemOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RenderMenuItems();
        }

        private void RenderMenuItems()
        {
            menuStrip.SuspendLayout();
            try
            {
                while (menuStrip.Items.Count > 0)
                {
                    var oldItem = menuStrip.Items[0];
                    menuStrip.Items.RemoveAt(0);
                    oldItem.Dispose();
                }

                if (_menuItems == null) return;
                foreach (var menuItem in _menuItems)
                    menuStrip.Items.Add(CreateMenuItem(menuItem, true));
            }
            finally
            {
                menuStrip.ResumeLayout(true);
            }
        }

        private ToolStripMenuItem CreateMenuItem(
            NavigationMenuItemViewModel viewModel,
            bool topLevel)
        {
            var commandEnabled = viewModel.Command == null || viewModel.Command.CanExecute(null);
            var menuItem = new ToolStripMenuItem
            {
                AutoSize = true,
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Enabled = viewModel.IsEnabled && commandEnabled,
                ForeColor = UiTheme.Text,
                Name = "menuItem_" + viewModel.Key,
                Padding = topLevel
                    ? new Padding(10, 0, 10, 0)
                    : new Padding(10, 0, 18, 0),
                Tag = viewModel,
                Text = viewModel.Text,
                TextAlign = topLevel
                    ? ContentAlignment.MiddleCenter
                    : ContentAlignment.MiddleLeft,
                ToolTipText = viewModel.Text,
                Visible = viewModel.IsVisible
            };

            foreach (var child in viewModel.Children)
                menuItem.DropDownItems.Add(CreateMenuItem(child, false));

            if (menuItem.DropDownItems.Count > 0)
                ConfigureDropDown(menuItem);

            menuItem.Click += MenuItemOnClick;
            return menuItem;
        }

        private static void ConfigureDropDown(ToolStripMenuItem ownerItem)
        {
            var preferredItemWidth = 0;
            foreach (ToolStripItem childItem in ownerItem.DropDownItems)
            {
                preferredItemWidth = Math.Max(
                    preferredItemWidth,
                    childItem.GetPreferredSize(Size.Empty).Width);
            }

            ownerItem.DropDown.AutoSize = true;
            ownerItem.DropDown.LayoutStyle = ToolStripLayoutStyle.VerticalStackWithOverflow;
            ownerItem.DropDown.Padding = new Padding(1, 2, 1, 2);
            ownerItem.DropDown.MinimumSize = new Size(preferredItemWidth + 4, 0);
        }

        private static void MenuItemOnClick(object sender, EventArgs e)
        {
            var menuItem = sender as ToolStripMenuItem;
            var viewModel = menuItem?.Tag as NavigationMenuItemViewModel;
            if (viewModel?.Command == null || !viewModel.Command.CanExecute(null)) return;
            viewModel.Command.Execute(null);
        }

        private void RaiseHomeRequested(object sender, EventArgs e) => HomeRequested?.Invoke(this, EventArgs.Empty);
        private void RaiseRecordsRequested(object sender, EventArgs e) => RecordsRequested?.Invoke(this, EventArgs.Empty);
        private void RaiseParametersRequested(object sender, EventArgs e) => ParametersRequested?.Invoke(this, EventArgs.Empty);
        private void RaiseLogoutRequested(object sender, EventArgs e) =>
            LogoutRequested?.Invoke(this, EventArgs.Empty);
        private void RaiseOpenConfigDirectoryRequested(object sender, EventArgs e) =>
            OpenConfigDirectoryRequested?.Invoke(this, EventArgs.Empty);
        private void RaiseWorkflowPauseRequested(object sender, EventArgs e) =>
            WorkflowPauseRequested?.Invoke(this, EventArgs.Empty);
        private void RaiseWorkflowRestartRequested(object sender, EventArgs e) =>
            WorkflowRestartRequested?.Invoke(this, EventArgs.Empty);
    }
}
