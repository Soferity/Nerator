﻿using System;
using Conforyon;
using Nerator.CS;
using ReaLTaiizor.Forms;
using System.Windows.Forms;
using static Nerator.CS.Page;
using Nerator.UC.LIGHT.HISTORY;
using static Nerator.CS.History;
using static Nerator.CS.Setting;
using System.Collections.Generic;
using static Nerator.CS.Variable;
using static Nerator.CS.Strength;
using static Nerator.CS.Character;
using static Nerator.CS.Generator;

namespace Nerator.UI
{
    public partial class LIGHT : MaterialForm
    {
        public LIGHT()
        {
            InitializeComponent();
            LoadConfig();
            HistoryLoad();
        }

        private void CEB_Click(object sender, EventArgs e)
        {
            string GP = Create(GetInt(PWLN.ValueNumber.ToString(), PasswordLenght, MinimumPasswordLenght, MaximumPasswordLenght), GetAlphabetic(AMCB.SelectedItem, AlphabeticType.BS), GetSpecial(SMCB.SelectedItem, SpecialType.NS));
            PWDTB.Text = GP;
            if (HYS.Checked)
            {
                Add(HistoryFileName, GP, DefaultDateTime);
                HistoryAdd(GP, GetTime(DefaultDateTime, DefaultDateTime), GetDate(DefaultDateTime, DefaultDateTime));
            }
            PLPB.Value = StrengthMode(CheckScore2(GP));
            PLPB.Style = StyleMode(PLPB.Value);
            Status.Message = "Yeni şifre oluşturma başarıyla tamamlandı!";
        }

        private void CYB_Click(object sender, EventArgs e)
        {
            if (PWDTB.Text != Clipboard.GetText())
            {
                ClipBoard.CopyText(PWDTB.Text, true);
                if (PWDTB.Text == Clipboard.GetText())
                {
                    Status.Message = "Oluşturulan şifre başarıyla kopyalandı!";
                    PWDTB.Focus();
                }
                else
                {
                    Status.Message = "Oluşturulan şifre kopyalaması başarısız!";
                }
            }
        }

        private void TMCB_CheckedChanged(object sender, EventArgs e)
        {
            TopMost = TMCB.Checked;
            TopMostMode = TopMost;
        }

        private void HistoryAdd(string Password, string Time, string Date, DockStyle Style = DockStyle.Top)
        {
            HYP.Controls.Add(new PWD(Password, Time, Date) { Dock = Style });
        }

        private void HistoryLoad()
        {
            if (HYP.Controls.Count > 1)
            {
                HYP.Controls.Clear();
            }

            _ = new History(HistoryFileName);

            Dictionary<string, string> History = Loader(HistoryFileName);

            foreach (string PKey in History.Keys)
            {
                if (++ListPasswordCount <= MaximumHistoryList)
                {
                    HistoryAdd(PKey, GetTime(GetLong(History[PKey], DefaultDateTime), DefaultDateTime), GetDate(GetLong(History[PKey], DefaultDateTime), DefaultDateTime));
                }
                else
                {
                    break;
                }
            }

            History.Clear();
        }

        private void STATUST_Tick(object sender, EventArgs e)
        {
            try
            {
                long Result = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - Status.ChangedStatus;
                if (Result >= 3)
                {
                    Status.Message = Status.DefaultStatus;
                }
            }
            catch (Exception Ex)
            {
                Status.Message = "Hata - " + Ex.Source + ": " + Ex.Message;
            }
        }

        private void STATUSMT_Tick(object sender, EventArgs e)
        {
            try
            {
                SSBR.Text = Status.Message;
            }
            catch (Exception Ex)
            {
                Status.Message = "Hata - " + Ex.Source + ": " + Ex.Message;
            }
        }

        private void LoadConfig()
        {
            HYS.Checked = HistoryMode;

            TMCB.Checked = TopMostMode;
            if (TMCB.Checked)
            {
                TMCB_CheckedChanged(null, null);
            }

            SMCB.SelectedIndex = SMCB.Items.IndexOf(GetSpecial(SpecialMode));
            AMCB.SelectedIndex = AMCB.Items.IndexOf(GetAlphabetic(AlphabeticMode));

            PWLN.ValueNumber = PasswordLenght;
            MTC.SelectedTab = OpenPageMode(PageMode);
            MTS.BaseTabControl = MTC;
        }

        private TabPage OpenPageMode(PageType Type)
        {
            return Type switch
            {
                PageType.History => History,
                PageType.Setting => Setting,
                _ => Generate,
            };
        }

        private void LIGHT_FormClosed(object sender, FormClosedEventArgs e)
        {
            HistoryMode = HYS.Checked;
            TopMostMode = TMCB.Checked;
            PageMode = GetPageMode(MTC.SelectedTab.Text);
            SpecialMode = GetSpecial(SMCB.SelectedItem, SpecialType.NS);
            AlphabeticMode = GetAlphabetic(AMCB.SelectedItem, AlphabeticType.BS);
            PasswordLenght = GetInt(PWLN.ValueNumber.ToString(), PasswordLenght, MinimumPasswordLenght, MaximumPasswordLenght);
            Save(ConfigFileName);
        }
    }
}