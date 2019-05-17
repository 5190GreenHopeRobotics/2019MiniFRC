#include <SoftwareSerial.h>
#include <AFMotor.h>
#include <SimpleSoftwareServo.h>
#include <Adafruit-Motor-Shield-library-master.h>

SoftwareSerial bluetooth(A0, A1); //RX,TX

AF_DCMotor mLeft(3);
AF_DCMotor mRight(4);

AF_DCMotor mClimber1(1);
AF_DCMotor mClimber2(2);

AF_DCMotor mExtra1(1);
AF_DCMotor mExtra2(2);

SimpleSoftwareServo servo1;
SimpleSoftwareServo servo2;
SimpleSoftwareServo armservo1;
SimpleSoftwareServo armservo2;
SimpleSoftwareServo armservo3;
SimpleSoftwareServo armservo4;

int xAxisMultiplier = -1;
int yAxisMultiplier = -1;

// Variables used to receive data from the driverstation and calculate drive logic
float xAxis, yAxis;
int velocityL, velocityR, velocityClimb;

//this variable is used to control your servo
float button1;
float button2;
float button3;
float button4;
float serve1butt1, serve1butt2;
float serve2butt1, serve2butt2;
float serve3butt1, serve3butt2;
float serve4butt1, serve5butt2;

float climber1, climber2;

// In setup, we tell bluetooth communication to start and set all of our motors to not move
void setup() {
  velocityClimb = 100;

  Serial.begin(9600);
  Serial.println("Starting...");

  bluetooth.begin(9600);
  drive(0, 0);
  servo1.attach(10);
  servo2.attach(9);
  climb(0, 0);
}

void loop() {
  while (Serial.available() > 0) {                                 // This line checks for any new data in the buffer from the driverstation
    if ((Serial.read()) == 'z') {                                  // We use 'z' as a delimiter to ensure that the data doesn't desync
      xAxis = (Serial.parseFloat()) * (100) * xAxisMultiplier;     // For each item the driver station sends, we have a variable here to recieve it
      yAxis = (Serial.parseFloat()) * (100) * yAxisMultiplier;
      button1 = Serial.parseFloat();
      button2 = Serial.parseFloat();
      climber1 = Serial.parseFloat();
      climber2 = Serial.parseFloat();

      //these lines control your servo. You may have to change them in order to get the desired result from your servo
      if (button1 == 1) {
        servo1.write(180);
      } else {
        servo1.write(90);
      }
      if (button2 == 1) {
        servo2.write(180);
      } else {
        servo2.write(90);
      }
      if (serve1butt1 == 1) {
        armservo1.write(armservo1.read()+5);
      }
      if (serve1butt2 == 1) {
        armservo1.write(armsevro1.read()-5);
      }
      if (serve2butt1 == 1) {
        armservo2.write(armservo2.read()+5);
      }
      if (serve2butt2 == 1) {
        armservo2.write(armsevro2.read()-5);
      }
      if (serve3butt1 == 1) {
        armservo3.write(armservo3.read()+5);
      }
      if (serve3butt2 == 1) {
        armservo3.write(armsevro3.read()-5);
      }
      if (serve4butt1 == 1) {
        armservo4.write(armservo4.read()+5);
      }
      if (serve4butt2 == 1) {
        armservo4.write(armsevro4.read()-5);
      }
      SimpleSoftwareServo::refresh();

      //these lines control your extra motor. You may have to change them in order to get the desired result from your motor
      if (button3 == 1) {
        mExtra1.run(FORWARD);
        mExtra1.setSpeed(255);
      } else {
        mExtra1.run(FORWARD);
        mExtra1.setSpeed(0);
      }

      //these lines control your extra motor. You may have to change them in order to get the desired result from your motor
      if (button4 == 1) {
        mExtra2.run(FORWARD);
        mExtra2.setSpeed(255);
      } else {
        mExtra2.run(FORWARD);
        mExtra2.setSpeed(0);
      }


      // This line tells the drive function what speed and direction to move the motors in
      drive(xAxis, yAxis);
      climb(mClimber1, climber1);
      climb(mClimber2, climber2);
    }
  }
}

// This function handles drive logic and actuation. Don't change this unless you know what you're doing.
void drive(int xAxis, int yAxis) {
  //  Serial.print("X:");
  //  Serial.print(xAxis);
  //  Serial.print(" Y:");
  //  Serial.println(yAxis);
  float V = (100 - abs(xAxis)) * (yAxis / 100) + yAxis;  // This is where the X and Y axis inputs are converted into tank drive logic
  float W = (100 - abs(yAxis)) * (xAxis / 100) + xAxis;
  velocityL = ((((V - W) / 2) / 100) * 255);
  velocityR = ((((V + W) / 2) / 100) * 255);

  mRight.run((velocityR >= 0) ? FORWARD : BACKWARD);     // These comands tell the motors what speed and direction to move at
  mRight.setSpeed(abs(velocityR));
  mLeft.run((velocityL >= 0) ? FORWARD : BACKWARD);
  mLeft.setSpeed(abs(velocityL));
}

void climb(AF_DCMotor climberMotor, int xAxis) {
  Serial.print("X:");
  Serial.println(xAxis);
  //  Serial.print(" Y:");
  //  Serial.println(yAxis);

  climberMotor.run((xAxis > 0) ? FORWARD : BACKWARD);     // These commands tell the motors what speed and direction to move at
  if (xAxis != 0) {
    climberMotor.setSpeed(velocityClimb);
  } else {
    climberMotor.setSpeed(0);
  }
}
