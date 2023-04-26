using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using System.Data;


namespace TTS_2019.Tools.Controls
{
    /// <summary>
	/// Pager.xaml 的交互逻辑
	/// </summary>
	public partial class Pager : UserControl
    {

        private int pageNo = 1;  // 当前页
        private int pageSize = 5;  // 每页记录数
        private int totalCount = 0;  // 总记录数
        private int currentCount = 0; // 当前页记录数
        private int pageCount = 1;  // 总页数

        private bool gotoFirstPageAfterLoaded = true;  // 控件初始化后是否自动加载第一页数据

        private bool hasInit = false;

        public Pager()
        {
            InitializeComponent();

            this.prePageBtn.IsEnabled = false;
        }

        /// <summary>
        /// 获取数据委托，返回总记录数
        /// </summary>
        /// <param name="pageNo">请求页</param>
        /// <param name="pageSize">每页记录数</param>
        /// <returns>总记录数</returns>
        public delegate int GetDataDelegate(int pageNo, int pageSize);
        private GetDataDelegate getDataDelegateHandler;

        /// <summary>
        /// 刷新当前页
        /// </summary>
        public void Refresh()
        {
            GotoPage(pageNo);
        }

        public void GotoFirstPage()
        {
            GotoPage(1);
        }

        public void GotoLastPage()
        {
            GotoPage(pageCount);
        }

        public void GotoPage(int pageNo)
        {
            if (pageNo <= 0)
            {
                pageNo = 1;
            }

            this.pageNo = pageNo;

            try
            {
                totalCount = getDataDelegateHandler(pageNo, pageSize);
                pageCount = totalCount % pageSize == 0 ? totalCount / pageSize : (totalCount / pageSize + 1);
                currentCount = pageNo == pageCount ? (totalCount - (pageNo - 1) * pageSize) : pageSize;

                // 页码显示
                this.currentCountTbk.Text = currentCount + "";
                this.totalCountTbk.Text = totalCount + "";
                this.pageNoTbk.Text = pageNo + "";
                this.pageCountTbk.Text = pageCount + "";
                this.pageSizeTb.Text = pageSize + "";

                // 按钮状态
                this.prePageBtn.IsEnabled = pageNo > 1 ? true : false;
                this.firstPageBtn.IsEnabled = pageNo > 1 ? true : false;
                this.nextPageBtn.IsEnabled = pageNo < pageCount ? true : false;
                this.lastPageBtn.IsEnabled = pageNo < pageCount ? true : false;
            }
            catch (Exception)
            {
                this.pageNoTbk.Text = "";
                this.pageCountTbk.Text = "";
            }
        }

        // 设置页显示记录数 
        private void setPageSizeBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int pageSize = Convert.ToInt32(this.pageSizeTb.Text);
                if (pageSize > 0)
                {
                    this.pageSize = pageSize;
                    this.GotoFirstPage();
                }
                else
                {
                    this.pageSizeTb.Text = this.pageSize + "";
                }
            }
            catch (Exception)
            {
                this.pageSizeTb.Text = this.pageSize + "";
            }
        }

        // 首页事件   
        private void firstPageBtn_Click(object sender, RoutedEventArgs e)
        {
            GotoFirstPage();
        }

        // 上一页事件   
        private void prePageBtn_Click(object sender, RoutedEventArgs e)
        {
            if (pageNo > 1)
            {
                pageNo -= 1;
                GotoPage(pageNo);
            }
        }

        // 下一页事件  
        private void nextPageBtn_Click(object sender, RoutedEventArgs e)
        {
            if (pageNo == 1 || pageNo < pageCount)
            {
                pageNo += 1;
                GotoPage(pageNo);
            }
        }

        // 末页事件  
        private void lastPageBtn_Click(object sender, RoutedEventArgs e)
        {
            GotoLastPage();
        }

        // 跳转事件  
        private void gotoBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int pageNo = Convert.ToInt32(this.gotoPageNoTb.Text);
                if (pageNo >= 1 && pageNo <= pageCount)
                {
                    GotoPage(pageNo);
                }
                else
                {
                    MessageBox.Show("请输入正确的页码范围：1 ~ " + pageCount);
                }
            }
            catch (Exception)
            {
            }
        }

        // 刷新
        private void refreshBtn_Click(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!hasInit)
            {
                if (gotoFirstPageAfterLoaded)
                {
                    GotoPage(1);
                }

                hasInit = true;
            }
        }

        // getter setter

        public int PageSize
        {
            get
            {
                return pageSize;
            }
            set
            {
                if (value > 0)
                {
                    pageSize = value;
                }
            }
        }

        /// <summary>
        /// 控件初始化后是否自动加载第一页数据
        /// </summary>
        public bool GotoFirstPageAfterLoaded
        {
            get
            {
                return gotoFirstPageAfterLoaded;
            }
            set
            {
                gotoFirstPageAfterLoaded = value;
            }
        }

        public Pager.GetDataDelegate GetDataDelegateHandler
        {
            set
            {
                getDataDelegateHandler = value;
            }
        }

    }

}
