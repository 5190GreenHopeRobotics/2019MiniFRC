//instructions for downloading and installing libraries can be found here: https://www.instructables.com/id/Downloading-All-the-Software-Youll-Need-for-MiniFR/
#include <SoftwareSerial.h>         //this library is part of Arduino by default
#include <AFMotor.h>                //you must download and install this library: https://drive.google.com/file/d/1zsMywqJjvzgMBoVZyrYly-2hXePFXFzw/view?usp=sharing
#include <SimpleSoftwareServo.h>    //you must download and install this library: https://drive.google.com/open?id=12Yz_uNNuAiASnTsu424Bm_q8Sj3SzlFx
                                  
/* <==============================================================>
 *  You will need to change the following variables depending on what
 *  analog pins on your motor shield you are using, which motor goes to
 *  which port, and if your drive logic is flipped. */

//change A0 and A1 to match whatever pins you are useing for your bluetooth chip
SoftwareSerial bluetooth(A0, A1); //RX,TX

//These lines declare which ports your motors will be connected to on the motor shield.
AF_DCMotor mLeft(3);
AF_DCMotor mRight(4);

AF_DCMotor mClimber1(1);
AF_DCMotor mClimber2(2);


//this line declares which port your extra motor is on
AF_DCMotor mExtra1(1);
AF_DCMotor mExtra2(2);


//this line creates a servo called "servo1"
SimpleSoftwareServo servo1;
SimpleSoftwareServo servo2;

int xAxisMultiplier = -1;      // Change this variable to -1 if your robot turns the wrong way
int yAxisMultiplier = -1;       // Change ths variable to -1 if your robot drives backward when it should be going forward

/* You shouldn't need to change anything past here unless you're adding
 *  something like an automode, extra motor, or servo. 
 *  <==============================================================> */

// Variables used to receive data from the driverstation and calculate drive logic
float xAxis, yAxis;
int velocityL, velocityR, velocityClimb;

//this variable is used to control your servo
float button1;
float button2;
float button3;
float button4;

float climber1, climber2;

// In setup, we tell bluetooth communication to start and set all of our motors to not move
void setup() {
  velocityClimb=100;
  
  Serial.begin(9600);
  Serial.println("Starting...");

  bluetooth.begin(9600);
  drive(0, 0);
  servo1.attach(10);          //this line tells the robot that your servo is on pin 9. (pin 9 is servo port 1, pin 10 is servo port 2)
  servo2.attach(9);          //this line tells the robot that your servo is on pin 9. (pin 9 is servo port 1, pin 10 is servo port 2)
  climb(0,0);
}

void loop() {
  while(bluetooth.available() > 0){                                   // This line checks for any new data in the buffer from the driverstation
    if ((bluetooth.read()) == 'z'){                                   // We use 'z' as a delimiter to ensure that the data doesn't desync
      xAxis = (bluetooth.parseFloat()) * (100) * xAxisMultiplier;     // For each item the driver station sends, we have a variable here to recieve it
      yAxis = (bluetooth.parseFloat()) * (100) * yAxisMultiplier;
      button1 = bluetooth.parseFloat();
      button2 = bluetooth.parseFloat();
      climber1 = bluetooth.parseFloat();
      climber2 = bluetooth.parseFloat();

      //these lines control your servo. You may have to change them in order to get the desired result from your servo
      if (button1 == 1){
        servo1.write(180);
      } else {
        servo1.write(0);
      }
      if (button2 == 1){
        servo2.write(180);
      } else {
        servo2.write(0);
      }
      SimpleSoftwareServo::refresh();

      //these lines control your extra motor. You may have to change them in order to get the desired result from your motor
      if (button3 == 1){
        mExtra1.run(FORWARD);
        mExtra1.setSpeed(255);
      } else {
        mExtra1.run(FORWARD);
        mExtra1.setSpeed(0);
      }

      //these lines control your extra motor. You may have to change them in order to get the desired result from your motor
      if (button4 == 1){
        mExtra2.run(FORWARD);
        mExtra2.setSpeed(255);
      } else {
        mExtra2.run(FORWARD);
        mExtra2.setSpeed(0);
      }

     
      // This line tells the drive function what speed and direction to move the motors in
      drive(xAxis, yAxis);
      climb(mClimber1,climber1);
      climb(mClimber2,climber2);
    } 
  }
}

// This function handles drive logic and actuation. Don't change this unless you know what you're doing.
void drive(int xAxis, int yAxis) {
//  Serial.print("X:");
//  Serial.print(xAxis);
//  Serial.print(" Y:");
//  Serial.println(yAxis);
  float V = (100 - abs(xAxis)) * (yAxis/100) + yAxis;    // This is where the X and Y axis inputs are converted into tank drive logic
  float W = (100 - abs(yAxis)) * (xAxis/100) + xAxis;
  velocityL = ((((V-W)/2)/100)*255);
  velocityR = ((((V+W)/2)/100)*255);

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
  if(xAxis != 0){
    climberMotor.setSpeed(velocityClimb);
  } else {
    climberMotor.setSpeed(0);
  }
}
