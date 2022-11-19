using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Itami
{
    public partial class Load : Form
    {

        public Load()
        {
            InitializeComponent();
        }
        public string randomTIP()
        {
            string[] didyouknow = { "Did you know, Cats sleep 12-16 hours per day?", "Did you know,1 year of a cats life equals to 15 years of a humans live.", "Did you know,The smallest cat breed is a Singapura.", "Did you know, Most of the time a cat will purr when it is happy and content.", "Did you know,The oldest living cat was 38 years old." };
            Random random = new Random();
            var index = random.Next(0, didyouknow.Length);
            return didyouknow[index];
        }

        private void Load_Load(object sender, EventArgs e)
        {
            trans1.Show(Statu, false, Guna.UI2.AnimatorNS.Animation.Transparent);
            label1.Text = randomTIP();
            trans1.Show(label1, false, Guna.UI2.AnimatorNS.Animation.Transparent);
            

        }
        public void SetStatus(string Status)
        {
            Invoke((MethodInvoker)delegate
            {
                trans1.HideSync(Statu, false, Guna.UI2.AnimatorNS.Animation.Transparent);
                Statu.Text = Status;
                trans1.ShowSync(Statu, false, Guna.UI2.AnimatorNS.Animation.Transparent);
            });
           
        }
        private void CRClose(object sender, FormClosingEventArgs e)
        {
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void prog1_Click(object sender, EventArgs e)
        {

        }

        private void guna2CirclePictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void Statu_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void bunifuLabel1_Click(object sender, EventArgs e)
        {

        }

        private void guna2PictureBox11_Click(object sender, EventArgs e)
        {

        }
    }
}
