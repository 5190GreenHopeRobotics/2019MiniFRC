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
AF_DCMotor mLeft(2);
AF_DCMotor mRight(3);
AF_DCMotor mSmallMotor1(4);
AF_DCMotor mElevator(1);

//this line creates a servo called "servo1"
SimpleSoftwareServo servo1;

int xAxisMultiplier = 1;      // Change this variable to -1 if your robot turns the wrong way
int yAxisMultiplier = 1;       // Change ths variable to -1 if your robot drives backward when it should be going forward

/* You shouldn't need to change anything past here unless you're adding
 *  something like an automode, extra motor, or servo. 
 *  <==============================================================> */

// Variables used to recive data from the driverstation and calculate drive logic
float xAxis, yAxis, testAxis;
int velocityL, velocityR;

//this variable is used to control your servo
float button = 0;
float button2 = 0;
float button3 = 0;
float button4 = 0;
float button5 = 0;
float button6 = 0;
float button7 = 0;
float button8 = 0;
boolean button5pressed = false;
boolean button6pressed = false;
boolean turned = false;
int pos = 0;

// In setup, we tell bluetooth communication to start and set all of our motors to not move
void setup() {
  Serial.begin(9600);
  Serial.println("starting robot...");
  bluetooth.begin(9600);
  drive(0,0);
  servo1.attach(9);          //this line tells the robot that your servo is on pin 9. (pin 9 is servo port 1, pin 10 is servo port 2)
  boolean button5pressed = false;
}

void loop() {
  while(bluetooth.available() > 0){                                   // This line checks for any new data in the buffer from the driverstation
    
    if ((bluetooth.read()) == 'z'){                                   // We use 'z' as a delimiter to ensure that the data doesn't desync
      xAxis = (bluetooth.parseFloat()) * (100) * xAxisMultiplier;     // For each item the driver station sends, we have a variable here to recieve it
      yAxis = (bluetooth.parseFloat()) * (100) * yAxisMultiplier;
      button = bluetooth.parseFloat();
      button2 = bluetooth.parseFloat();
      button3 = bluetooth.parseFloat();
      button4 = bluetooth.parseFloat();
      button5 = bluetooth.parseFloat();
      button6 = bluetooth.parseFloat();
      button7 = bluetooth.parseFloat();
      button8 = bluetooth.parseFloat();

      if (button5 == 0) {
        //Serial.println("Inside button 5 became 0 ...");
        button5pressed = false;
      }

      if (button6 == 0) {
        //Serial.println("Inside button 6 became 0 ...");
        button6pressed = false;
      }

      // This line tells the drive function what speed and direction to move the motors in
        drive(xAxis, yAxis);


      //Elevator Motor
      if (button7 == 1) {
        mElevator.run(FORWARD);     // These comands tell the motors what speed and direction to move at
        mElevator.setSpeed(100);
      }
      if (button8 == 1){
        mElevator.run(BACKWARD);     // These comands tell the motors what speed and direction to move at
        mElevator.setSpeed(-100);
      }
      if (button7 != 1 && button8 != 1) {
        mElevator.setSpeed(0);
      }
                    
      //little motor
      if (button2 == 1) {
        mSmallMotor1.run((FORWARD));     // These comands tell the motors what speed and direction to move at
        mSmallMotor1.setSpeed(700);
      } else {
        mSmallMotor1.setSpeed(0);
      }

      //go straight auto control (Auto1)
      if (button3 == 1) {
        drive(-100,0);
        delay(500);
        //delay(200);
        drive(0,0);
      }

      //rotate servo motor to 180 degrees
      if (button == 1){
        servo1.write(120);
      } 
      //rotate servo motor back to 0 degrees
      if (button4 == 1) {
        servo1.write(0);
      }
      SimpleSoftwareServo::refresh();

      //better auto (Auto2)
      if ((button5 == 1) && (!button5pressed)) {
        Serial.println("Inside button 5 ...");
        button5pressed = true;
        // these lines will make it drive forward, turn 90 degrees, go forward, and stop
        drive(-100,0);
        delay(1000);
        drive(-100,100);
        delay(1000);
        drive(-100,0);
        delay(1000);
        drive(0,0);

        //rotate servo to 180 degrees
        servo1.write(120);
        for(int i = 0; i < 250; i++) {
          SimpleSoftwareServo::refresh();
          delay(1);
        }
        servo1.write(0);
      }
      
       //Rotate 180 degrees back and forth
       if ((button6 == 1) && (!button6pressed)) {
        button6pressed = true;
        if (turned == false) {
          //turn right
          drive(-100,100);
          delay(1000);
          drive(0,0);
          turned = true;
        } else {
          //turn left
          drive(100,-100);
          delay(1000);
          drive(0,0);
          turned = false;
        }
      }
    }
  }
}

//drive(-100, 100) for 1000 ms is 90 deg turn, so 2000 ms is 180 deg turn and 500 ms is 45 deg turn

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
 
