#include <SoftwareSerial.h>
#include <AFMotor.h>
#include <SimpleSoftwareServo.h>

SoftwareSerial bluetooth(A0, A1); //RX,TX

AF_DCMotor mLeft(3);
AF_DCMotor mRight(4);

float xAxis, yAxis;
int velocityL, velocityR, velocityClimb;

int botMode = 0;

void setup() {
  velocityClimb = 100;

  Serial.begin(9600);
  Serial.println("Starting...");

  bluetooth.begin(9600);
  drive(0, 0);
}

void loop() {
  while (Serial.available() > 0) {
    if ((Serial.read()) == 'z') {
      botMode = Serial.parseInt();
      if(botMode == 0){
        off();
      }
      else
      {
        xAxis = Serial.parseFloat() * -100;
        yAxis = Serial.parseFloat() * -100;

        if(botMode = 1){
          drive(xAxis, yAxis);
          off();
        }
        
        if(botMode = 2){
          drive(xAxis, yAxis);
          delay(Serial.parseInt());
          Serial.write("done");
          off();
        } 
      }
    }
  }
}

void off() {
  drive(0, 0);
}

void drive(int xAxis, int yAxis) {
  //  Serial.print("X:");
  //  Serial.print(xAxis);
  //  Serial.print(" Y:");
  //  Serial.println(yAxis);
  float V = (100 - abs(xAxis)) * (yAxis / 100) + yAxis;
  float W = (100 - abs(yAxis)) * (xAxis / 100) + xAxis;
  velocityL = ((((V - W) / 2) / 100) * 255);
  velocityR = ((((V + W) / 2) / 100) * 255);

  mRight.run((velocityR >= 0) ? FORWARD : BACKWARD);
  mRight.setSpeed(abs(velocityR));
  mLeft.run((velocityL >= 0) ? BACKWARD : FORWARD);
  mLeft.setSpeed(abs(velocityL));
}

void climb(AF_DCMotor climberMotor, int xAxis) {
  Serial.print("X:");
  Serial.println(xAxis);

  climberMotor.run((xAxis > 0) ? FORWARD : BACKWARD);
  if (xAxis != 0) {
    climberMotor.setSpeed(velocityClimb);
  } else {
    climberMotor.setSpeed(0);
  }
}
