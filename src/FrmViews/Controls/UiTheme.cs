using System;
using System.Drawing;
using System.Windows.Forms;

namespace FrmViews.Controls
{
    internal static class UiTheme
    {
        public static readonly Color Page = Color.FromArgb(245, 247, 250);
        public static readonly Color Surface = Color.White;
        public static readonly Color SurfaceMuted = Color.FromArgb(248, 250, 252);
        public static readonly Color Text = Color.FromArgb(24, 32, 45);
        public static readonly Color Muted = Color.FromArgb(100, 112, 128);
        public static readonly Color Border = Color.FromArgb(220, 225, 232);
        public static readonly Color Input = Color.FromArgb(249, 250, 252);
        public static readonly Color Primary = Color.FromArgb(36, 99, 235);
        public static readonly Color PrimarySoft = Color.FromArgb(235, 242, 255);
        public static readonly Color PrimaryHover = Color.FromArgb(28, 78, 216);
        public static readonly Color Success = Color.FromArgb(21, 146, 78);
        public static readonly Color Danger = Color.FromArgb(220, 38, 38);
        public static readonly Color Warning = Color.FromArgb(202, 120, 12);
        public static readonly Color Canvas = Color.FromArgb(21, 29, 40);
        public static readonly Color CameraCanvas = Color.Navy;

        public static Button StyleMenuButton(Button button)
        {
            button.AutoSize = true;
            button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            button.BackColor = Surface;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(229, 235, 246);
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(240, 244, 250);
            button.FlatStyle = FlatStyle.Flat;
            button.Font = new Font("Microsoft YaHei UI", 8.5F, FontStyle.Regular);
            button.ForeColor = Text;
            button.Margin = Padding.Empty;
            button.Padding = new Padding(8, 0, 8, 0);
            button.Height = 46;
            button.UseVisualStyleBackColor = false;
            return button;
        }

        public static Button StyleIconButton(Button button)
        {
            button.BackColor = SurfaceMuted;
            button.FlatAppearance.BorderColor = SurfaceMuted;
            button.FlatAppearance.BorderSize = 0;
            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(225, 232, 242);
            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(239, 243, 248);
            button.FlatStyle = FlatStyle.Flat;
            button.Font = new Font("Segoe MDL2 Assets", 14F);
            button.ForeColor = Primary;
            button.Margin = new Padding(0, 7, 4, 7);
            button.Size = new Size(40, 40);
            button.UseVisualStyleBackColor = false;
            return button;
        }

        public static Button StyleCommandButton(Button button, bool primary)
        {
            button.BackColor = primary ? Primary : Color.FromArgb(247, 249, 252);
            button.FlatAppearance.BorderColor = primary ? Primary : Border;
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.MouseDownBackColor = primary
                ? Color.FromArgb(24, 68, 190)
                : Color.FromArgb(231, 235, 241);
            button.FlatAppearance.MouseOverBackColor = primary ? PrimaryHover : Color.FromArgb(241, 244, 248);
            button.FlatStyle = FlatStyle.Flat;
            button.Font = new Font("Microsoft YaHei UI", 9F);
            button.ForeColor = primary ? Color.White : Text;
            button.Size = new Size(108, 36);
            button.UseVisualStyleBackColor = false;
            return button;
        }

        public static Label StyleFieldLabel(Label label)
        {
            label.AutoSize = true;
            label.Font = new Font("Microsoft YaHei UI", 9F);
            label.ForeColor = Muted;
            label.Margin = new Padding(0, 6, 0, 5);
            return label;
        }

        public static TextBox StyleTextBox(TextBox textBox)
        {
            textBox.BackColor = Input;
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.Font = new Font("Microsoft YaHei UI", 9.5F);
            return textBox;
        }
    }

    internal sealed class ModernTabControl : TabControl
    {
        public ModernTabControl()
        {
            DrawMode = TabDrawMode.OwnerDrawFixed;
            ItemSize = new Size(116, 42);
            SizeMode = TabSizeMode.Fixed;
            Padding = new Point(16, 4);
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(UiTheme.Surface);
            using (var divider = new Pen(UiTheme.Border))
                e.Graphics.DrawLine(divider, 0, ItemSize.Height - 1, Width, ItemSize.Height - 1);

            for (var index = 0; index < TabCount; index++)
            {
                var selected = index == SelectedIndex;
                var bounds = GetTabRect(index);
                bounds.Y = 0;
                bounds.Height = ItemSize.Height;
                var textColor = selected ? UiTheme.Primary : UiTheme.Muted;
                using (var font = new Font("Microsoft YaHei UI", 9F,
                           selected ? FontStyle.Bold : FontStyle.Regular))
                {
                    TextRenderer.DrawText(e.Graphics, TabPages[index].Text, font, bounds, textColor,
                        TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                        TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
                }

                if (selected)
                {
                    using (var pen = new Pen(UiTheme.Primary, 3F))
                        e.Graphics.DrawLine(pen, bounds.Left + 24, ItemSize.Height - 2,
                            bounds.Right - 24, ItemSize.Height - 2);
                }
            }
        }
    }

    internal sealed class ModernMenuRenderer : ToolStripProfessionalRenderer
    {
        public ModernMenuRenderer() : base(new ModernMenuColorTable())
        {
            RoundedEdges = false;
        }
    }

    internal sealed class ModernMenuColorTable : ProfessionalColorTable
    {
        public override Color MenuItemSelected => UiTheme.PrimarySoft;
        public override Color MenuItemBorder => UiTheme.PrimarySoft;
        public override Color MenuItemPressedGradientBegin => UiTheme.PrimarySoft;
        public override Color MenuItemPressedGradientMiddle => UiTheme.PrimarySoft;
        public override Color MenuItemPressedGradientEnd => UiTheme.PrimarySoft;
        public override Color MenuBorder => UiTheme.Border;
        public override Color ToolStripDropDownBackground => UiTheme.Surface;
        public override Color ImageMarginGradientBegin => UiTheme.SurfaceMuted;
        public override Color ImageMarginGradientMiddle => UiTheme.SurfaceMuted;
        public override Color ImageMarginGradientEnd => UiTheme.SurfaceMuted;
        public override Color SeparatorDark => UiTheme.Border;
        public override Color SeparatorLight => UiTheme.Border;
        public override Color ToolStripGradientBegin => UiTheme.Surface;
        public override Color ToolStripGradientMiddle => UiTheme.Surface;
        public override Color ToolStripGradientEnd => UiTheme.Surface;
    }

}
