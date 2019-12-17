#include <SoftwareSerial.h>  
#include <NewPing.h>

// Sensor pin location
#define SENSOR0 (11)
#define SENSOR1 (6)
// Number of sensors
#define SONAR_NUM (2)  
//Tx pin of bluesmirf
#define TXBT (2)
//Rx pin of bluesmirf
#define RXBT (3)

// arduino debug option
#define DEBUG (1)
// debug option to monitor sensor output
#define DEBUG_SENSOR (1)
// debug option to monitor bluetooth connection from Game to Arduino
#define DEBUG_BT_ARDUINO (2)
// debug option to monitor bluetooth connection from Arduino to game
#define DEBUG_BT_GAME (4)

// Bluetooth serial data transmission line
SoftwareSerial bluetooth(TXBT, RXBT);
// Ultrasonic sensor measuring instance
NewPing sonar[SONAR_NUM] = {   // Sensor object array.
  NewPing(SENSOR0, SENSOR0), // Trigger and echo pin are same since signals dont overlap 
  NewPing(SENSOR1, SENSOR1)
};
//array of sensor pins
uint8_t sensor_pin[SONAR_NUM]={
  SENSOR0,
  SENSOR1
};
// var to temporarily store measurement 
unsigned int measurement;
// actual result to send to game
unsigned int result;

void setup()
{
  // Setup bluetooth communication
  bluetooth.begin(115200);  // The Bluetooth Mate defaults to 115200bps
  bluetooth.print("$");  // Print three times individually
  bluetooth.print("$");
  bluetooth.print("$");  // Enter command mode
  delay(100);  // Short delay, wait for the Mate to send back CMD
  bluetooth.println("U,9600,N");  // Temporarily Change the baudrate to 9600, no parity
  // 115200 can be too fast at times for NewSoftSerial to relay the data reliably
  bluetooth.begin(9600);  // Start bluetooth serial at 9600
  // Begin the serial monitor at 9600bps
  Serial.begin(9600);  
}
 
void loop() {
  
  uint8_t i = 0;
  
  unsigned int meas;
  for (uint8_t i = 0; i < SONAR_NUM; i++) { // Loop through each sensor and display results.
    delay(50); // Wait 50ms between pings (about 20 pings/sec). 29ms should be the shortest delay between pings.
    meas = sonar[i].ping_cm();
    i += meas > 0;
    result += meas;
  }
  // Send data if results are valid
  if(result > 0)
    bluetooth.println(result);
  
  // Debugging code, optional
  // Print output of ultrasonic sensor
#if (DEBUG & DEBUG_SENSOR) == DEBUG_SENSOR
  Serial.print("Ping: ");
  Serial.println(result);
#endif

  // Send data from Game to Arduino
#if (DEBUG & DEBUG_BT_ARDUINO) == DEBUG_BT_ARDUINO
  if (bluetooth.available())
    Serial.write(bluetooth.read());
#endif

  // Send data from Arduino to Game
#if (DEBUG & DEBUG_BT_GAME) == DEBUG_BT_GAME
  if (Serial.available())
    bluetooth.write(Serial.read());
#endif
}
