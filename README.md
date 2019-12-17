# Push-Up-VR-Game
This is a Unity game configured for the Google Cardboard where you control a asteroid dodging spaceship by doing push-ups.
This is done by having an arduino connected to an Ultrasonic sensor (in my case a HC-SR04) and transmit the measurements to 
the game using a bluetooth radio. A simple UI interface to calibrate the hieght range is included to make the translation
of the players physical distance from the sensor can be mapped. This project also features the arduino code used so if
the controls don't work or are noisy feel free to modify. To play, simply download the project, extract the files, 
add the project in unity hub, build the measurement hardware, and play away.



List of hardware parts:
HC-SR04
https://www.sparkfun.com/products/15569

BlueSMiRF Silver
https://www.sparkfun.com/products/12577

Arduino Uno
https://www.sparkfun.com/products/11021