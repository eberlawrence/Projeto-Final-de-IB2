#include <Timer.h>
#define fs 500
#define ts 1000/fs // 1/hz
#define fw 20

int F_AD;   //recebe o valor original filtrado
int numbers[fw]; //vetor com os valores para média móvel
uint16_t AD;
uint16_t convAD; //inteiro 16bits com valores positivos (p o uno os valores são 10bits com 6 bits de dont care
int funcEvent;
int NONRET_AD;

Timer T;

int LED = 13;
void setup() {
  
  // put your setup code here, to run once:
  pinMode(LED, OUTPUT);
  Serial.begin(57600); 
}

//função do timer
//le e envia dados pela porta serial
void func()
{
  NONRET_AD = analogRead(A0)- 333;//ler o valor da entrada analogica
  AD = abs(NONRET_AD);
  F_AD = moving_average();
  F_AD *= 50; 
  //Serial.println(F_AD);
  Serial.write(F_AD);
  Serial.write(F_AD >> 8);
}

long moving_average()
{
   //desloca os elementos do vetor de média móvel
   for(int i=fw-1 ; i>0 ; i--) 
   {
      numbers[i] = numbers[i-1];
   }

   numbers[0] = AD; //posição inicial do vetor recebe a leitura original

   long acc = 0;          //acumulador para somar os pontos da média móvel

   for(int i = 0 ; i < fw ; i++)
   {
      acc += numbers[i]; //faz a somatória do número de pontos
   }

   return acc/fw;  //retorna a média móvel
 
} 

void loop() 
{
  T.update();
  if(Serial.available()) //se existir alguma coisa na porta serial
  {
    byte x = Serial.read();
    if(x == 1)
    {
      T.every(ts,func); //ativa o timer 
    }
    else if(x == 0)
    {
      T.stop(funcEvent); //para o timer
    }    
  }  
}
