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
using Microsoft.SPOT;

namespace CTRE
{
    class Stopwatch
    {
        private long _t0 = 0;
        private long _t1 = 0;
        private float _scalar = 0.001f / TimeSpan.TicksPerMillisecond;
        public void Start()
        {
            _t0 = DateTime.Now.Ticks;
        }
        public float Duration
        {
            get
            {
                _t1 = DateTime.Now.Ticks;
                long retval = _t1 - _t0;
                if (retval < 0)
                    retval = 0;
                return retval * _scalar;
            }
        }
        public uint DurationMs
        {
            get
            {
                return (uint)(Duration * 1000);
            }
        }

        public String Caption
        {
            get
            {
                float timeS = Duration;
                if (timeS < 0.000001)
                {
                    return "" + (int)(timeS * 1000 * 1000) + " us";
                }
                else if (timeS < 0.001)
                {
                    return "" + (int)(timeS * 1000) + " ms";
                }
                return "" + (int)timeS + " sec";
            }
        }
    }
}