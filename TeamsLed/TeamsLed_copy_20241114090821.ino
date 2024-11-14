#include <FastLED.h>

#define LED_PIN 13
#define NUM_LEDS 1

CRGB leds[NUM_LEDS];

boolean ready = false;
String receiveVal;
char *str;
int brightness, red, green, blue;
int brPos, rPos, gPos, bPos;

void setup() {
  Serial.begin(9600);
  brightness = 120;
  red = 0;
  green = 0;
  blue = 10;
  FastLED.addLeds<WS2812, LED_PIN, GRB>(leds, NUM_LEDS);
}

void loop() {
  if (ready) {
    leds[0] = CRGB(red, green, blue);
    FastLED.setBrightness(brightness);
    FastLED.show();
    ready = false;
  } else if (Serial.available() == 0) {
    brightness = 120;
    red = 0;
    green = 0;
    blue = 10;
    ready = true;
  } else
    while (Serial.available() > 0) {
      receiveVal = Serial.readStringUntil(';\r\n');

      brPos = receiveVal.indexOf(',');
      brightness = receiveVal.substring(0, brPos).toInt();

      rPos = receiveVal.indexOf(',', brPos + 1);
      red = receiveVal.substring(brPos + 1, rPos + 1).toInt();

      gPos = receiveVal.indexOf(',', rPos + 1);
      green = receiveVal.substring(rPos + 1, gPos + 1).toInt();

      blue = receiveVal.substring(gPos + 1).toInt();

      ready = true;
    }

  delay(1000);
}