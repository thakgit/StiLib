// F# Script File : CenterSurround_2.fsx
//
// Center-Surround Experiment(two marker)
//
// Copyright (c) 2009-05-16 Zhang Li

#r @"StiLib.dll"
#r @"Microsoft.Xna.Framework.dll"

open System
open System.Windows.Forms
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open StiLib.Core
open StiLib.Vision

// Our Custom Experiment is inherited from StiLib.Core.SLForm
type MyEx = class
    inherit SLForm
    
    val mutable text: Text
    val mutable ex: SLExperiment
    val mutable cgrating: Grating
    val mutable sgrating: Grating
    val mutable cmask: Primitive
    
    new() as this = 
        { inherit SLForm(800, 600, 0, true, true); cgrating = null; text = null; ex = null; sgrating = null; cmask = null }
        then
        this.text <- new Text(this.GraphicsDevice, this.Services, this.SLConfig.["content"], "Thames")
        this.ex <- new SLExperiment()
        this.ex.AddExType(ExType.Context)
        this.ex.AddCondition(ExPara.Orientation, 2)
        this.ex.AddCondition(ExPara.Orientation, 2 + 1)
        this.ex.Expara.trial <- 15
        this.ex.Expara.srestT <- 0.5f
        this.ex.Expara.trestT <- 0.5f
        this.ex.Expara.durT <- 1.0f
        this.ex.Expara.bgcolor <- Color.Black
        
        let mutable gpara = GratingPara.Default
        gpara.tf <- 0.0f
        gpara.sf <- 0.8f
        gpara.sphase <- 0.0f
        gpara.BasePara.diameter <- 6.0f
        gpara.BasePara.center <- new Vector3(4.0f, 0.0f, 0.0f)
        this.cgrating <- new Grating(this.GraphicsDevice, this.Services, this.SLConfig.["content"], gpara)
        
        this.cmask <- new Primitive(this.GraphicsDevice, PrimitivePara.Circle(gpara.BasePara.diameter, this.ex.Expara.bgcolor, gpara.BasePara.center, 100, true))
        this.cmask.SetVisible(false)
        
        gpara.BasePara.diameter <- 12.0f
        this.sgrating <- new Grating(this.GraphicsDevice, this.Services, this.SLConfig.["content"], gpara)
        
    override this.SetFlow() = 
        this.ex.Flow.TCount <- 0
        this.ex.Flow.SCount <- 0
        this.ex.Flow.IsPred <- false
        this.ex.Flow.IsRested <- false
        
    override this.MarkHead() = 
        this.DrawTip(ref this.text, this.ex.Expara.bgcolor, SLConstant.MarkHead)
        
        this.ex.Expara.stimuli.[0] <- (this.ex.Cond.[0].VALUE.ValueN + 1) * this.ex.Cond.[1].VALUE.ValueN
        this.ex.Rand.RandomizeSeed()
        this.ex.Rand.RandomizeSequence(this.ex.Expara.stimuli.[0])
        
        this.ex.PPort.MarkerEncode(this.ex.Extype.[0].Value)
        this.ex.PPort.MarkerEncode(this.ex.Cond.[0].SKEY)
        this.ex.PPort.MarkerEncode(this.ex.Cond.[0].VALUE.ValueN)
        this.ex.PPort.MarkerEncode(this.ex.Cond.[1].SKEY)
        this.ex.PPort.MarkerEncode(this.ex.Cond.[1].VALUE.ValueN)
        this.ex.PPort.MarkerEncode(this.ex.Rand.RSeed)
        this.ex.PPort.MarkerEncode(this.ex.Expara.trial)
        
        this.ex.PPort.MarkerSeparatorEncode()
        
        this.cgrating.Para.Encode(this.ex.PPort)
        this.sgrating.Para.Encode(this.ex.PPort)

        this.ex.PPort.MarkerEndEncode()
        this.ex.Flow.IsStiOn <- true
        
    override this.Update() = 
        if this.GO_OVER = true then
            this.Update_fGrating()
    override this.Draw() = 
        this.GraphicsDevice.Clear(this.ex.Expara.bgcolor)
        if this.GO_OVER = true then
            this.sgrating.Draw(this.GraphicsDevice)
            this.cmask.IndexDraw(this.GraphicsDevice, PrimitiveType.TriangleFan)
            this.cgrating.Draw(this.GraphicsDevice)
            this.ex.Flow.Info <- this.ex.Flow.TCount.ToString() + " / " + this.ex.Expara.trial.ToString() + " Trials\n" + 
                                        this.ex.Flow.SCount.ToString() + " / " + this.ex.Expara.stimuli.[0].ToString() + " Stimuli"
            this.text.Draw(this.ex.Flow.Info)
        else
            this.text.Draw()
    member this.Update_fGrating() = 
        if this.ex.Flow.IsStiOn = true then
            this.ex.Flow.IsStiOn <- false
            this.ex.PPort.timer.ReStart()
            do this.ex.PPort.Trigger()
        if this.ex.PPort.timer.ElapsedSeconds < float this.ex.Expara.durT then
            if this.ex.Flow.IsPred = false then
                this.ex.Flow.IsPred <- true
                
                this.ex.Flow.RCount <- int( Math.Floor( float(this.ex.Rand.RSequence.[this.ex.Flow.SCount] / this.ex.Cond.[1].VALUE.ValueN) ) )
                this.ex.Flow.CCount <- this.ex.Rand.RSequence.[this.ex.Flow.SCount] % this.ex.Cond.[1].VALUE.ValueN
                
                let mutable crad = 0.0f // cgrating orientation
                // Single condition of cgrating
                if this.ex.Cond.[0].VALUE.ValueN = 0 then
                    crad <- this.cgrating.Para.direction * float32 SLConstant.RadpDeg
                    this.ex.Flow.Translate <- Matrix.CreateRotationZ(crad) * Matrix.CreateTranslation(this.cgrating.Para.BasePara.center)
                    this.cgrating.SetWorld(this.ex.Flow.Translate)
                    this.cgrating.SetVisible(true)
                else // Multiple condition of cgrating
                    if this.ex.Flow.RCount = 0 then // Blank
                        this.cgrating.SetVisible(false)
                        this.cmask.SetVisible(true)
                    else
                        crad <- float32 (this.ex.Flow.RCount - 1) * float32 Math.PI / float32 this.ex.Cond.[0].VALUE.ValueN
                        this.ex.Flow.Translate <- Matrix.CreateRotationZ(crad) * Matrix.CreateTranslation(this.cgrating.Para.BasePara.center)
                        this.cgrating.SetWorld(this.ex.Flow.Translate)
                        this.cgrating.SetVisible(true)
                
                if this.ex.Flow.CCount = 0 then // Blank
                    this.sgrating.SetVisible(false)
                else
                    this.ex.Flow.Translate <- Matrix.CreateRotationZ(crad + float32(this.ex.Flow.CCount - 1) * float32 Math.PI / float32( this.ex.Cond.[1].VALUE.ValueN - 1 )) * Matrix.CreateTranslation(this.sgrating.Para.BasePara.center)
                    this.sgrating.SetWorld(this.ex.Flow.Translate)
                    this.sgrating.SetVisible(true)
                    
            this.cgrating.SetTime(float32 this.ex.PPort.timer.ElapsedSeconds)
            this.sgrating.SetTime(float32 this.ex.PPort.timer.ElapsedSeconds)
        else
            if this.ex.Flow.IsRested = false then
                do this.ex.PPort.Trigger()
                this.ex.Flow.IsRested <- true
                this.cgrating.SetVisible(false)
                this.sgrating.SetVisible(false)
                this.cmask.SetVisible(false)
            if this.ex.Flow.SCount - this.ex.Expara.stimuli.[0] < -1 then
                if this.ex.PPort.timer.ElapsedSeconds > float this.ex.Expara.durT + float this.ex.Expara.srestT then
                    this.ex.Flow.IsStiOn <- true
                    this.ex.Flow.IsPred <- false
                    this.ex.Flow.IsRested <- false
                    this.ex.Flow.SCount <- this.ex.Flow.SCount + 1
            else
                if this.ex.Flow.TCount - this.ex.Expara.trial < -1 then
                    if this.ex.PPort.timer.ElapsedSeconds > float this.ex.Expara.durT + float this.ex.Expara.trestT then
                        this.ex.Rand.RandomizeSequence(this.ex.Expara.stimuli.[0])
                        this.ex.Flow.IsStiOn <- true
                        this.ex.Flow.IsPred <- false
                        this.ex.Flow.IsRested <- false
                        this.ex.Flow.TCount <- this.ex.Flow.TCount + 1
                        this.ex.Flow.SCount <- 0
                else
                    this.GO_OVER <- false
        
end

let MyExperiment = new MyEx(Text = "F# Scripting CenterSurround_2")
Application.Run(MyExperiment)