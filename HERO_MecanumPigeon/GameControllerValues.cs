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
    public class GameControllerValues
    {
        public float [] axes = new float[6] { 0, 0, 0, 0, 0, 0 };
        public UInt32 btns = 0;          //!< Bitfield where '1' is pressed, '0' is not pressed.
        //public UInt32 btnChanges = 0;    //!< Bitfield where '1' means a button has transitioned not-pressed => pressed since last call.
        //public UInt32 btnsLast = 0;    //!< Bitfield where '1' means a button has transitioned not-pressed => pressed since last call.
        public Int32 pov = 0;           //!< -1 if POV is not pressed, degress when POV is used (0,45,90,135,180,225,270,315).
        public UInt32 vid = 0;
        public UInt32 pid = 0;
        public int[] vendorSpecI = null;
        public float[] vendorSpecF = null;
        public UInt32 flagBits = 0;

        public GameControllerValues()
        {
            
        }
        public GameControllerValues(GameControllerValues rhs)
        {
            Copy(rhs);
        }
        public void Copy(GameControllerValues rhs)
        {
            axes[0] = rhs.axes[0];
            axes[1] = rhs.axes[1];
            axes[2] = rhs.axes[2];
            axes[3] = rhs.axes[3];
            axes[4] = rhs.axes[4];
            axes[5] = rhs.axes[5];
            btns = rhs.btns;
            //btnChanges = rhs.btnChanges;
            //btnsLast = rhs.btnsLast;
            pov = rhs.pov;
            vid = rhs.vid;
            pid = rhs.pid;
            vendorSpecI = rhs.vendorSpecI;
            vendorSpecF = rhs.vendorSpecF;
            flagBits = rhs.flagBits;
            /* leave commands alone */
        }

        internal static GameControllerValues ZeroGameControllerValues = new GameControllerValues();
    }
}
