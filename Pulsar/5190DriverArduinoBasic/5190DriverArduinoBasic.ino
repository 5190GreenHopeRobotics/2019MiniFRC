#include <SoftwareSerial.h>
#include <AFMotor.h>
#include <SimpleSoftwareServo.h>

SoftwareSerial bluetooth(A0, A1); //RX,TX

AF_DCMotor mLeft(3);
AF_DCMotor mRight(4);
AF_DCMotor mAlt(1);

float xAxis, yAxis;
float aAxis;
int velocityL, velocityR, velocityClimb;

int readMode = 0;

void setup() {
  velocityClimb = 100;

  Serial.begin(9600);
  Serial.println("Starting...");

  bluetooth.begin(9600);
  drive(0, 0);
}

void loop() {
  while (bluetooth.available() > 0) {
    if ((bluetooth.read()) == 'z') {
      readMode = bluetooth.parseInt();

      //parse data in the order of the data in the drive station
      xAxis = bluetooth.parseFloat() * -100;
      yAxis = bluetooth.parseFloat() * -100;
      aAxis = bluetooth.parseFloat() * 100;
      
      switch(readMode){
        case 0: teleop(); break;      // 0 means read it in teleop mode
        case 1: autonomous(); break;  // 1 means read it in autonomous mode
      }
    }
  }
}

void teleop(){
  drive(xAxis, yAxis);
  aMotor(mAlt, aAxis);
}

void autonomous(){
  drive(xAxis, yAxis);
  delay(bluetooth.parseInt());
  bluetooth.write("done");
}

void emergency(){
  xAxis = 0;
  yAxis = 0;
  aAxis = 0;
}

void drive(int xAxis, int yAxis) {
  //  bluetooth.print("X:");
  //  bluetooth.print(xAxis);
  //  bluetooth.print(" Y:");
  //  bluetooth.println(yAxis);
  float V = (100 - abs(xAxis)) * (yAxis / 100) + yAxis;
  float W = (100 - abs(yAxis)) * (xAxis / 100) + xAxis;
  velocityL = ((((V - W) / 2) / 100) * 255);
  velocityR = ((((V + W) / 2) / 100) * 255);

  mRight.run((velocityR >= 0) ? FORWARD : BACKWARD);
  mRight.setSpeed(abs(velocityR));
  mLeft.run((velocityL >= 0) ? BACKWARD : FORWARD);
  mLeft.setSpeed(abs(velocityL));
}

void aMotor(AF_DCMotor climberMotor, int xAxis) {
  bluetooth.print("X:");
  bluetooth.println(xAxis);

  climberMotor.run((xAxis > 0) ? FORWARD : BACKWARD);
  if (xAxis != 0) {
    climberMotor.setSpeed(velocityClimb);
  } else {
    climberMotor.setSpeed(0);
  }
}
