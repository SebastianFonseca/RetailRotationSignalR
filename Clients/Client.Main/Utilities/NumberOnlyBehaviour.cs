#region using statements

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

#endregion

namespace Client.Main.Utilities
{
    public static class NumberOnlyBehaviour
    {
        public static readonly DependencyProperty ModeProperty =
      DependencyProperty.RegisterAttached("Mode", typeof(NumberOnlyBehaviourModes?),
      typeof(NumberOnlyBehaviour), new UIPropertyMetadata(null, OnValueChanged));

        public static NumberOnlyBehaviourModes? GetMode(DependencyObject o)
        { return (NumberOnlyBehaviourModes?)o.GetValue(ModeProperty); }

        public static void SetMode(DependencyObject o, NumberOnlyBehaviourModes? value)
        { o.SetValue(ModeProperty, value); }

        private static void OnValueChanged(DependencyObject dependencyObject,
                DependencyPropertyChangedEventArgs e)
        {
            var uiElement = dependencyObject as Control;
            if (uiElement == null) return;
            if (e.NewValue is NumberOnlyBehaviourModes)
            {
                uiElement.PreviewTextInput += OnTextInput;
                uiElement.PreviewKeyDown += OnPreviewKeyDown;
                DataObject.AddPastingHandler(uiElement, OnPaste);
            }

            else
            {
                uiElement.PreviewTextInput -= OnTextInput;
                uiElement.PreviewKeyDown -= OnPreviewKeyDown;
                DataObject.RemovePastingHandler(uiElement, OnPaste);
            }
        }

        private static void OnTextInput(object sender, TextCompositionEventArgs e)
        {
            string adjustedText = string.Empty;
            var dependencyObject = sender as DependencyObject;
            var txtBox = sender as TextBox;
            // Right now only handle special cases if TextBox
            var mode = txtBox == null ? NumberOnlyBehaviourModes.PositiveWholeNumber : GetMode(dependencyObject);

            switch (mode)
            {
                case NumberOnlyBehaviourModes.Decimal:
                    if (e.Text.Any(c => !char.IsDigit(c))) e.Handled = true;
                    HandleSigns();
                    HandleDecimalPoint();
                    break;
                case NumberOnlyBehaviourModes.PositiveWholeNumber:
                    if (e.Text.Any(c => !char.IsDigit(c))) e.Handled = true;
                    break;
                case NumberOnlyBehaviourModes.WholeNumber:
                    if (e.Text.Any(c => !char.IsDigit(c))) e.Handled = true;
                    HandleSigns();
                    break;
            }

            // Handle plus and minus signs. Plus always makes positive, minus reverses sign
            void HandleSigns()
            {
                // Handle minus sign, changing sign of number if pressed
                if (e.Text[0] == '-')
                {
                    var nonSelectedTest = GetNonSelectedTest(txtBox);

                    if (nonSelectedTest.Length == 0)
                    {
                        e.Handled = false;
                    }
                    else if (nonSelectedTest.First() == '-')
                    {
                        var startPos = txtBox.SelectionStart;
                        txtBox.Text = nonSelectedTest.Substring(1);
                        txtBox.SelectionStart = startPos - 1;
                    }
                    else
                    {
                        var startPos = txtBox.SelectionStart;
                        txtBox.Text = "-" + nonSelectedTest;
                        txtBox.SelectionStart = startPos + 1;
                    }
                }

                // Handle plus sign, forcing number to be positive
                else if (e.Text[0] == '+')
                {
                    var nonSelectedTest = GetNonSelectedTest(txtBox);

                    if (nonSelectedTest.Length > 0 && nonSelectedTest.First() == '-')
                    {
                        var startPos = txtBox.SelectionStart;
                        txtBox.Text = nonSelectedTest.Substring(1);
                        txtBox.SelectionStart = startPos - 1;
                    }
                }
            }

            // Handle decimal sign, always putting decimal at current position even when there is a decimal point
            void HandleDecimalPoint()
            {
                if (e.Text[0] == '.')
                {
                    var nonSelectedTest = GetNonSelectedTest(txtBox);

                    if (nonSelectedTest.Contains("."))
                    {
                        var startPos = txtBox.SelectionStart;
                        var decimalIndex = nonSelectedTest.IndexOf(".");
                        var newText = nonSelectedTest.Replace(".", "");
                        if (startPos > decimalIndex) startPos--;
                        txtBox.Text = newText.Substring(0, startPos) + "."
                           + newText.Substring(startPos);
                        txtBox.SelectionStart = startPos + 1;
                        e.Handled = true;
                    }
                    else
                    {
                        e.Handled = false;
                    }
                }
            }
        }

        private static string GetNonSelectedTest(TextBox txtBox)
        {
            var startText = txtBox.SelectionStart == 0 ? string.Empty : txtBox.Text.Substring(0, txtBox.SelectionStart);
            var endText = txtBox.SelectionStart + txtBox.SelectionLength == txtBox.Text.Length ? string.Empty
                : txtBox.Text.Substring(txtBox.SelectionStart + txtBox.SelectionLength);
            return startText + endText;
        }

        private static void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space) e.Handled = true;
        }

        private static void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                var text = Convert.ToString(e.DataObject.GetData(DataFormats.Text)).Trim();
                if (text.Any(c => !char.IsDigit(c))) { e.CancelCommand(); }
            }
            else
            {
                e.CancelCommand();
            }
        }
    }

    public enum NumberOnlyBehaviourModes
    {
        WholeNumber,
        PositiveWholeNumber,
        Decimal
    }

}
