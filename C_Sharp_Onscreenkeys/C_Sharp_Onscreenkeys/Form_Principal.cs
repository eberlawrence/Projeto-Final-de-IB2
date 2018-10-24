using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
using System.Management;

namespace C_Sharp_Onscreenkeys
{
    public partial class form_Principal : Form
    {
        menuConfig MenuConfigGeral;
        Button colorCursor = new Button();
        Label LimiarValor = new Label();

        SerialPort Port;
        bool Loop = false;
        const int tam = 1000;
        CircularBuffer Buffer = new CircularBuffer(tam);
        double[,] Valor = new double[1, 2];

        Thread Read;
        Thread Write;

        bool Flag_Timer_Panel = false; // Indica se o timer do panel está ativo ou inativo;
        bool Flag_Timer_Labels = false; // Indica se o timer da label está ativo ou inativo;
        bool isClicked; //verifica se já ocorreu um evento de clique

        int selectedCollumn = 0; // Determina a posição inicial para o cursor do timer Panel
        int selectedLabel = 0; // Determina a posição inicial para o cursor do timer Label

        Dictionary<int, Panel> allCollumns = new Dictionary<int, Panel>(); // Cria um dicionário para armazenar todos os panels dentro do panel
        Dictionary<Panel, List<Label>> allLabels = new Dictionary<Panel, List<Label>>(); // Cria um dicionário para ermazenar uma lista de labels dentro de um panel


        public form_Principal()
        {
            InitializeComponent();
            this.DoubleBuffered = true;

            int i = 1;

            
            foreach(object P in pn_Desenha.Controls)
            {
                if (P is Panel)
                {
                    allCollumns.Add(i++, (Panel)P); // Coloca todos os panels dentro do panel no dicionário de panels

                    var tempList = new List<Label>(); // Cria uma nova lista
                    foreach(object L in ((Panel)P).Controls)
                    {
                        if (L is Label)
                            tempList.Add((Label)L); // Coloca todas as labels dentro do panel atual em uma lista
                    }
                    tempList = tempList.OrderBy(L => L.Location.Y).ToList(); // Ordena as labels de acordo com a posição em Y
                    allLabels.Add((Panel)P, tempList); // Coloca a lista de labels dentro do dicionário de labels
                }
            }
            
        }


        private void Form_Principal_Load(object sender, EventArgs e)
        {
            
            Port = new SerialPort();
            Port.BaudRate = 57600;
            COM_ports();
            MenuConfigGeral = new menuConfig(ref bt_Click, ref colorCursor, ref pn_Desenha, 
                                             ref Timer_Panels, ref Timer_Labels, ref LimiarValor);

            bt_Click.BackColor = Color.White;
            colorCursor.BackColor = Color.Chartreuse;

            LimiarValor.Text = "1024";
            pb_Power.Minimum = 0;
            pb_Power.Maximum = 1024;

            pn_Fundo.Controls.Add(MenuConfigGeral);

            bt_Click.Enabled = false;

        }       


        private void bt_Conecta_Click(object sender, EventArgs e)
        {
            if (LimiarValor.Text != "1024")
            {
                if (Port.IsOpen)
                {
                    Loop = false;
                    Port.Write(new byte[] { 0 }, 0, 1);
                    Port.DiscardInBuffer();
                    Port.Close();

                    bt_Conecta.Text = "conectar";
                }

                else
                {
                    Port.PortName = cb_Port.SelectedItem.ToString();
                    Port.Open();
                    bt_Conecta.Text = "Desconectar";
                    lb_Status.Text = "Sistema conectado";
                }
            }
            else
                MessageBox.Show("Limiar não definido. \nPor favor, realize a calibrção do sistema nas configurações.", "Erro de conexão", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            
        }


        private void bt_Start_Click(object sender, EventArgs e)
        {
            try
            {                
                if (Loop == false && Port.IsOpen)
                {
                    Loop = true;
                    Port.Write(new byte[] { 1 }, 0, 1);                    

                    Read = new Thread(Recebe_Dados);
                    Write = new Thread(Escreve_Dados);

                    Read.Start();
                    Write.Start();
                }

                else
                {
                    Loop = false;
                    Port.Write(new byte[] { 0 }, 0, 1);
                    Port.DiscardInBuffer();                    
                }
                OnOffTime();
                tb_Write.Text = "";

            }

            catch 
            {
                MessageBox.Show("Sistema não conectado!", "Erro de conexão", MessageBoxButtons.OK, MessageBoxIcon.Error);              
            }
            
        }


        void Recebe_Dados()
        {
            int BytesDisponiveis;
            byte MSB;
            byte LSB;
            byte[] Buff;

            while (Loop)
            {
                //Console.WriteLine("Entrou no Loop1");
                BytesDisponiveis = Port.BytesToRead;
                if (BytesDisponiveis >= 2)
                {
                    Buff = new byte[BytesDisponiveis];
                    Port.Read(Buff, 0, (BytesDisponiveis % 2 == 0 ? BytesDisponiveis : BytesDisponiveis - 1));
                    LSB = Buff[1] < 4 ? Buff[0] : Buff[1];
                    MSB = Buff[1] < 4 ? Buff[1] : Buff[2];
                    int amostra = MSB << 8 | LSB;
                    Valor[0, 1] = amostra;
                    if (!Buffer.Push(Valor))
                    {
                        Console.WriteLine("Sobrescrevendo");
                    }

                }
            }
            Console.WriteLine("Saiu do Loop1");
        }


        void Escreve_Dados()
        {
            int amostra;

            while (Loop)
            {
                //Console.WriteLine("Entrou no Loop2");
                double[,] RecebeValor = new double[1, 2];
                if (Buffer.pop(ref RecebeValor))
                {
                    if (RecebeValor[0, 1] < 1023)
                    {
                        amostra = Convert.ToInt16(RecebeValor[0, 1]);
                        this.ss_Value.BeginInvoke(new Action(() =>
                        {
                            tb_Sinal.Text = Convert.ToString(amostra);
                            Click_Musculo(amostra);
                        }));
                        this.ss_Value.BeginInvoke(new Action(() =>
                        {
                            pb_Power.Value = amostra;
                            ChangeColorPower();
                            Click_Musculo(amostra);
                        }));
                    }
                }
            }
            Console.WriteLine("Saiu do Loop2");
        }


        void ChangeColorPower()
        {
            pb_Power.Maximum = 1024;
            pb_Power.Minimum = 0;

            if (pb_Power.Value <= 333)
            {
                pb_Power.ForeColor = Color.Green;
            }
            if (pb_Power.Value > 333 && pb_Power.Value <= 666)
            {
                pb_Power.ForeColor = Color.Orange;
            }
            if (pb_Power.Value > 666)
            {
                pb_Power.ForeColor = Color.Red;
            }
        }


        void OnOffTime()
        {
            if (Flag_Timer_Panel | Flag_Timer_Labels)
            {
                // Desativa todos os timers e coloca as flags como timers inativos   
                Timer_Panels.Stop();
                Flag_Timer_Panel = false;
                Timer_Labels.Stop();
                Flag_Timer_Labels = false;

                ConformacaoInicial();
                selectedCollumn = 0; // Volta o cursor do timer pra posição inicial
                bt_Start.Text = "Iniciar"; //Troca o texto do botão para Start
            }

            else
            {
                Timer_Panels.Start(); // Inicia o timer do panel
                Flag_Timer_Panel = true; // Coloca a flag do panel como timer ativo
                bt_Start.Text = "Parar"; //Troca o texto do botão para Stop
            }
        }


        void ConformacaoInicial()
        {
            foreach (var p in pn_Desenha.Controls)
            {
                if (p is Panel)
                    foreach (Label labels in ((Panel)p).Controls)
                    {
                        if (labels != null)
                            labels.BackColor = bt_Click.BackColor; // Coloca todas as labels brancas, como inicialmente
                    }
            }

        }


        private void Timer_Panels_Tick(object sender, EventArgs e)
        {
            
            if (selectedCollumn >= allCollumns.Count)
                selectedCollumn = 0;

            ConformacaoInicial();


            selectedCollumn++;
            if (selectedCollumn == 0)
            {
                selectedCollumn = 1;
            }
            CursorPanel_ChangeColor(allCollumns[selectedCollumn]);
                    
        }


        void CursorPanel_ChangeColor(Panel sender)
        {
            foreach (Label labels in sender.Controls)
            {
                if (labels != null)
                    labels.BackColor = colorCursor.BackColor;
            }
        }


        private void Timer_Labels_Tick(object sender, EventArgs e)
        {
            if(selectedCollumn == 0)
            {
                selectedCollumn = 1;
            }
            var myLabelList = allLabels[allCollumns[selectedCollumn]];
            if (selectedLabel >= myLabelList.Count)
                selectedLabel = 0;

            myLabelList.ForEach(L => L.BackColor = bt_Click.BackColor);

            
            CursorLabel_ChangeColor(myLabelList[selectedLabel]);
            selectedLabel++;
        }


        void CursorLabel_ChangeColor(Label sender)
        {
            sender.BackColor = colorCursor.BackColor;
        }


        private void ClickHandle()
        {
            if (Flag_Timer_Panel)
            {
                Timer_Panels.Stop();
                Flag_Timer_Panel = false;
                Timer_Labels.Start();
                Flag_Timer_Labels = true;
            }

            else if (Flag_Timer_Labels)
            {
                try
                {
                    if (allLabels[allCollumns[selectedCollumn]][selectedLabel - 1].Text == "␣")
                    {
                        tb_Write.Text += " ";
                    }

                    else if (allLabels[allCollumns[selectedCollumn]][selectedLabel - 1].Text == "←")
                    {
                        tb_Write.Text = tb_Write.Text.Remove(tb_Write.Text.Count() - 1);
                    }

                    else if (allLabels[allCollumns[selectedCollumn]][selectedLabel - 1].Text == "☒")
                    {
                        tb_Write.Text = "";
                    }

                    else
                    {
                        tb_Write.Text += allLabels[allCollumns[selectedCollumn]][selectedLabel - 1].Text;
                    }

                    Timer_Panels.Start();
                    Flag_Timer_Panel = true;
                    Timer_Labels.Stop();
                    Flag_Timer_Labels = false;
                    selectedCollumn = 0;
                    selectedLabel = 0;
                }
                catch { }
            }
        }


        private void Click_Musculo(int Limiar)
        {
            double LimiarFinal = Convert.ToDouble(LimiarValor.Text);
            if (Limiar > LimiarFinal && !isClicked)
            {
                ClickHandle();
                isClicked = true;
            }
            if (Limiar < LimiarFinal && isClicked)
            {
                isClicked = false;
            }
        }


        private void bt_Click_Click(object sender, EventArgs e)
        {
            ClickHandle();
        }


        private void bt_Config_Click(object sender, EventArgs e)
        {
            if (bt_Config.Text == "Configurações")
            {
                pn_Desenha.Hide();
                MenuConfigGeral.Show();
                bt_Config.Text = "Voltar";
                bt_Menu.Enabled = false;
                bt_Start.Enabled = false;
                bt_Offline.Enabled = false;

            }
            else if (bt_Config.Text == "Voltar")
            {
                MenuConfigGeral.Hide();
                pn_Desenha.Show();
                bt_Config.Text = "Configurações";
                bt_Menu.Enabled = true;
                bt_Start.Enabled = true;
                bt_Offline.Enabled = true;
            }
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

        private void form_Principal_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(Port.IsOpen)
            {
                MessageBox.Show("Sistema conectado. Por favor, desconecte o sistema!", "Erro de conexão", 
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;                
            }

            else if (MessageBox.Show("Deseja encerrar o sistema?", "Keyboard 1.0", 
                                     MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void bt_Offline_Click(object sender, EventArgs e)
        {
            if(bt_Offline.Text == "Modo offline")
            {
                bt_Offline.Text = "Desativar modo Offline";

                bt_Click.Enabled = true;
                bt_Start.Enabled = false;
                bt_Config.Enabled = false;
                bt_Menu.Enabled = false;

                Timer_Panels.Start();
                Flag_Timer_Panel = true;
            }
            else if (bt_Offline.Text == "Desativar modo Offline")
            {
                

                bt_Offline.Text = "Modo offline";

                bt_Click.Enabled = false;
                bt_Start.Enabled = true;
                bt_Config.Enabled = true;
                bt_Menu.Enabled = true;

                Timer_Panels.Stop();
                Flag_Timer_Panel = false;
                Timer_Labels.Stop();
                Flag_Timer_Labels = false;

                selectedCollumn = 0;
                selectedLabel = 0;
                ConformacaoInicial();

                tb_Write.Text = "";

            }
        }
    }


    public class VerticalProgressBar : ProgressBar
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= 0x04;
                return cp;
            }
        }
    }


    //Usa DoubleBuffer para evitar que a tela fique piscando.
    class myPanel : System.Windows.Forms.Panel
    {
        public myPanel()
        {
            this.SetStyle(ControlStyles.UserPaint |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer, true);
        }
    }
}
