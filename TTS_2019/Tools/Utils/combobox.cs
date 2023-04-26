using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TTS_2019.Tools.Utils
{
   public class combobox
    {
        //站点名称
        private string siteName;
        public string site_name
        {
            get { return siteName; }
            set { siteName = value; }
        }
        //站点ID
        private int SiteId;
        public int site_id
        {
            get { return SiteId; }
            set { SiteId = value; }
        }
    }
}
