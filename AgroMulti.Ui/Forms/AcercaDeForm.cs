using System;
using System.Windows.Forms;

namespace CentroFermentacionSecado
{
    public partial class AcercaDeForm : Form
    {
        public AcercaDeForm()
        {
            InitializeComponent();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
