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
AF_DCMotor mLong(1);
AF_DCMotor mFat(2);

//this line creates a servo called "servo1"
SimpleSoftwareServo servo1, servo2;

int xAxisMultiplier = 1;      // Change this variable to -1 if your robot turns the wrong way
int yAxisMultiplier = 1;       // Change ths variable to -1 if your robot drives backward when it should be going forward

/* You shouldn't need to change anything past here unless you're adding
 *  something like an automode, extra motor, or servo. 
 *  <==============================================================> */

// Variables used to recive data from the driverstation and calculate drive logic
float xAxis, yAxis;
float longAxis, fatAxis;
int velocityL, velocityR;

//this variable is used to control your servo
float buttonLeft;
float buttonRight;
float servoButton;

// In setup, we tell bluetooth communication to start and set all of our motors to not move
void setup() {
  bluetooth.begin(9600);
  drive(0, 0);
  servo1.attach(9);
  servo2.attach(10);
}

void loop() {
  while(bluetooth.available() > 0){                                   // This line checks for any new data in the buffer from the driverstation
    if ((bluetooth.read()) == 'z'){                                   // We use 'z' as a delimiter to ensure that the data doesn't desync
      xAxis = (bluetooth.parseFloat()) * (100) * xAxisMultiplier;     // For each item the driver station sends, we have a variable here to recieve it
      yAxis = (bluetooth.parseFloat()) * (100) * yAxisMultiplier;
      buttonLeft = bluetooth.parseFloat();
      buttonRight = bluetooth.parseFloat();
      servoButton = bluetooth.parseFloat();
      longAxis = (bluetooth.parseFloat()) * (100);
      fatAxis = (bluetooth.parseFloat()) * (100);

      //these lines control your servo. You may have to change them in order to get the desired result from your servo
      if (buttonLeft == 1){
        servo1.write(servo1.read()+1);
      }
      if (buttonRight == 1){
        servo1.write(servo1.read()-1);
      }
      if (servoButton == 1){
        servo2.write(180);
      }else{
        servo2.write(0);
      }
      SimpleSoftwareServo::refresh();
     
      // This line tells the drive function what speed and direction to move the motors in
      drive(xAxis, yAxis);

      mLong.run(longAxis ? FORWARD : BACKWARD);
      mFat.run(fatAxis ? FORWARD : BACKWARD);
    } 
  }
}

// This function handles drive logic and actuation. Don't change this unless you know what you're doing.
void drive(int xAxis, int yAxis) {
  float V = (100 - abs(xAxis)) * (yAxis/100) + yAxis;    // This is where the X and Y axis inputs are converted into tank drive logic
  float W = (100 - abs(yAxis)) * (xAxis/100) + xAxis;
  velocityL = ((((V-W)/2)/100)*255);
  velocityR = ((((V+W)/2)/100)*255);

  mRight.run((velocityR >= 0) ? FORWARD : BACKWARD);     // These comands tell the motors what speed and direction to move at
  mRight.setSpeed(abs(velocityR));
  mLeft.run((velocityL >= 0) ? FORWARD : BACKWARD);
  mLeft.setSpeed(abs(velocityL));
}
