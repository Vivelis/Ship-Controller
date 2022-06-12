//initialisation librairie pour le LCD
#include <LiquidCrystal.h>
LiquidCrystal lcd(12, 11, 5, 4, 3, 2);

#include <SoftwareSerial.h>
#include <SerialCommand.h>
SerialCommand sCmd;

//pin photorésistance rouge
#define redSensorPin = A0;
//sa variable
int redSensorValue = 0;

void setup() {
  Serial.begin(9600);
  //inscription du nombre de colomnes et de lignes du lcd
  lcd.begin(16, 2);
  while (!Serial);
  sCmd.addCommand("PING", pingHandler);
}

void loop () {
  if (Serial.available() > 0)
    sCmd.readSerial();
}

void pingHandler (const char *command) {
  Serial.println("PONG");
}

void echoHandler () {
  char *arg;
  arg = sCmd.next();
  if (arg != NULL)
    Serial.println(arg);
  else
    Serial.println("nothing to echo");
}