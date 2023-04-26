using System;
using System.Collections;
using System.ComponentModel;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TTS_2019.Tools.Controls
{
    /// <summary>
    /// AutoComplete.xaml 的交互逻辑
    /// </summary>
    public partial class AutoComplete : ComboBox
    {
        #region Events and Delegates
        /// <summary>
        ///用于传递自动完成控件的参数
        /// </summary>
        public class AutoCompleteArgs : EventArgs
        {
            private string _pattern;
            /// <summary>
            /// 在当前模式中自动完成
            /// </summary>
            public string Pattern
            {
                get
                {
                    return this._pattern;
                }
            }

            private IEnumerable _dataSource;
            /// <summary>
            /// 用于自动完成的新数据源
            /// </summary>
            public IEnumerable DataSource
            {
                get
                {
                    return this._dataSource;
                }
                set
                {
                    this._dataSource = value;
                }
            }

            /// <summary>
            /// 默认的错误
            /// </summary>
            private bool _cancelBinding = false;
            /// <summary>
            /// 决定是否将数据源绑定到自动完成控件
            /// </summary>
            public bool CancelBinding
            {
                get
                {
                    return this._cancelBinding;
                }
                set
                {
                    this._cancelBinding = value;
                }
            }

            public AutoCompleteArgs(string Pattern)
            {
                this._pattern = Pattern;
            }
        }

        /// <summary>
        ///自动完成模式的事件处理程序更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public delegate void AutoCompleteHandler(object sender, AutoCompleteArgs args);
        /// <summary>
        /// 模式更改时的事件
        /// </summary>
        public event AutoCompleteHandler PatternChanged;
        #endregion

        #region Fields
        private Timer _interval;
        private int _maxRecords = 10;
        private bool _typeAhead = false;
        private bool IsKeyEvent = false;
        private bool _clearOnEmpty = true;
        private int _delay = DEFAULT_DELAY;
        private const int DEFAULT_DELAY = 800; // 1 秒延迟
        #endregion

        #region Properties
        /// <summary>
        /// 按键和模式之间的时间平移改变事件
        /// </summary>
        [DefaultValue(DEFAULT_DELAY)]
        public int Delay
        {
            get
            {
                return this._delay;
            }
            set
            {
                this._delay = value;
            }
        }
        /// <summary>
        /// 下拉列表中显示的最大记录数
        /// </summary>
        public int MaxRecords
        {
            get
            {
                return this._maxRecords;
            }
            set
            {
                this._maxRecords = value;
            }
        }

        /// <summary>
        /// 确定天气文本框是否提前键入
        /// </summary>
        public bool TypeAhead
        {
            get
            {
                return this._typeAhead;
            }
            set
            {
                this._typeAhead = value;
            }
        }

        /// <summary>
        /// 获取负责组合框可编辑部分的文本框。
        /// </summary>
        protected TextBox EditableTextBox
        {
            get
            {
                return base.GetTemplateChild("PART_EditableTextBox") as TextBox;
            }
        }

        /// <summary>
        /// 返回所选项的文本表示形式
        /// </summary>
        public string SelectedText
        {
            get
            {
                try
                {
                    if (this.SelectedIndex == -1) return string.Empty;

                    return this.SelectedItem.GetType().GetProperty(this.DisplayMemberPath).GetValue(this.SelectedItem, null).ToString();
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// 当文本设置为空字符串时，指定天气或不清除数据源
        /// </summary>
        /// <remarks>当此属性设置为true时，当文本为空时将不会触发patternchanged事件。</remarks>
        public bool ClearOnEmpty
        {
            get
            {
                return this._clearOnEmpty;
            }
            set
            {
                this._clearOnEmpty = true;
            }
        }
        #endregion

        #region Constructor(s)
        public AutoComplete()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this._interval = new Timer(this.Delay);
                this._interval.AutoReset = true;
                this._interval.Elapsed += new ElapsedEventHandler(_interval_Elapsed);
            }
        }
        #endregion

        #region Event Handlers
        void _interval_Elapsed(object sender, ElapsedEventArgs e)
        {
            IsKeyEvent = false;
            // 暂停计时器
            this._interval.Stop();

            // 当用户开始输入时显示下拉列表，然后停止
            this.Dispatcher.BeginInvoke((Action)delegate
            {
                this.IsDropDownOpen = true;
                // 从事件源加载
                if (this.PatternChanged != null)
                {
                    AutoCompleteArgs args = new AutoCompleteArgs(this.Text);
                    this.PatternChanged(this, args);
                    // 如果用户未取消，则绑定新数据源
                    if (!args.CancelBinding)
                    {
                        this.ItemsSource = args.DataSource;

                        if (!this.TypeAhead)
                        {
                            //覆盖自动完成行为
                            this.Text = args.Pattern;
                            this.EditableTextBox.CaretIndex = args.Pattern.Length;
                        }
                    }
                }
            },
                System.Windows.Threading.DispatcherPriority.ApplicationIdle);
        }

        private void ComboBox_LostFocus(object sender, RoutedEventArgs e)
        {
            this.IsKeyEvent = false;
            this.IsDropDownOpen = false;
            // 防止误解用户输入了一些信息
            if (this.SelectedIndex == -1)
                this.Text = string.Empty;
            // syncronize text
            else
                this.Text = this.SelectedText;
            // 计时器释放资源
            this._interval.Close();

            try
            {
                this.EditableTextBox.CaretIndex = 0;
            }
            catch { }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this._interval.Stop();
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //加载文本框控件
            if (this.EditableTextBox != null)
            {
                this.EditableTextBox.PreviewKeyDown += new KeyEventHandler(EditableTextBox_PreviewKeyDown);
                this.EditableTextBox.TextChanged += new TextChangedEventHandler(EditableTextBox_TextChanged);
            }
        }

        void EditableTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            this.IsKeyEvent = true;
        }

        void EditableTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // 当文本为空时清除项目源
            if (this.ClearOnEmpty && string.IsNullOrEmpty(this.EditableTextBox.Text.Trim()))
                this.ItemsSource = null; // 这也应该明确选择
            else if (IsKeyEvent)
                this.ResetTimer();
        }

        protected void ResetTimer()
        {
            this._interval.Stop();
            this._interval.Start();
        }
        #endregion
    }
}
