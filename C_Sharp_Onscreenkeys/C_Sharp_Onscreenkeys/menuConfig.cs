using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace C_Sharp_Onscreenkeys
{
    public partial class menuConfig : UserControl
    {
        int BytesDisponiveis;
        byte MSB;
        byte LSB;
        byte[] Buff;

        Button bt_color;
        Button bt_TESTE;
        Label Limiar;


        Panel pn_Desenha;
        Timer Timer_Panels;
        Timer Timer_Labels;
        ComboBox cb_Port = new ComboBox();

        int TamanhoCalibra = 200;
        SerialPort Port;
        double[] Valor;
        int n;

        double Media;
        double DesvioPadrao;
        double LimiarValor;


        public menuConfig(ref Button _bt_color, ref Button _TESTE, ref Panel _pn_Desenha, 
                          ref Timer _Timer_Panels, ref Timer _Timer_Labels, ref Label _Limiar)
        {                         
            InitializeComponent();

            bt_color = _bt_color;
            bt_TESTE = _TESTE;
            pn_Desenha = _pn_Desenha;
            Timer_Panels = _Timer_Panels;
            Timer_Labels = _Timer_Labels;
            Limiar = _Limiar;
        }


        private void UserControl1_Load(object sender, EventArgs e)
        {
            Valor = new double[TamanhoCalibra];
            Port = new SerialPort();
            Port.BaudRate = 57600;
            COM_ports();
            lb_COM.Text = cb_Port.SelectedItem.ToString();

            tb_TimerPanel.Text = Timer_Panels.Interval.ToString();
            tb_TimerLabel.Text = Timer_Labels.Interval.ToString();


            
            tb_Porcentagem.Maximum = 100;
            tb_Porcentagem.Minimum = 0;


        }


        private void bt_FundoTela_Click(object sender, EventArgs e)
        {
            Palheta_FundoTela.ShowDialog(pn_Config);
            bt_FundoTela.BackColor = Palheta_FundoTela.Color;
            pn_Desenha.BackColor = Palheta_FundoTela.Color;
        }


        private void bt_TeclaPadrao_Click(object sender, EventArgs e)
        {
            Palheta_TeclaPadrao.ShowDialog(pn_Config);
            bt_TeclaPadrao.BackColor = Palheta_TeclaPadrao.Color;
            bt_color.BackColor = bt_TeclaPadrao.BackColor;
            foreach (var p in pn_Desenha.Controls)
            {
                if (p is Panel)
                    foreach (Label labels in ((Panel)p).Controls)
                    {
                        if (labels != null)
                            labels.BackColor = bt_TeclaPadrao.BackColor; // Coloca todas as labels brancas, como inicialmente
                    }
            }
        }


        private void bt_TeclaCursor_Click(object sender, EventArgs e)
        {
            Palheta_TeclaCursor.ShowDialog(pn_Config);
            bt_TeclaCursor.BackColor = Palheta_TeclaCursor.Color;
            bt_TESTE.BackColor = bt_TeclaCursor.BackColor;
        }

                
        private void bt_SalvaVelo_Click(object sender, EventArgs e)
        {
            int ValorTimerPanel = 0;
            int ValorTimerLabel = 0;

            Int32.TryParse(tb_TimerPanel.Text, out ValorTimerPanel);
            Int32.TryParse(tb_TimerLabel.Text, out ValorTimerLabel);

            if (ValorTimerPanel > 0 && ValorTimerLabel > 0)
            {
                Timer_Panels.Interval = ValorTimerPanel;
                Timer_Labels.Interval = ValorTimerLabel;
                MessageBox.Show("Alterado com sucesso");
            }
            else if(tb_TimerPanel.Text == "" || tb_TimerLabel.Text == "" )
            {
                MessageBox.Show("Insira um valor de tempo!");
            }
            else if(ValorTimerPanel < 0 || ValorTimerLabel < 0)
                MessageBox.Show("Não é possível inserir valor negaivo!");
        }


        void COM_ports()
        {
            int i;
            bool quantDiferente; //flag para sinalizar que a quantidade de portas mudou

            i = 0;
            quantDiferente = false;

            //se a quantidade de portas mudou
            if (cb_Port.Items.Count == SerialPort.GetPortNames().Length)
            {
                foreach (string s in SerialPort.GetPortNames())
                {
                    if (cb_Port.Items[i++].Equals(s) == false)
                    {
                        quantDiferente = true;
                    }
                }
            }
            else
            {
                quantDiferente = true;
            }

            //Se não foi detectado diferença
            if (quantDiferente == false)
            {
                return;                     //retorna
            }

            //limpa comboBox
            cb_Port.Items.Clear();

            //adiciona todas as COM diponíveis na lista
            foreach (string s in SerialPort.GetPortNames())
            {
                cb_Port.Items.Add(s);
            }
            //seleciona a primeira posição da lista
            cb_Port.SelectedIndex = 0;
        }


        private void bt_Config_Click(object sender, EventArgs e)
        {
            try
            {
                if (!Port.IsOpen)
                {
                    Port.PortName = cb_Port.SelectedItem.ToString();
                    lb_COM.Text = "Calibrando";
                    Port.Open();
                    n = 0;
                    pb_Calibra.Maximum = TamanhoCalibra;
                    pb_Calibra.Minimum = 0;



                    Port.Write(new byte[] { 1 }, 0, 1);
                    timer_Config.Start();
                }
            }
            catch 
            {
                MessageBox.Show("É necessário desconectar o sistema para realizar um calibração!");
            }

        }
        

        private void timer_Config_Tick(object sender, EventArgs e)
        {            
            BytesDisponiveis = Port.BytesToRead;
            if (BytesDisponiveis >= 2)
            {
                Buff = new byte[BytesDisponiveis];
                Port.Read(Buff, 0 , (BytesDisponiveis % 2 == 0 ? BytesDisponiveis : BytesDisponiveis -1));
                LSB = Buff[1] < 4 ? Buff[0] : Buff[1];
                MSB = Buff[1] < 4 ? Buff[1] : Buff[2];
                int amostra = MSB << 8 | LSB;
                /*  1011 0010 0100 0001 = 45633
                    0000 0001 1011 0010 = 434
                 
                
                */



                Valor[n] = (amostra <= 1023 ? amostra : 0);
                Console.WriteLine(Valor[n]);
                n++;
                pb_Calibra.Value = n;
            }

            if (n >= TamanhoCalibra)
            {
                Port.Write(new byte[] { 0 }, 0, 1);
                
                Media = Math.Round(Valor.Average(), 2);
                lb_Media.Text = Convert.ToString(Media);


                DesvioPadrao = Math.Round(Math.Sqrt(Valor.Average(v => Math.Pow(v - Media, 2))), 2);
                lb_DesvioPadrao.Text = Convert.ToString(DesvioPadrao);


                
                LimiarValor = Math.Round((Media*tb_Porcentagem.Value)/100, 2);
                lb_Limiar.Text = Convert.ToString(LimiarValor);


                Port.DiscardInBuffer();
                timer_Config.Stop();
                Port.Close();
            }
        }

        private void lb_Limiar_TextChanged(object sender, EventArgs e)
        {
            Limiar.Text = lb_Limiar.Text;
        }

        private void tb_Porcentagem_Scroll(object sender, EventArgs e)
        {
            lb_Porcentagem.Text = Convert.ToString(tb_Porcentagem.Value);
        }
    }
}
