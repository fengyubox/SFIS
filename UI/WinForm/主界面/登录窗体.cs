﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using RefWebService_BLL;
using System.Xml;
using System.IO;
using System.Threading;
using System.DirectoryServices;

namespace SFIS_V2
{
    public partial class Login : Office2007Form// Form
    {
        public Login(MainParent mp)
        {
            InitializeComponent();
            this.mpm = mp;
        }
        MainParent mpm;
        private IAsyncResult iasyncresult;
        private delegate void delegatecreatedll();

        /// <summary>
        /// 应用程序池回收
        /// </summary>
        private void cre()
        {
            //如果应用程序池当前状态为停止,则会发生异常报错
            string AppPoolName = "myweb";// this.textBox1.Text.Trim();
            string method = "Recycle";

            try
            {
                DirectoryEntry appPool = new DirectoryEntry("IIS://localhost/W3SVC/AppPools");
                DirectoryEntry findPool = appPool.Children.Find(AppPoolName, "myweb");
                findPool.Invoke(method, null);
                appPool.CommitChanges();
                appPool.Close();
                MessageBox.Show("应用程序池名称回收成功", "回收成功");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "回收失败");
            }

        }
        private void CreateWebserviceDll()
        {
            try
            {
                EnableLogin(false);
                CreateWebService.CreateWebServices.CreateWebServiceDll();
                EnableLogin(true);
            }
            catch
            {
                //ShowMsg("服务器连接失败,请检查设置");
            }
        }
        private void EnableLogin(bool msg)
        {
            this.bt_login.Invoke(new EventHandler(delegate
                {
                    this.bt_login.Enabled = msg;
                }));
        }
        private void Login_Load(object sender, EventArgs e)
        {
            try
            {
                delegatecreatedll createdll = new delegatecreatedll(CreateWebserviceDll);
                iasyncresult = createdll.BeginInvoke(null, null);

                if (!Directory.Exists(Application.StartupPath + @"\Database"))
                {
                    Directory.CreateDirectory(Application.StartupPath + @"\Database");
                }
                if (File.Exists(Application.StartupPath + @"\Database\dbtemp.mdb"))
                {
                    File.Delete(Application.StartupPath + @"\Database\dbtemp.mdb");
                }
                FrmBLL.CreateAccessDb CDB = new FrmBLL.CreateAccessDb();
                CDB._CreateMDB();
                CDB._CreateWoSnrule();
                CDB.CreateBomData();
                CDB.CreateKpDetalt();
                CDB.CreateSmtSoftKpnumber();
                CDB.CreateTargetPlan();
                CDB.CreatetProduct();
                CDB.CreatetWorkOrderInfo();
                CDB.CreatetLineInfo();
                CDB.CreatetCraftInfo();
                CDB.CreatetStationRec();
                CDB.CreatetErrorCode();
                CDB.CreatetReasonCode();
                CDB.CreatetRepair();
                CDB.CloseDB();

            }
            catch (Exception ex)
            {
                MessageBoxEx.Show(ex.Message);
            }
        }
        // private DataTable _dt = new DataTable("dd");
        private void bt_login_Click(object sender, EventArgs e)
        {
            if (!File.Exists(System.IO.Directory.GetCurrentDirectory() + "\\getWebServices.dll"))
            {
                this.Close();
                return;
            }
            if (iasyncresult != null && !iasyncresult.IsCompleted)
            {
                this.mpm.ShowPrgMsg("正在与服务器通讯中,请稍后.");
                return;
            }

            this.bt_CfgIp.Enabled = false;
            try
            {
                if (string.IsNullOrEmpty(this.tb_username.Text))
                    throw new Exception("用户名不能为空!!");
                if (string.IsNullOrEmpty(this.tb_password.Text))
                    throw new Exception("密码不能为空!!");
                DataTable _dt = FrmBLL.ReleaseData.arrByteToDataTable(refWebtUserInfo.Instance.GetUserInfo(this.tb_username.Text.Trim(),null,tb_password.Text.Trim()));
                if (_dt == null || _dt.Rows.Count < 1 )
                    throw new Exception("用户名或密码输入错误\n请重新输入!!");
                if (_dt.Rows[0]["USERSTATUS"].ToString()!="1")
                    throw new Exception(string.Format("用户名[{0}]已经停用\n请重新输入!!", tb_username.Text));

                this.mpm.gUserInfo = new tUserInfo()
                {
                    userId = _dt.Rows[0]["userId"].ToString(),
                    pwd = tb_password.Text.Trim(),
                    username = _dt.Rows[0]["username"].ToString(),
                    useremail = _dt.Rows[0]["useremail"].ToString(),
                    userphone = _dt.Rows[0]["userphone"].ToString(),
                    userstatus = Convert.ToBoolean(int.Parse(_dt.Rows[0]["userstatus"].ToString())),
                    deptname = _dt.Rows[0]["deptname"].ToString(),
                    facId = _dt.Rows[0]["facId"].ToString(),
                    rolecaption = _dt.Rows[0]["rolecaption"].ToString(),
                    userPopList = FrmBLL.ReleaseData.arrByteToDataTable(RefWebService_BLL.refWebtUserInfo.Instance.GetUserJurisdictionByUserId(this.tb_username.Text.Trim()))
                };
                this.mpm.loginOk = true;
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                mpm.ShowPrgMsg(ex.Message);
                MessageBox.Show(ex.Message, "提示");
                this.tb_username.SelectAll();
                this.tb_username.Focus();
            }
        }

        private void bt_exit_Click(object sender, EventArgs e)
        {
            this.mpm.loginOk = false;
            this.mpm.gUserInfo = new  tUserInfo()
            {
                userId = "",
                username = "",
            };
            this.DialogResult = DialogResult.OK;
            this.Dispose();
            this.mpm.imbt_exit_Click(sender, e);
        }

        private void tb_password_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
                bt_login_Click(sender, e);
        }

        private void tb_username_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                this.tb_password.SelectAll();
                this.tb_password.Focus();
            }
        }
        public string WebServiceIpAddress { get; set; }
        private void bt_CfgIp_Click(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            string XmlName = "DllConfig.xml";
            doc.Load(XmlName);
            string ipadd = ((XmlElement)doc.SelectSingleNode("AutoCreate").SelectSingleNode("ServerIP")).GetAttribute("IP").ToString();
            ServerIpConfig sic = new ServerIpConfig(this, ipadd);
            if (sic.ShowDialog() == DialogResult.OK)
            {
                ((XmlElement)doc.SelectSingleNode("AutoCreate").SelectSingleNode("NewIP")).SetAttribute("IP",
                    this.WebServiceIpAddress);
                doc.Save(XmlName);
                if (File.Exists(System.IO.Directory.GetCurrentDirectory() + "\\getWebServices.dll"))
                    File.Delete(System.IO.Directory.GetCurrentDirectory() + "\\getWebServices.dll");
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            cre();
        }

        private void imbtInitwebser_Click(object sender, EventArgs e)
        {
            try
            {
                this.mpm.RecyclePools();
            }
            catch
            {

                MessageBoxEx.Show("回收失败!!", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
