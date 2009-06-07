﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StiLib.Core;
using StiLib.Vision;

namespace Cell_Hunter
{
    public partial class Form1 : SLForm
    {
        public Form1() : base(1024, 768, 120, true, false)
        {
        }

        Text SLText;
        Matrix World;
        SLTimer Timer;
        SLInput Input;
        Bar Bar;
        Grating Grating;
        Primitive Cross;
        Color Bgcolor;
        VSType CurrentSti;
        GratingType GratingType;
        Shape GratingShape;
        MaskType GratingMask;
        int HelpText;

        protected override void Initialize()
        {
            SLText = new Text(GraphicsDevice, Services, "Content", "Arial");
            Timer = new SLTimer();
            Timer.Start();
            Input = new SLInput();

            BarPara bpara = BarPara.Default;
            bpara.width = 5.0f;
            bpara.height = 1.0f;
            bpara.BasePara.orientation = 90.0f;
            Bar = new Bar(GraphicsDevice, bpara);

            Grating = new Grating(GraphicsDevice, Services, "Content", GratingPara.Default);
            GratingType = Grating.Para.gratingtype;
            GratingShape = Grating.Para.shape;
            GratingMask = Grating.Para.maskpara.MaskType;
            Cross = new Primitive(GraphicsDevice, PrimitivePara.Cross(1.0f, Color.Black, Vector3.Zero));

            Bgcolor = Color.Black;
            CurrentSti = VSType.Bar;
            HelpText = 0;
        }

        protected override void Update()
        {
            Input.Update();

            // Show Help Text
            if (Input.IsKeyPressed(Keys.Tab))
            {
                HelpText += 1;
                if (HelpText > 2) HelpText = 0;
            }
            // Change Stimulus
            if (Input.IsKeyPressed(Keys.Q))
            {
                CurrentSti += 1;
                if ((int)CurrentSti > 1) CurrentSti = 0;
            }

            Refresh_Location();
            Refresh_BgColor();

            switch (CurrentSti)
            {
                case VSType.Grating:
                    Update_Grating();
                    break;
                default:
                    Update_Bar();
                    break;
            }
        }

        protected override void Draw()
        {
            GraphicsDevice.Clear(Bgcolor);

            string Tip = null, Help = null;
            switch (CurrentSti)
            {
                case VSType.Grating:
                    Grating.Draw(GraphicsDevice);

                    Tip = "Diameter: " + Grating.Para.BasePara.diameter.ToString("F1") +
                    "\nDirection: " + Grating.Para.direction.ToString() +
                    "\nLocation: [" + Grating.Para.BasePara.center.X.ToString("F1") + ", " + Grating.Para.BasePara.center.Y.ToString("F1") + "]" +
                    "\nSF: " + Grating.Para.sf.ToString("F1") + " TF: " + Grating.Para.tf.ToString("F1");
                    Help = "Diameter(W/S) Type(Space)\nSF(D/F) TF(Z/X) Shape(E)\nMask(M) MaskSigma(T/Y)" +
                                "\nLHColor(R/G/B + Up/Down)\nRLColor(R/G/B + Left/Right)\nAlpha(A + Up/Down)";

                    break;
                default:
                    Bar.Draw(GraphicsDevice);

                    Tip = "Width: " + Bar.Para.width.ToString("F1") + " Height: " + Bar.Para.height.ToString("F1") +
                    "\nOrientation: " + Bar.Para.BasePara.orientation.ToString() +
                    "\nLocation: [" + Bar.Para.BasePara.center.X.ToString("F1") + ", " + Bar.Para.BasePara.center.Y.ToString("F1") + "]";
                    Help = "Width(A/D) Height(W/S)\nColor(Z/X) BgColor(C/V)\n  ReverseColor(Space)";

                    break;
            }

            Cross.IndexDraw(GraphicsDevice, PrimitiveType.LineList);

            if (HelpText < 2)
            {
                SLText.Draw(Tip);
                SLText.Draw(new Vector2(5, GraphicsDevice.Viewport.Height - 25), "Help(Tab) / Stimulus(Q)", Color.SkyBlue);

                if (HelpText > 0)
                {
                    SLText.Draw(new Vector2(GraphicsDevice.Viewport.Width - 250, 5), Help, Color.Tomato);
                }
            }
        }


        void Refresh_Location()
        {
            float x = Input.MOUSE.X - GraphicsDevice.DisplayMode.Width / 2.0f;
            float y = Input.MOUSE.Y - GraphicsDevice.DisplayMode.Height / 2.0f;
            x = x / Bar.pixel_p_deg;
            y = -y / Bar.pixel_p_deg;

            switch (CurrentSti)
            {
                case VSType.Grating:
                    Grating.Para.BasePara.center = new Vector3(x, y, 0.0f);
                    break;
                default:
                    Bar.Para.BasePara.center = new Vector3(x, y, 0.0f);
                    break;
            }
        }

        void Refresh_BgColor()
        {
            if (Input.IsKeyDown(Keys.C))
            {
                Bgcolor = new Color(new Vector4(0.005f) + Bgcolor.ToVector4());
            }
            if (Input.IsKeyDown(Keys.V))
            {
                Bgcolor = new Color(new Vector4(-0.005f) + Bgcolor.ToVector4());
            }
        }


        void Update_Bar()
        {
            Bar_Ori();
            Bar_Scale();
            Bar_Color();
        }

        void Bar_Ori()
        {
            if (Input.IsMouseLeftButtonDown())
            {
                Bar.Para.BasePara.orientation = (Bar.Para.BasePara.orientation + 1.0f) % 180;
            }
            if (Input.IsMouseRightButtonDown())
            {
                Bar.Para.BasePara.orientation = (Bar.Para.BasePara.orientation - 1.0f) % 180;
            }
            World = Matrix.CreateRotationZ((float)(Bar.Para.BasePara.orientation * Math.PI / 180.0)) *
                        Matrix.CreateTranslation(Bar.Para.BasePara.center);
            Bar.SetWorld(World);
        }

        void Bar_Scale()
        {
            if (Input.IsKeyDown(Keys.A))
            {
                Bar.Scale_Width(1 + 0.005f);
            }
            if (Input.IsKeyDown(Keys.D))
            {
                Bar.Scale_Width(1 - 0.005f);
            }
            if (Input.IsKeyDown(Keys.W))
            {
                Bar.Scale_Height(1 + 0.005f);
            }
            if (Input.IsKeyDown(Keys.S))
            {
                Bar.Scale_Height(1 - 0.005f);
            }
            if (Input.MouseWheelDelta != 0)
            {
                Bar.ScaleSize(1 + Input.MouseWheelDelta * 0.1f);
            }
        }

        void Bar_Color()
        {
            if (Input.IsKeyDown(Keys.Z))
            {
                Bar.ScaleColor(0.005f);
            }
            if (Input.IsKeyDown(Keys.X))
            {
                Bar.ScaleColor(-0.005f);
            }
            if (Input.IsKeyPressed(Keys.Space))
            {
                Color c = Bar.barvertex[0].Color;
                Bar.SetColor(Bgcolor);
                Bgcolor = c;
            }
        }


        void Update_Grating()
        {
            Grating_Dir();
            Grating_Scale();
            Grating_Freq();
            Grating_Type();
            Grating_Color();
        }

        void Grating_Dir()
        {
            if (Input.IsMouseLeftButtonDown())
            {
                Grating.Para.direction = (Grating.Para.direction + 1.0f) % 360;
            }
            if (Input.IsMouseRightButtonDown())
            {
                Grating.Para.direction = (Grating.Para.direction - 1.0f) % 360;
            }

            World = Matrix.CreateRotationZ((float)(Grating.Para.direction * Math.PI / 180.0)) *
                         Matrix.CreateTranslation(Grating.Para.BasePara.center);
            Grating.SetWorld(World);
            Grating.SetTime((float)Timer.ElapsedSeconds);
        }

        void Grating_Scale()
        {
            if (Input.IsKeyDown(Keys.W))
            {
                Grating.SetDiameter(Grating.Para.BasePara.diameter * 1.005f);
            }
            if (Input.IsKeyDown(Keys.S))
            {
                Grating.SetDiameter(Grating.Para.BasePara.diameter * 0.995f);
            }
        }

        void Grating_Freq()
        {
            if (Input.IsKeyDown(Keys.D))
            {
                Grating.SetSF(Grating.Para.sf * 1.01f);
            }
            if (Input.IsKeyDown(Keys.F))
            {
                Grating.SetSF(Grating.Para.sf * 0.99f);
            }
            if (Input.IsKeyDown(Keys.Z))
            {
                Grating.SetTF(Grating.Para.tf * 1.01f);
            }
            if (Input.IsKeyDown(Keys.X))
            {
                Grating.SetTF(Grating.Para.tf * 0.99f);
            }
        }

        void Grating_Type()
        {
            if (Input.IsKeyPressed(Keys.Space))
            {
                GratingType += 1;
                if ((int)GratingType > 2) GratingType = 0;
                Grating.SetGratingType(GratingType);
            }
            if (Input.IsKeyPressed(Keys.E))
            {
                if ((int)GratingShape == 6)
                {
                    GratingShape = Shape.Circle;
                }
                else
                {
                    GratingShape = Shape.Quadrate;
                }
                Grating.SetShape(GratingShape);
            }
            if (Input.IsKeyPressed(Keys.M))
            {
                GratingMask += 1;
                if ((int)GratingMask > 1) GratingMask = 0;
                Grating.SetMask(GratingMask);
            }
            if (Input.IsKeyDown(Keys.T))
            {
                Grating.SetSigma(Grating.Para.maskpara.BasePara.diameter * 1.01f);
            }
            if (Input.IsKeyDown(Keys.Y))
            {
                Grating.SetSigma(Grating.Para.maskpara.BasePara.diameter * 0.99f);
            }
        }

        void Grating_Color()
        {
            // Red Color
            if (Input.IsKeyHold(Keys.R))
            {
                if (Input.IsKeyDown(Keys.Up))
                {
                    Vector4 c = Grating.MaxColor;
                    c.X += 0.01f;
                    if (c.X > 1) c.X = 1;
                    Grating.SetMaxMinColor(c, Grating.MinColor);
                }
                if (Input.IsKeyDown(Keys.Down))
                {
                    Vector4 c = Grating.MaxColor;
                    c.X -= 0.01f;
                    if (c.X < 0) c.X = 0;
                    Grating.SetMaxMinColor(c, Grating.MinColor);
                }
                if (Input.IsKeyDown(Keys.Left))
                {
                    Vector4 c = Grating.MinColor;
                    c.X += 0.01f;
                    if (c.X > 1) c.X = 1;
                    Grating.SetMaxMinColor(Grating.MaxColor, c);
                }
                if (Input.IsKeyDown(Keys.Right))
                {
                    Vector4 c = Grating.MinColor;
                    c.X -= 0.01f;
                    if (c.X < 0) c.X = 0;
                    Grating.SetMaxMinColor(Grating.MaxColor, c);
                }
            }

            // Green Color
            if (Input.IsKeyHold(Keys.G))
            {
                if (Input.IsKeyDown(Keys.Up))
                {
                    Vector4 c = Grating.MaxColor;
                    c.Y += 0.01f;
                    if (c.Y > 1) c.Y = 1;
                    Grating.SetMaxMinColor(c, Grating.MinColor);
                }
                if (Input.IsKeyDown(Keys.Down))
                {
                    Vector4 c = Grating.MaxColor;
                    c.Y -= 0.01f;
                    if (c.Y < 0) c.Y = 0;
                    Grating.SetMaxMinColor(c, Grating.MinColor);
                }
                if (Input.IsKeyDown(Keys.Left))
                {
                    Vector4 c = Grating.MinColor;
                    c.Y += 0.01f;
                    if (c.Y > 1) c.Y = 1;
                    Grating.SetMaxMinColor(Grating.MaxColor, c);
                }
                if (Input.IsKeyDown(Keys.Right))
                {
                    Vector4 c = Grating.MinColor;
                    c.Y -= 0.01f;
                    if (c.Y < 0) c.Y = 0;
                    Grating.SetMaxMinColor(Grating.MaxColor, c);
                }
            }

            // Blue Color
            if (Input.IsKeyHold(Keys.B))
            {
                if (Input.IsKeyDown(Keys.Up))
                {
                    Vector4 c = Grating.MaxColor;
                    c.Z += 0.01f;
                    if (c.Z > 1) c.Z = 1;
                    Grating.SetMaxMinColor(c, Grating.MinColor);
                }
                if (Input.IsKeyDown(Keys.Down))
                {
                    Vector4 c = Grating.MaxColor;
                    c.Z -= 0.01f;
                    if (c.Z < 0) c.Z = 0;
                    Grating.SetMaxMinColor(c, Grating.MinColor);
                }
                if (Input.IsKeyDown(Keys.Left))
                {
                    Vector4 c = Grating.MinColor;
                    c.Z += 0.01f;
                    if (c.Z > 1) c.Z = 1;
                    Grating.SetMaxMinColor(Grating.MaxColor, c);
                }
                if (Input.IsKeyDown(Keys.Right))
                {
                    Vector4 c = Grating.MinColor;
                    c.Z -= 0.01f;
                    if (c.Z < 0) c.Z = 0;
                    Grating.SetMaxMinColor(Grating.MaxColor, c);
                }
            }

            // Alpha Color
            if (Input.IsKeyHold(Keys.A))
            {
                if (Input.IsKeyDown(Keys.Up))
                {
                    float a = Math.Max(Grating.MaxColor.W, Grating.MinColor.W);
                    a += 0.01f;
                    if (a > 1) a = 1;
                    Grating.SetTransparency(a);
                }
                if (Input.IsKeyDown(Keys.Down))
                {
                    float a = Math.Max(Grating.MaxColor.W, Grating.MinColor.W);
                    a -= 0.01f;
                    if (a < 0) a = 0;
                    Grating.SetTransparency(a);
                }
            }

        }

    }
}
