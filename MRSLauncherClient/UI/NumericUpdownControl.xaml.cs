using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MRSLauncherClient
{
    /// <summary>
    /// NumericUpdownControl.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class NumericUpdownControl : UserControl
    {
        public NumericUpdownControl()
        {
            InitializeComponent();
        }

        private int c_value = 0;

        public int Value
        {
            get {return c_value;}
            set
            {
                c_value = value;
                updateControl();
            }
        }

        public int Minimum { get; set; } = 2048;
        public int Maximum { get; set; } = int.MaxValue;

        public int Interval { get; set; } = 1;

        public event EventHandler ValueChanged;
        public event EventHandler ValueCompleted;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            updateControl();
        }

        private void TxtNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = true;
            return;

            var regex = new System.Text.RegularExpressions.Regex("[^0-9]+");

            e.Handled = regex.IsMatch(e.Text) && Convert.ToInt32(e.Text) >= 2048;
            return;
        }

        private void BtnUp_Click(object sender, RoutedEventArgs e)
        {
            c_value += Interval;
            if (c_value > Maximum)
                c_value = Maximum;
            updateControl();
        }

        private void BtnDown_Click(object sender, RoutedEventArgs e)
        {
            c_value -= Interval;
            if (c_value < Minimum)
                c_value = Minimum;
            updateControl();
        }

        private void TxtNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
//            if (txtNumber.Text == "" || txtNumber.Text == "\0")
//                txtNumber.Text = "2048";

            c_value = int.Parse(txtNumber.Text);

            ValueChanged?.Invoke(this, new EventArgs());
        }

        private void updateControl()
        {
            txtNumber.Text = c_value.ToString();
        }

        private void TxtNumber_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                ValueCompleted?.Invoke(this, new EventArgs());
        }
    }
}
