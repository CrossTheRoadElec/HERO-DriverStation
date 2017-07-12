using System;
using Microsoft.SPOT;
using CTRE;

namespace Mechanum_Fun
{
    public class Program
    {
        static CTRE.FRC.DriverStation ds = new CTRE.FRC.DriverStation(CTRE.HERO.IO.Port1);
        static CTRE.Controller.GameController controller = new CTRE.Controller.GameController(ds, 0);

        static TalonSrx leftF = new TalonSrx(1);
        static TalonSrx leftB = new TalonSrx(2);
        static TalonSrx rightB = new TalonSrx(3);
        static TalonSrx rightF = new TalonSrx(4);

        static PigeonImu pigeon = new PigeonImu(leftB);

        static Stopwatch timer = new Stopwatch();

        const float turnP = 0.01f;
        public static void Main()
        {
            float turn = 0;
            timer.Start();

            bool leftLast = false;
            bool rightLast = false;
            while(true)
            {
                ds.update();
                ds.SendBattery(12.34f);
                float t = deltaTime();
                CTRE.FRC.DriverStation.State s = ds.GetState();
                if (ds.IsEnabled())
                {
                    float x = controller.GetAxis(0);
                    float y = controller.GetAxis(1);
                    float twist = -controller.GetAxis(4);
                    if (twist * twist < 0.005)
                        twist = 0;
                    turn += twist * t * 180;

                    if (controller.GetButton(6) && !leftLast) turn -= 90;
                    if (controller.GetButton(5) && !rightLast) turn += 90;
                    leftLast = controller.GetButton(6);
                    rightLast = controller.GetButton(5);

                    drive(x, y, turn);
                }
                else
                {
                    float[] ypr = new float[3];
                    pigeon.GetYawPitchRoll(ypr);
                    turn = ypr[0];
                }

                byte[] b = new byte[] { (byte)'h', (byte)'e', (byte)'l', (byte)'l', (byte)'o' };
                ds.SendUDP(2550, b);
            }
        }
        
        static float turnPID(float error)
        {
            float kP = turnP * error;
            return kP;
        }

        static float deltaTime()
        {
            float x = timer.Duration;
            timer.Start();
            return x;
        }

        static void drive(float x, float y, float turn)
        {
            float[] ypr = new float[3];
            pigeon.GetYawPitchRoll(ypr);
            float turnPower = turnPID(turn - ypr[0]);
            leftF.Set((y - x + turnPower));
            leftB.Set((y + x + turnPower));
            rightB.Set(-(y - x - turnPower));
            rightF.Set(-(y + x - turnPower));
        }
    }
}
