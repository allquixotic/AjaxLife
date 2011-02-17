#region License
/* Copyright (c) 2008, Katharine Berry
 * Copyright (c) 2011, Sean McNamara
 * All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of Katharine Berry nor the names of any contributors
 *       may be used to endorse or promote products derived from this software
 *       without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY KATHARINE BERRY ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL KATHARINE BERRY BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 ******************************************************************************/
#endregion

using System;
using System.Net;
using System.IO;
using System.Timers;

namespace AjaxLife
{
    public class WhiteList
    {
        private string[] Whites = {};
        private Timer Time;
        private void UpdateList()
        {
            AjaxLife.Debug("WhiteList", "Loading white list from "+AjaxLife.WHITE_LIST+"...");
            try
            {
                string swhitelist = "";
                if (AjaxLife.WHITE_LIST.StartsWith("http://"))
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(AjaxLife.WHITE_LIST);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    StreamReader reader = new System.IO.StreamReader(response.GetResponseStream());
                    swhitelist = reader.ReadToEnd();
                    reader.Close();
                    response.Close();
                }
                else
                {
                    swhitelist = File.ReadAllText(AjaxLife.WHITE_LIST);
                }
                char[] newline = {'\n'};
                this.Whites = swhitelist.Split(newline);
                for(int i = 0; i < this.Whites.Length; ++i)
                {
                    this.Whites[i] = this.Whites[i].Trim();
                }
                AjaxLife.Debug("WhiteList", "White list up to date. "+Whites.Length+" whitelisted names.");
            }
            catch(Exception e)
            {
                AjaxLife.Debug("WhiteList", "White list update failed: "+e.Message);
            }
        }
        
        public WhiteList()
        {
            if(AjaxLife.WHITE_LIST != "")
            {
                if(AjaxLife.WHITE_UPDATE_TIME > 0)
                {
                    Time = new Timer();
                    Time.Interval = AjaxLife.WHITE_UPDATE_TIME * 1000.0;
                    Time.AutoReset = true;
                    Time.Elapsed += new ElapsedEventHandler(TimerElapsed);
                    Time.Start();
                    AjaxLife.Debug("WhiteList", "Set whitelist update timer for "+AjaxLife.WHITE_UPDATE_TIME+" seconds.");
                }
                else
                {
                    AjaxLife.Debug("WhiteList", "Whitelist update timer disabled.");
                }
                UpdateList();
            }
        }
        
        ~WhiteList()
        {
            Time.Stop();
            Time.Close();
        }
        
        public void TimerElapsed(object obj, ElapsedEventArgs args)
        {
            AjaxLife.Debug("WhiteList", "Timer elapsed.");
            UpdateList();
        }
        
        public bool IsWhiteListed(string first, string last)
        {
            return IsWhiteListed(first+" "+last);
        }
        
        public bool IsWhiteListed(string name)
        {
            return (Array.IndexOf(this.Whites, name) > -1);
        }
        
        public bool UseWhitelist()
        {
            return AjaxLife.WHITE_LIST != ""; //If a white list was specified on CLI, only allow logins allowed by that whitelist, whether it exists or not.
        }
    }
}










