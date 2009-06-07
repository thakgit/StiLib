// F# Script File : RF_fBar.fsx
//
// Two Flashing Bar Reverse-Correlation Receptive Field Mapping Diagram
//
// Copyright (c) 2008-11-09 Zhang Li

#r @"StiLib.dll"
#r @"Microsoft.Xna.Framework.dll"

open System
open System.Windows.Forms
open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open StiLib.Core
open StiLib.Vision
open StiLib.Vision.Stimuli

// Our Custom Experiment is inherited from StiLib.Vision.Stimuli.RF_fBar
type ExRF_fBar() = class
    inherit RF_fBar(800, 600, 0, true, true)
    
    override self.Initialize() =
        self.text <- new Text(self.GraphicsDevice, self.Services, "E:\Programs\Stimulus\StiLib\Release\Content", "Arial")
        self.ex <- new SLExperiment()
        
        self.ex.AddExType(ExType.RF_fBar)
        self.ex.AddCondition(ExPara.Orientation, 0)
        self.ex.Expara.trial <- 60
        self.ex.Expara.durT <- 0.030f
        self.ex.Expara.bgcolor <- Color.Gray
        
        let mutable bpara = BarPara.Default
        bpara.width <- 0.5f
        bpara.height <- 1.5f
        bpara.BasePara.orientation <- 0.0f
        bpara.BasePara.movearea <- 8.0f
        bpara.BasePara.center <- new Vector3(3.0f, 3.0f, 0.0f)
        bpara.BasePara.color <- Color.Black
        self.Bar.[0].Init(self.GraphicsDevice, bpara)
        
        bpara.BasePara.color <- Color.White
        self.Bar.[1].Init(self.GraphicsDevice, bpara)
        
        self.InitGrid()
        
end

let Experiment = new ExRF_fBar(Text = "F# Scripting RF_fBar")
Application.Run(Experiment)

#if COMPILED
[<STAThread>]
do Application.Run(Experiment)
#endif