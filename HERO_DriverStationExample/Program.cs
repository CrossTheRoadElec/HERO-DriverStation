/*
 *  Software License Agreement
 *
 * Copyright (C) Cross The Road Electronics.  All rights
 * reserved.
 * 
 * Cross The Road Electronics (CTRE) licenses to you the right to 
 * use, publish, and distribute copies of CRF (Cross The Road) binary firmware files (*.crf) 
 * and software example source ONLY when in use with Cross The Road Electronics hardware products.
 * 
 * THE SOFTWARE AND DOCUMENTATION ARE PROVIDED "AS IS" WITHOUT
 * WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT
 * LIMITATION, ANY WARRANTY OF MERCHANTABILITY, FITNESS FOR A
 * PARTICULAR PURPOSE, TITLE AND NON-INFRINGEMENT. IN NO EVENT SHALL
 * CROSS THE ROAD ELECTRONICS BE LIABLE FOR ANY INCIDENTAL, SPECIAL, 
 * INDIRECT OR CONSEQUENTIAL DAMAGES, LOST PROFITS OR LOST DATA, COST OF
 * PROCUREMENT OF SUBSTITUTE GOODS, TECHNOLOGY OR SERVICES, ANY CLAIMS
 * BY THIRD PARTIES (INCLUDING BUT NOT LIMITED TO ANY DEFENSE
 * THEREOF), ANY CLAIMS FOR INDEMNITY OR CONTRIBUTION, OR OTHER
 * SIMILAR COSTS, WHETHER ASSERTED ON THE BASIS OF CONTRACT, TORT
 * (INCLUDING NEGLIGENCE), BREACH OF WARRANTY, OR OTHERWISE
 */

using System;
using System.Threading;
using Microsoft.SPOT;
using System.Text;

namespace HERO_DriverStationExample
{
    public class Program
    {
        static CTRE.HERO.PortDefinition wifiport = new CTRE.HERO.Port1Definition();
        static CTRE.FRC.DriverStation ds;
        static CTRE.Controller.GameController _gamepad;
        static CTRE.TalonSrx left1;
        static CTRE.TalonSrx right1;
        static StringBuilder stringBuilder = new StringBuilder();
        
        //Password to module wifi is "password1"

        /** entry point of the application */
        public static void Main()
        {
            ds = new CTRE.FRC.DriverStation(wifiport); //Create new Driver station object at port 4
            _gamepad = new CTRE.Controller.GameController(ds, 0); //Set controller to look at DS with ID 0

            //Create two TalonSrx objects and set their control mode
            left1 = new CTRE.TalonSrx(1);
            right1 = new CTRE.TalonSrx(2);
            left1.SetControlMode(CTRE.TalonSrx.ControlMode.kPercentVbus);
            right1.SetControlMode(CTRE.TalonSrx.ControlMode.kPercentVbus);

            //Set IP module looks for and is configured with, Computer must set to Static IP
            ds.SendIP(new byte[] { 10, 0, 33, 2 }, new byte[] { 10, 0, 33, 5 });


            while (true)
            {
                //Must be called for DS to function
                ds.update();

                //Send Battery Voltage information to Driver Station
                ds.SendBattery(12.12f);


                if (ds.IsAuton() && ds.IsEnabled())
                {
                    //Auton Code while enabled
                }
                else if (ds.IsAuton() && !ds.IsEnabled())
                {
                    //Auton Code while disabled
                }
                else if(!ds.IsAuton() && ds.IsEnabled())
                {
                    //Teleop Code while enabled
                    Drive();
                    Debug.Print(stringBuilder.ToString());
                    ds.SendUDP(2550,  Encoding.UTF8.GetBytes(stringBuilder.ToString()));
                    stringBuilder.Clear();
                }
                else if(!ds.IsAuton() && !ds.IsEnabled())
                {
                    //Teleop Code while disabled
                }
            }
        }

        static void Deadband(ref float value)
        {
            if (value < -0.10)
            {
                /* outside of deadband */
            }
            else if (value > +0.10)
            {
                /* outside of deadband */
            }
            else
            {
                /* within 10% so zero it */
                value = 0;
            }
        }

        static void Drive()
        {

            float y = _gamepad.GetAxis(1);
            float twist = _gamepad.GetAxis(4);

            Deadband(ref y);
            Deadband(ref twist);

            float leftThrot = y + twist;
            float rightThrot = y - twist;

            left1.Set(leftThrot);
            right1.Set(-rightThrot);
            
            stringBuilder.Append("\t");
            stringBuilder.Append(y);
            stringBuilder.Append("\t");
            stringBuilder.Append(twist);
        }
    }
}
