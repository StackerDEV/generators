using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Globalization;
using Un4seen.Bass;

namespace some_keygen
{
    public partial class FrmMain : Form
    {
        
        public FrmMain()
        {
            InitializeComponent();
            if (Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
            {
                int stream = Bass.BASS_StreamCreateFile("sfx.xm", 0L, 0L, BASSFlag.BASS_DEFAULT);
                if (stream != 0)
                {
                    Bass.BASS_ChannelPlay(stream, false);
                }
                else
                {
                    // error
                    MessageBox.Show(("Stream error: " + Bass.BASS_ErrorGetCode()));
                }

            }
        }

        static public string GenerateKey(string name)
        {
            string ID, nLenght, Name, Tip;
            string DY, DM, DD;
            if (name.Length < 13)
            {
                do
                {
                    name = name + Convert.ToChar(0);
                } while (name.Length < 13);
            }
            string[] OemLIC =
			{
				"YDHSWFKMTBZOZETAHDAHJAIGAJDBABUFBWJQQCIWUCALKYPGHELMCDIABOWE",
				"FTHTDRTKRKIAZVBCJAIDKISAHHCXNIAHCAIAAHCAICAHDAIEPBGDQAVGITFE",
				"AHDAICOWWKNGCGRAFSUXBHMVOVWIVHUVEAGCSPXFTVDXUWVHROVNJTZOUMEK",
				"AJBBCIBCGBCFBEDAHEBFHBGBBGCBHBBIHBJJBJCHFAOQSCBZOIQVODFDOBSZ",
				"PRGLHLKBSIFKNYOVPHBUAFFUHCGESVQTLFLMKROTCIGRZAHEAIAAIHBACBAC"
			};
            DY = FrmMain.Encode(FrmMain.EncodeTwo("99")); //2099/12/12
            DM = FrmMain.Encode(FrmMain.EncodeTwo("12"));
            DD = FrmMain.Encode(FrmMain.EncodeTwo("12"));
            ID = FrmMain.Encode(FrmMain.EncodeTwo("10001")); // ID 10001
            int nLng = name.Length;
            nLenght = FrmMain.Encode(FrmMain.EncodeTwo(nLng.ToString())); // 13
            Name = FrmMain.Encode(name);
            Tip = FrmMain.Encode("2"); // CASE 2 = skip dayz left count crap
            OemLIC = OemLIC.Select(s => s.Replace("AHDAHJAIGAJDBAB", ID)).ToArray();
            OemLIC = OemLIC.Select(s => s.Replace("AHDAIE", DY.ToString())).ToArray();
            OemLIC = OemLIC.Select(s => s.Replace("AHCAIA", DM.ToString())).ToArray();
            OemLIC = OemLIC.Select(s => s.Replace("AAHCAI", DD.ToString())).ToArray();
            OemLIC = OemLIC.Select(s => s.Replace("AHDAIC", nLenght)).ToArray();
            OemLIC = OemLIC.Select(s => s.Replace("AJBBCIBCGBCFBEDAHEBFHBGBBGCBHBBIHBJJBJC", Name)).ToArray();
            OemLIC = OemLIC.Select(s => s.Replace("AFF", Tip)).ToArray();
            string LicS = OemLIC[0] + "\r\n" + OemLIC[1] + "\r\n" + OemLIC[2] + "\r\n" + OemLIC[3] + "\r\n" + OemLIC[4];
            string[] array = Regex.Split(LicS, "\n");
            int crc = FrmMain.Checksum(string.Concat(new string[]
			{
				array[0],
				array[1],
				array[2],
				array[3],
				array[4].Substring(0, 45)
			}));
            string CRCsum = FrmMain.Encode(FrmMain.EncodeTwo(crc.ToString()));
            OemLIC = OemLIC.Select(s => s.Replace("AHEAIAAIHBACBAC", CRCsum)).ToArray();
            return OemLIC[0] + "\r\n" + OemLIC[1] + "\r\n" + OemLIC[2] + "\r\n" + OemLIC[3] + "\r\n" + OemLIC[4];
        }

        static public string Encode(string instr)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(instr);
            StringBuilder stringBuilder = new StringBuilder();
            int num = 7;

            for (int i = 0; i < instr.Length; i += 1)
            {
                int b = 65;
                int st = bytes[i];
                st += num;
                int[] highrun = Array.ConvertAll(st.ToString().ToArray(), x => (int)x);
                int[] lowrun = Array.ConvertAll(st.ToString().ToArray(), x => (int)x);

                if (st > 100)
                {
                    string HighA = Char.ConvertFromUtf32(highrun[0]);
                    string HighB = Char.ConvertFromUtf32(highrun[1]);
                    string HighC = Char.ConvertFromUtf32(highrun[2]);
                    int one = Int32.Parse(HighA) + b;
                    int two = Int32.Parse(HighB) + b;
                    int three = Int32.Parse(HighC) + b;
                    stringBuilder.Append((char)one);
                    stringBuilder.Append((char)two);
                    stringBuilder.Append((char)three);
                }
                else
                {
                    string lowA = Char.ConvertFromUtf32(lowrun[0]);
                    string LowB = Char.ConvertFromUtf32(lowrun[1]);
                    int one = Int32.Parse(lowA) + b;
                    int two = Int32.Parse(LowB) + b;
                    stringBuilder.Append("A");
                    stringBuilder.Append((char)one);
                    stringBuilder.Append((char)two);
                }
                num += 7;
            }
            return stringBuilder.ToString();
        }

        static public string EncodeTwo(string St)
        {
            StringBuilder stringBuilder = new StringBuilder();
            byte[] bytes = Encoding.ASCII.GetBytes(St);
            for (int i = 0; i < St.Length; i++)
            {
                string lowA = Char.ConvertFromUtf32(bytes[i]);
                int one = Int32.Parse(lowA) + 65;
                stringBuilder.Append((char)one);
            }
            return stringBuilder.ToString();
        }

        static public int Checksum(string st)
        {
            int num = 0;
            byte[] bytes = Encoding.ASCII.GetBytes(st);
            for (int i = 0; i < st.Length; i++)
            {
                num += (int)bytes[i];
            }
            num = num - 52; //52
            return num;
        }

        private void gen_Click(object sender, EventArgs e)
        {
            gen.Text = FrmMain.GenerateKey(lic.Text);
        }

        private void FrmMain_Closing(object sender, EventArgs e)
        {
            int stream = Bass.BASS_StreamCreateFile("sfx.xm", 0L, 0L, BASSFlag.BASS_DEFAULT);
            Bass.BASS_StreamFree(stream);
            Bass.BASS_Free();
        }


        private void bExit_Click(object sender, EventArgs e)
        {
            int stream = Bass.BASS_StreamCreateFile("sfx.xm", 0L, 0L, BASSFlag.BASS_DEFAULT);
            Bass.BASS_StreamFree(stream);
            Bass.BASS_Free();
            System.Windows.Forms.Application.Exit();
        }

    }
}
