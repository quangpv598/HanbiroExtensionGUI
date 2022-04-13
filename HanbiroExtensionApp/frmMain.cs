using CefSharp.OffScreen;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HanbiroExtensionApp
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();

            var chrome = new ChromiumWebBrowser("http://infoplusvn.hanbiro.net/");

            var hanbiroRequestHandler = new HanbiroRequestHandler();
            chrome.RequestHandler = hanbiroRequestHandler;
        }
    }
}
