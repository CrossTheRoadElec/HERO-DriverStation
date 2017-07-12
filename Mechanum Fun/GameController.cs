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

namespace CTRE.Controller
{
    public class GameController
    {
        private IGameControllerValuesProvider _provider;
        private GameControllerValues _values = new GameControllerValues();

        private UInt32 _rumbleL = 0;
        private UInt32 _rumbleR = 0;
        private UInt32 _ledCode = 0;
        private UInt32 _controlFlags = 0;
        private uint _index;


        public GameController(IGameControllerValuesProvider provider, uint idx)
        {
            _provider = provider;
            if (_provider is CTRE.FRC.DriverStation)
            {
                _index = idx;
                _provider.SetRef(this, idx);
            }
            else
            {
                if(idx > 0)
                    throw new ArgumentException("Can only have index 0 in USB Game Controller");
                _index = 0;
            }
        }
        /**
         * @param buttonIdx (One-indexed button).  '1' for button1 (first button).
         * @return true if specified button is true.
         */
        public bool GetButton(uint buttonIdx)
        {
            if (buttonIdx > 0)
                --buttonIdx;
            _provider.Get(ref _values, _index);
            return (_values.btns >> (int)buttonIdx & 1) == 1;
        }
        /**
         * @param axisIdx (Zero-indexed axis).  '0' is typically the first X axis.
         * @return floating point value within [-1,1].
         */
        public float GetAxis(uint axisIdx)
        {
            _provider.Get(ref _values, _index);
            if (axisIdx >= _values.axes.Length)
                return 0;
            return _values.axes[axisIdx];
        }
        /**
         * Retrieves a copy of the internal gamepadvalues structure used in decoding signals.
         * This can be used to retrieve signals not readily available through the Gamepad API (such as vendor specific signals or VID/PID).
         * To use this function, first create a gamepadValues instance and pass by reference.
         * <pre>{@code
         *      GamepadValues values = new GamepadValues(); // Create only once and use functiont to update it periodically.
         *      ...
         *      gamepad.GetAllValues(gamepadValues); // Get latest values
         * }</pre>
         * @param gamepadValues reference to update with latest values.
         * @return object reference to gamepadValues for function chaining.
         */
        public GameControllerValues GetAllValues(ref GameControllerValues gamepadValues)
        {
            /* get latest copy if there is new data */
            _provider.Get(ref _values, _index);
            /* copy it to caller */
            gamepadValues.Copy(_values);
            return gamepadValues;
        }
        /**
         * Get the connection status of the Usb device.
         */
        public UsbDeviceConnection GetConnectionStatus()
        {
            int code = _provider.Get(ref _values, _index);
            if (code >= 0)
                return UsbDeviceConnection.Connected;
            return UsbDeviceConnection.NotConnected;
        }

        /**
         * Set the Right Rumble strength.
		 * @param strength 0 for off, [1,255] 
		 * 			for on with increasing strength.
		 * @return int error code, 0 for success.
         */
        public int SetLeftRumble(byte strength)
        {
            _rumbleL = (uint)strength;
            return _provider.Sync(ref _values, _rumbleL, _rumbleR, _ledCode, _controlFlags, _index);
        }
        /**
         * Set the Right Rumble strength.
		 * @param strength 0 for off, [1,255] 
		 * 			for on with increasing strength.
		 * @return int error code, 0 for success.
         */
        public int SetRightRumble(byte strength)
        {
            _rumbleR = (uint)strength;
            return _provider.Sync(ref _values, _rumbleL, _rumbleR, _ledCode, _controlFlags, _index);
        }
        /**
         * Set the Left/Right Rumble strength at the same time.
		 * @param leftStrength 0 for off, [1,255] 
		 * 			for on with increasing strength.
		 * @param rightStrength 0 for off, [1,255] 
		 * 			for on with increasing strength.
		 * @return int error code, 0 for success.
         */
        public int SetRumble(byte leftStrength, byte rightStrength)
        {
            _rumbleL = (uint)leftStrength;
            _rumbleR = (uint)rightStrength;
            return _provider.Sync(ref _values, _rumbleL, _rumbleR, _ledCode, _controlFlags, _index);
        }
        /**
         * Set the Xbox LED code.
		 * @param Valid values are [6,9] for the four LEDs.
		 * @return int error code, 0 for success.
         */
        public int SetLEDCode(byte ledCode)
        {
            _ledCode = (uint)ledCode;
            return _provider.Sync(ref _values, _rumbleL, _rumbleR, _ledCode, _controlFlags, _index);
        }
        public int SetControlFlags(uint mask)
        {
            _controlFlags |= mask;
            return _provider.Sync(ref _values, _rumbleL, _rumbleR, _ledCode, _controlFlags, _index);
        }
        public int ClearControlFlags(uint mask)
        {
            _controlFlags &= ~mask;
            return _provider.Sync(ref _values, _rumbleL, _rumbleR, _ledCode, _controlFlags, _index);
        }
    }
}
