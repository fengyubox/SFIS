﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomUIControls;
using System.Drawing;
using System.Windows.Forms;

namespace MsgNotice
{
     
   public class MsgNotice
    {
           TaskbarNotifier taskbarNotifier3;
           public  MsgNotice()
           {
               taskbarNotifier3 = new TaskbarNotifier();
               taskbarNotifier3.SetBackgroundBitmap(new Bitmap(GetType(), "skin3.bmp"), Color.FromArgb(255, 0, 255));
               taskbarNotifier3.SetCloseBitmap(new Bitmap(GetType(), "close.bmp"), Color.FromArgb(255, 0, 255), new Point(280, 57));
               taskbarNotifier3.TitleRectangle = new Rectangle(150, 57, 125, 28);
               taskbarNotifier3.ContentRectangle = new Rectangle(75, 92, 215, 55);
               taskbarNotifier3.TitleClick += new EventHandler(TitleClick);
               taskbarNotifier3.ContentClick += new EventHandler(ContentClick);
               taskbarNotifier3.CloseClick += new EventHandler(CloseClick);
           }
           void CloseClick(object obj, EventArgs ea)
           {
             //  MessageBox.Show("Closed was Clicked");
           }

           void TitleClick(object obj, EventArgs ea)
           {
             //  MessageBox.Show("Title was Clicked");
           }

           void ContentClick(object obj, EventArgs ea)
           {
               MessageBox.Show(textContent);
           }
           string textContent = string.Empty;
           public  void ShowMsg(string textBoxContent)
           {
               string textBoxTitle = "提示消息";
               //  textBoxContent = "错误显示消息,啊哈哈胜多负少蓝色的肌肤了开始就了开始就地方塑料袋肌肤是的水电费 水电费水电费事情啊 矮人是否撒俄方啥地方艾丝凡错误显示消息,啊哈哈胜多负少蓝色的肌肤了开始就了开始就地方塑料袋肌肤是的水电费 水电费水电费事情啊 矮人是否撒俄方啥地方艾丝凡";
               //taskbarNotifier3.CloseClickable = true;
               //taskbarNotifier3.TitleClickable = false;
               //taskbarNotifier3.ContentClickable = true;
               //taskbarNotifier3.EnableSelectionRectangle = true;
               //taskbarNotifier3.KeepVisibleOnMousOver = true;// Added Rev 002
               //taskbarNotifier3.ReShowOnMouseOver = true;		// Added Rev 002
               textContent = textBoxContent;
               taskbarNotifier3.Show(textBoxTitle, textBoxContent, 300, 5000, 400);
           }
          
    }
}
