using System;
using System.Drawing;
using System.Windows.Forms;
using xadrez;
using tabuleiro;

namespace XADEZ
{
    public partial class Form1 : Form
    {
        private PartidaDeXadrez partida;
        private const int TAMANHO_CASA = 80;
        private Posicao origem = null;
        private bool[,] posicoesPossiveis;

        public Form1()
        {
            InitializeComponent();
            this.ClientSize = new Size(TAMANHO_CASA * 8 + 200, TAMANHO_CASA * 8 + 40);
            this.Text = "Xadrez";
            this.DoubleBuffered = true;
            partida = new PartidaDeXadrez();
            this.Paint += new PaintEventHandler(DesenharTabuleiro);
            this.MouseClick += new MouseEventHandler(ClicarTabuleiro);
        }

        private void DesenharTabuleiro(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            Font fonte = new Font("Arial", 22, FontStyle.Bold);

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    // Cor da casa
                    Color corCasa;
                    if (posicoesPossiveis != null && posicoesPossiveis[i, j])
                        corCasa = Color.LightGreen;
                    else if ((i + j) % 2 == 0)
                        corCasa = Color.Wheat;
                    else
                        corCasa = Color.SaddleBrown;

                    g.FillRectangle(new SolidBrush(corCasa), j * TAMANHO_CASA, i * TAMANHO_CASA, TAMANHO_CASA, TAMANHO_CASA);

                    // Desenhar peça
                    var peca = partida.tab.peca(i, j);
                    if (peca != null)
                    {
                        Color corPeca = peca.cor == Cor.Branca ? Color.White : Color.Black;
                        g.DrawString(peca.ToString(), fonte, new SolidBrush(corPeca),
                            j * TAMANHO_CASA + 20, i * TAMANHO_CASA + 18);
                    }
                }
            }

            // Letras e números
            Font fonteCoord = new Font("Arial", 12);
            string[] letras = { "a", "b", "c", "d", "e", "f", "g", "h" };
            for (int i = 0; i < 8; i++)
            {
                g.DrawString(letras[i], fonteCoord, Brushes.Black, i * TAMANHO_CASA + 30, 8 * TAMANHO_CASA + 5);
                g.DrawString((8 - i).ToString(), fonteCoord, Brushes.Black, 8 * TAMANHO_CASA + 10, i * TAMANHO_CASA + 30);
            }
        }

        private void ClicarTabuleiro(object sender, MouseEventArgs e)
        {
            int col = e.X / TAMANHO_CASA;
            int lin = e.Y / TAMANHO_CASA;

            if (col >= 8 || lin >= 8) return;

            Posicao pos = new Posicao(lin, col);

            if (origem == null)
            {
                try
                {
                    partida.validarPosicaoDeOrigem(pos);
                    origem = pos;
                    posicoesPossiveis = partida.tab.peca(pos).movimentosPossiveis();
                    Invalidate();
                }
                catch (TabuleiroException ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                try
                {
                    partida.validarPosicaoDeDestino(origem, pos);
                    partida.realizaJogada(origem, pos);
                    origem = null;
                    posicoesPossiveis = null;

                    if (partida.terminada)
                    {
                        MessageBox.Show("XEQUEMATE! Vencedor: " + partida.jogadorAtual);
                    }
                    else if (partida.xeque)
                    {
                        MessageBox.Show("XEQUE!");
                    }

                    Invalidate();
                }
                catch (TabuleiroException ex)
                {
                    MessageBox.Show(ex.Message);
                    origem = null;
                    posicoesPossiveis = null;
                    Invalidate();
                }
            }
        }
    }
}