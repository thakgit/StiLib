﻿#region File Description
//-----------------------------------------------------------------------------
// dGrating.cs
//
// StiLib Drifting Grating Stimulus
// Copyright (c) Zhang Li. 2009-03-11.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StiLib.Core;
#endregion

namespace StiLib.Vision.Stimuli
{
    /// <summary>
    /// Drifting Grating Stimulus
    /// </summary>
    public class dGrating : SLForm
    {
        /// <summary>
        /// Default SLForm Settings
        /// </summary>
        public dGrating() : base()
        {
        }

        /// <summary>
        /// Custom SLForm Settings
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="refreshrate"></param>
        /// <param name="isvsync"></param>
        /// <param name="isshowcursor"></param>
        public dGrating(int width, int height, int refreshrate, bool isvsync, bool isshowcursor)
            : base(width, height, refreshrate, isvsync, isshowcursor)
        {
        }


        /// <summary>
        /// Text to draw Message
        /// </summary>
        public Text text;
        /// <summary>
        /// Hold the Information about the Experiment
        /// </summary>
        public SLExperiment ex;
        /// <summary>
        /// Drifting Grating
        /// </summary>
        public Grating Grating;


        /// <summary>
        /// Init all information of the experiment
        /// </summary>
        protected override void Initialize()
        {
            text = new Text(GraphicsDevice, Services, "Content", "Arial");

            // Init Experiment Parameter
            ex = new SLExperiment();
            ex.AddExType(ExType.dGrating);
            ex.AddCondition(ExPara.Direction, 4);
            ex.Expara.trial = 3;
            ex.Expara.trestT = 1.0f;
            ex.Expara.srestT = 0.5f;
            ex.Expara.preT = 0.25f;
            ex.Expara.durT = 1.0f;
            ex.Expara.posT = 0.25f;
            ex.Expara.bgcolor = Color.Gray;

            // Init Grating Parameter
            GratingPara gpara = GratingPara.Default;
            gpara.gratingtype = GratingType.Sinusoidal;
            gpara.shape = Shape.Circle;
            gpara.tf = 2.0f;
            gpara.sf = 1.0f;
            gpara.luminance = 0.5f;
            gpara.contrast = 1.0f;
            gpara.BasePara.diameter = 5.0f;
            gpara.BasePara.center = new Vector3(-5.0f, 0.0f, 0.0f);
            gpara.lhcolor = Color.RosyBrown;
            gpara.rlcolor = Color.Blue;
            Grating = new Grating(GraphicsDevice, Services, "Content", gpara);
        }

        /// <summary>
        /// Set Flow to control experiment
        /// </summary>
        protected override void  SetFlow()
        {
            ex.Flow.SCount = 0;
            ex.Flow.TCount = 0;
            ex.Flow.IsPred = false;
            ex.Flow.IsRested = false;
            ex.Flow.IsBlanked = false;
            ex.Flow.PreDurTime = ex.Expara.preT + ex.Expara.durT;
            ex.Flow.StiTime = ex.Flow.PreDurTime + ex.Expara.posT;
        }

        /// <summary>
        /// Send crucial information in MarkerHeader 
        /// </summary>
        protected override void MarkHead()
        {
            // Single Condition
            if (ex.Cond[0].VALUE.ValueN == 0) 
            {
                MarkHead_sdGrating();
            }
            else // Multiple Conditions
            {
                MarkHead_mdGrating();
            }
        }

        void MarkHead_sdGrating()
        {
            ex.Expara.stimuli[0] = 1;

            // Experiment Type Encoding
            ex.PPort.MarkerEncode(ex.Extype[0].Value);
            // Condition Parameter Type Encoding
            ex.PPort.MarkerEncode(ex.Cond[0].SKEY);
            // Condition Number Encoding
            ex.PPort.MarkerEncode(ex.Cond[0].VALUE.ValueN);
            // Random Seed Encoding
            ex.PPort.MarkerEncode(ex.Rand.RSeed);
            // Experiment Trials
            ex.PPort.MarkerEncode(ex.Expara.trial);

            // Keywords Group Seperator
            ex.PPort.MarkerSeparatorEncode();

            // Custom Parameters Encoding
            ex.PPort.MarkerEncode((int)Math.Floor(Grating.Para.tf * 10.0));
            ex.PPort.MarkerEncode((int)Math.Floor(Grating.Para.sf * 100.0));
            ex.PPort.MarkerEncode((int)Math.Floor((double)Grating.Para.direction));
            ex.PPort.MarkerEncode((int)Math.Floor(Grating.Para.luminance * 10.0));
            ex.PPort.MarkerEncode((int)Math.Floor(Grating.Para.contrast * 10.0));
            ex.PPort.MarkerEncode((int)Math.Floor((Grating.Para.BasePara.center.X + 60.0f) * 10.0));
            ex.PPort.MarkerEncode((int)Math.Floor((Grating.Para.BasePara.center.Y + 60.0f) * 10.0));
            ex.PPort.MarkerEncode((int)Math.Floor(Grating.Para.BasePara.diameter));

            // End of Header Encoding
            ex.PPort.MarkerEndEncode();
            // Set ready to begin
            ex.Flow.IsStiOn = true;
        }

        void MarkHead_mdGrating()
        {
            ex.Expara.stimuli[0] = ex.Cond[0].VALUE.ValueN + 1;
            ex.Rand.RandomizeSeed();
            ex.Rand.RandomizeSequence(ex.Expara.stimuli[0]);

            // Experiment Type Encoding
            ex.PPort.MarkerEncode(ex.Extype[0].Value);
            // Condition Parameter Type Encoding
            ex.PPort.MarkerEncode(ex.Cond[0].SKEY);
            // Condition Number Encoding
            ex.PPort.MarkerEncode(ex.Cond[0].VALUE.ValueN);
            // Random Seed Encoding
            ex.PPort.MarkerEncode(ex.Rand.RSeed);
            // Experiment Trials
            ex.PPort.MarkerEncode(ex.Expara.trial);

            // Keywords Group Seperator
            ex.PPort.MarkerSeparatorEncode();

            // Custom Parameters Encoding
            ex.PPort.MarkerEncode((int)Math.Floor(Grating.Para.tf * 10.0));
            ex.PPort.MarkerEncode((int)Math.Floor(Grating.Para.sf * 100.0));
            ex.PPort.MarkerEncode((int)Math.Floor(Grating.Para.luminance * 10.0));
            ex.PPort.MarkerEncode((int)Math.Floor(Grating.Para.contrast * 10.0));
            ex.PPort.MarkerEncode((int)Math.Floor((Grating.Para.BasePara.center.X + 60.0f) * 10.0));
            ex.PPort.MarkerEncode((int)Math.Floor((Grating.Para.BasePara.center.Y + 60.0f) * 10.0));
            ex.PPort.MarkerEncode((int)Math.Floor(Grating.Para.BasePara.diameter));

            // End of Header Encoding
            ex.PPort.MarkerEndEncode();
            // Set ready to begin
            ex.Flow.IsStiOn = true;
        }

        /// <summary>
        /// Update experiment
        /// </summary>
        protected override void Update()
        {
            if (GO_OVER)
            {
                // Single Condition
                if (ex.Cond[0].VALUE.ValueN == 0) 
                {
                    Update_sdGrating();
                }
                else // Multiple Conditions
                {
                    Update_mdGrating();
                }
            }
        }

        /// <summary>
        /// Draw Stimuli
        /// </summary>
        protected override void Draw()
        {
            GraphicsDevice.Clear(ex.Expara.bgcolor);

            if (GO_OVER)
            {
                Grating.Draw(GraphicsDevice);

                ex.Flow.Info = ex.Flow.TCount.ToString() + " / " + ex.Expara.trial.ToString() + " Trials\n" +
                                     ex.Flow.SCount.ToString() + " / " + ex.Expara.stimuli[0].ToString() + " Stimuli";
                text.Draw(ex.Flow.Info);
            }
            else
            {
                text.Draw();
            }
        }

        void Update_sdGrating()
        {
            if (ex.Flow.IsStiOn)
            {
                ex.Flow.IsStiOn = false;
                ex.PPort.timer.ReStart();
                // Stimulus Onset Marker
                ex.PPort.Trigger();
            }

            ex.Flow.LastTime = ex.PPort.timer.ElapsedSeconds;

            // In Presentation
            if (ex.Flow.LastTime < ex.Flow.StiTime)
            {
                if (!ex.Flow.IsPred)
                {
                    ex.Flow.IsPred = true;

                    ex.Flow.Translate = Matrix.CreateRotationZ((float)(Grating.Para.direction * Math.PI / 180.0)) *
                                                 Matrix.CreateTranslation(Grating.Para.BasePara.center);
                    Grating.SetWorld(ex.Flow.Translate);
                    Grating.SetVisible(true);
                }

                if (ex.Flow.LastTime > ex.Expara.preT && ex.Flow.LastTime < ex.Flow.PreDurTime)
                {
                    Grating.SetTime((float)ex.Flow.LastTime - ex.Expara.preT);
                }
            }
            else // End of Presentation
            {
                if (!ex.Flow.IsRested)
                {
                    // Stimulus Offset Marker
                    ex.PPort.Trigger();

                    ex.Flow.IsRested = true;
                    Grating.SetVisible(false);
                }

                if (ex.Flow.TCount < ex.Expara.trial - 1)
                {
                    if (ex.Flow.LastTime > ex.Flow.StiTime + ex.Expara.trestT)
                    {
                        ex.Flow.IsStiOn = true;
                        ex.Flow.IsPred = false;
                        ex.Flow.IsRested = false;
                        ex.Flow.TCount += 1;
                        // Set Temporal Phase back to zero for new stimulus
                        Grating.SetTime(0.0f);
                    }
                }
                else
                {
                    GO_OVER = false;
                }

            }
        }

        void Update_mdGrating()
        {
            if (ex.Flow.IsStiOn)
            {
                ex.Flow.IsStiOn = false;
                ex.PPort.timer.ReStart();
                // Stimulus Onset Marker
                ex.PPort.Trigger();
            }

            ex.Flow.LastTime = ex.PPort.timer.ElapsedSeconds;

            // In Presentation
            if (ex.Flow.LastTime < ex.Flow.StiTime)
            {
                // Blank Control
                if (ex.Rand.RSequence[ex.Flow.SCount] == 0)
                {
                    if (!ex.Flow.IsBlanked)
                    {
                        ex.Flow.IsBlanked = true;
                        Grating.SetVisible(false);
                    }
                }
                else // Normal Stimulus
                {
                    if (!ex.Flow.IsPred)
                    {
                        ex.Flow.IsPred = true;

                        float rad = (float)((ex.Rand.RSequence[ex.Flow.SCount] - 1) * (2 * Math.PI / ex.Cond[0].VALUE.ValueN));
                        ex.Flow.Translate = Matrix.CreateRotationZ(rad) * 
                                                     Matrix.CreateTranslation(Grating.Para.BasePara.center);
                        Grating.SetWorld(ex.Flow.Translate);
                        Grating.SetVisible(true);
                    }

                    if (ex.Flow.LastTime > ex.Expara.preT && ex.Flow.LastTime < ex.Flow.PreDurTime)
                    {
                        Grating.SetTime((float)ex.Flow.LastTime - ex.Expara.preT);
                    }
                }
            }
            else // End of Presentation
            {
                if (!ex.Flow.IsRested)
                {
                    // Stimulus Offset Marker
                    ex.PPort.Trigger();

                    ex.Flow.IsRested = true;
                    Grating.SetVisible(false);
                }

                if (ex.Flow.SCount < ex.Expara.stimuli[0] - 1)
                {
                    if (ex.Flow.LastTime > ex.Flow.StiTime + ex.Expara.srestT)
                    {
                        ex.Flow.IsStiOn = true;
                        ex.Flow.IsPred = false;
                        ex.Flow.IsRested = false;
                        ex.Flow.SCount += 1;
                        // Set Temporal Phase back to zero for new stimulus
                        Grating.SetTime(0.0f);
                    }
                }
                else
                {
                    if (ex.Flow.TCount < ex.Expara.trial - 1)
                    {
                        if (ex.Flow.LastTime > ex.Flow.StiTime + ex.Expara.trestT)
                        {
                            // Each trial has different random sequence of stimulus
                            ex.Rand.RandomizeSequence(ex.Expara.stimuli[0]);
                            ex.Flow.IsStiOn = true;
                            ex.Flow.IsPred = false;
                            ex.Flow.IsRested = false;
                            ex.Flow.IsBlanked = false;
                            ex.Flow.TCount += 1;
                            ex.Flow.SCount = 0;
                            // Set Temporal Phase back to zero for new stimulus
                            Grating.SetTime(0.0f);
                        }
                    }
                    else
                    {
                        GO_OVER = false;
                    }
                }
            }
        }

    }
}
