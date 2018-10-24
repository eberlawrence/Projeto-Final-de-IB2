using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace C_Sharp_Onscreenkeys
{
    class CircularBuffer
    {
        double[,] buffer; //definiu o tipo dele
        int tamBuffer; //tamanho do buffer
        public int contAmostras; //quantidade de amostras que existem 
        int PWR, PRD; //ponteiros de escrite e leitura


        //
        /// <summary>
        /// criando um novo buffer
        /// </summary>
        /// <param name="tam">Tamanho do buffer circular</param>
        public CircularBuffer(int tam)
        {
            tamBuffer = tam;
            buffer = new double[tam, 2]; //cria um novo vetor de 2 colunas e tam linhas
            PWR = 0;
            PRD = 0;
            contAmostras = 0;
        }

        /// <summary>
        /// coloca um novo valor
        /// </summary>
        /// <param name="valor">valor que vai ser inserido</param>
        /// <returns>true: se for possivel inserir - False: se for não for possivel inserir</returns>
        public bool Push(double[,] valor)
        {
            bool res = true;
            buffer[PWR, 0] = valor[0, 0]; //tempo
            buffer[PWR, 1] = valor[0, 1]; // sinal
            if (contAmostras >= tamBuffer - 1)
            {
                res = false; //nao é possivel inserir dados no buffer
            }
            else
            {
                contAmostras++; //aumenta a quantidade de amostras
            }

            PWR++; //incrementar o ponteiro de escrita

            //se o ponteiro de escrita ultrapassa o limite do tamanho, ele dá a volta e recomeça em zero
            if (PWR >= tamBuffer - 1)
            {
                PWR = 0;
            }

            return res;
        }

        /// <summary>
        /// Retira um elemento do buffer circular
        /// </summary>
        /// <param name="v">onde será alocado o valor por referencia no vetor 2D de 1 linha 2 col</param>
        /// <returns>true: se tiver amostras no buffer - false: se o buffer estiver vazio</returns>
        public bool pop(ref double[,] v)
        {
            if (contAmostras <= 0) //retorna falso se não tem nada pra ler
                return false;
            else //se existirem amostras
            {
                //alocação dos valores a partir da posição do ponteiro de leitura
                v[0, 0] = buffer[PRD, 0];
                v[0, 1] = buffer[PRD, 1];
                //decremento o numero de amostras
                contAmostras--;
                PRD++;//caminho com o ponteiro de leitura
                if (PRD >= tamBuffer - 1) //se o ponteiro chega no limete, retornamos ele ao começo
                {
                    PRD = 0;

                }
            }
            return true;
        }

    }
}